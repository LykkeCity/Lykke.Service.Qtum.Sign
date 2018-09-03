using Lykke.Service.Qtum.Sign.Core.Services;
using Lykke.Service.Qtum.Sign.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Lykke.Service.Qtum.Sign.Controllers
{
    [Route("api/wallets")]
    public class WalletsController : Controller
    {
        private readonly IQtumService _qtumService;

        public WalletsController(IQtumService qtumService)
        {
            _qtumService = qtumService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(WalletResponse), (int)HttpStatusCode.OK)]
        public WalletResponse Post()
        {
            var privateKey = _qtumService.GetPrivateKey();
            var publicAddress = _qtumService.GetPublicAddress(privateKey);

            return new WalletResponse()
            {
                PrivateKey = privateKey,
                PublicAddress = publicAddress
            };
        }
    }
}
