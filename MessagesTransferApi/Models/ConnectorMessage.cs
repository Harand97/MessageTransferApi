using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MessagesTransferApi.Models
{
    public class ConnectorMessage
    {
        [Required(ErrorMessage = "You need to input sender id")]
        public string SenderId { get; set; }

        [Required(ErrorMessage = "You need to input receiver access token")]
        public string AccessToken { get; set; }

        [Required(ErrorMessage = "You need to input message text")]
        public string Text { get; set; }
    }
}
