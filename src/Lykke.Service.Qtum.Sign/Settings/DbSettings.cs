using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.Qtum.Sign.Settings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }
    }
}
