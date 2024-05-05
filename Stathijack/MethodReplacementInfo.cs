using System.Reflection;

namespace Stathijack
{
    internal record MethodReplacementInfo(MethodInfo targetMethod, MethodInfo hijackerMethod);
}
