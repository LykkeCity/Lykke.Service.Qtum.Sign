using Lykke.HttpClientGenerator;

namespace Lykke.Service.Qtum.Sign.Client
{
    /// <summary>
    /// Qtum.Sign API aggregating interface.
    /// </summary>
    public class QtumSignClient : IQtumSignClient
    {
        // Note: Add similar Api properties for each new service controller

        /// <summary>Inerface to Qtum.Sign Api.</summary>
        public IQtumSignApi Api { get; private set; }

        /// <summary>C-tor</summary>
        public QtumSignClient(IHttpClientGenerator httpClientGenerator)
        {
            Api = httpClientGenerator.Generate<IQtumSignApi>();
        }
    }
}
