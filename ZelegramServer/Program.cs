using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ZelegramServer.Entity;
using ZelegramServer.Models;

namespace ZelegramServer
{
    class Program
    {

        static void Main(string[] args)
        {


            using (var client = new HttpClient())
            {

                HttpListener listener = new HttpListener();
                foreach (var item in Enum.GetNames(typeof(SendType)))
                {
                    listener.Prefixes.Add("http://localhost:8888/" + item + '/');
                }
                listener.Start();
                Console.WriteLine("Listener started ");

                while (true)
                {
                    HttpListenerContext context = listener.GetContext();
                    Console.WriteLine("Client is connected.");

                    HttpListenerRequest req = context.Request;
                    HttpListenerResponse res = context.Response;
                    Console.WriteLine($"User host name : {req.UserHostName}");
                    List<byte> test = new List<byte>();
                    using (var stream = req.InputStream)
                    {
                        int size = 512;
                        while (true)
                        {
                            byte[] temp = new byte[size];
                            var x = stream.Read(temp, 0, size);
                            test.AddRange(temp);
                            if (x < size) break;
                        }
                    }

                    object obj = null;


                    if (req.Url.LocalPath == $"/{Enum.GetName(typeof(SendType), SendType.Finduser)}/")
                    {
                        Console.WriteLine("FindAccount");
                        var str = Encoding.UTF8.GetString(test.ToArray());
                        var user = JsonConvert.DeserializeObject<User>(str);
                        obj = FindUser(user);
                    }
                    else if (req.Url.LocalPath == $"/{Enum.GetName(typeof(SendType), SendType.CreateUser)}/")
                    {
                        Console.WriteLine("CreateUser");
                        var str = Encoding.UTF8.GetString(test.ToArray());
                        var user = JsonConvert.DeserializeObject<User>(str);
                        obj = CreateUser(user);
                    }
                    else if (req.Url.LocalPath == $"/{Enum.GetName(typeof(SendType), SendType.CreateChat)}/")
                    {
                        Console.WriteLine("CreatingChat");
                        var str = Encoding.UTF8.GetString(test.ToArray());
                        var chat = JsonConvert.DeserializeObject<ChatForSending>(str);
                        obj = CreateChat(chat);
                    }
                    else if (req.Url.LocalPath == $"/{Enum.GetName(typeof(SendType), SendType.GetAllUsers)}/")
                    {
                        Console.WriteLine("GetAllUsers");
                        obj = GetAllUsers();
                    }
                    else if (req.Url.LocalPath == $"/{Enum.GetName(typeof(SendType), SendType.GetChats)}/")
                    {
                        Console.WriteLine("GetChats");
                        var str = Encoding.UTF8.GetString(test.ToArray());
                        var chat = JsonConvert.DeserializeObject<Chat>(str);
                        obj = GetChat(chat);
                    }
                    else if (req.Url.LocalPath == $"/{Enum.GetName(typeof(SendType), SendType.AddMessage)}/")
                    {
                        Console.WriteLine("AddMessage");
                        var str = Encoding.UTF8.GetString(test.ToArray());
                        var chat = JsonConvert.DeserializeObject<Chat>(str);
                        obj = AddMessage(chat);
                    }
                    else if (req.Url.LocalPath == $"/{Enum.GetName(typeof(SendType), SendType.GetUsersByName)}/")
                    {
                        Console.WriteLine("GetUsersByName");
                        var str = Encoding.UTF8.GetString(test.ToArray());
                        var username = JsonConvert.DeserializeObject<string>(str);
                        obj = GetUserByName(username);
                    }
                    else if (req.Url.LocalPath == $"/{Enum.GetName(typeof(SendType), SendType.GetChatsForUser)}/")
                    {
                        Console.WriteLine("GetChatsForUser");
                        var str = Encoding.UTF8.GetString(test.ToArray());
                        var user = JsonConvert.DeserializeObject<User>(str);
                        obj = GetChatsForCurrentUser(user);
                    }
                    byte[] buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));

                    res.ContentLength64 = buffer.Length;

                    using (var output = res.OutputStream)
                    {
                        output.Write(buffer, 0, buffer.Length);
                    }

                }
            }


        }

        private static object GetChatsForCurrentUser(User user)
        {
            using (var db = new ZelegramDb())
            {
                var some = db.Users.Include("Chats").FirstOrDefault(x => x.Id == user.Id).Chats.ToList();
                return db.Users.Include("Chats").FirstOrDefault(x => x.Id == user.Id).Chats.ToList();
            }
        }

        private static List<Chat> CreateChat(ChatForSending newChat)
        {
            using (var db = new ZelegramDb())
            {
                var user = new List<User>();
                var currentUser = newChat.Users[0];
                foreach (var item in newChat.Users)
                {
                    user.Add(db.Users.FirstOrDefault(x => x.Id == item.Id));

                }
                var chat = new Chat()
                {
                    Name = newChat.Name,
                    Users = user
                };
                db.Chats.Add(chat);
                db.SaveChanges();
                return db.Users.Include("Chats").FirstOrDefault(x => x.Id == currentUser.Id).Chats.ToList();

            }        
        }

        private static List<User> GetUserByName(string username)
        {
            using (var db = new ZelegramDb())
            {
                return db.Users.Where(x => x.UserName.StartsWith(username) ).ToList();
            }
        }
        private static Chat AddMessage (Chat chat)
        {
            using (var db = new ZelegramDb())
            {

                var countMsg = chat.Messages.Count;
                var lastMsg = chat.Messages.ToList()[countMsg - 1];
                db.Chats.Include("Messages").FirstOrDefault(x=>x.Id==chat.Id).Messages.Add(lastMsg);
                db.SaveChanges();

                return db.Chats.Include("Messages").FirstOrDefault(x => x.Id == chat.Id);


                //var countMsg = user.SendingMessages.Count;
                //var lastMsg = user.SendingMessages.ToList()[countMsg - 1];
                //db.Users. FirstOrDefault(x => x.Id == user.Id).SendingMessages.Add(lastMsg);
                //db.SaveChanges();
                //return db.Users.FirstOrDefault(x => x.Id == user.Id);
            }
        }

        private static Chat GetChat(Chat selectedChat)
        {
            using (var db = new ZelegramDb())
            {
                var searchedChat = db.Chats.Include("Messages").FirstOrDefault(x => x.Id == selectedChat.Id);
                return searchedChat;
            }
        }

        public static User FindUser(User user)
        {
            using (var db= new ZelegramDb())
            {
                var findedUser= db.Users.Include("Chats").FirstOrDefault(x => x.UserName ==user.UserName && x.Password == user.Password);
                if (findedUser != null)
                {
                    return findedUser;
                }
                else return null;
            }
        }
        public static List<User> GetAllUsers()
        {
            using (var db = new ZelegramDb())
            {
                return db.Users.Include("Chats"). ToList<User>(); ;
                 
            }
        }
        public static User CreateUser(User user)
        {
            using (var db = new ZelegramDb())
            {
                var findedUser = db.Users.FirstOrDefault(x => x.UserName == user.UserName);
                if (findedUser == null)
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                    return user;
                }
                else return null;
            }
        }
       
    }
}
