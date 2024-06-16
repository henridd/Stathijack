namespace Stathijack.Samples.RealEntities
{
    public static class Factory
    {
        public const string DefaultEntityName = "Real";

        public static Entity CreateEntity()
            => new Entity() { Name = DefaultEntityName };

        public static Entity CreateEntity(string name)
            => new Entity() { Name = name };

        public static Entity CreateEntity(string name, int id)
            => new Entity() { Name = name, Id = id };

        public static Entity CreateEntityNonMatching()
            => new Entity() { Name = DefaultEntityName };

        public static Entity CreateEntityNonMatching(string name)
            => new Entity() { Name = name };
    }
}
