using System.Reflection;

namespace Stathijack
{
    public interface IHijackRegister : IDisposable
    {
        /// <summary>
        /// Register a class as a hijacker. It will scan the hijacker for methods with the same name and parameters as in the target
        /// class, and for every match, will redirect the method calls to the hijacker.
        /// </summary>
        /// <param name="target">The original class</param>
        /// <param name="hijacker">The fake class to redirect calls to</param>
        /// <param name="bindingFlags">Binding flags used to find the methods in the target class</param>
         IEnumerable<HijackedMethodData> Register(Type target, Type hijacker);

        /// <summary>
        /// Register a class as a hijacker. It will scan the hijacker for methods with the same name and parameters as in the target
        /// class, and for every match, will redirect the method calls to the hijacker.
        /// </summary>
        /// <param name="target">The original class</param>
        /// <param name="hijacker">The fake class to redirect calls to</param>
        /// <param name="bindingFlags">Binding flags used to find the methods in the target class</param>
         IEnumerable<HijackedMethodData> Register(Type target, Type hijacker, BindingFlags bindingFlags);

        /// <summary>
        /// Register a the provided mappings for hijacking.
        /// </summary>
         IEnumerable<HijackedMethodData> Register(IEnumerable<MethodReplacementMapping> mappings, object? target);
    }
}
