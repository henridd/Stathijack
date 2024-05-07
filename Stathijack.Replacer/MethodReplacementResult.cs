using System.Reflection;

namespace Stathijack.Replacer
{
    public unsafe struct MethodReplacementResult
    {
        public long* OriginalValue { get; set; }
        public long* Location { get; set; }
    }
}
