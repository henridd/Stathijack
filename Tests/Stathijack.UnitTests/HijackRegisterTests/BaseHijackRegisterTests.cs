using Moq;
using Stathijack.Dynamic;
using Stathijack.Replacer;

namespace Stathijack.UnitTests.HijackRegisterTests
{
    internal abstract class BaseHijackRegisterTests
    {
        protected Mock<IMethodMatcher> MethodMatcherMock { get; private set; }
        protected Mock<ITypeMethodReplacer> MethodReplacerMock { get; private set; }
        protected Mock<IDynamicTypeFactory> DynamicTypeFactoryMock { get; private set; }

        protected HijackRegister CreateTestHijacker()
        {
            MethodMatcherMock = new Mock<IMethodMatcher>();
            MethodReplacerMock = new Mock<ITypeMethodReplacer>();
            DynamicTypeFactoryMock = new Mock<IDynamicTypeFactory>();
            var hijackerUnderTest = new HijackRegister(MethodMatcherMock.Object, MethodReplacerMock.Object, DynamicTypeFactoryMock.Object);

            return hijackerUnderTest;
        }
    }
}
