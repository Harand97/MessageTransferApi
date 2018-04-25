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
    public class ConnectorController : Controller
    {
        private DataContext _context;

        private IAggregatorSenderService _aggregatorSender;

        public ConnectorController(DataContext context, IAggregatorSenderService aggregatorSender)
        {
            this._context = context;
            this._aggregatorSender = aggregatorSender;
        }

        [HttpGet]
        public IActionResult GetConnectors()
        {
            return Json(_context.Connectors);
        }

        [HttpPost]
        public async Task<IActionResult> AttachConnector([FromBody] ConnectorData connectorData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Connector connector = new Connector()
            {
                NetworkName = connectorData.NetworkName,
                Url = connectorData.Url
            };

            _context.Connectors.Add(connector);

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        [Route("Messages")]
        public async Task<IActionResult> SendMessage([FromBody] ConnectorMessage messageData, [FromQuery] string networkName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            NetworkAuthData account = await _context
                .Accounts
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.PlatformName == networkName && a.AccessToken == messageData.AccessToken);
            if (account == null)
            {
                return BadRequest("Wrong account info");
            }

            Message message = new Message()
            {
                NetworkName = networkName,
                Text = messageData.Text,
                NetworkUserId = messageData.SenderId
            };

            _aggregatorSender.SendMessage(account.User, message);

            return Ok();
        }
    }
}