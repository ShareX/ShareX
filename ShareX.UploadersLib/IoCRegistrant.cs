using System.Reflection;
using SimpleInjector;

namespace ShareX.UploadersLib
{
    public static class IoCRegistrant
    {
        public static void Register(Container container)
        {
            Assembly[] uploaderAssembly = new[] { typeof(IoCRegistrant).Assembly };

            container.RegisterCollection<IURLShortenerService>(uploaderAssembly);

            container.RegisterSingleton<IURLShortenerServiceFactory, URLShortenerServiceFactory>();
        }
    }
}