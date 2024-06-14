namespace Stathijack.UnitTests.HijackedMethodController
{
    using Unit = Stathijack.HijackedMethodController;

    public class GetRootMethodInfoTests
    {
        [Test]
        public void ShouldReturnMethodInfo()
        {
            // Act
            var methodInfo = Unit.GetRootMethodInfo();

            // Assert
            Assert.That(methodInfo, Is.Not.Null);
        }
    }
}
