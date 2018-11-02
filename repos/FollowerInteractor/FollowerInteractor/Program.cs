using FollowerInteractor.Database.Entities;
using FollowersGetter;
using InstaSharper.API;
using InstaSharper.API.Builder;
using InstaSharper.Classes;
using InstaSharper.Classes.Models;
using InstaSharper.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FollowerInteractor
{
    public class Program
    {
        private static IInstaApi instaApi;
        private static DataRepository dataRepository = new DataRepository();
        private static List<InstaUser> users;
        private static bool isfirst;
        private static List<InstaUser> accountsthatarefinished;
        private static void Main(string[] args)
        {
            Initialize();
            Console.ReadKey();
        }

        private static void Initialize()
        {
            isfirst = true;
            Login();
        }
        private static async void Login()
        {
            var userSessionData = new UserSessionData
            {
                UserName = "vibeclubeurope",
                Password = "bonez187"
            };
            int i = 0;
            int y = 300;
            var delay = RequestDelay.FromSeconds(i, y);
            instaApi = InstaApiBuilder.CreateBuilder()
                .SetUser(userSessionData)
                .UseLogger(new DebugLogger(LogLevel.Exceptions)) // use logger for requests and debug messages
                .SetRequestDelay(delay)
                .Build();

            again:
            var loginrequest = await instaApi.LoginAsync();
            try
            {
                if (loginrequest.Succeeded)
                {
                    Console.WriteLine("Login succeeded");
                    Interact();
                    Console.WriteLine("Logout failed");                   
                }
                else
                {
                    Console.WriteLine("Login failed");
                    i++;
                    delay = RequestDelay.FromSeconds(i, i);
                    goto again;
                }
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
        }

        private static async void Interact()
        {
            var templates = dataRepository.GetAllDirectMessageTemplates();
            var persons = dataRepository.GetAllPersonsToInteract();
            List<InstaUser> accounts = new List<InstaUser>();
            foreach(var person in persons)
            {
                var user = await instaApi.GetUserAsync(person.username);
                user.Value.HashtagId = person.fk_hashtag;
                accounts.Add(user.Value);
            }
           
            foreach(var account in accounts)
            {
                string message = null;
                //specific Message
                if(templates.Any(template => template.fk_hashtag == account.HashtagId))
                {
                    message = templates.First(x => x.fk_hashtag == account.HashtagId).message;
                }
                else
                {
                    var allhashtags = dataRepository.LoadAllHashtags();
                    var category = allhashtags.Where(x => x.hashtagid == account.HashtagId).First().category;
                    var hashtagsInCategory = allhashtags.Where(x => x.category == category).Select(x => x.hashtagid);
                    foreach(var hashtagid in hashtagsInCategory)
                    {
                        foreach(var template in templates)
                        {
                            if(hashtagid == template.fk_hashtag)
                            {
                                message = templates.First(x => x.fk_hashtag == hashtagid).message;
                            }
                        }
                    }
                }

                //Default
                if (message == null)
                {
                    Random rnd = new Random();
                    List<string> defaultMessages = templates.Where(x => x.fk_hashtag == 0).Select(x => x.message).ToList();
                    int r = rnd.Next(defaultMessages.Count());                
                    message = defaultMessages[r].ToString();
                }
                Thread.Sleep(30000);
                message = message.Replace("username", account.UserName);
                var result = await instaApi.SendDirectMessage(account.Pk.ToString(),string.Empty, message);
                dataRepository.DeleteAccountToCheck(persons.Where(x => x.username == account.UserName).First());
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Interaction completed for User:" + account.UserName);
            }
         }
    }
}
