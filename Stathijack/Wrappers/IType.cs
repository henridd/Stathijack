using System.Reflection;

namespace Stathijack.Wrappers
{
    public interface IType
    {
        Type Type { get; }

        bool IsValueType { get; }

        string? FullName { get; }

        IMethodInfo? GetMethod(string name, BindingFlags bindingAttr);
    }
}
