using Stathijack.Samples.RealEntities;

namespace Stathijack.Samples.TestEntities
{
    internal class MockFactory
    {
        public static Entity CreateEntity()
            => new Entity() { Name = "Fake" };
    }
}
