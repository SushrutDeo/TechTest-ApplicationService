namespace Services.Factories
{
    public class AdminstrationServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public AdminstrationServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
    }
}
