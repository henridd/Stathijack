using System.Reflection;

namespace Stathijack.Mocking
{
    public partial class MockingHijacker
    {
        /// <summary>
        /// Hijacks every method that matches the methodName in the target type.
        /// </summary>
        /// <param name="methodName">The name of the method</param>
        /// <param name="action">The action to replace the original method</param>
        public void MockAll(string methodName, Action action)
            => DoMockAll(methodName, action.Method);

        public void MockAll<T>(string methodName, Func<T> function)
            => DoMockAll(methodName, function.Method);

        public void MockAll<TIn, TOut>(string methodName, Func<TIn, TOut> function)
            => DoMockAll(methodName, function.Method);

        public void MockAll<T1, T2, TOut>(string methodName, Func<T1, T2, TOut> function)
            => DoMockAll(methodName, function.Method);

        public void MockAll<T1, T2, T3, TOut>(string methodName, Func<T1, T2, T3, TOut> function)
            => DoMockAll(methodName, function.Method);

        public void MockAll<T1, T2, T3, T4, TOut>(string methodName, Func<T1, T2, T3, T4, TOut> function)
            => DoMockAll(methodName, function.Method);

        public void MockAll<T1, T2, T3, T4, T5, TOut>(string methodName, Func<T1, T2, T3, T4, T5, TOut> function)
            => DoMockAll(methodName, function.Method);

        private void DoMockAll(string methodName, MethodInfo method)
        {
            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentException("Method name cannot be null", nameof(methodName));

            if (method == null)
                throw new ArgumentNullException("You must provide a method to replace the original method", nameof(method));

            var mappings = CreateMappingForAllMethods(methodName, method);
            _hijackRegister.Register(mappings);
        }
    }
}
