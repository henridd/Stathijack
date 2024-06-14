namespace Stathijack.Exceptions
{
    /// <summary>
    /// Occurs when dynamically generating the mock type that controls the hijacked method invocation.
    /// </summary>
    internal class MockTypeGenerationException : Exception
    {
        public MockTypeGenerationException(string message) : base(message) { }
    }
}
