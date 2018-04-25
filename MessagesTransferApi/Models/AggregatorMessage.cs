using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MessagesTransferApi.Models
{
    public class AggregatorMessage
    {
        [Required(ErrorMessage = "You need to input network name")]
        public string NetworkName { get; set; }

        [Required(ErrorMessage = "You need to input receiver id")]
        public string ReceiverId { get; set; }

        [Required(ErrorMessage = "You need to input message text")]
        public string Text { get; set; }
    }
}
