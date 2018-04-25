using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MessagesTransferApi.Models
{
    public class UserData
    {
        [Required(ErrorMessage ="You need to input login!")]
        public string Login { get; set; }

        public string Url { get; set; }
    }
}
