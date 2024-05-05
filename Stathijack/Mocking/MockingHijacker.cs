using System.Reflection;

namespace Stathijack.Mocking
{
    public class MockingHijacker
    {
        private readonly Type _target;
        private readonly IHijackRegister _hijackRegister;

        public MockingHijacker(Type target, IHijackRegister hijackRegister)
        {
            _target = target;
            _hijackRegister = hijackRegister;
        }

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

        private List<MethodReplacementMapping> CreateMappingForAllMethods(string methodName, MethodInfo replacement)
        {
            var mappings = new List<MethodReplacementMapping>();
            foreach (var method in _target.GetMethods())
            {
                if (method.Name == methodName)
                {
                    mappings.Add(new MethodReplacementMapping(method, replacement));
                }
            }
            return mappings;
        }
    }
}
