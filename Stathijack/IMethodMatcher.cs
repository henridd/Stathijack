using System.Reflection;

namespace Stathijack
{
    internal interface IMethodMatcher
    {
        List<MethodReplacementInfo> MatchMethods(Type target, Type hijacker, BindingFlags bindingFlags);
    }
}