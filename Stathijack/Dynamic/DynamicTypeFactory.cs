using Stathijack.Exceptions;
using System.Reflection;
using System.Reflection.Emit;

namespace Stathijack.Dynamic
{
    internal class DynamicTypeFactory : IDynamicTypeFactory
    {
        public Type GenerateMockTypeForMethod(MethodInfo targetMethod, MethodInfo hijackMethod)
        {
            var typeBuilder = InitializeBuilder(targetMethod);

            var targetMethodParameters = targetMethod.GetParameters().Select(x => x.ParameterType).ToArray();

            DynamicMethodGenerator.GenerateInvokeMethod(typeBuilder, targetMethodParameters, hijackMethod.ReturnType, hijackMethod);
            DynamicMethodGenerator.GenerateInvokeDefaultMethod(typeBuilder, targetMethodParameters, hijackMethod.ReturnType, targetMethod);

            var type = typeBuilder.CreateType();

            return type;
        }

        private TypeBuilder InitializeBuilder(MethodInfo methodToHijack)
        {
            AssemblyName asmName = new AssemblyName();
            asmName.Name = "Stathijack.Dynamic";

            AssemblyBuilder asmBuilder = AssemblyBuilder.DefineDynamicAssembly(
                                           asmName,
                                           AssemblyBuilderAccess.Run);

            ModuleBuilder moduleBuilder = asmBuilder.DefineDynamicModule(asmName.Name);

            var declaringType = methodToHijack.DeclaringType;
            if (declaringType == null)
            {
                throw new MockTypeGenerationException($"The method {methodToHijack.Name} has no declaring type");
            }

            var dynamicTypeName = $"{asmName.Name}.{declaringType.FullName}.{methodToHijack.Name}";

            return moduleBuilder.DefineType(dynamicTypeName, TypeAttributes.Public);
        }
    }
}
