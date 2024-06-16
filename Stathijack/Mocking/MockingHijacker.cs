using Stathijack.Exceptions;
using System.Reflection;

namespace Stathijack.Mocking
{
    public class MockingHijacker : IDisposable
    {
        private readonly Type _target;
        private readonly IHijackRegister _hijackRegister;

        /// <summary>
        /// Creates a new MockingHijacker for the provided type. A new HijackRegister will be automatically created.
        /// </summary>
        /// <param name="target">The type that is going to be mocked.</param>
        public MockingHijacker(Type target) : this(target, new HijackRegister()) { }

        /// <summary>
        /// Creates a new MockingHijacker for the provided type.
        /// </summary>
        /// <param name="target">The type that is going to be mocked.</param>
        /// <param name="hijackRegister">The HijackRegister to register the mocks</param>
        public MockingHijacker(Type target, IHijackRegister hijackRegister)
        {
            _target = target;
            _hijackRegister = hijackRegister;
        }

        public void Dispose()
        {
            _hijackRegister.Dispose();
        }

        /// <summary>
        /// Hijacks the method that matches the methodName and the parameterTypes.
        /// </summary>
        /// <param name="methodName">The name of the method</param>
        /// <param name="action">The action to replace the original method</param>
        public HijackedMethodData MockMethod(string methodName, Action action)
            => DoMockMethod(methodName, action.Method, action.Target);

        public HijackedMethodData MockMethod<T>(string methodName, Func<T> function)
            => DoMockMethod(methodName, function.Method, function.Target);

        public HijackedMethodData MockMethod<T, TOut>(string methodName, Func<T, TOut> function)
            => DoMockMethod(methodName, function.Method, function.Target);

        public HijackedMethodData MockMethod<T1, T2, TOut>(string methodName, Func<T1, T2, TOut> function)
            => DoMockMethod(methodName, function.Method, function.Target);

        public HijackedMethodData MockMethod<T1, T2, T3, TOut>(string methodName, Func<T1, T2, T3, TOut> function)
            => DoMockMethod(methodName, function.Method, function.Target);

        public HijackedMethodData MockMethod<T1, T2, T3, T4, TOut>(string methodName, Func<T1, T2, T3, T4, TOut> function)
            => DoMockMethod(methodName, function.Method, function.Target);

        public HijackedMethodData MockMethod<T1, T2, T3, T4, T5, TOut>(string methodName, Func<T1, T2, T3, T4, T5, TOut> function)
            => DoMockMethod(methodName, function.Method, function.Target);

        private HijackedMethodData DoMockMethod(string methodName, MethodInfo method, object? target)
        {
            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentException("Method name cannot be null", nameof(methodName));

            if (method == null)
                throw new ArgumentNullException("You must provide a method to replace the original method", nameof(method));

            var mappings = CreateMappingForMethods(methodName, method.GetParameters().Select(x => x.ParameterType).ToArray(), method);

            if (mappings.Count > 1)
            {
                throw new MethodHijackingException("There were multiple methods found with the same name and parameters.");
            }

            return _hijackRegister.Register(mappings, target).First();
        }

        private List<MethodReplacementMapping> CreateMappingForMethods(string methodName, Type[]? parameterTypes, MethodInfo replacement)
        {
            var mappings = new List<MethodReplacementMapping>();
            foreach (var method in _target.GetMethods())
            {
                if (method.Name != methodName)
                    continue;

                if (parameterTypes != null && !parameterTypes.SequenceEqual(method.GetParameters().Select(x => x.ParameterType)))
                    continue;

                mappings.Add(new MethodReplacementMapping(method, replacement));
            }
            return mappings;
        }
    }
}
