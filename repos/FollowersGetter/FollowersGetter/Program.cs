using FollowersGetter.Database.Entities;
using InstaSharper.API;
using InstaSharper.API.Builder;
using InstaSharper.Classes;
using InstaSharper.Classes.Models;
using InstaSharper.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FollowersGetter
{
    public class Program
    {
        private static IInstaApi instaApi;
        private static DataRepository dataRepository = new DataRepository();
        private static Config configuration;
        private static List<InstaUser> users;
        private static bool isfirst;
        private static List<InstaUser> accountsthatarefinished;
        private static void Main(string[] args)
        {
            configuration = new Config();
            configuration.SetDefault();
            configuration.HashtagCategory = "music";
            configuration.Username = "vibeclubeurope";
            configuration.Password = "bonez187";
            configuration.MaxFollowers = 1000000000000000;
            configuration.MinFollowers = 0;
            configuration.MinLikes = 0;
            configuration.MaxComments = 10000000000;
            configuration.MaxMediasOfUser = 200;
            configuration.UsersToSearch = 110;
            Initialize();
            Console.ReadKey();
        }

        private static void Initialize()
        {
            isfirst = true;
            var hashtags = dataRepository.LoadHashtagsByCategory(configuration.HashtagCategory);
            Logiin(hashtags);
        }

        private static async void Login(List<Hashtags> tags)
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

            againlogin:
            var loginrequest = await instaApi.LoginAsync();
            try
            {
                if (loginrequest.Succeeded)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Login succeeded\n");
                    Console.WriteLine("Username: '" + configuration.Username + "'\nPassword: '" + configuration.Password + "'\n\n");

                    Console.WriteLine("Process:");
                    GetPeopleByHashtags(tags);
                    var logout = await instaApi.LogoutAsync();
                }
                else
                {
                    Console.WriteLine("Login failed");
                    goto againlogin;
                }
            }
            catch (Exception e)
            {
                goto againlogin; 
            }
        }
        private static async void Logiin(List<Hashtags> tags)
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
                    GetPeopleByHashtags(tags);
                    //var logout = await _instaApi.LogoutAsync();
                    //  if (logout.Succeeded)
                    //  {
                    //     Console.WriteLine("Logout succeeded");
                    //      LikePictures();
                    //  }
                    //  }
                    //   else
                    //    {
                    Console.WriteLine("Logout failed");
                    //    }
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
        private static async void GetPeopleByHashtags(List<Hashtags> hashtags)
        {
            accountsthatarefinished = new List<InstaUser>();
            var badwords = dataRepository.LoadBadWords();
            var insertedAccounts = new List<AccountsToCheck>();
            var tagobjects = new List<TagObject>();
            var insertspertag = Math.Round(configuration.UsersToSearch / (double)hashtags.Count);
            foreach (var tag in hashtags)
            {
                tagobjects.Add(new TagObject()
                {
                    Hashtag = tag,
                    NumberOfInserts = 0
                });
            }

            again:
            foreach (var obj in tagobjects)
            {
                if (obj.NumberOfInserts >= insertspertag)
                {
                    hashtags.Remove(obj.Hashtag);
                }
            }

            try
            {
                foreach (var tag in hashtags)
                {
                    if (isfirst)
                    {
                        IResult<InstaTagFeed> mediasWithTag = await instaApi.GetTagFeedAsync(tag.hashtag, PaginationParameters.MaxPagesToLoad(5));
                        users = mediasWithTag.Value.Medias.Select(media => media.User).ToList();
                    }
                    foreach (var user in users)
                    {
                        accountsthatarefinished.Add(user);
                        if (insertedAccounts.Any(acc => acc.account_pk == user.Pk.ToString()))
                        {
                            continue;
                        }
                        var medias = await instaApi.GetUserMediaAsync(user.UserName, PaginationParameters.MaxPagesToLoad(1));
                        var mediasofUser = medias.Value.ToList();
                        if (!UserIsAppropiate(user, mediasofUser, badwords))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("User " + user.UserName + " is not appropriate\n");
                            Console.ForegroundColor = ConsoleColor.White;
                            continue;
                        }

                        var userIsInterestedInHashtag = CheckUserInterest(user, mediasofUser, hashtags);
                        if (userIsInterestedInHashtag)
                        {
                            var entity = CreateEntity(user, tag);
                            dataRepository.InsertAccountToLike(entity);
                            insertedAccounts.Add(entity);
                            var tagobject = tagobjects.Where(obj => obj.Hashtag.hashtagid == tag.hashtagid).Single();
                            tagobject.NumberOfInserts++;
                            Console.ForegroundColor = ConsoleColor.Green;

                            Console.Write("User " + user.UserName + " is logged by tag-category '" + tag.category + "'\n");
                            Console.ForegroundColor = ConsoleColor.White;

                            if (insertedAccounts.Count() >= configuration.UsersToSearch)
                            {
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("Process finished, " + insertedAccounts.Count() + " inserted");
                                return;
                            }

                            if (tagobject.NumberOfInserts >= insertspertag)
                            {
                                goto again;
                            }
                        }

                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("User " + user.UserName.ToString() + " is not interested in tag-category '" + tag.category.ToString() + "'\n");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }
                }

                if (insertedAccounts.Count < configuration.UsersToSearch)
                {
                    isfirst = true;
                    goto again;
                }
            }
            catch (Exception e)
            {
                Console.Write(e);
                isfirst = false;
                var newlist = new List<InstaUser>();
                newlist.AddRange(users);
                Console.Write("\nEXCEPTION OCCURED \n");
                foreach (var account in accountsthatarefinished)
                {
                    foreach (var acc in users)
                    {
                        if (account.UserName == acc.UserName)
                        {
                            newlist.Remove(acc);
                        }
                    }
                }

                users = newlist;
                goto again;
            }
        }

        private static bool UserIsAppropiate(InstaUser user, List<InstaMedia> medias, List<Badword> badwords)
        {
            //VERIFIZIERT
            if (configuration.HasToBeVerified && !user.IsVerified)
            {
                return false;
            }
            if (!configuration.HasToBeVerified && user.IsVerified)
            {
                return false;
            }
            //ANZAHL FOLLOWER
            if (user.FollowersCount < configuration.MinFollowers)
            {
                return false;
            }
            if (user.FollowersCount > configuration.MaxFollowers)
            {
                return false;
            }
            //BADWORDS IN NAME / USERNAME
            if (badwords.Any(word => user.UserName.Contains(word.badword)))
            {
                return false;
            }
            //LIKES + COMMENTS FÜR 1stes BILDs
            if (medias.Count != 0)
            {
                if (badwords.Any(word => user.FullName.Contains(word.badword)))
                {
                    return false;
                }
                if (medias.First().LikesCount < configuration.MinLikes)
                {
                    return false;
                }
                if (Int32.Parse(medias.First().CommentsCount) > configuration.MaxComments)
                {
                    return false;
                }
            }
            return true;
        }

        private static bool CheckUserInterest(InstaUser user, List<InstaMedia> mediasOfUser, List<Hashtags> hashtag)
        {
            int indizesForInterest = 0;
            if (hashtag.Any(x => user.FullName.Contains(x.hashtag)))
            {
                indizesForInterest++;
            }
            foreach (var media in mediasOfUser)
            {

                if (media.Caption != null)
                {
                    if (hashtag.Any(x => media.Caption.Text.Contains(x.hashtag)))
                    {
                        indizesForInterest++;
                    }
                }

                foreach (var comment in media.PreviewComments)
                {
                    if (hashtag.Any(x => comment.Text.Contains(x.hashtag)))
                    {
                        indizesForInterest++;
                    }
                }
            }

            if (indizesForInterest > 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static AccountsToCheck CreateEntity(InstaUser user, Hashtags tag)
        {
            return new AccountsToCheck()
            {
                account_pk = user.Pk.ToString(),
                username = user.UserName,
                fk_hashtag = tag.hashtagid
            };
        }
    }
}
