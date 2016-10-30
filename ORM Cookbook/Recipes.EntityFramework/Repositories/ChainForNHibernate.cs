using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Entity;
using Tortuga.Chain;
using Tortuga.Chain.DataSources;

namespace Recipes.NHibernate.Repositories
{
    public static class ChainForEntityFramework
    {
        static ConcurrentDictionary<string, IRootDataSource> s_DataSources = new ConcurrentDictionary<string, IRootDataSource>();

        /// <summary>
        /// Registers the data source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">An example context. Really the connection string is what's being registered.</param>
        /// <param name="dataSource">The data source.</param>
        public static void RegisterDataSource<T>(this DbContext context, T dataSource) where T : IRootDataSource, IClass2DataSource
        {
            s_DataSources[context.Database.Connection.ConnectionString] = dataSource;
        }

        /// <summary>
        /// Registers the data source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">An example context. Really the connection string is what's being registered.</param>
        /// <param name="dataSourceFactory">The data source factory.</param>
        public static void RegisterDataSource<T>(this DbContext context, Func<string, T> dataSourceFactory) where T : IRootDataSource, IClass2DataSource
        {
            s_DataSources[context.Database.Connection.ConnectionString] = dataSourceFactory(context.Database.Connection.ConnectionString);
        }

        public static IClass2DataSource Chain(this DbContext context)
        {
            var connection = context.Database.Connection;
            var transaction = context.Database.CurrentTransaction?.UnderlyingTransaction;

            //If context.SaveChanges is called, the connection will be immediately closed.
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            return (IClass2DataSource)s_DataSources[context.Database.Connection.ConnectionString].CreateOpenDataSource(connection, transaction);
        }

    }
}

