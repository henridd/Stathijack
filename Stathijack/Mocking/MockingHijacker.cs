using System.Reflection;

namespace Stathijack.Mocking
{
    public partial class MockingHijacker
    {
        private readonly Type _target;
        private readonly IHijackRegister _hijackRegister;

        public MockingHijacker(Type target, IHijackRegister hijackRegister)
        {
            _target = target;
            _hijackRegister = hijackRegister;
        }

        private List<MethodReplacementMapping> CreateMappingForAllMethods(string methodName, MethodInfo replacement)
            => CreateMappingForAllMethods(methodName, null, replacement);

        private List<MethodReplacementMapping> CreateMappingForAllMethods(string methodName, Type[]? types, MethodInfo replacement)
        {
            var mappings = new List<MethodReplacementMapping>();
            foreach (var method in _target.GetMethods())
            {
                if (method.Name != methodName)
                    continue;

                if (types != null && !types.SequenceEqual(method.GetParameters().Select(x => x.ParameterType)))
                    continue;

                mappings.Add(new MethodReplacementMapping(method, replacement));
            }
            return mappings;
        }
    }
}
