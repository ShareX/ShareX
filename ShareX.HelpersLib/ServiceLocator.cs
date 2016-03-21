using System;
using SimpleInjector;

namespace ShareX.HelpersLib
{
    public static class ServiceLocator
    {
        private static Container Container;

        public static void SetContainer(Container container)
        {
            Container = container;
        }

        public static TService GetInstance<TService>() where TService : class
        {
            EnsureContainerIsSet();
            return Container.GetInstance<TService>();
        }

        private static void EnsureContainerIsSet()
        {
            if (Container == null)
                throw new InvalidOperationException(@"ServiceLocator used before a container was set");
        }
    }
}