using System.Reflection;

namespace Stathijack.Wrappers
{
    internal class TypeWrapper : IType
    {
        public Type Type { get; }

        public bool IsValueType
            => Type.IsValueType;

        public string? FullName
            => Type.FullName;

        public TypeWrapper(Type type)
        {
            Type = type;
        }

        public IMethodInfo? GetMethod(string name, BindingFlags bindingAttr)
        {
            var method = Type.GetMethod(name, bindingAttr);
            if (method == null)
            {
                return null;
            }

            var methodInfoWrapper = new MethodInfoWrapper(method);
            return methodInfoWrapper;
        }

    }
}
