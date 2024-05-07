using System.Reflection;

namespace Stathijack
{
    public struct HijackedMethodsDictionaryKey
    {
        public string Value { get; }

        public HijackedMethodsDictionaryKey(MethodInfo targetMethod)
        {
            Value = $"{targetMethod.DeclaringType.FullName}.{targetMethod.Name}";
        }
    }
}
