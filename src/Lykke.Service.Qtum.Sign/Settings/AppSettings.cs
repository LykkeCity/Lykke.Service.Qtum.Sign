using JetBrains.Annotations;
using Lykke.Sdk.Settings;

namespace Lykke.Service.Qtum.Sign.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public QtumSignSettings QtumSignService { get; set; }
    }
}
