using System.Reflection;

namespace Stathijack
{
    public record MethodReplacementMapping(MethodInfo TargetMethod, MethodInfo HijackerMethod);
}
