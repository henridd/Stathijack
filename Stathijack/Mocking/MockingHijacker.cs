namespace Stathijack.Mocking
{
    public partial class MockingHijacker
    {
        private readonly Type _target;
        private readonly IHijackRegister _hijackRegister;

        public MockingHijacker(Type target, IHijackRegister hijackRegister)
        {
            _target = target;
            _hijackRegister = hijackRegister;
        }
    }
}
