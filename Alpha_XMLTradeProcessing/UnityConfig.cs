using Alpha_XMLTradeProcessing.Interfaces;
using Alpha_XMLTradeProcessing.Services;
using Unity;

namespace Alpha_XMLTradeProcessing
{
    public static class UnityConfig
    {
        private static IUnityContainer _container;

        public static IUnityContainer Container
        {
            get
            {
                if (_container == null)
                {
                    _container = new UnityContainer();
                    RegisterTypes();
                }
                return _container;
            }
        }

        private static void RegisterTypes()
        {
            _container.RegisterType<IFileReader, XmlFileReader>();
            _container.RegisterType<ISecurityService, SecurityService>();
            _container.RegisterType<ITradeService, TradeService>();
        }

        public static T Resolve<T>()
        {
            return Container.Resolve<T>();
        }
    }
}
