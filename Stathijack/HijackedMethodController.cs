using Stathijack.Exceptions;
using System.Reflection;

namespace Stathijack
{
    internal static class HijackedMethodController
    {
        private static Dictionary<string, MethodHijackInfo> _hijackedMethods = new();

        // This method must be public.
        public static object? Invoke(object?[]? parameters)
        {
            var stackTrace = new System.Diagnostics.StackTrace();
            var firstFrame = stackTrace.GetFrame(1);
            if (firstFrame == null)
            {
                throw new HijackedMethodInvocationException("Unable to identify the hijacked type when invoking");
            }

            var targetMethodInfo = firstFrame.GetMethod();
            if (targetMethodInfo == null)
            {
                throw new HijackedMethodInvocationException("Unable to get information about the calling method. Analyzed stack frame: " + firstFrame.ToString());
            }

            var declaringType = targetMethodInfo.DeclaringType;
            if (declaringType == null)
            {
                throw new HijackedMethodInvocationException("The calling method does not have a declared type. Method: " + targetMethodInfo.Name);
            }

            var dynamicNamespaceFullName = declaringType.FullName;
            if (dynamicNamespaceFullName == null)
            {
                throw new HijackedMethodInvocationException("Unable to obtain the full name of the declaring type. Type: " + declaringType.Name);
            }

            var methodExecutionInfo = _hijackedMethods[dynamicNamespaceFullName].GetMethodToInvoke();

            var returnValue = methodExecutionInfo.Method.Invoke(methodExecutionInfo.Target, parameters);

            RegisterInvocation(parameters, returnValue, dynamicNamespaceFullName);

            return returnValue;
        }

        private static void RegisterInvocation(object?[]? parameters, object? returnValue, string dynamicNamespaceFullName)
        {
            var methodInvocationResult = new MethodInvocationResult(parameters, returnValue);
            _hijackedMethods[dynamicNamespaceFullName].HijackedMethodData.RegisterInvocation(methodInvocationResult);
        }

        internal static bool MethodHasBeenHijacked(string dynamicNamespaceFullName)
            => _hijackedMethods.ContainsKey(dynamicNamespaceFullName);

        internal static MethodInfo GetRootMethodInfo()
        {
            return typeof(HijackedMethodController).GetMethod(nameof(Invoke), BindingFlags.Public | BindingFlags.Static)!;
        }

        /// <summary>
        /// Adds a new hijack without providing a clone of the original method. Will throw an exception
        /// if the method is called after the hijack has been removed.
        /// </summary>
        internal static void AddNewHijack(MethodInfo hijackerMethod, object? target, string dynamicNamespaceFullName, HijackedMethodData hijackedMethodData) 
            => DoAddNewHijack(new MethodHijackInfo(hijackedMethodData), hijackerMethod, target, dynamicNamespaceFullName);

        /// <summary>
        /// Adds a new hijack, providing a clone of the original method.
        /// </summary>
        internal static void AddNewHijack(MethodInfo targetMethodClone, MethodInfo hijackerMethod, object? target, string dynamicNamespaceFullName, HijackedMethodData hijackedMethodData) 
            => DoAddNewHijack(new MethodHijackInfo(new MethodExecutionInfo(target, targetMethodClone), hijackedMethodData), hijackerMethod, target, dynamicNamespaceFullName);

        internal static void AppendHijack(MethodInfo hijackerMethod, object? target, string dynamicNamespaceFullName)
        {
            _hijackedMethods[dynamicNamespaceFullName].AddHijack(new MethodExecutionInfo(target, hijackerMethod));
        }

        internal static void RemoveHijack(string item)
        {
            if (_hijackedMethods.ContainsKey(item))
            {
                _hijackedMethods[item].ClearHijackStack();
                _hijackedMethods[item].HijackedMethodData.ResetInvocations();
            }
        }

        internal static HijackedMethodData GetHijackedMethodData(string dynamicNamespaceFullName)
            => _hijackedMethods[dynamicNamespaceFullName].HijackedMethodData;

        private static void DoAddNewHijack(MethodHijackInfo methodHijackInfo, MethodInfo hijackerMethod, object? target, string dynamicNamespaceFullName)
        {
            if (!_hijackedMethods.ContainsKey(dynamicNamespaceFullName))
            {
                _hijackedMethods.Add(dynamicNamespaceFullName, methodHijackInfo);
            }

            _hijackedMethods[dynamicNamespaceFullName].AddHijack(new MethodExecutionInfo(target, hijackerMethod));
        }        

        private class MethodHijackInfo
        {
            private readonly MethodExecutionInfo? _targetMethodClone;
            private readonly Stack<MethodExecutionInfo> _hijackStack = new Stack<MethodExecutionInfo>();

            public HijackedMethodData HijackedMethodData { get; }

            public MethodHijackInfo(HijackedMethodData hijackedMethodData)
            {
                HijackedMethodData = hijackedMethodData;
            }

            public MethodHijackInfo(MethodExecutionInfo targetMethodClone, HijackedMethodData hijackedMethodData)
            {
                _targetMethodClone = targetMethodClone;
                HijackedMethodData = hijackedMethodData;
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