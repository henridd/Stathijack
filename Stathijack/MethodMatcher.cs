using Stathijack.Wrappers;
using System.Reflection;

namespace Stathijack
{
    internal class MethodMatcher : IMethodMatcher
    {
        public List<MethodReplacementMapping> MatchMethods(Type target, Type hijacker, BindingFlags bindingFlags)
        {
            if(target == null)
                throw new ArgumentNullException("You must specify the target type", nameof(target));

            if (hijacker == null)
                throw new ArgumentNullException("You must specify the hijacker type", nameof(hijacker));

            var matchedMethods = new List<MethodReplacementMapping>();

            foreach (var method in target.GetMethods(bindingFlags))
            {
                var methodParameterTypes = method.GetParameters().Select(x => x.ParameterType).ToArray();
                var methodToReplace = hijacker.GetMethod(method.Name, bindingFlags, methodParameterTypes);
                if (methodToReplace == null)
                    continue;

                if (methodToReplace.ReturnParameter.ParameterType != method.ReturnParameter.ParameterType)
                    continue;

                matchedMethods.Add(new MethodReplacementMapping(new MethodInfoWrapper(method), new MethodInfoWrapper(methodToReplace)));
            }

            return matchedMethods;
        }
    }
}
