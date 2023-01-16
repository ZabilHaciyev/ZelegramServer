using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZelegramServer.Models
{

    public  class User
    {
       
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Mail { get; set; }
        public byte[] Photo { get; set; }
        public ICollection<Chat> Chats { get; set; }
    }
}
