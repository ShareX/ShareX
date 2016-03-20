using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ShareX.UploadersLib
{
    public interface IUploadService<out TEnum>
    {
        [Localizable(false)]
        string ServiceId { get; }

        TEnum EnumValue { get; }
    }

    public interface IURLShortenerService : IUploadService<UrlShortenerType>
    {
        IURLShortener CreateShortener(UploadersConfig config);
    }

    public interface ITextUploadService : IUploadService<TextDestination>
    {
        ITextUploader CreateUploader(UploadersConfig config, string textFormat);
    }

    public interface IServiceFactory<out TService, in TEnum>
        where TService : IUploadService<TEnum>
    {
        TService GetServiceById(string serviceId);

        TService GetServiceByEnumValue(TEnum enumValue);
    }

    public interface IURLShortenerServiceFactory : IServiceFactory<IURLShortenerService, UrlShortenerType>
    {
    }

    internal abstract class UploadServiceFactory<TService, TEnum> : IServiceFactory<TService, TEnum>
        where TService : IUploadService<TEnum>
    {
        private readonly IEnumerable<IUploadService<TEnum>> _services;

        protected UploadServiceFactory(IEnumerable<IUploadService<TEnum>> services)
        {
            _services = services;
        }

        public TService GetServiceById(string serviceId)
        {
            return (TService)_services.FirstOrDefault(s => s.ServiceId == serviceId);
        }

        public TService GetServiceByEnumValue(TEnum enumValue)
        {
            return (TService)_services.FirstOrDefault(s => enumValue.Equals(s.EnumValue));
        }
    }

    internal class URLShortenerServiceFactory : UploadServiceFactory<IURLShortenerService, UrlShortenerType>, IURLShortenerServiceFactory
    {
        public URLShortenerServiceFactory(IEnumerable<IURLShortenerService> services)
            : base (services)
        {
        }
    }

    internal class TextUploadServiceFactory : UploadServiceFactory<ITextUploadService, TextDestination>, ITextUploaderServiceFactory
    {

        public TextUploadServiceFactory(IEnumerable<ITextUploadService> services)
            : base(services)
        {
        }
    }

    public interface ITextUploaderServiceFactory : IServiceFactory<ITextUploadService, TextDestination>
    {
    }
}