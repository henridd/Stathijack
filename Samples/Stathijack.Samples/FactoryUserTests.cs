using Stathijack.Mocking;
using Stathijack.Samples.RealEntities;
using Stathijack.Samples.TestEntities;
using System.Data;

namespace Stathijack.Samples
{
    internal class FactoryUserTests
    {
        [Test]
        public void UsingAFakeClass()
        {
            // Arrange
            using var hijacker = new HijackRegister();
            hijacker.Register(typeof(Factory), typeof(MockFactory));
            var factoryConsumer = new FactoryConsumer();

            // Act
            var entity = factoryConsumer.UseFactory();

            // Assert
            Assert.That(entity.Name, Is.EqualTo("Fake"));
        }
        
        /// <summary>
        /// In this example, the mock shouldn't work, as we didn't match the parameters.
        /// </summary>
        [Test]
        public void UsingMockingHijacker_MockSpecific_NoMatch()
        {
            // Arrange
            var expectedName = "The actual name";
            using var hijacker = new HijackRegister();
            var mockingHijacker = new MockingHijacker(typeof(Factory), hijacker);
            mockingHijacker.MockSpecific(nameof(Factory.CreateEntity), () => { return new Entity() { Name = "Whoops this wont match" }; });
            var factoryConsumer = new FactoryConsumer();

            // Act
            var entity = factoryConsumer.UseFactory(expectedName);

            // Assert
            Assert.That(entity.Name, Is.EqualTo(expectedName));
        }

        /// <summary>
        /// In this example, the mock SHOULD work, as we hope to match the signature. We are also using the
        /// values in the provided in the parameters.
        /// </summary>
        [Test]
        public void UsingMockingHijacker_MockSpecificWithSeveralParameters_UsingProvidedValues()
        {
            // Arrange
            const string namePrefix = "FAKE"; // Note that it must be const
            const string expectedName = "The actual name"; // Note that it must be const

            using var hijacker = new HijackRegister();
            var mockingHijacker = new MockingHijacker(typeof(Factory), hijacker);
            mockingHijacker.MockSpecific(nameof(Factory.CreateEntity), (string name, int _) => { return new Entity() { Name = namePrefix + expectedName }; });
            var factoryConsumer = new FactoryConsumer();

            // Act
            var entity = factoryConsumer.UseFactory("Random value that will not be used", 0);

            // Assert
            Assert.That(entity.Name, Is.EqualTo(namePrefix + expectedName));
        }

        [Test]
        public void ReplacingTheHijacker()
        {
            // Arrange
            const string firstExpectedName = "The first name"; // Note that it must be const
            const string secondExpectedName = "The second name"; // Note that it must be const
            var realEntityName = "I am real";

            var factoryConsumer = new FactoryConsumer();

            // First time mocking
            using (var hijacker = new HijackRegister())
            {
                var mockingHijacker = new MockingHijacker(typeof(Factory), hijacker);
                mockingHijacker.MockSpecific(nameof(Factory.CreateEntity), () => { return new Entity() { Name = firstExpectedName }; });

                var firstEntity = factoryConsumer.UseFactory();
                Assert.That(firstEntity.Name, Is.EqualTo(firstExpectedName));
            }

            // Ensure that the method is executing normally again
            var realEntity = factoryConsumer.UseFactory(realEntityName);
            Assert.That(realEntity.Name, Is.EqualTo(realEntityName));

            // Use a different mock
            using (var hijacker = new HijackRegister())
            {
                var mockingHijacker = new MockingHijacker(typeof(Factory), hijacker);
                mockingHijacker.MockSpecific(nameof(Factory.CreateEntity), () => { return new Entity() { Name = secondExpectedName }; });

                var secondEntity = factoryConsumer.UseFactory();
                Assert.That(secondEntity.Name, Is.EqualTo(secondExpectedName));
            }

        }
    }
}
