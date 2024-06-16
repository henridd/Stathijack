using Stathijack.IntegrationTests.RealEntities;
using Stathijack.Mocking;

namespace Stathijack.IntegrationTests
{
    public class InvocationsVerificationTests
    {
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(10)]
        public void ShouldIncrementNumberOfInvocations(int numberOfInvocations)
        {
            // Arrange
            using var mockingHijacker = new MockingHijacker(typeof(Factory));
            var hijackedMethodData = mockingHijacker.MockMethod(nameof(Factory.CreateEntity), () => { return new Entity() { Name = "Something we don't care about" }; });
            var factoryConsumer = new FactoryConsumer();

            // Act
            for (int i = 0; i < numberOfInvocations; i++)
                factoryConsumer.UseFactory();

            // Assert
            Assert.That(hijackedMethodData.Invocations.Count, Is.EqualTo(numberOfInvocations));
        }

        [Test]
        public void ShouldBeAbleToReadTheParameters()
        {
            // Arrange
            const string ExpectedName = "Green Mario";
            using var mockingHijacker = new MockingHijacker(typeof(Factory));
            var hijackedMethodData = mockingHijacker.MockMethod(nameof(Factory.CreateEntity), (string providedName) => { return new Entity() { Name = providedName }; });
            var factoryConsumer = new FactoryConsumer();

            // Act
            factoryConsumer.UseFactory(ExpectedName);

            // Assert
            var result = hijackedMethodData.Invocations.First();
            Assert.That(result.Parameters.Single().ToString(), Is.EqualTo(ExpectedName));
        }

        [Test]
        public void ShouldMatchTheResult()
        {
            // Arrange
            const string ExpectedName = "Zelda the Hero of Hyrule";
            using var mockingHijacker = new MockingHijacker(typeof(Factory));
            var hijackedMethodData = mockingHijacker.MockMethod(nameof(Factory.CreateEntity), () => { return new Entity() { Name = ExpectedName }; });
            var factoryConsumer = new FactoryConsumer();

            // Act
            var result = factoryConsumer.UseFactory();

            // Assert
            var invocationResult = hijackedMethodData.Invocations.First();
            Assert.That(result.Name, Is.EqualTo(((Entity)invocationResult.ReturnValue).Name));
        }
    }
}
