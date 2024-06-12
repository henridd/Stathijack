using Stathijack.Dynamic;
using Stathijack.Replacer;
using System.Reflection;

namespace Stathijack
{
    public class HijackRegister : IHijackRegister
    {
        private readonly IMethodMatcher _methodMatcher;
        private readonly ITypeMethodReplacer _typeMethodReplacer;
        private readonly IDynamicTypeFactory _dynamicTypeFactory;

        private HashSet<string> _hijackedClasses = new();
        private bool _disposed;

        /// <summary>
        /// When enabled, it will try to duplicate the behavior of the original method, so that when the HijackRegister
        /// is disposed, the method performs its normal logic. 
        /// Must be set before registering the the mocks.
        /// </summary>
        public bool EnableExperimentalDefaultInvoking { get; set; }

        /// <summary>
        /// Creates a new HijackRegister, used to control which methods of which types have been hijacked.
        /// </summary>
        public HijackRegister() : this(new MethodMatcher(), new TypeMethodReplacer(), new DynamicTypeFactory()) { }

        internal HijackRegister(IMethodMatcher methodMatcher, ITypeMethodReplacer typeMethodReplacer, IDynamicTypeFactory dynamicTypeFactory)
        {
            _methodMatcher = methodMatcher;
            _typeMethodReplacer = typeMethodReplacer;
            _dynamicTypeFactory = dynamicTypeFactory;
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

            foreach (var info in methodsToHijack)
            {
                HijackMethod(info.TargetMethod, info.HijackerMethod, null);
            }
        }

        /// <inheritdoc/>
        public void Register(IEnumerable<MethodReplacementMapping> mappings, object target)
        {
            if (mappings == null)
                throw new ArgumentNullException("A valid list of mappings must be provided", nameof(mappings));

            foreach (var mapping in mappings)
            {
                HijackMethod(mapping.TargetMethod, mapping.HijackerMethod, target);
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            foreach (var item in _hijackedClasses)
                HijackedMethodController.RemoveHijack(item);

            _disposed = true;
        }

        private void HijackMethod(MethodInfo targetMethod, MethodInfo hijackerMethod, object? target)
        {
            var hijackType = _dynamicTypeFactory.GenerateMockTypeForMethod(targetMethod, HijackedMethodController.GetRootMethodInfo());
            var invokeMethodInfo = hijackType.GetMethod("Invoke", BindingFlags.Public | BindingFlags.Static);
            var dynamicNamespaceFullName = invokeMethodInfo.DeclaringType.FullName;
            _typeMethodReplacer.Replace(targetMethod, invokeMethodInfo);

            if (HijackedMethodController.MethodHasBeenHijacked(dynamicNamespaceFullName))
            {
                HijackedMethodController.AppendHijack(hijackerMethod, target, dynamicNamespaceFullName);
                return;
            }

            if (EnableExperimentalDefaultInvoking)
            {
                var invokeDefaultMethodInfo = hijackType.GetMethod("InvokeDefault", BindingFlags.Public | BindingFlags.Static);
                HijackedMethodController.AddNewHijack(invokeDefaultMethodInfo, hijackerMethod, target, dynamicNamespaceFullName);
            }
            else
            {
                HijackedMethodController.AddNewHijack(hijackerMethod, target, dynamicNamespaceFullName);
            }

            _hijackedClasses.Add(invokeMethodInfo.DeclaringType.FullName);
        }
    }
}
