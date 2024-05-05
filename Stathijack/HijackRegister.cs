using Stathijack.Replacer;
using System.Reflection;

namespace Stathijack
{
    public class HijackRegister
    {
        private readonly Dictionary<Type, Type> _hijackMapping = new();
        private readonly IMethodMatcher _methodMatcher;

        public HijackRegister() : this(new MethodMatcher())
        {
            
        }

        internal HijackRegister(IMethodMatcher methodMatcher)
        {
            _methodMatcher = methodMatcher;
        }

        /// <summary>
        /// Marks a class as a hijacker. It will scan the hijacker for methods with the same name and parameters as in the target
        /// class, and for every match, will redirect the method calls to the hijacker.
        /// </summary>
        /// <param name="target">The original class</param>
        /// <param name="hijacker">The fake class to redirect calls to</param>
        /// <param name="bindingFlags">Binding flags used to find the methods in the target class</param>
        public void RegisterHijacker(Type target, Type hijacker)
            => RegisterHijacker(target, hijacker, BindingFlags.Static | BindingFlags.Public);

        /// <summary>
        /// Marks a class as a hijacker. It will scan the hijacker for methods with the same name and parameters as in the target
        /// class, and for every match, will redirect the method calls to the hijacker.
        /// </summary>
        /// <param name="target">The original class</param>
        /// <param name="hijacker">The fake class to redirect calls to</param>
        /// <param name="bindingFlags">Binding flags used to find the methods in the target class</param>
        public void RegisterHijacker(Type target, Type hijacker, BindingFlags bindingFlags)
        {
            _hijackMapping.Add(target, hijacker);
            var methodsToHijack = _methodMatcher.MatchMethods(target, hijacker, bindingFlags);

            foreach(var info in methodsToHijack)
            {
                TypeMethodReplacer.Replace(info.targetMethod, info.hijackerMethod);
            }
        }
    }
}
