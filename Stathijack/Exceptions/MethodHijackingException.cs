namespace Stathijack.Exceptions
{
    /// <summary>
    /// Occurs when hijacking a method.
    /// </summary>
    internal class MethodHijackingException : Exception
    {
        public MethodHijackingException(string message) : base(message) { }
    }
}
