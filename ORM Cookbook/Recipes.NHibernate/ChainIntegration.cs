using NHibernate;
using Recipes.NHibernate.Models;
using Recipes.NHibernate.Repositories;
using System;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Recipes.NHibernate
{

    public class ChainIntegration
    {
        [Fact]
        public void ChainIntegration_NoTransactionTest()
        {
            int startingRowCount = GetRowCount();

            var newHibernateRecord = new EmployeeClassification();
            newHibernateRecord.EmployeeClassificationName = "Hibernate " + DateTime.Now.Ticks;

            var newChainRecord = new EmployeeClassification();
            newChainRecord.EmployeeClassificationName = "Chain " + DateTime.Now.Ticks;


            using (ISession session = NHibernateHelper.OpenSession())
            {
                session.Save(newHibernateRecord);
                session.Chain().Insert("HR.EmployeeClassification", newChainRecord).WithRefresh().Execute();
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


            using (ISession session = NHibernateHelper.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {

                session.Save(newHibernateRecord);
                session.Chain().Insert("HR.EmployeeClassification", newChainRecord).WithRefresh().Execute();

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


            using (ISession session = NHibernateHelper.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {

                session.Save(newHibernateRecord);
                session.Chain().Insert("HR.EmployeeClassification", newChainRecord).WithRefresh().Execute();

                transaction.Rollback();

            }

            int finalRowCount = GetRowCount();
            Assert.Equal(startingRowCount, finalRowCount);

        }
        static int GetRowCount()
        {
            int finalRowCount;
            using (ISession session = NHibernateHelper.OpenSession())
            {
                finalRowCount = session
                    .CreateCriteria(typeof(EmployeeClassification))
                    .List<EmployeeClassification>()
                    .Count;
            }
            return finalRowCount;
        }
    }
}
