﻿using System.Reflection;

namespace Stathijack
{
    internal static class HijackedMethodController
    {
        private static Dictionary<string, MethodHijackInfo> _hijackedMethods = new();

        // This method must be public.
        public static object? Invoke(object?[]? parameters)
        {
            var dynamicNamespaceFullName = (new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().DeclaringType.FullName;

            var methodExecutionInfo = _hijackedMethods[dynamicNamespaceFullName].GetMethodToInvoke();

            return methodExecutionInfo.Method.Invoke(methodExecutionInfo.Target, parameters);
        }

        internal static bool MethodHasBeenHijacked(string dynamicNamespaceFullName)
            => _hijackedMethods.ContainsKey(dynamicNamespaceFullName);

        internal static MethodInfo GetRootMethodInfo()
        {
            return typeof(HijackedMethodController).GetMethod(nameof(Invoke), BindingFlags.Public | BindingFlags.Static);
        }

        /// <summary>
        /// Adds a new hijack, providing a clone of the original method.
        /// </summary>
        internal static void AddNewHijack(MethodInfo targetMethodClone, MethodInfo hijackerMethod, object? target, string dynamicNamespaceFullName)
        {
            if (!_hijackedMethods.ContainsKey(dynamicNamespaceFullName))
            {
                var hijackInfo = new MethodHijackInfo(new MethodExecutionInfo(target, targetMethodClone));

                _hijackedMethods.Add(dynamicNamespaceFullName, hijackInfo);
            }
            _hijackedMethods[dynamicNamespaceFullName].AddHijack(new MethodExecutionInfo(target, hijackerMethod));
        }

        /// <summary>
        /// Adds a new hijack without providing a clone of the original method. Will throw an exception
        /// if the method is called after the hijack has been removed.
        /// </summary>
        internal static void AddNewHijack(MethodInfo hijackerMethod, object? target, string dynamicNamespaceFullName)
        {
            if (!_hijackedMethods.ContainsKey(dynamicNamespaceFullName))
            {
                _hijackedMethods.Add(dynamicNamespaceFullName, new MethodHijackInfo());
            }
            _hijackedMethods[dynamicNamespaceFullName].AddHijack(new MethodExecutionInfo(target, hijackerMethod));
        }

        internal static void AppendHijack(MethodInfo hijackerMethod, object? target, string dynamicNamespaceFullName)
        {
            _hijackedMethods[dynamicNamespaceFullName].AddHijack(new MethodExecutionInfo(target, hijackerMethod));
        }

        internal static void RemoveHijack(string item)
        {
            if (_hijackedMethods.ContainsKey(item))
            {
                _hijackedMethods[item].ClearHijackStack();
            }
        }

        private class MethodHijackInfo
        {
            private readonly MethodExecutionInfo? _targetMethodClone;
            private readonly Stack<MethodExecutionInfo> _hijackStack = new Stack<MethodExecutionInfo>();

            public MethodHijackInfo() { }

            public MethodHijackInfo(MethodExecutionInfo targetMethodClone)
            {
                _targetMethodClone = targetMethodClone;
            }

            public void AddHijack(MethodExecutionInfo methodExecutionInfo)
                => _hijackStack.Push(methodExecutionInfo);

            public MethodExecutionInfo GetMethodToInvoke()
            {
                if (_hijackStack.Any())
                {
                    return _hijackStack.Peek();
                }

                if (_targetMethodClone.HasValue)
                {
                    return _targetMethodClone.Value;
                }

                throw new InvalidOperationException("No behavior defined for this method.");
            }

            public void ClearHijackStack()
                => _hijackStack.Clear();
        }

        private struct MethodExecutionInfo
        {
            public object? Target { get; }
            public MethodInfo Method { get; }

            public MethodExecutionInfo(object? target, MethodInfo method)
            {
                Target = target;
                Method = method;
            }
        }
    }
}