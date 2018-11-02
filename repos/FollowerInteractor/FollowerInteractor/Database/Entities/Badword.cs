using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FollowerInteractor.Database.Entities
{
    public class Badword
    {
        public virtual long id { get; set; }
        public virtual string badword { get; set; }
    }
}
