using Stathijack.IntegrationTests.RealEntities;

namespace Stathijack.IntegrationTests
{
    internal class FakeStaticClassTests
    {
        [Test]
        public void ShouldCallMethodsInTheHijackedClass()
        {
            // Arrange
            using var hijacker = new HijackRegister();
            hijacker.Register(typeof(Factory), typeof(MockFactory));
            var factoryConsumer = new FactoryConsumer();

            // Act
            var entity = factoryConsumer.UseFactory();

            // Assert
            Assert.That(entity.Name, Is.EqualTo(MockFactory.DefaultName));
        }

        [Test]
        public void ShouldCallTheOriginalMethodIfNoMatchingOverloadIsFound()
        {
            // Arrange
            const string expectedName = "Test";
            using var hijacker = new HijackRegister();
            hijacker.Register(typeof(Factory), typeof(MockFactory));
            var factoryConsumer = new FactoryConsumer();

            // Act
            var entity = factoryConsumer.UseFactory(expectedName);

            // Assert
            Assert.That(entity.Name, Is.EqualTo(expectedName));
        }

        [Test]
        public void ShouldBeAbleToReplaceHijacker()
        {
            // First mock
            using (var hijacker = new HijackRegister())
            {
                hijacker.Register(typeof(Factory), typeof(MockFactory));
                var factoryConsumer = new FactoryConsumer();
                var entity = factoryConsumer.UseFactory();
                Assert.That(entity.Name, Is.EqualTo(MockFactory.DefaultName));
            }

            //Second mock
            using (var hijacker = new HijackRegister())
            {
                hijacker.Register(typeof(Factory), typeof(MockFactoryB));
                var factoryConsumer = new FactoryConsumer();
                var entity = factoryConsumer.UseFactory();
                Assert.That(entity.Name, Is.EqualTo(MockFactoryB.DefaultName));
            }
        }

        private class MockFactory
        {
            public const string DefaultName = "Fake";

            public static Entity CreateEntity()
                => new Entity() { Name = DefaultName };
        }

        private class MockFactoryB
        {
            public const string DefaultName = "Another Fake";

            public static Entity CreateEntity()
                => new Entity() { Name = DefaultName };
        }
    }
}
