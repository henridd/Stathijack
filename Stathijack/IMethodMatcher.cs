using System.Reflection;

namespace Stathijack
{
    internal interface IMethodMatcher
    {
        List<MethodReplacementMapping> MatchMethods(Type target, Type hijacker, BindingFlags bindingFlags);
    }
}