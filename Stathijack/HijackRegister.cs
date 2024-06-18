using Stathijack.Dynamic;
using Stathijack.Exceptions;
using Stathijack.Replacer;
using Stathijack.Wrappers;
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
        public IEnumerable<HijackedMethodData> Register(Type target, Type hijacker)
            => Register(target, hijacker, BindingFlags.Static | BindingFlags.Public);

        /// <inheritdoc/>
        public IEnumerable<HijackedMethodData> Register(Type target, Type hijacker, BindingFlags bindingFlags)
        {
            if (target.Equals(hijacker))
            {
                throw new ArgumentException("The target and hijacker cannot be the same.");
            }

            var methodsToHijack = _methodMatcher.MatchMethods(target, hijacker, bindingFlags);

            return RegisterMappedHijacks(methodsToHijack, null);
        }

        /// <inheritdoc/>
        public IEnumerable<HijackedMethodData> Register(IEnumerable<MethodReplacementMapping> mappings, object? target)
        {
            if (mappings == null)
                throw new ArgumentNullException("A valid list of mappings must be provided", nameof(mappings));

            return RegisterMappedHijacks(mappings, target);
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

        private IEnumerable<HijackedMethodData> RegisterMappedHijacks(IEnumerable<MethodReplacementMapping> methodsToHijack, object? target)
        {
            var hijackedMethodDataList = new List<HijackedMethodData>();
            foreach (var info in methodsToHijack)
            {
                hijackedMethodDataList.Add(HijackMethod(info.TargetMethod, info.HijackerMethod, target));
            }

            return hijackedMethodDataList;
        }

        private HijackedMethodData HijackMethod(IMethodInfo targetMethod, IMethodInfo hijackerMethod, object? target)
        {
            var hijackType = _dynamicTypeFactory.GenerateMockTypeForMethod(targetMethod, HijackedMethodController.GetRootMethodInfo());
            var invokeMethodInfo = hijackType.GetMethod("Invoke", BindingFlags.Public | BindingFlags.Static);
            if (invokeMethodInfo == null)
            {
                throw new MethodHijackingException("Unable to find the Invoke method in the generated mock type");
            }

            var declaringType = invokeMethodInfo.DeclaringType;
            if (declaringType == null)
            {
                throw new MethodHijackingException("The generated Invoke method has no declaring type");
            }

            var dynamicNamespaceFullName = declaringType.FullName;
            if (dynamicNamespaceFullName == null)
            {
                throw new MethodHijackingException("The FullName of the generated mock type is null");
            }

            _typeMethodReplacer.Replace(targetMethod.MethodInfo, invokeMethodInfo.MethodInfo);

            _hijackedClasses.Add(dynamicNamespaceFullName);

            if (HijackedMethodController.MethodHasBeenHijacked(dynamicNamespaceFullName))
            {
                HijackedMethodController.AppendHijack(hijackerMethod, target, dynamicNamespaceFullName);
                return HijackedMethodController.GetHijackedMethodData(dynamicNamespaceFullName);
            }

            var hijackedMethodData = new HijackedMethodData();

            AddNewHijack(hijackerMethod, target, hijackType, dynamicNamespaceFullName, hijackedMethodData);

            return hijackedMethodData;
        }

        private void AddNewHijack(IMethodInfo hijackerMethod, object? target, IType hijackType, string dynamicNamespaceFullName, HijackedMethodData hijackedMethodData)
        {
            if (EnableExperimentalDefaultInvoking)
            {
                var invokeDefaultMethodInfo = hijackType.GetMethod("InvokeDefault", BindingFlags.Public | BindingFlags.Static);
                if (invokeDefaultMethodInfo == null)
                {
                    throw new MethodHijackingException("Unable to find the InvokeDefault method in the generated mock type");
                }

                HijackedMethodController.AddNewHijack(new MethodInfoWrapper(invokeDefaultMethodInfo.MethodInfo), hijackerMethod, target, dynamicNamespaceFullName, hijackedMethodData);
            }
            else
            {
                HijackedMethodController.AddNewHijack(hijackerMethod, target, dynamicNamespaceFullName, hijackedMethodData);
            }
        }
    }
}
