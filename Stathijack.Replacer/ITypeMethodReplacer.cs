using System.Reflection;

namespace Stathijack.Replacer
{
    public interface ITypeMethodReplacer
    {
        MethodReplacementResult Replace(MethodInfo targetMethod, MethodInfo hijackerMethod);

        void RollbackReplacement(MethodReplacementResult result);
    }
}
