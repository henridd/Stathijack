using System.Reflection;

namespace Stathijack.Replacer
{
    public interface ITypeMethodReplacer
    {
        void Replace(MethodInfo targetMethod, MethodInfo hijackerMethod);
    }
}
