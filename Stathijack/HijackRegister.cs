using Stathijack.Replacer;
using System.Reflection;

namespace Stathijack
{
    public class HijackRegister : IDisposable
    {
        private readonly List<MethodReplacementResult> _hijackedMethods = new();
        private readonly IMethodMatcher _methodMatcher;
        private readonly ITypeMethodReplacer _typeMethodReplacer;

        public IReadOnlyList<MethodReplacementResult> HijackedMethods => _hijackedMethods;

        public HijackRegister() : this(new MethodMatcher(), new TypeMethodReplacer())
        {
            
        }

        internal HijackRegister(IMethodMatcher methodMatcher, ITypeMethodReplacer typeMethodReplacer)
        {
            _methodMatcher = methodMatcher;
            _typeMethodReplacer = typeMethodReplacer;
        }

        /// <summary>
        /// Register a class as a hijacker. It will scan the hijacker for methods with the same name and parameters as in the target
        /// class, and for every match, will redirect the method calls to the hijacker.
        /// </summary>
        /// <param name="target">The original class</param>
        /// <param name="hijacker">The fake class to redirect calls to</param>
        /// <param name="bindingFlags">Binding flags used to find the methods in the target class</param>
        public void Register(Type target, Type hijacker)
            => Register(target, hijacker, BindingFlags.Static | BindingFlags.Public);

        /// <summary>
        /// Register a class as a hijacker. It will scan the hijacker for methods with the same name and parameters as in the target
        /// class, and for every match, will redirect the method calls to the hijacker.
        /// </summary>
        /// <param name="target">The original class</param>
        /// <param name="hijacker">The fake class to redirect calls to</param>
        /// <param name="bindingFlags">Binding flags used to find the methods in the target class</param>
        public void Register(Type target, Type hijacker, BindingFlags bindingFlags)
        {
            if (target.Equals(hijacker))
            {
                throw new ArgumentException("The target and hijacker cannot be the same.");
            }

            var methodsToHijack = _methodMatcher.MatchMethods(target, hijacker, bindingFlags);

            foreach(var info in methodsToHijack)
            {
                var result = _typeMethodReplacer.Replace(info.targetMethod, info.hijackerMethod);
                _hijackedMethods.Add(result);
            }
        }

        public void Dispose()
        {
            foreach(var result in _hijackedMethods)
            {
                _typeMethodReplacer.RollbackReplacement(result);
            }
        }
    }
}
