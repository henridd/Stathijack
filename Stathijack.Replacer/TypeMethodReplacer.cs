﻿using System.Reflection;

namespace Stathijack.Replacer
{
    public class TypeMethodReplacer : ITypeMethodReplacer
    {
        /// <summary>
        /// Swaps the targetMethod handle for the hijackerMethod one
        /// 
        /// Known issue: will not work if the method has been called at least once before hijacking. No exception will be thrown, but the original method will be executed.
        /// </summary>
        public unsafe void Replace(MethodInfo targetMethod, MethodInfo hijackerMethod)
        {
            long* hijackerMethodPointer = (long*)hijackerMethod.MethodHandle.Value.ToPointer() + 1;
            long* targetMethodPointer = (long*)targetMethod.MethodHandle.Value.ToPointer() + 1;
            *targetMethodPointer = *hijackerMethodPointer;
        }
    }
}
