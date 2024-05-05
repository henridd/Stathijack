using Moq;
using System.Reflection;

namespace Stathijack.UnitTests.HijackRegisterTests
{
    internal class RegisterTests : BaseHijackRegisterTests
    {
        [Test]
        public void ShouldReplaceEveryMatchedMethod()
        {
            // Arrange
            var hijacker = CreateTestHijacker();
            var matchedMethods = new List<MethodReplacementMapping>()
            {
                new MethodReplacementMapping(null, null),
                new MethodReplacementMapping(null, null),
            };

            MethodMatcherMock.Setup(x => x.MatchMethods(typeof(FakeTypeA), typeof(FakeTypeB), It.IsAny<BindingFlags>())).Returns(matchedMethods);

            // Act
            hijacker.Register(typeof(FakeTypeA), typeof(FakeTypeB));

            // Assert
            MethodReplacerMock.Verify(x => x.Replace(It.IsAny<MethodInfo>(), It.IsAny<MethodInfo>()), Times.Exactly(matchedMethods.Count));
        }

        [Test]
        public void TypesAreEqual_ShouldThrowException()
        {
            // Arrange
            var hijacker = CreateTestHijacker();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => hijacker.Register(typeof(FakeTypeA), typeof(FakeTypeA)));
        }
    }
}
