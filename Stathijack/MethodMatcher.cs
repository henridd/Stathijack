using System.Reflection;

namespace Stathijack
{
    internal class MethodMatcher : IMethodMatcher
    {
        public List<MethodReplacementMapping> MatchMethods(Type target, Type hijacker, BindingFlags bindingFlags)
        {
            var matchedMethods = new List<MethodReplacementMapping>();

            foreach (var method in target.GetMethods(bindingFlags))
            {
                var methodParameterTypes = method.GetParameters().Select(x => x.ParameterType).ToArray();
                var methodToReplace = hijacker.GetMethod(method.Name, bindingFlags, methodParameterTypes);
                if (methodToReplace == null)
                    continue;

                if (methodToReplace.ReturnParameter.ParameterType != method.ReturnParameter.ParameterType)
                    continue;

                matchedMethods.Add(new MethodReplacementMapping(method, methodToReplace));
            }

            return matchedMethods;
        }
    }
}
