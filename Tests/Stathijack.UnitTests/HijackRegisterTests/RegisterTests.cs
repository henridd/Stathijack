using Moq;
using Stathijack.Exceptions;
using Stathijack.Wrappers;
using System.Reflection;

namespace Stathijack.UnitTests.HijackRegisterTests
{
    using HijackedMethodControllerImpl = Stathijack.HijackedMethodController;

    internal class RegisterTests : BaseHijackRegisterTests
    {
        [Test]
        public void TypesAreEqual_ShouldThrowException()
        {
            // Arrange
            var hijacker = CreateTestHijacker();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => hijacker.Register(typeof(FakeTypeA), typeof(FakeTypeA)));
        }

        [Test]
        public void NoInvokeMethod_ShouldThrowException()
        {
            // Arrange
            var hijacker = CreateTestHijacker();
            MockMethodReplacementMapping();

            var mockCloneClass = new Mock<IType>();
            DynamicTypeFactoryMock.Setup(x => x.GenerateMockTypeForMethod(It.IsAny<IMethodInfo>(), It.IsAny<IMethodInfo>())).Returns(mockCloneClass.Object);

            // Act & Assert
            var exception = Assert.Throws< MethodHijackingException>(() => hijacker.Register(typeof(FakeTypeA), typeof(FakeTypeB)));
            Assert.That(exception.Message, Is.EqualTo("Unable to find the Invoke method in the generated mock type"));
        }

        [Test]
        public void InvokeMethodHasNoDeclaringType_ShouldThrowException()
        {
            // Arrange
            var hijacker = CreateTestHijacker();
            MockMethodReplacementMapping();

            var mockCloneClass = new Mock<IType>();
            DynamicTypeFactoryMock.Setup(x => x.GenerateMockTypeForMethod(It.IsAny<IMethodInfo>(), It.IsAny<IMethodInfo>())).Returns(mockCloneClass.Object);

            var mockInvokeMethod = new Mock<IMethodInfo>();
            mockCloneClass.Setup(x => x.GetMethod(It.IsAny<string>(), It.IsAny<BindingFlags>())).Returns(mockInvokeMethod.Object);

            // Act & Assert
            var exception = Assert.Throws<MethodHijackingException>(() => hijacker.Register(typeof(FakeTypeA), typeof(FakeTypeB)));
            Assert.That(exception.Message, Is.EqualTo("The generated Invoke method has no declaring type"));
        }

        [Test]
        public void DeclaringTypeDoesNotHaveFullName_ShouldThrowException()
        {
            // Arrange
            var hijacker = CreateTestHijacker();
            MockMethodReplacementMapping();

            var mockCloneClass = new Mock<IType>();
            DynamicTypeFactoryMock.Setup(x => x.GenerateMockTypeForMethod(It.IsAny<IMethodInfo>(), It.IsAny<IMethodInfo>())).Returns(mockCloneClass.Object);

            var mockInvokeMethod = new Mock<IMethodInfo>();
            mockCloneClass.Setup(x => x.GetMethod(It.IsAny<string>(), It.IsAny<BindingFlags>())).Returns(mockInvokeMethod.Object);
            mockInvokeMethod.Setup(x => x.DeclaringType).Returns(mockCloneClass.Object);

            // Act & Assert
            var exception = Assert.Throws<MethodHijackingException>(() => hijacker.Register(typeof(FakeTypeA), typeof(FakeTypeB)));
            Assert.That(exception.Message, Is.EqualTo("The FullName of the generated mock type is null"));
        }

        [Test]
        public void ShouldReplaceType()
        {
            // Arrange
            var hijacker = CreateTestHijacker();
            MockMethodReplacementMapping();

            var mockCloneClass = new Mock<IType>();
            mockCloneClass.Setup(x => x.FullName).Returns("Mahatma Gandhi");
            DynamicTypeFactoryMock.Setup(x => x.GenerateMockTypeForMethod(It.IsAny<IMethodInfo>(), It.IsAny<IMethodInfo>())).Returns(mockCloneClass.Object);

            var mockInvokeMethod = new Mock<IMethodInfo>();
            mockCloneClass.Setup(x => x.GetMethod(It.IsAny<string>(), It.IsAny<BindingFlags>())).Returns(mockInvokeMethod.Object);
            mockInvokeMethod.Setup(x => x.DeclaringType).Returns(mockCloneClass.Object);

            // Act
            hijacker.Register(typeof(FakeTypeA), typeof(FakeTypeB));

            // Assert
            MethodReplacerMock.Verify(x => x.Replace(It.IsAny<MethodInfo>(), It.IsAny<MethodInfo>()));
        }

        [Test]
        public void NewHijack_ShouldAddNewHijackInTheHijackedMethodController()
        {
            // Arrange
            const string classFullName = "Albert Einstein";
            var hijacker = CreateTestHijacker();
            MockMethodReplacementMapping();

            var mockCloneClass = new Mock<IType>();
            mockCloneClass.Setup<string>(x => x.FullName).Returns(classFullName);
            DynamicTypeFactoryMock.Setup(x => x.GenerateMockTypeForMethod(It.IsAny<IMethodInfo>(), It.IsAny<IMethodInfo>())).Returns(mockCloneClass.Object);

            var mockInvokeMethod = new Mock<IMethodInfo>();
            mockCloneClass.Setup(x => x.GetMethod(It.IsAny<string>(), It.IsAny<BindingFlags>())).Returns(mockInvokeMethod.Object);
            mockInvokeMethod.Setup(x => x.DeclaringType).Returns(mockCloneClass.Object);

            // Act
            hijacker.Register(typeof(FakeTypeA), typeof(FakeTypeB));

            // Assert
            Assert.That(HijackedMethodControllerImpl.MethodHasBeenHijacked(classFullName));
        }

        [Test]
        public void ShouldReturnHijackedMethodDataWhenAdding()
        {
            // Arrange
            var hijacker = CreateTestHijacker();
            MockMethodReplacementMapping();

            var mockCloneClass = new Mock<IType>();
            mockCloneClass.Setup<string>(x => x.FullName).Returns("Terry Crews");
            DynamicTypeFactoryMock.Setup(x => x.GenerateMockTypeForMethod(It.IsAny<IMethodInfo>(), It.IsAny<IMethodInfo>())).Returns(mockCloneClass.Object);

            var mockInvokeMethod = new Mock<IMethodInfo>();
            mockCloneClass.Setup(x => x.GetMethod(It.IsAny<string>(), It.IsAny<BindingFlags>())).Returns(mockInvokeMethod.Object);
            mockInvokeMethod.Setup(x => x.DeclaringType).Returns(mockCloneClass.Object);

            // Act
            var hijackedMethodData = hijacker.Register(typeof(FakeTypeA), typeof(FakeTypeB));

            // Assert
            Assert.That(hijackedMethodData, Is.Not.Null);
        }        

        private void MockMethodReplacementMapping()
        {
            var mockMethodInfo = new Mock<IMethodInfo>();
            mockMethodInfo.Setup(x => x.MethodInfo).Returns((MethodInfo)null);
            var matchedMethods = new List<MethodReplacementMapping>()
            {
                new MethodReplacementMapping(mockMethodInfo.Object, mockMethodInfo.Object),
            };

            MethodMatcherMock.Setup(x => x.MatchMethods(typeof(FakeTypeA), typeof(FakeTypeB), It.IsAny<BindingFlags>())).Returns(matchedMethods);
        }
    }
}
