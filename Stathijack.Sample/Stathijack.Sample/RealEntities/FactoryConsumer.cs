namespace Stathijack.Sample.RealEntities
{
    public class FactoryConsumer
    {
        public Entity UseFactory() 
            => Factory.CreateEntity();
    }
}
