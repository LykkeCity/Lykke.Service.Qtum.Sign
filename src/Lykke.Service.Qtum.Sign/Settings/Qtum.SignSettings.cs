using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.Qtum.Sign.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class QtumSignSettings
    {
        public DbSettings Db { get; set; }
    }
}
