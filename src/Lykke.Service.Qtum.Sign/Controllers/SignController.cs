using Lykke.Service.Qtum.Sign.Core.Services;
using Lykke.Service.Qtum.Sign.Models;
using Lykke.Service.Qtum.Sign.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Lykke.Service.Qtum.Sign.Controllers
{
    [Route("api/sign")]
    public class SignController : Controller
    {
        private readonly IQtumService _qtumService;

        public SignController(IQtumService qtumService)
        {
            _qtumService = qtumService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(SignResponse), (int)HttpStatusCode.OK)]
        public IActionResult SignTransaction([FromBody]SignTransactionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.ToErrorResponse());
            }

            var hex = _qtumService.SignTransaction(request.Tx, request.Coins, request.Keys);

            return Ok(new SignResponse()
            {
                SignedTransaction = hex
            });
        }        
    }
}
