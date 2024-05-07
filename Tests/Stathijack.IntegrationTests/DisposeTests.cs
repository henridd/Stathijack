using Stathijack.IntegrationTests.RealEntities;
using Stathijack.Mocking;

namespace Stathijack.IntegrationTests
{
    internal class DisposeTests
    {
        [Test]
        public void ShouldBeAbleToReplaceTheHijackedMethod()
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
                mockingHijacker.MockSpecific(nameof(Factory.CreateEntity), new List<Type>() { typeof(string), typeof(int) }, (string name, int id) => { return new Entity() { Name = firstExpectedName }; });

                var firstEntity = factoryConsumer.UseFactory("Random value that will not be used", 1);
                Assert.That(firstEntity.Name, Is.EqualTo(firstExpectedName));
            }

            // Use a different mock
            using (var hijacker = new HijackRegister())
            {
                var mockingHijacker = new MockingHijacker(typeof(Factory), hijacker);
                mockingHijacker.MockSpecific(nameof(Factory.CreateEntity), new List<Type>() { typeof(string), typeof(int) }, (string name, int id) => { return new Entity() { Name = secondExpectedName }; });

                var secondEntity = factoryConsumer.UseFactory("Random value that will not be used", 1);
                Assert.That(secondEntity.Name, Is.EqualTo(secondExpectedName));
            }
        }

        [Test]
        public void ShouldBeAbleToRestoreTheOriginalBehavior()
        {
            // Arrange
            const string firstExpectedName = "The first name"; // Note that it must be const
            var realEntityName = "I am real";

            var factoryConsumer = new FactoryConsumer();

            // First time mocking
            using (var hijacker = new HijackRegister())
            {
                hijacker.EnableExperimentalDefaultInvoking = true;
                var mockingHijacker = new MockingHijacker(typeof(Factory), hijacker);
                mockingHijacker.MockSpecific(nameof(Factory.CreateEntity), new List<Type>() { typeof(string), typeof(int) }, (string name, int id) => { return new Entity() { Name = firstExpectedName }; });

                var firstEntity = factoryConsumer.UseFactory("Random value that will not be used", 1);
                Assert.That(firstEntity.Name, Is.EqualTo(firstExpectedName));
            }

            // Ensure the method is executing as normal again
            var realEntity = factoryConsumer.UseFactory(realEntityName);
            Assert.That(realEntity.Name, Is.EqualTo(realEntityName));
        }
    }
}
