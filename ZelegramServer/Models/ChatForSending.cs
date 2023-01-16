using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZelegramServer.Models
{
    public class ChatForSending
    {
        public int Id { get; set; }
        public List<Message> Messages { get; set; }
        public string Name { get; set; }
        public List<User> Users { get; set; }
    }
}
