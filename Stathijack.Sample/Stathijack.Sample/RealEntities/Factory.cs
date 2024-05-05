namespace Stathijack.Sample.RealEntities
{
    public static class Factory
    {
        public static Entity CreateEntity() 
            => new Entity() { Name = "Real" };
    }
}
