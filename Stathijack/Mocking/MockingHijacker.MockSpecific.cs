namespace Stathijack.Mocking
{
    public partial class MockingHijacker
    {
        public void MockSpecific(string methodName, IEnumerable<Type> parameterTypes, Action action)
        {
            var mappings = CreateMappingForAllMethods(methodName, parameterTypes.ToArray(), action.Method);
            _hijackRegister.Register(mappings);
        }

        public void MockSpecific<T>(string methodName, IEnumerable<Type> parameterTypes, Func<T> function)
        {
            var mappings = CreateMappingForAllMethods(methodName, parameterTypes.ToArray(), function.Method);
            _hijackRegister.Register(mappings);
        }

        public void MockSpecific<T, TOut>(string methodName, IEnumerable<Type> parameterTypes, Func<T, TOut> function)
        {
            var mappings = CreateMappingForAllMethods(methodName, parameterTypes.ToArray(), function.Method);
            _hijackRegister.Register(mappings);
        }

        public void MockSpecific<T1, T2, TOut>(string methodName, IEnumerable<Type> parameterTypes, Func<T1, T2, TOut> function)
        {
            var mappings = CreateMappingForAllMethods(methodName, parameterTypes.ToArray(), function.Method);
            _hijackRegister.Register(mappings);
        }

        public void MockSpecific<T1, T2, T3, TOut>(string methodName, IEnumerable<Type> parameterTypes, Func<T1, T2, T3, TOut> function)
        {
            var mappings = CreateMappingForAllMethods(methodName, parameterTypes.ToArray(), function.Method);
            _hijackRegister.Register(mappings);
        }

        public void MockSpecific<T1, T2, T3, T4, TOut>(string methodName, IEnumerable<Type> parameterTypes, Func<T1, T2, T3, T4, TOut> function)
        {
            var mappings = CreateMappingForAllMethods(methodName, parameterTypes.ToArray(), function.Method);
            _hijackRegister.Register(mappings);
        }

        public void MockSpecific<T1, T2, T3, T4, T5, TOut>(string methodName, IEnumerable<Type> parameterTypes, Func<T1, T2, T3, T4, T5, TOut> function)
        {
            var mappings = CreateMappingForAllMethods(methodName, parameterTypes.ToArray(), function.Method);
            _hijackRegister.Register(mappings);
        }
    }
}
