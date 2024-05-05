using System.Reflection;
using System.Runtime.CompilerServices;

namespace Stathijack.Replacer
{
    public static class TypeMethodReplacer
    {
        /// <summary>
        /// Swaps the targetMethod handle for the hijackerMethod one. Does NOT work
        /// </summary>
        public static unsafe MethodReplacementResult Replace(MethodInfo targetMethod, MethodInfo hijackerMethod)
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
            var result = new MethodReplacementResult();
#if DEBUG
            tar = *(IntPtr*)tar + 1;
            inj = *(IntPtr*)inj + 1;
            result.NewValue = tar;
            result.OriginalValue = new IntPtr(*(int*)tar);

            *(int*)tar = *(int*)inj + (int)(long)inj - (int)(long)tar;
#else
            result.NewValue = tar;
            result.OriginalValue = *(IntPtr*)tar;
            * (IntPtr*)tar = *(IntPtr*)inj;
#endif
            return result;
        }

        public static unsafe void RollbackReplacement(MethodReplacementResult result)
        {
#if DEBUG
            *(int*)result.NewValue = (int)result.OriginalValue;
#else
            *(IntPtr*)result.NewValue = result.OriginalValue;
#endif
        }
    }
}
