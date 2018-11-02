using FollowerInteractor.Database;
using FollowerInteractor.Database.Entities;
using NHibernate;
using System;
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

        public List<Hashtags> LoadAllHashtags()
        {
            List<Hashtags> hashtags = null;
            using (ISession session = NhibernateSession.OpenSession())
            {
                hashtags = session.Query<Hashtags>().ToList();
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

        public List<AccountsToCheck> GetAllPersonsToInteract()
        {
            List<AccountsToCheck> person = null;
            using (ISession session = NhibernateSession.OpenSession())
            {
                person = session.Query<AccountsToCheck>().ToList();
            }
            return person;
        }


        public List<DirectMessageTemplate> GetAllDirectMessageTemplates()
        {
            List<DirectMessageTemplate> templates = null;
            using (ISession session = NhibernateSession.OpenSession())
            {
                templates = session.Query<DirectMessageTemplate>().ToList();
            }
            return templates;
        }

        public void DeleteAccountToCheck(AccountsToCheck account)
        {
            using (ISession session = NhibernateSession.OpenSession())
            {    
                session.Delete(account);
            }
        }
    } 
}