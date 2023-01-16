using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZelegramServer.Models
{
    public enum SendType
    {
        CreateUser, GetAllUsers, UpdateUser, DeleteUser, Finduser,
        CreateChat, UpdateChat, DeleteChat, GetChats,
        AddMessage,GetUsersByName, GetChatsForUser
    }
}
