using Stathijack.IntegrationTests.RealEntities;
using Stathijack.Mocking;

namespace Stathijack.IntegrationTests
{
    internal class MockMethodTests
    {
        [Test]
        public void ParametersDoNotMatch_ShouldNotReplace()
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
        public void ParametersMatch_ShouldReplace()
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
        public void WhenUsingConstructorWithoutHijackRegister_ParametersMatch_ShouldReplace()
        {
            // Arrange
            const string expectedName = "The actual name"; // Note that it must be const
            using var mockingHijacker = new MockingHijacker(typeof(Factory));
            mockingHijacker.MockMethod(nameof(Factory.CreateEntity), () => { return new Entity() { Name = expectedName }; });
            var factoryConsumer = new FactoryConsumer();

            // Act
            var entity = factoryConsumer.UseFactory();

            // Assert
            Assert.That(entity.Name, Is.EqualTo(expectedName));
        }

        [Test]
        public void ShouldNotGoBananasWhenCallingDisposeTwice()
        {
            const string expectedName = "Random value"; // Note that it must be const

            Assert.DoesNotThrow(() =>
            {
                using var hijacker = new HijackRegister();
                using var mockingHijacker = new MockingHijacker(typeof(Factory), hijacker);
                mockingHijacker.MockMethod(nameof(Factory.CreateEntity), () => { return new Entity() { Name = expectedName }; });
                var factoryConsumer = new FactoryConsumer();
                var entity = factoryConsumer.UseFactory();
            });
        }

        [Test]
        public void ParametersMatch_SeveralParameters_ShouldReplace()
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
        public void ParameterValuesShouldBeAccessibleInTheFunc()
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

        [Test]
        public void ShouldWorkWithStructs()
        {
            // Arrange
            const string namePrefix = "FAKE"; // Note that it must be const
            const string expectedName = "The actual name"; // Note that it must be const

            using var hijacker = new HijackRegister();
            var mockingHijacker = new MockingHijacker(typeof(Factory), hijacker);
            mockingHijacker.MockMethod(nameof(Factory.CreateEntity), (CreateFactoryPayload payload) => { return new Entity() { Name = namePrefix + payload.Name }; });
            var factoryConsumer = new FactoryConsumer();

            // Act
            var entity = factoryConsumer.UseFactory(new CreateFactoryPayload(1, expectedName));

            // Assert
            Assert.That(entity.Name, Is.EqualTo(namePrefix + expectedName));
        }

        private class FakeClass
        {
            public string? Name { get; set; }
            public int SomeValue { get; set; }
        }
    }
}
