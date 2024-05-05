using Stathijack.Replacer;
using System.Reflection;

namespace Stathijack
{
    public class HijackRegister : IHijackRegister, IDisposable
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

        /// <inheritdoc/>
        public void Register(Type target, Type hijacker)
            => Register(target, hijacker, BindingFlags.Static | BindingFlags.Public);

        /// <inheritdoc/>
        public void Register(Type target, Type hijacker, BindingFlags bindingFlags)
        {
            if (target.Equals(hijacker))
            {
                throw new ArgumentException("The target and hijacker cannot be the same.");
            }

            var methodsToHijack = _methodMatcher.MatchMethods(target, hijacker, bindingFlags);

            foreach(var info in methodsToHijack)
            {
                var result = _typeMethodReplacer.Replace(info.TargetMethod, info.HijackerMethod);
                _hijackedMethods.Add(result);
            }
        }

        /// <inheritdoc/>
        public void Register(IEnumerable<MethodReplacementMapping> mappings)
        {
            foreach (var mapping in mappings)
            {
                var result = _typeMethodReplacer.Replace(mapping.TargetMethod, mapping.HijackerMethod);
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
