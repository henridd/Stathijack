using System.Reflection;

namespace Stathijack.Mocking
{
    public partial class MockingHijacker
    {
        /// <summary>
        /// Hijacks the method that matches the methodName and the parameterTypes.
        /// </summary>
        /// <param name="methodName">The name of the method</param>
        /// <param name="action">The action to replace the original method</param>
        public void MockSpecific(string methodName, Action action)
            => DoMockSpecific(methodName, action.Method, action.Target);

        public void MockSpecific<T>(string methodName, Func<T> function)
            => DoMockSpecific(methodName, function.Method, function.Target);

        public void MockSpecific<T, TOut>(string methodName, Func<T, TOut> function)
            => DoMockSpecific(methodName, function.Method, function.Target);

        public void MockSpecific<T1, T2, TOut>(string methodName, Func<T1, T2, TOut> function)
            => DoMockSpecific(methodName, function.Method, function.Target);

        public void MockSpecific<T1, T2, T3, TOut>(string methodName, Func<T1, T2, T3, TOut> function)
            => DoMockSpecific(methodName, function.Method, function.Target);

        public void MockSpecific<T1, T2, T3, T4, TOut>(string methodName, Func<T1, T2, T3, T4, TOut> function)
            => DoMockSpecific(methodName, function.Method, function.Target);

        public void MockSpecific<T1, T2, T3, T4, T5, TOut>(string methodName, Func<T1, T2, T3, T4, T5, TOut> function)
            => DoMockSpecific(methodName, function.Method, function.Target);

        private void DoMockSpecific(string methodName, MethodInfo method, object target)
        {
            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentException("Method name cannot be null", nameof(methodName));

            if (method == null)
                throw new ArgumentNullException("You must provide a method to replace the original method", nameof(method));

            var mappings = CreateMappingForAllMethods(methodName, method.GetParameters().Select(x => x.ParameterType).ToArray(), method);
            _hijackRegister.Register(mappings, target);
        }
    }
}
