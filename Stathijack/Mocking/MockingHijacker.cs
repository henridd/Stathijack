using System.Reflection;

namespace Stathijack.Mocking
{
    public partial class MockingHijacker : IDisposable
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

        private List<MethodReplacementMapping> CreateMappingForAllMethods(string methodName, MethodInfo replacement)
            => CreateMappingForAllMethods(methodName, null, replacement);

        private List<MethodReplacementMapping> CreateMappingForAllMethods(string methodName, Type[]? parameterTypes, MethodInfo replacement)
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
