using System.Reflection;
using System.Text;

namespace Stathijack.Dynamic
{
    internal static class DynamicTypeReader
    {
        public static string GenerateCsFile(Type type)
        {
            StringBuilder sb = new StringBuilder();

            // Namespace and class declaration
            sb.AppendLine("using System;");
            sb.AppendLine();
            if (!string.IsNullOrEmpty(type.Namespace))
            {
                sb.AppendLine($"namespace {type.Namespace}");
                sb.AppendLine("{");
            }
            sb.AppendLine($"    public class {type.Name}");
            sb.AppendLine("    {");

            // Fields
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
            {
                string accessModifier = GetAccessModifier(field);
                sb.AppendLine($"        {accessModifier} {field.FieldType.Name} {field.Name};");
            }
            sb.AppendLine();

            // Properties
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
            {
                string accessModifier = GetAccessModifier(prop);
                sb.AppendLine($"        {accessModifier} {prop.PropertyType.Name} {prop.Name} {{ get; set; }}");
            }
            sb.AppendLine();

            // Methods
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
            {
                if (method.IsSpecialName) // Skip property methods
                    continue;

                string accessModifier = GetAccessModifier(method);
                string parameters = string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
                sb.AppendLine($"        {accessModifier} {method.ReturnType.Name} {method.Name}({parameters})");
                sb.AppendLine("        {");
                sb.AppendLine("            // Method body");
                sb.AppendLine("        }");
                sb.AppendLine();
            }

            sb.AppendLine("    }");
            if (!string.IsNullOrEmpty(type.Namespace))
            {
                sb.AppendLine("}");
            }

            // Write the generated code to a file
            return sb.ToString();
        }

        private static string GetAccessModifier(MemberInfo member)
        {
            if (member is FieldInfo field)
            {
                if (field.IsPublic) return "public";
                if (field.IsPrivate) return "private";
                if (field.IsFamily) return "protected";
                if (field.IsAssembly) return "internal";
                if (field.IsFamilyOrAssembly) return "protected internal";
            }
            else if (member is MethodBase method)
            {
                if (method.IsPublic) return "public";
                if (method.IsPrivate) return "private";
                if (method.IsFamily) return "protected";
                if (method.IsAssembly) return "internal";
                if (method.IsFamilyOrAssembly) return "protected internal";
            }
            else if (member is PropertyInfo property)
            {
                var getMethod = property.GetGetMethod(true);
                if (getMethod != null) return GetAccessModifier(getMethod);
            }

            return "private"; // Default access modifier
        }
    }
}
