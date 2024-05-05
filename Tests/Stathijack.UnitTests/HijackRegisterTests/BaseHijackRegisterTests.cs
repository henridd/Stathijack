using Moq;
using Stathijack.Replacer;

namespace Stathijack.UnitTests.HijackRegisterTests
{
    internal abstract class BaseHijackRegisterTests
    {
        protected Mock<IMethodMatcher> MethodMatcherMock { get; private set; }
        protected Mock<ITypeMethodReplacer> MethodReplacerMock { get; private set; }

        protected HijackRegister CreateTestHijacker()
        {
            MethodMatcherMock = new Mock<IMethodMatcher>();
            MethodReplacerMock = new Mock<ITypeMethodReplacer>();
            var hijackerUnderTest = new HijackRegister(MethodMatcherMock.Object, MethodReplacerMock.Object);

            return hijackerUnderTest;
        }
    }
}
