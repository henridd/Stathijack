using System.Reflection;

namespace Stathijack.Mocking
{
    public partial class MockingHijacker
    {
        /// <summary>
        /// Hijacks every method that matches the methodName in the target type.
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="action"></param>
        public void MockAll(string methodName, Action action)
        {
            var mappings = CreateMappingForAllMethods(methodName, action.Method);
            _hijackRegister.Register(mappings);
        }

        public void MockAll<T>(string methodName, Func<T> function)
        {
            var mappings = CreateMappingForAllMethods(methodName, function.Method);
            _hijackRegister.Register(mappings);
        }

        public void MockAll<TIn, TOut>(string methodName, Func<TIn, TOut> function)
        {
            var mappings = CreateMappingForAllMethods(methodName, function.Method);
            _hijackRegister.Register(mappings);
        }

        public void MockAll<T1, T2, TOut>(string methodName, Func<T1, T2, TOut> function)
        {
            var mappings = CreateMappingForAllMethods(methodName, function.Method);
            _hijackRegister.Register(mappings);
        }

        public void MockAll<T1, T2, T3, TOut>(string methodName, Func<T1, T2, T3, TOut> function)
        {
            var mappings = CreateMappingForAllMethods(methodName, function.Method);
            _hijackRegister.Register(mappings);
        }

        public void MockAll<T1, T2, T3, T4, TOut>(string methodName, Func<T1, T2, T3, T4, TOut> function)
        {
            var mappings = CreateMappingForAllMethods(methodName, function.Method);
            _hijackRegister.Register(mappings);
        }

        public void MockAll<T1, T2, T3, T4, T5, TOut>(string methodName, Func<T1, T2, T3, T4, T5, TOut> function)
        {
            var mappings = CreateMappingForAllMethods(methodName, function.Method);
            _hijackRegister.Register(mappings);
        }
	}
}
