
using System.Reflection;

namespace Stathijack.Wrappers
{
    internal class MethodInfoWrapper : IMethodInfo
    {
        public IType ReturnType { get; private set; }

        public IType? DeclaringType { get; private set; }

        public string Name => MethodInfo.Name;

        public MethodInfo MethodInfo { get; }

        public MethodInfoWrapper(MethodInfo methodInfo)
        {
            MethodInfo = methodInfo;
            ReturnType = new TypeWrapper(methodInfo.ReturnType);

            if(methodInfo.DeclaringType != null)
            {
                DeclaringType = new TypeWrapper(methodInfo.DeclaringType);
            }
        }

        public ParameterInfo[] GetParameters()
            => MethodInfo.GetParameters();

        public object? Invoke(object? target, object?[]? parameters)
            => MethodInfo.Invoke(target, parameters);

        public MethodBody? GetMethodBody() 
            => MethodInfo.GetMethodBody();
    }
}
