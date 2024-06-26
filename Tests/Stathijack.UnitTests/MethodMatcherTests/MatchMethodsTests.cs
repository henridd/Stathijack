﻿using System.Reflection;

namespace Stathijack.UnitTests.MethodMatcherTests
{
    internal class MatchMethodsTests
    {
        [Test]
        public void ShouldMatchAllMethodsWithTheSameNameAndParameters()
        {
            // Arrange
            var matcher = new MethodMatcher();

            // Act
            var matches = matcher.MatchMethods(typeof(TypeWithSameMethodsA), typeof(TypeWithSameMethodsB), BindingFlags.Public | BindingFlags.Static);

            // Assert
            // We expect to match the two signatures of SameMethodA
            Assert.That(matches.Count, Is.EqualTo(2));

            //// Verify we got SameMethodA()
            Assert.That(matches.Any(x 
                => x.HijackerMethod.Name == nameof(TypeWithSameMethodsA.SameMethodA) &&
                x.TargetMethod.Name == nameof(TypeWithSameMethodsB.SameMethodA) &&
                x.HijackerMethod.GetParameters().Length == 0 &&
                x.TargetMethod.GetParameters().Length == 0));

            //// Verify we got SameMethodA(bool arg)
            Assert.That(matches.Any(x 
                => x.HijackerMethod.Name == nameof(TypeWithSameMethodsA.SameMethodA) &&
                x.TargetMethod.Name == nameof(TypeWithSameMethodsB.SameMethodA) &&
                x.HijackerMethod.GetParameters().Length == 1 &&
                x.TargetMethod.GetParameters().Length == 1));
        }

        [Test]
        public void DifferentReturnType_ShouldNotMatch()
        {
            // Arrange
            var matcher = new MethodMatcher();

            // Act
            var matches = matcher.MatchMethods(typeof(TypeWithSameMethodsButDifferentReturnTypeA), typeof(TypeWithSameMethodsButDifferentReturnTypeB), BindingFlags.Public | BindingFlags.Static);

            // Assert
            Assert.That(matches.Count, Is.EqualTo(0));
        }

        [Test]
        public void DifferentNumberOfParameters_ShouldNotMatch()
        {
            // Arrange
            var matcher = new MethodMatcher();

            // Act
            var matches = matcher.MatchMethods(typeof(TypeWithSameMethodsButDifferentNumberOfParametersA), typeof(TypeWithSameMethodsButDifferentNumberOfParametersB), BindingFlags.Public | BindingFlags.Static);

            // Assert
            Assert.That(matches.Count, Is.EqualTo(0));
        }

        [Test]
        public void DifferentTypeOfParameters_ShouldNotMatch()
        {
            // Arrange
            var matcher = new MethodMatcher();

            // Act
            var matches = matcher.MatchMethods(typeof(TypeWithSameMethodsButDifferentParameterTypeA), typeof(TypeWithSameMethodsButDifferentParameterTypeB), BindingFlags.Public | BindingFlags.Static);

            // Assert
            Assert.That(matches.Count, Is.EqualTo(0));
        }

        [Test]
        public void TypeIsNull_ShouldThrowException()
        {
            // Arrange
            var matcher = new MethodMatcher();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => matcher.MatchMethods(null!, typeof(TypeWithSameMethodsButDifferentReturnTypeB), BindingFlags.Public | BindingFlags.Static));
        }

        [Test]
        public void HijackerIsNull_ShouldThrowException()
        {
            // Arrange
            var matcher = new MethodMatcher();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => matcher.MatchMethods(typeof(TypeWithSameMethodsButDifferentReturnTypeB), null!, BindingFlags.Public | BindingFlags.Static));
        }

        private static class TypeWithSameMethodsA
        {
            public static void SameMethodA() { }
            public static void SameMethodA(bool arg) { }
        }

        private static class TypeWithSameMethodsB
        {
            public static void SameMethodA() { }
            public static void SameMethodA(bool arg) { }
        }

        private static class TypeWithSameMethodsButDifferentReturnTypeA
        {
            public static string DifferentMethod(bool arg) { return "1"; }
        }

        private static class TypeWithSameMethodsButDifferentReturnTypeB
        {
            public static int DifferentMethod(bool arg) { return 1; }
        }

        private static class TypeWithSameMethodsButDifferentNumberOfParametersA
        {
            public static int DifferentMethod(bool arg) { return 1; }
        }

        private static class TypeWithSameMethodsButDifferentNumberOfParametersB
        {
            public static int DifferentMethod() { return 1; }
        }

        private static class TypeWithSameMethodsButDifferentParameterTypeA
        {
            public static int DifferentMethod(bool arg) { return 1; }
        }

        private static class TypeWithSameMethodsButDifferentParameterTypeB
        {
            public static int DifferentMethod(int arg) { return 1; }
        }
    }
}
