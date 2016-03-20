using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ShareX.UploadersLib
{
    public interface IURLShortenerService
    {
        [Localizable(false)]
        string ServiceId { get; }

        UrlShortenerType EnumValue { get; }

        IURLShortener CreateShortener(UploadersConfig config);
    }

    public interface IServiceFactory<out TService, in TEnum>
    {
        TService GetServiceById(string serviceId);

        TService GetServiceByEnumValue(TEnum enumValue);
    }

    public interface IURLShortenerServiceFactory : IServiceFactory<IURLShortenerService, UrlShortenerType>
    {
    }

    internal class URLShortenerServiceFactory : IURLShortenerServiceFactory
    {
        private readonly IEnumerable<IURLShortenerService> _services;

        public URLShortenerServiceFactory(IEnumerable<IURLShortenerService> services)
        {
            _services = services;
        }

        public IURLShortenerService GetServiceById(string serviceId)
        {
            return _services.FirstOrDefault(s => s.ServiceId == serviceId);
        }

        public IURLShortenerService GetServiceByEnumValue(UrlShortenerType enumValue)
        {
            return _services.FirstOrDefault(s => s.EnumValue == enumValue);
        }
    }
}