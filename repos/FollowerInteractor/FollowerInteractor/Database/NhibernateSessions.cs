using NHibernate;
using NHibernate.Cfg;
using System.IO;

namespace FollowerInteractor.Database
{
    public class NhibernateSession
    {
        public static ISession OpenSession()
        {
            var configuration = new Configuration();
            configuration.Configure(GetExactPath(@"Database/hibernate.cfg.xml"));

            var hashtagsconfigfile = GetExactPath(@"Database/Mappings/hashtags.hbm.xml");
            var accountstocheckconfigfile = GetExactPath(@"Database/Mappings/accountstocheck.hbm.xml");
            var badwordconfigfile = GetExactPath(@"Database/Mappings/badword.hbm.xml");
            var dmconfigfile = GetExactPath(@"Database/Mappings/directmessagetemplate.hbm.xml");
            configuration.AddFile(hashtagsconfigfile).AddFile(accountstocheckconfigfile).AddFile(badwordconfigfile).AddFile(dmconfigfile);

            ISessionFactory sessionFactory = configuration.BuildSessionFactory();
            return sessionFactory.OpenSession();
        }

        private static string GetExactPath(string relativePath)
        {
            string exactPath = Path.GetFullPath(relativePath);
            exactPath = exactPath.Replace("bin\\Debug\\", "");
            return exactPath;
        }
    }
}