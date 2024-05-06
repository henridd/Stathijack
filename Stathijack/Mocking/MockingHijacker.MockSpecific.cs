using System.Reflection;

namespace Stathijack.Mocking
{
    public partial class MockingHijacker
    {
        public void MockSpecific(string methodName, IEnumerable<Type> parameterTypes, Action action)
            => DoMockSpecific(methodName, parameterTypes, action.Method);

        public void MockSpecific<T>(string methodName, IEnumerable<Type> parameterTypes, Func<T> function)
            => DoMockSpecific(methodName, parameterTypes, function.Method);

        public void MockSpecific<T, TOut>(string methodName, IEnumerable<Type> parameterTypes, Func<T, TOut> function)
            => DoMockSpecific(methodName, parameterTypes, function.Method);

        public void MockSpecific<T1, T2, TOut>(string methodName, IEnumerable<Type> parameterTypes, Func<T1, T2, TOut> function)
            => DoMockSpecific(methodName, parameterTypes, function.Method);

        public void MockSpecific<T1, T2, T3, TOut>(string methodName, IEnumerable<Type> parameterTypes, Func<T1, T2, T3, TOut> function)
            => DoMockSpecific(methodName, parameterTypes, function.Method);

        public void MockSpecific<T1, T2, T3, T4, TOut>(string methodName, IEnumerable<Type> parameterTypes, Func<T1, T2, T3, T4, TOut> function)
            => DoMockSpecific(methodName, parameterTypes, function.Method);

        public void MockSpecific<T1, T2, T3, T4, T5, TOut>(string methodName, IEnumerable<Type> parameterTypes, Func<T1, T2, T3, T4, T5, TOut> function)
            => DoMockSpecific(methodName, parameterTypes, function.Method);

        private void DoMockSpecific(string methodName, IEnumerable<Type> parameterTypes, MethodInfo method)
        {
            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentException("Method name cannot be null", nameof(methodName));

            if (method == null)
                throw new ArgumentNullException("You must provide a method to replace the original method", nameof(method));

            parameterTypes ??= Array.Empty<Type>();

            var mappings = CreateMappingForAllMethods(methodName, parameterTypes.ToArray(), method);
            _hijackRegister.Register(mappings);
        }
    }
}
