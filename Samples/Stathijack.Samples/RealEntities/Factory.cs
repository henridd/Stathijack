namespace Stathijack.Samples.RealEntities
{
    public static class Factory
    {
        public static Entity CreateEntity()
            => new Entity() { Name = "Real" };

        public static Entity CreateEntity(string name)
            => new Entity() { Name = name };
    }
}
