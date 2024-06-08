using Stathijack.IntegrationTests.RealEntities;
using Stathijack.Mocking;

namespace Stathijack.IntegrationTests
{
    internal class MockMethodTests
    {
        [Test]
        public void UsingMockingHijacker_MockMethod_NoMatch()
        {
            // Arrange
            var expectedName = "The actual name";
            using var hijacker = new HijackRegister();
            var mockingHijacker = new MockingHijacker(typeof(Factory), hijacker);
            mockingHijacker.MockMethod(nameof(Factory.CreateEntity), () => { return new Entity() { Name = "Whoops this wont match" }; });
            var factoryConsumer = new FactoryConsumer();

            // Act
            var entity = factoryConsumer.UseFactory(expectedName);

            // Assert
            Assert.That(entity.Name, Is.EqualTo(expectedName));
        }

        [Test]
        public void UsingMockingHijacker_MockMethodWithNoParameters_SuccessfulMatch()
        {
            // Arrange
            const string expectedName = "The actual name"; // Note that it must be const
            using var hijacker = new HijackRegister();
            var mockingHijacker = new MockingHijacker(typeof(Factory), hijacker);
            mockingHijacker.MockMethod(nameof(Factory.CreateEntity), () => { return new Entity() { Name = expectedName }; });
            var factoryConsumer = new FactoryConsumer();

            // Act
            var entity = factoryConsumer.UseFactory();

            // Assert
            Assert.That(entity.Name, Is.EqualTo(expectedName));
        }

        [Test]
        public void UsingMockingHijacker_MockMethodWithSeveralParameters_SuccessfulMatch()
        {
            // Arrange
            const string expectedName = "The actual name"; // Note that it must be const
            const int expectedId = 10;
            using var hijacker = new HijackRegister();
            var mockingHijacker = new MockingHijacker(typeof(Factory), hijacker);
            mockingHijacker.MockMethod(nameof(Factory.CreateEntity), (string _, int _) => { return new Entity() { Name = expectedName, Id = expectedId }; });
            var factoryConsumer = new FactoryConsumer();

            // Act
            var entity = factoryConsumer.UseFactory("Random value that will not be used", 0);

            // Assert
            Assert.That(entity.Name, Is.EqualTo(expectedName));
            Assert.That(entity.Id, Is.EqualTo(expectedId));
        }

        [Test]
        public void UsingMockingHijacker_MockMethodWithSeveralParameters_UsingProvidedValues()
        {
            // Arrange
            const string namePrefix = "FAKE"; // Note that it must be const
            const string expectedName = "The actual name"; // Note that it must be const

            using var hijacker = new HijackRegister();
            var mockingHijacker = new MockingHijacker(typeof(Factory), hijacker);
            mockingHijacker.MockMethod(nameof(Factory.CreateEntity), (string name, int _) => { return new Entity() { Name = namePrefix + expectedName }; });
            var factoryConsumer = new FactoryConsumer();

            // Act
            var entity = factoryConsumer.UseFactory("Random value that will not be used", 0);

            // Assert
            Assert.That(entity.Name, Is.EqualTo(namePrefix + expectedName));
        }
    }
}
