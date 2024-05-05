using System.Reflection;

namespace Stathijack
{
    internal record MethodReplacementMapping(MethodInfo targetMethod, MethodInfo hijackerMethod);
}
