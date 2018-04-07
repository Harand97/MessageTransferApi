using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MessagesTransferApi.Data.Contexts;
using MessagesTransferApi.Logic;
using MessagesTransferApi.Models;
using MessagesTransferApi.Data.Models;

namespace MessagesTransferApi.Controllers
{
    [Route("api/[Controller]")]
    public class AggregatorController : Controller
    {
        private DataContext _context;

        private ITokenGeneratorService _tokenGenerator;

        private IConnectorSenderService _connectorSender;

        public AggregatorController(DataContext context, ITokenGeneratorService tokenGenerator, IConnectorSenderService connetorSender)
        {
            this._context = context;
            this._tokenGenerator = tokenGenerator;
            this._connectorSender = connetorSender;
        }

        [HttpPost]
        [Route("Users")]
        public ActionResult AddUser([FromBody] UserData userData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            string userToken = _tokenGenerator.GenerateToken(userData.Login);

            User user = new User()
            {
                Login = userData.Login,
                FeedbackUrl = userData.Url,
                UserToken = userToken
            };

            _context.Users.Add(user);

            _context.SaveChanges();

            return Json(new
            {
                generatedToken = userToken
            });
        }

        [HttpPost]
        [Route("Webhook")]
        public ActionResult AttachWebhook([FromBody] UserData userData, [FromBody] string userToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            User user = _context.Users.FirstOrDefault(u => u.UserToken == userToken);
            if (user == null)
            {
                return NotFound("Invalid token");
            }

            user.FeedbackUrl = userData.Url;

            _context.SaveChanges();

            return Ok();
        }

        [HttpPost]
        [Route("Users/Accounts")]
        public ActionResult AttachAccount([FromBody] Account account, [FromBody] string userToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            User user = _context.Users.FirstOrDefault(u => u.UserToken == userToken);
            if (user == null)
            {
                return NotFound("Invalid token");
            }

            Connector connector = _context.Connectors.FirstOrDefault(c => c.NetworkName == account.NetworkName);
            if (connector == null)
            {
                return NotFound("Not implemented network");
            }

            NetworkAuthData networkAuthData = new NetworkAuthData()
            {
                UserId = user.Id,
                User = user,
                PlatformName = account.NetworkName,
                AccessToken = account.AccessToken
            };

            _connectorSender.SendAccountData(connector, networkAuthData);

            user.Accounts.Add(networkAuthData);

            _context.Accounts.Add(networkAuthData);

            _context.SaveChanges();

            return Ok();
        }

        [HttpPost]
        [Route("Messages")]
        public ActionResult SendMessage([FromBody] Models.Message message, [FromBody] string userToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            User user = _context.Users.FirstOrDefault(u => u.UserToken == userToken);
            if (user == null)
            {
                return NotFound("Invalid token");
            }

            string accessToken = user.Accounts.FirstOrDefault(a => a.PlatformName == message.NetworkName).AccessToken;
            if (accessToken == null)
            {
                return BadRequest("Access token for network not found");
            }

            Connector connector = _context.Connectors.FirstOrDefault(c => c.NetworkName == message.NetworkName);
            Data.Models.Message dataMessage = new Data.Models.Message()
            {
                NetworkName = message.NetworkName,
                ReceiverId = message.ReceiverId,
                Text = message.Text
            };

            _connectorSender.SendMessage(connector, dataMessage, accessToken);

            return Ok();
        }
    }
}