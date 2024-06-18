using System.Reflection;
using Stathijack.Wrappers;

namespace Stathijack.Dynamic
{
    internal interface IDynamicTypeFactory
    {
        IType GenerateMockTypeForMethod(IMethodInfo targetMethod, IMethodInfo hijackMethod);
    }
}
