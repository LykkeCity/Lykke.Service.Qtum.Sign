using JetBrains.Annotations;

namespace Lykke.Service.Qtum.Sign.Client
{
    /// <summary>
    /// Qtum.Sign client interface.
    /// </summary>
    [PublicAPI]
    public interface IQtumSignClient
    {
        /// <summary>Application Api interface</summary>
        IQtumSignApi Api { get; }
    }
}
