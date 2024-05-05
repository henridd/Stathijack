using Stathijack.Mocking;
using Stathijack.Sample.RealEntities;
using Stathijack.Sample.TestEntities;

namespace Stathijack.Sample
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
        public void UsingMockingHijacker()
        {
            // Arrange
            var fakeEntityName = "Fake";
            var hijacker = new HijackRegister();
            var mockingHijacker = new MockingHijacker(typeof(Factory), hijacker);
            mockingHijacker.MockAll(nameof(Factory.CreateEntity), () => { return new Entity() { Name = fakeEntityName }; });
            var factoryConsumer = new FactoryConsumer();

            // Act
            var entity = factoryConsumer.UseFactory();

            // Assert
            Assert.That(entity.Name, Is.EqualTo(fakeEntityName));
        }
    }
}
