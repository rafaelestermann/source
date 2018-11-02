using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FollowerInteractor.Database.Entities
{
    public class AccountsToCheck
    {
        public virtual long id { get; set; }
        public virtual string account_pk { get; set; }
        public virtual string username { get; set; }
        public virtual long fk_hashtag { get; set; }
    }
}
