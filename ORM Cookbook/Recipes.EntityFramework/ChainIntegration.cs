using Recipes.EF.Models;
using Recipes.NHibernate.Repositories;
using System;
using System.Linq;
using Tortuga.Chain;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Recipes.NHibernate
{

    public class ChainIntegration
    {

        static ChainIntegration()
        {
            using (var context = new OrmCookbook())
            {
                context.RegisterDataSource(cs => new SqlServerDataSource(cs));
            }
        }

        [Fact]
        public void ChainIntegration_NoTransactionTest()
        {
            int startingRowCount = GetRowCount();

            var newHibernateRecord = new EmployeeClassification();
            newHibernateRecord.EmployeeClassificationName = "Hibernate " + DateTime.Now.Ticks;

            var newChainRecord = new EmployeeClassification();
            newChainRecord.EmployeeClassificationName = "Chain " + DateTime.Now.Ticks;


            using (var context = new OrmCookbook())
            {
                context.EmployeeClassifications.Add(newHibernateRecord);
                context.SaveChanges();
                context.Chain().Insert("HR.EmployeeClassification", newChainRecord).WithRefresh().Execute();

            }

            Assert.True(newHibernateRecord.EmployeeClassificationKey > 0);
            Assert.True(newChainRecord.EmployeeClassificationKey > 0);


            int finalRowCount = GetRowCount();
            Assert.Equal(startingRowCount + 2, finalRowCount);
        }


        [Fact]
        public void ChainIntegration_TransactionTest()
        {
            int startingRowCount = GetRowCount();

            var newHibernateRecord = new EmployeeClassification();
            newHibernateRecord.EmployeeClassificationName = "Hibernate " + DateTime.Now.Ticks;

            var newChainRecord = new EmployeeClassification();
            newChainRecord.EmployeeClassificationName = "Chain " + DateTime.Now.Ticks;


            using (var context = new OrmCookbook())
            using (var transaction = context.Database.BeginTransaction())
            {

                context.EmployeeClassifications.Add(newHibernateRecord);
                context.SaveChanges();
                context.Chain().Insert("HR.EmployeeClassification", newChainRecord).WithRefresh().Execute();

                transaction.Commit();

            }

            Assert.True(newHibernateRecord.EmployeeClassificationKey > 0);
            Assert.True(newChainRecord.EmployeeClassificationKey > 0);

            int finalRowCount = GetRowCount();
            Assert.Equal(startingRowCount + 2, finalRowCount);

        }

        [Fact]
        public void ChainIntegration_RollbackTransactionTest()
        {
            int startingRowCount = GetRowCount();


            var newHibernateRecord = new EmployeeClassification();
            newHibernateRecord.EmployeeClassificationName = "Hibernate " + DateTime.Now.Ticks;

            var newChainRecord = new EmployeeClassification();
            newChainRecord.EmployeeClassificationName = "Chain " + DateTime.Now.Ticks;


            using (var context = new OrmCookbook())
            using (var transaction = context.Database.BeginTransaction())
            {

                context.EmployeeClassifications.Add(newHibernateRecord);
                context.SaveChanges();
                context.Chain().Insert("HR.EmployeeClassification", newChainRecord).WithRefresh().Execute();

                transaction.Rollback();

            }

            int finalRowCount = GetRowCount();
            Assert.Equal(startingRowCount, finalRowCount);

        }

        static int GetRowCount()
        {
            using (var context = new OrmCookbook())
            {
                return context.EmployeeClassifications.Count();
            }
        }
    }
}


