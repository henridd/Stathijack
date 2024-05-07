using Stathijack.IntegrationTests.RealEntities;
using Stathijack.Mocking;

namespace Stathijack.IntegrationTests
{
    internal class MockSpecificTests
    {
        [Test]
        public void UsingMockingHijacker_MockSpecific_NoMatch()
        {
            // Arrange
            var expectedName = "The actual name";
            using var hijacker = new HijackRegister();
            var mockingHijacker = new MockingHijacker(typeof(Factory), hijacker);
            mockingHijacker.MockSpecific(nameof(Factory.CreateEntity), Array.Empty<Type>(), () => { return new Entity() { Name = "Whoops this wont match" }; });
            var factoryConsumer = new FactoryConsumer();

            // Act
            var entity = factoryConsumer.UseFactory(expectedName);

            // Assert
            Assert.That(entity.Name, Is.EqualTo(expectedName));
        }

        [Test]
        public void UsingMockingHijacker_MockSpecificWithNoParameters_SuccessfulMatch()
        {
            // Arrange
            const string expectedName = "The actual name"; // Note that it must be const
            using var hijacker = new HijackRegister();
            var mockingHijacker = new MockingHijacker(typeof(Factory), hijacker);
            mockingHijacker.MockSpecific(nameof(Factory.CreateEntity), null, () => { return new Entity() { Name = expectedName }; });
            var factoryConsumer = new FactoryConsumer();

            // Act
            var entity = factoryConsumer.UseFactory();

            // Assert
            Assert.That(entity.Name, Is.EqualTo(expectedName));
        }

        [Test]
        public void UsingMockingHijacker_MockSpecificWithSeveralParameters_SuccessfulMatch()
        {
            // Arrange
            const string expectedName = "The actual name"; // Note that it must be const
            const int expectedId = 10;
            using var hijacker = new HijackRegister();
            var mockingHijacker = new MockingHijacker(typeof(Factory), hijacker);
            mockingHijacker.MockSpecific(nameof(Factory.CreateEntity), [typeof(string), typeof(int)], (string _, int _) => { return new Entity() { Name = expectedName, Id = expectedId }; });
            var factoryConsumer = new FactoryConsumer();

            // Act
            var entity = factoryConsumer.UseFactory("Random value that will not be used", 0);

            // Assert
            Assert.That(entity.Name, Is.EqualTo(expectedName));
            Assert.That(entity.Id, Is.EqualTo(expectedId));
        }

        [Test]
        public void UsingMockingHijacker_MockSpecificWithSeveralParameters_UsingProvidedValues()
        {
            // Arrange
            const string namePrefix = "FAKE"; // Note that it must be const
            const string expectedName = "The actual name"; // Note that it must be const

            using var hijacker = new HijackRegister();
            var mockingHijacker = new MockingHijacker(typeof(Factory), hijacker);
            mockingHijacker.MockSpecific(nameof(Factory.CreateEntity), [typeof(string), typeof(int)], (string name, int _) => { return new Entity() { Name = namePrefix + expectedName }; });
            var factoryConsumer = new FactoryConsumer();

            // Act
            var entity = factoryConsumer.UseFactory("Random value that will not be used", 0);

            // Assert
            Assert.That(entity.Name, Is.EqualTo(namePrefix + expectedName));
        }
    }
}
