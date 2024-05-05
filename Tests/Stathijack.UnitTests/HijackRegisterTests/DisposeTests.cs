using Moq;
using Stathijack.Replacer;
using System.Reflection;

namespace Stathijack.UnitTests.HijackRegisterTests
{
    internal class DisposeTests : BaseHijackRegisterTests
    {
        [Test]
        public void ShouldRollbackAllHijacks()
        {
            // Arrange
            var hijacker = CreateTestHijacker();
            var matchedMethods = new List<MethodReplacementMapping>()
            {
                new MethodReplacementMapping(null, null),
                new MethodReplacementMapping(null, null),
            };

            MethodMatcherMock.Setup(x => x.MatchMethods(typeof(FakeTypeA), typeof(FakeTypeB), It.IsAny<BindingFlags>())).Returns(matchedMethods);
            hijacker.Register(typeof(FakeTypeA), typeof(FakeTypeB));

            // Act
            hijacker.Dispose();

            // Assert
            MethodReplacerMock.Verify(x => x.RollbackReplacement(It.IsAny<MethodReplacementResult>()), Times.Exactly(matchedMethods.Count));
        }
    }
}
