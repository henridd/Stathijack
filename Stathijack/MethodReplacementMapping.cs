using Stathijack.Wrappers;

namespace Stathijack
{
    public record MethodReplacementMapping(IMethodInfo TargetMethod, IMethodInfo HijackerMethod);
}
