using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FollowerInteractor.Database.Entities
{
    public class DirectMessageTemplate
    {
        public virtual long id { get; set; }
        public virtual string message { get; set; }
        public virtual long fk_hashtag { get; set; }
    }
}
