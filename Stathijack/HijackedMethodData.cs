using System.Collections.ObjectModel;

namespace Stathijack
{
    public class HijackedMethodData
    {
        private Stack<MethodInvocationResult> _invocations = new Stack<MethodInvocationResult>();

        public IReadOnlyCollection<MethodInvocationResult> Invocations
            => new ReadOnlyCollection<MethodInvocationResult>(_invocations.ToList());

        internal void RegisterInvocation(MethodInvocationResult invocation)
            => _invocations.Push(invocation);

        internal void ResetInvocations()
            => _invocations.Clear();
    }
}
