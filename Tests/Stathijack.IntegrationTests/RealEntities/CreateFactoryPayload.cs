namespace Stathijack.IntegrationTests.RealEntities
{
    public struct CreateFactoryPayload
    {
        public int Id { get; }
        public string Name { get; }

        public CreateFactoryPayload(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
