namespace FollowersGetter
{
    public class Config
    {
        public void SetDefault()
        {
            HashtagCategory = "music";
            Username = "vibeclubeurope";
            Password = "bonez187";
            MaxFollowers = 1000;
            MinFollowers = 80;
            MaxComments = 25;
            MinLikes = 60;
            MaxMediasOfUser = 12;
            UsersToSearch = 100;
        }
        public string HashtagCategory { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public long UsersToSearch { get; set; }
        public long MinFollowers { get; set; }
        public long MinLikes { get; set; }
        public long MaxFollowers { get; set; }
        public long MaxComments { get; set; }
        public long MinComments { get; set; }
        public long MaxMediasOfUser { get; set; }
        public bool HasToBeVerified { get; set; }
    }
}
