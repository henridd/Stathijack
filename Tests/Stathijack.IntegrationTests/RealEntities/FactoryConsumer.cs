namespace Stathijack.IntegrationTests.RealEntities
{
    public class FactoryConsumer
    {
        public Entity UseFactory()
            => Factory.CreateEntity();

        public Entity UseFactory(string name)
            => Factory.CreateEntity(name);

        public Entity UseFactory(string name, int id)
            => Factory.CreateEntity(name, id);

        public Entity UseFactory(CreateFactoryPayload createFactoryPayload)
            => Factory.CreateEntity(createFactoryPayload);
    }
}
