using Stathijack.Mocking;
using Stathijack.Samples.RealEntities;
using Stathijack.Samples.TestEntities;

namespace Stathijack.Samples
{
    internal class FactoryUserTests
    {
        [Test]
        public void UsingAFakeClass()
        {
            // Arrange
            var hijacker = new HijackRegister();
            hijacker.Register(typeof(Factory), typeof(MockFactory));
            var factoryConsumer = new FactoryConsumer();

            // Act
            var entity = factoryConsumer.UseFactory();

            // Assert
            Assert.That(entity.Name, Is.EqualTo("Fake"));
        }

        [Test]
        public void UsingMockingHijacker_MockAll()
        {
            // Arrange
            const string fakeEntityName = "Fake"; // Note that it must be const
            var hijacker = new HijackRegister();
            var mockingHijacker = new MockingHijacker(typeof(Factory), hijacker);
            mockingHijacker.MockAll(nameof(Factory.CreateEntity), () => { return new Entity() { Name = fakeEntityName }; }); 
            var factoryConsumer = new FactoryConsumer();

            // Act
            var entity = factoryConsumer.UseFactory();

            // Assert
            Assert.That(entity.Name, Is.EqualTo(fakeEntityName));
        }

        /// <summary>
        /// In this example, the mock shouldn't work, as we didn't match the parameters.
        /// </summary>
        [Test]
        public void UsingMockingHijacker_MockSpecific_NoMatch()
        {
            // Arrange
            var expectedName = "The actual name";
            var hijacker = new HijackRegister();
            var mockingHijacker = new MockingHijacker(typeof(Factory), hijacker);
            mockingHijacker.MockSpecific(nameof(Factory.CreateEntity), Array.Empty<Type>(), () => { return new Entity() { Name = "Whoops this wont match" }; });
            var factoryConsumer = new FactoryConsumer();

            // Act
            var entity = factoryConsumer.UseFactory(expectedName);

            // Assert
            Assert.That(entity.Name, Is.EqualTo(expectedName));
        }

        /// <summary>
        /// In this example, the mock SHOULD work, as we hope to match the signature
        /// </summary>
        [Test]
        public void UsingMockingHijacker_MockSpecific_SuccessfulMatch()
        {
            // Arrange
            const string expectedName = "The actual name"; // Note that it must be const
            var hijacker = new HijackRegister();
            var mockingHijacker = new MockingHijacker(typeof(Factory), hijacker);
            mockingHijacker.MockSpecific(nameof(Factory.CreateEntity), [typeof(string)], () => { return new Entity() { Name = expectedName }; });
            var factoryConsumer = new FactoryConsumer();

            // Act
            var entity = factoryConsumer.UseFactory("Random value that will not be used");

            // Assert
            Assert.That(entity.Name, Is.EqualTo(expectedName));
        }
    }
}
