using NHibernate;
using NHibernate.Cfg;
using Tortuga.Chain;

namespace Recipes.NHibernate.Repositories
{
    public class NHibernateHelper
    {
        private static ISessionFactory _sessionFactory;

        private static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                {
                    var configuration = new Configuration();
                    configuration.Configure();
                    configuration.AddAssembly(typeof(EmployeeClassificationRepository).Assembly);
                    _sessionFactory = configuration.BuildSessionFactory();

                    //Add Chain support to NHibernate
                    _sessionFactory.RegisterDataSource(cs => new SqlServerDataSource(cs));
                }
                return _sessionFactory;
            }
        }

        public static ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }
    }
}
