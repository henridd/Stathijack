using Stathijack;
using Stathijack.Sample.RealEntities;

namespace Stathijack.Sample.TestEntities
{
    internal class MockFactory
    {
        public static Entity CreateEntity() 
            => new Entity() { Name = "Fake" };
    }
}
