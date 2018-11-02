using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FollowersGetter.Database.Entities
{
    public class Hashtags
    {
        public virtual long hashtagid { get; set; }
        public virtual string category { get; set; }
        public virtual string hashtag { get; set; }
    }
}
