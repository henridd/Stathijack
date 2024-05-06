namespace Stathijack.Samples.RealEntities
{
    public class FactoryConsumer
    {
        public Entity UseFactory()
            => Factory.CreateEntity();

        public Entity UseFactory(string name)
            => Factory.CreateEntity(name);
    }
}
