using Stathijack.Mocking;
using Stathijack.Samples.RealEntities;
using Stathijack.Samples.TestEntities;
using System.Data;

namespace Stathijack.Samples
{
    internal class FactoryUserTests
    {
        /// <summary>
        /// In this example, the behavior of the static class gets replaced by another one
        /// </summary>
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
        public void UsingMockingHijacker_MockMethod_NoMatch()
        {
            // Arrange
            var expectedName = "The actual name";
            using var mockingHijacker = new MockingHijacker(typeof(Factory));
            mockingHijacker.MockMethod(nameof(Factory.CreateEntityNonMatching), () => { return new Entity() { Name = "Whoops this wont match" }; });
            var factoryConsumer = new FactoryConsumer();

            // Act
            var entity = factoryConsumer.UseFactoryNonMatching(expectedName);

            // Assert
            Assert.That(entity.Name, Is.EqualTo(expectedName));
        }

        /// <summary>
        /// In this example, the mock SHOULD work, as we hope to match the signature. We are also using the
        /// values in the provided in the parameters.
        /// </summary>
        [Test]
        public void UsingMockingHijacker_MockMethodWithSeveralParameters_UsingProvidedValues()
        {
            // Arrange
            const string namePrefix = "FAKE"; // Note that it must be const
            const string expectedName = "The actual name"; // Note that it must be const

            using var mockingHijacker = new MockingHijacker(typeof(Factory));
            mockingHijacker.MockMethod(nameof(Factory.CreateEntity), (string name, int _) => { return new Entity() { Name = namePrefix + expectedName }; });
            var factoryConsumer = new FactoryConsumer();

            // Act
            var entity = factoryConsumer.UseFactory("Random value that will not be used", 0);

            // Assert
            Assert.That(entity.Name, Is.EqualTo(namePrefix + expectedName));
        }

        /// <summary>
        /// In this example we verify what were the invocations on the mocked method.
        /// </summary>
        [Test]
        public void VerifyingMethodInvocations()
        {
            // Arrange
            const string expectedName = "The actual name"; // Note that it must be const

            using var mockingHijacker = new MockingHijacker(typeof(Factory));
            var mockedMethodData = mockingHijacker.MockMethod(nameof(Factory.CreateEntity), (string name) => { return new Entity() { Name = name }; });
            var factoryConsumer = new FactoryConsumer();

            // Act
            var result = factoryConsumer.UseFactory(expectedName);

            // Assert
            Assert.That(mockedMethodData.Invocations.Count, Is.EqualTo(1));

            var invocationResult = (Entity)mockedMethodData.Invocations.First().ReturnValue!;
            Assert.That(invocationResult!.Name, Is.EqualTo(result.Name));

            var invocationParameters = mockedMethodData.Invocations.First().Parameters!;
            Assert.That(invocationParameters.First(), Is.EqualTo(expectedName));
        }

        /// <summary>
        /// In this example we dispose an existing mock, and then we replace it by another one
        /// </summary>
        [Test]
        public void ReplacingTheHijacker()
        {
            // Arrange
            const string firstExpectedName = "The first name"; // Note that it must be const
            const string secondExpectedName = "The second name"; // Note that it must be const

            var factoryConsumer = new FactoryConsumer();

            // First time mocking
            using (var mockingHijacker = new MockingHijacker(typeof(Factory)))
            {                
                mockingHijacker.MockMethod(nameof(Factory.CreateEntity), () => { return new Entity() { Name = firstExpectedName }; });

                var firstEntity = factoryConsumer.UseFactory();
                Assert.That(firstEntity.Name, Is.EqualTo(firstExpectedName));
            }

            // Use a different mock
            using (var mockingHijacker = new MockingHijacker(typeof(Factory)))
            {                
                mockingHijacker.MockMethod(nameof(Factory.CreateEntity), () => { return new Entity() { Name = secondExpectedName }; });

                var secondEntity = factoryConsumer.UseFactory();
                Assert.That(secondEntity.Name, Is.EqualTo(secondExpectedName));
            }

        }
    }
}
