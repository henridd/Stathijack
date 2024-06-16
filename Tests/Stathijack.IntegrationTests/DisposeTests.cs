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
                mockingHijacker.MockMethod(nameof(Factory.CreateEntity), (string name, int id) => { return new Entity() { Name = firstExpectedName }; });

                var firstEntity = factoryConsumer.UseFactory("Random value that will not be used", 1);
                Assert.That(firstEntity.Name, Is.EqualTo(firstExpectedName));
            }

            // Use a different mock
            using (var hijacker = new HijackRegister())
            {
                var mockingHijacker = new MockingHijacker(typeof(Factory), hijacker);
                mockingHijacker.MockMethod(nameof(Factory.CreateEntity),(string name, int id) => { return new Entity() { Name = secondExpectedName }; });

                var secondEntity = factoryConsumer.UseFactory("Random value that will not be used", 1);
                Assert.That(secondEntity.Name, Is.EqualTo(secondExpectedName));
            }
        }

        // TODO: The experimental feature is currently broken.
        //[Test]
        //public void ShouldBeAbleToRestoreTheOriginalBehavior()
        //{
        //    const string firstExpectedName = "The first name"; // Note that it must be const

        //    var factoryConsumer = new FactoryWithEnableExperimentalDefaultInvokingConsumer();

        //    // First time mocking
        //    using (var hijacker = new HijackRegister())
        //    {
        //        hijacker.EnableExperimentalDefaultInvoking = true;
        //        var mockingHijacker = new MockingHijacker(typeof(FactoryWithEnableExperimentalDefaultInvoking), hijacker);
        //        mockingHijacker.MockMethod(nameof(FactoryWithEnableExperimentalDefaultInvoking.CreateEntity), (string name) => { return new Entity() { Name = firstExpectedName }; });

        //        var firstEntity = factoryConsumer.UseFactory("Random value that will not be used");
        //        Assert.That(firstEntity.Name, Is.EqualTo(firstExpectedName));
        //    }

        //    // Ensure the method is executing as normal again
        //    var realEntityName = "I am real";
        //    var realEntity = factoryConsumer.UseFactory(realEntityName);
        //    Assert.That(realEntity.Name, Is.EqualTo(realEntityName));
        //}

        //private static class FactoryWithEnableExperimentalDefaultInvoking
        //{
        //    public static Entity CreateEntity(string name)
        //        => new Entity() { Name = name };
        //}

        //private class FactoryWithEnableExperimentalDefaultInvokingConsumer
        //{
        //    public Entity UseFactory(string name)
        //        => FactoryWithEnableExperimentalDefaultInvoking.CreateEntity(name);
        //}
    }
}
