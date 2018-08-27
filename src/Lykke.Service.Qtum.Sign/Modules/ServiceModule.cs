using Autofac;
using Lykke.Service.Qtum.Sign.Core.Services;
using Lykke.Service.Qtum.Sign.Services;
using Lykke.Service.Qtum.Sign.Settings;
using Lykke.SettingsReader;

namespace Lykke.Service.Qtum.Sign.Modules
{
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<AppSettings> _appSettings;

        public ServiceModule(IReloadingManager<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<QtumService>()
                .As<IQtumService>()
                .SingleInstance()
                .WithParameter("network", _appSettings.Nested(s => s.Network).CurrentValue);
        }
    }
}
