using System.Reflection;
using System.Runtime.CompilerServices;

namespace Stathijack.Replacer
{
    public static class TypeMethodReplacer
    {
        /// <summary>
        /// Swaps the targetMethod handle for the hijackerMethod one. Does NOT work
        /// </summary>
        public static unsafe void Replace(MethodInfo targetMethod, MethodInfo hijackerMethod)
        {
            //#if DEBUG
            RuntimeHelpers.PrepareMethod(targetMethod.MethodHandle);
            RuntimeHelpers.PrepareMethod(hijackerMethod.MethodHandle);
            //#endif

            IntPtr tar = targetMethod.MethodHandle.Value;
            if (!targetMethod.IsVirtual)
                tar += 8;
            else
            {
                var index = (int)(((*(long*)tar) >> 32) & 0xFF);
                var classStart = *(IntPtr*)(targetMethod.DeclaringType.TypeHandle.Value + (IntPtr.Size == 4 ? 40 : 64));
                tar = classStart + IntPtr.Size * index;
            }
            var inj = hijackerMethod.MethodHandle.Value + 8;
            Console.WriteLine($"Replacing {targetMethod.DeclaringType.FullName}.{targetMethod.Name} with {hijackerMethod.DeclaringType.FullName}.{hijackerMethod.Name}");
#if DEBUG
            tar = *(IntPtr*)tar + 1;
            inj = *(IntPtr*)inj + 1;

            *(int*)tar = *(int*)inj + (int)(long)inj - (int)(long)tar;
#else
            *(IntPtr*)tar = *(IntPtr*)inj;
#endif
        }
    }
}
