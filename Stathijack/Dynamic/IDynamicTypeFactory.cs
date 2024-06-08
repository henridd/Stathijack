using System.Reflection;

namespace Stathijack.Dynamic
{
    internal interface IDynamicTypeFactory
    {
        Type GenerateMockTypeForMethod(MethodInfo targetMethod, MethodInfo hijackMethod);
    }
}
