using NHibernate;
using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using Tortuga.Chain;
using Tortuga.Chain.DataSources;

namespace Recipes.NHibernate.Repositories
{
    public static class ChainForNHibernate
    {
        static ConcurrentDictionary<string, IRootDataSource> s_DataSources = new ConcurrentDictionary<string, IRootDataSource>();

        public static void RegisterDataSource<T>(this ISessionFactory sessionFactory, T dataSource) where T : IRootDataSource, IClass2DataSource
        {
            using (var session = sessionFactory.OpenSession())
            {
                s_DataSources[session.Connection.ConnectionString] = dataSource;
            }
        }

        /// <summary>
        /// Registers the data source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sessionFactory">The session factory. The input parameter is a connection string extracted from the session factory.</param>
        /// <param name="dataSourceFactory">The data source factory.</param>
        public static void RegisterDataSource<T>(this ISessionFactory sessionFactory, Func<string, T> dataSourceFactory) where T : IRootDataSource, IClass2DataSource
        {
            using (var session = sessionFactory.OpenSession())
            {
                s_DataSources[session.Connection.ConnectionString] = dataSourceFactory(session.Connection.ConnectionString);
            }
        }

        public static IClass2DataSource Chain(this ISession session)
        {

            //These casts won't be necessary with Chain Version 1.1.
            DbConnection connection = (DbConnection)session.Connection;
            DbTransaction transaction = (DbTransaction)GetTransaction(session);

            return (IClass2DataSource)s_DataSources[session.Connection.ConnectionString].CreateOpenDataSource(connection, transaction);
        }

        //http://ayende.com/blog/1583/i-hate-this-code

        private static IDbTransaction GetTransaction(ISession session)

        {
            using (var command = session.Connection.CreateCommand())
            {
                session.Transaction.Enlist(command);
                return command.Transaction;
            }
        }

    }
}
