using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.Qtum.Sign.Client 
{
    /// <summary>
    /// Qtum.Sign client settings.
    /// </summary>
    public class QtumSignServiceClientSettings 
    {
        /// <summary>Service url.</summary>
        [HttpCheck("api/isalive")]
        public string ServiceUrl {get; set;}
    }
}
