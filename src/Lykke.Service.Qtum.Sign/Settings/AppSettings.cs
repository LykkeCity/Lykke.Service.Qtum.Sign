using JetBrains.Annotations;
using Lykke.Sdk.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Service.Qtum.Sign.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public QtumSignSettings QtumSignService { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public string Network { get; set; }
    }
}
