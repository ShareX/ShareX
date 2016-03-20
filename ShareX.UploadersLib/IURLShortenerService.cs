using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ShareX.UploadersLib.Controls;

namespace ShareX.UploadersLib
{
    public interface IUploadService<out TEnum>
    {
        [Localizable(false)]
        string ServiceId { get; }

        TEnum EnumValue { get; }

        IUploadServiceConfig CreateConfig();
    }

    public interface IUploadServiceConfig
    {
        string TabText { get; }
        object TabImage { get; }

        BaseConfigControl CreateConfigControl(UploadersConfig config);
    }

    public interface IURLShortenerService : IUploadService<UrlShortenerType>
    {
        IURLShortener CreateShortener(UploadersConfig config);
    }

    public interface ITextUploadService : IUploadService<TextDestination>
    {
        ITextUploader CreateUploader(UploadersConfig config, string textFormat);
    }

    public interface IUploadServiceFactory<out TService, in TEnum>
        where TService : IUploadService<TEnum>
    {
        TService GetServiceById(string serviceId);

        TService GetServiceByEnumValue(TEnum enumValue);

        IEnumerable<TService> GetAllServices();
    }

    public interface IURLShortenerServiceFactory : IUploadServiceFactory<IURLShortenerService, UrlShortenerType>
    {
    }

    internal abstract class UploadServiceFactory<TService, TEnum> : IUploadServiceFactory<TService, TEnum>
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

        public IEnumerable<TService> GetAllServices() => _services.Cast<TService>();
    }

    internal class URLShortenerServiceFactory : UploadServiceFactory<IURLShortenerService, UrlShortenerType>, IURLShortenerServiceFactory
    {
        public URLShortenerServiceFactory(IEnumerable<IURLShortenerService> services)
            : base (services)
        {
        }
    }

    internal class TextUploadServiceFactory : UploadServiceFactory<ITextUploadService, TextDestination>, ITextUploadServiceFactory
    {

        public TextUploadServiceFactory(IEnumerable<ITextUploadService> services)
            : base(services)
        {
        }
    }

    public interface ITextUploadServiceFactory : IUploadServiceFactory<ITextUploadService, TextDestination>
    {
    }

    public interface IUploadServicesFactory
    {
        ITextUploadServiceFactory TextUpload { get; }

        IURLShortenerServiceFactory URLShortener { get; }
    }

    internal class UploadServicesFactory : IUploadServicesFactory
    {
        public ITextUploadServiceFactory TextUpload { get; }
        public IURLShortenerServiceFactory URLShortener { get; }

        public UploadServicesFactory(ITextUploadServiceFactory textUpload, IURLShortenerServiceFactory urlShortener)
        {
            TextUpload = textUpload;
            URLShortener = urlShortener;
        }
    }
}