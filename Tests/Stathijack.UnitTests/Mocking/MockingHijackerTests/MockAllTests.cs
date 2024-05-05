using Moq;
using Stathijack.Mocking;

namespace Stathijack.UnitTests.Mocking.MockingHijackerTests
{
    internal class MockAllTests
    {
        private Mock<IHijackRegister> _hijackRegisterMock;

        [Test]
        public void ShouldCreateMappingForEveryMethod()
        {
            // Arrange
            var mockingHijacker = CreateMockingHijacker();

            // Act
            mockingHijacker.MockAll(nameof(TypeWithSameMethods.SameMethod), () => { });

            // Assert
            _hijackRegisterMock.Verify(x => x.Register(It.Is<IEnumerable<MethodReplacementMapping>>(x => x.Count() == 2)));
        }

        private MockingHijacker CreateMockingHijacker()
        {
            _hijackRegisterMock = new Mock<IHijackRegister>();

            return new MockingHijacker(typeof(TypeWithSameMethods), _hijackRegisterMock.Object);
        }

        private static class TypeWithSameMethods
        {
            public static void SameMethod() { }
            public static void SameMethod(bool arg) { }
        }
    }
}
