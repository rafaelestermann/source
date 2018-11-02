using FollowersGetter.Database;
using FollowersGetter.Database.Entities;
using NHibernate;
using System.Collections.Generic;
using System.Linq;

namespace FollowersGetter
{
    public class DataRepository
    {
        //LOAD ALL
        public List<Hashtags> LoadHashtagsByCategory(string category)
        {
            List<Hashtags> hashtags = null;
            using (ISession session = NhibernateSession.OpenSession())
            {
                hashtags = session.Query<Hashtags>().Where(hashtag => hashtag.category == category).ToList();
            }
            return hashtags;
        }

        public List<Badword> LoadBadWords()
        {
            List<Badword> words = null;
            using (ISession session = NhibernateSession.OpenSession())
            {
                words = session.Query<Badword>().ToList();
            }
            return words;
        }
        public void InsertAccountToLike(AccountsToCheck account)
        {
            var session = NhibernateSession.OpenSession();
            using (var transaction = session.BeginTransaction())
            {
                session.SaveOrUpdate(account);
                transaction.Commit();
            }
        }
        public void InsertAccountsToLike(IEnumerable<AccountsToCheck> accounts)
        {
            var session = NhibernateSession.OpenSession();
            using (var transaction = session.BeginTransaction())
            {
                accounts = accounts.ToList();              

                foreach (var entity in accounts)
                    session.SaveOrUpdate(entity);

                transaction.Commit();
            }
        }
    } 
}