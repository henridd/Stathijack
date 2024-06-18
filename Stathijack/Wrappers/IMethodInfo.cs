using System.Reflection;

namespace Stathijack.Wrappers
{
    public interface IMethodInfo
    {
        IType ReturnType { get; }

        IType? DeclaringType { get; }

        string Name { get; }

        MethodInfo MethodInfo { get; }

        MethodBody? GetMethodBody();

        ParameterInfo[] GetParameters();

        object? Invoke(object? target, object?[]? parameters);
    }
}
