using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZelegramServer.Models;

namespace ZelegramServer.Entity
{
    public class ZelegramDb:DbContext
    {
        public ZelegramDb() : base("Connection") { }
        public DbSet<User> Users { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
       
    }
}
