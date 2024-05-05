using System.Reflection;

namespace Stathijack
{
    internal class MethodMatcher : IMethodMatcher
    {
        public List<MethodReplacementInfo> MatchMethods(Type target, Type hijacker, BindingFlags bindingFlags)
        {
            var matchedMethods = new List<MethodReplacementInfo>();

            foreach (var method in target.GetMethods(bindingFlags))
            {
                var methodParameterTypes = method.GetParameters().Select(x => x.ParameterType).ToArray();
                var methodToReplace = hijacker.GetMethod(method.Name, bindingFlags, methodParameterTypes);
                if (methodToReplace == null)
                    continue;

                matchedMethods.Add(new MethodReplacementInfo(method, methodToReplace));
            }

            return matchedMethods;
        }
    }
}
