using Stathijack.Replacer;
using System.Reflection;

namespace Stathijack.UnitTests
{
    internal static class FakeTypeMethodReplacer
    {
        public static MethodReplacementResult Replace(MethodInfo targetMethod, MethodInfo hijackerMethod)
        {
            var random = new Random();

            return new MethodReplacementResult()
            {
                OriginalValue = targetMethod?.MethodHandle.Value ?? random.Next(),
                NewValue = hijackerMethod?.MethodHandle.Value ?? random.Next()
            };
        }
    }
}
