namespace Stathijack.Exceptions
{
    /// <summary>
    /// Occurs when calling a hijacked method.
    /// </summary>
    public class HijackedMethodInvocationException : Exception
    {
        internal HijackedMethodInvocationException(string message):base(message) { }
    }
}
