using Stathijack.Sample.RealEntities;
using Stathijack.Sample.TestEntities;

namespace Stathijack.Sample
{
    internal class FactoryUserTests
    {
        [Test]
        public void test()
        {
            // Arrange
            ConfigureHijacker();
            var factoryUser = new FactoryConsumer();

            // Act
            var entity = factoryUser.UseFactory();

            // Assert
            Assert.That(entity.Name, Is.EqualTo("Fake"));
        }

        private static void ConfigureHijacker()
        {
            var hijacker = new HijackRegister();
            hijacker.Register(typeof(Factory), typeof(MockFactory));
        }
    }
}
