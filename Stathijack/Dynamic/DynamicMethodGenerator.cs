using System.Reflection;
using System.Reflection.Emit;

namespace Stathijack.Dynamic
{
    internal static class DynamicMethodGenerator
    {
        /// <summary>
        /// Creates the Invoke method, which routes method calls to the hijackMethod
        /// </summary>
        internal static void GenerateInvokeMethod(TypeBuilder typeBuilder, Type[] parameters, Type returnType, MethodInfo hijackMethod)
        {
            var methodBuilder = typeBuilder.DefineMethod(
                                                 "Invoke",
                                                 MethodAttributes.Public | MethodAttributes.Static,
                                                 returnType,
                                                 parameters);

            var ILout = methodBuilder.GetILGenerator();

            // Declare a local variable to hold the object array
            ILout.DeclareLocal(typeof(object[])); // local_0: object[] parameters

            // Create and initialize the object array
            ILout.Emit(OpCodes.Ldc_I4, parameters.Length); // Load the array size onto the stack
            ILout.Emit(OpCodes.Newarr, typeof(object)); // Create a new object array
            ILout.Emit(OpCodes.Stloc_0); // Store the array in the local variable (local_0)

            // Loop through the parameterTypes array and populate the object array
            for (int i = 0; i < parameters.Length; i++)
            {
                ILout.Emit(OpCodes.Ldloc_0); // Load the object array

                ILout.Emit(OpCodes.Ldc_I4, i); // Load the current index

                ILout.Emit(OpCodes.Ldarg, i); // Load the argument at the current index

                // Box the argument if it's a value type
                if (parameters[i].IsValueType)
                {
                    ILout.Emit(OpCodes.Box, parameters[i]);
                }

                ILout.Emit(OpCodes.Stelem_Ref); // Store the argument in the object array
            }

            ILout.Emit(OpCodes.Ldloc_0); // Load the object array

            ILout.EmitCall(OpCodes.Call, hijackMethod, null);
            ILout.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Duplicates the original method logic, so the default logic can be restored
        /// </summary>
        internal static void GenerateInvokeDefaultMethod(TypeBuilder typeBuilder, Type[] parameters, Type returnType, MethodInfo originalMethod)
        {
            var methodBody = originalMethod.GetMethodBody();
            var ilBytes = methodBody.GetILAsByteArray();

            var methodBuilder = typeBuilder.DefineMethod(
                                     "InvokeDefault",
                                     MethodAttributes.Public | MethodAttributes.Static,
                                     returnType,
                                     parameters);

            var ILout = methodBuilder.GetILGenerator();

            for (int i = 0; i < ilBytes.Length;)
            {
                OpCode opcode = OpCodes.Nop; // Default opcode

                // Read opcode
                if (i < ilBytes.Length)
                    opcode = GetOpCode(ilBytes[i++]);

                // Emit opcode
                switch (opcode.OperandType)
                {
                    case OperandType.InlineNone:
                        ILout.Emit(opcode);
                        break;
                    case OperandType.InlineBrTarget:
                    case OperandType.InlineField:
                    case OperandType.InlineI:
                    case OperandType.InlineMethod:
                    case OperandType.InlineSig:
                    case OperandType.InlineString:
                    case OperandType.InlineTok:
                    case OperandType.InlineType:
                        EmitInt32(ILout, BitConverter.ToInt32(ilBytes, i));
                        i += 4;
                        break;
                    case OperandType.ShortInlineBrTarget:
                    case OperandType.ShortInlineI:
                        ILout.Emit(opcode, ilBytes[i++]);
                        break;
                    case OperandType.ShortInlineR:
                        ILout.Emit(opcode, BitConverter.ToSingle(ilBytes, i));
                        i += 4;
                        break;
                    case OperandType.InlineR:
                        ILout.Emit(opcode, BitConverter.ToDouble(ilBytes, i));
                        i += 8;
                        break;
                    case OperandType.InlineVar:
                        ILout.Emit(opcode, ilBytes[i++]);
                        break;
                    case OperandType.ShortInlineVar:
                        ILout.Emit(opcode, (short)ilBytes[i++]);
                        break;
                    default:
                        throw new NotSupportedException("Operand type not supported.");
                }
            }

            ILout.Emit(OpCodes.Ret);
            //EmitIL(ILout, ilBytes!, originalMethod.Module);
        }

        // Helper method to convert byte to OpCode
        private static OpCode GetOpCode(byte opCodeByte)
        {
            if (opCodeByte == 0xfe)
                return OpCodes.Prefix2;
            else if (opCodeByte == 0xff)
                return OpCodes.Prefix3;
            else
                return OpCodes.Nop; // Placeholder, replace with appropriate opcode
        }

        // Helper method to emit Int32 operand
        private static void EmitInt32(ILGenerator ilGenerator, int value)
        {
            ilGenerator.Emit(OpCodes.Ldc_I4, value);
        }        
    }
}
