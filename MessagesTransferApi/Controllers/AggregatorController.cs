using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MessagesTransferApi.Data.Contexts;
using MessagesTransferApi.Logic;
using MessagesTransferApi.Models;
using MessagesTransferApi.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MessagesTransferApi.Controllers
{
    [Route("api/[Controller]")]
    public class AggregatorController : Controller
    {
        private DataContext _context;

        private ITokenGeneratorService _tokenGenerator;
        private IConnectorSenderService _connectorSender;

        public AggregatorController(DataContext context, ITokenGeneratorService tokenGenerator, 
                                        IConnectorSenderService connetorSender)
        {
            this._context = context;
            this._tokenGenerator = tokenGenerator;
            this._connectorSender = connetorSender;
        }

        [HttpGet]
        [Route("Users")]
        public IActionResult GetUsers()
        {
            return Json(_context.Users);
        }

        [HttpGet]
        [Route("Accounts")]
        public IActionResult GetAccounts()
        {
            return Json(_context.Accounts);
        }

        [HttpPost]
        [Route("Users")]
        public async Task<IActionResult> AddUser([FromBody] UserData userData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User existedUser = await _context
                .Users
                .FirstOrDefaultAsync(u => u.Login == userData.Login);
            if (existedUser != null)
            {
                return BadRequest("Already have such login");
            }

            string userToken = _tokenGenerator.GenerateToken(userData.Login);

            User user = new User()
            {
                Login = userData.Login,
                FeedbackUrl = userData.Url,
                UserToken = userToken
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return Json(new
            {
                generatedToken = userToken
            });
        }

        [HttpPost]
        [Route("Webhook")]
        public async Task<IActionResult> AttachWebhook([FromBody] string url, [FromQuery] string userToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User user = await _context
                .Users
                .FirstOrDefaultAsync(u => u.UserToken == userToken);
            if (user == null)
            {
                return NotFound("Invalid token");
            }

            user.FeedbackUrl = url;

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        [Route("Users/Accounts")]
        public async Task<IActionResult> AttachAccount([FromBody] Account account, [FromQuery] string userToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User user = await _context
                .Users
                .Include(u => u.Accounts)
                .FirstOrDefaultAsync(u => u.UserToken == userToken);

            if (user == null)
            {
                return NotFound("Invalid token");
            }

            Connector connector = _context
                .Connectors
                .FirstOrDefault(c => c.NetworkName == account.NetworkName);

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

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        [Route("Messages")]
        public async Task<IActionResult> SendMessage([FromBody] AggregatorMessage message, [FromQuery] string userToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User user = await _context
                .Users
                .Include(u => u.Accounts)
                .FirstOrDefaultAsync(u => u.UserToken == userToken);
            if (user == null)
            {
                return NotFound("Invalid token");
            }

            string accessToken = user
                .Accounts
                .FirstOrDefault(a => a.PlatformName == message.NetworkName)
                .AccessToken;
            if (accessToken == null)
            {
                return BadRequest("Access token for network not found");
            }

            Connector connector = await _context
                .Connectors
                .FirstOrDefaultAsync(c => c.NetworkName == message.NetworkName);
            Message dataMessage = new Message()
            {
                NetworkName = message.NetworkName,
                NetworkUserId = message.ReceiverId,
                Text = message.Text
            };

            _connectorSender.SendMessage(connector, dataMessage, accessToken);

            return Ok();
        }
    }
}