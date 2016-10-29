using NHibernate;
using Recipes.NHibernate.Models;
using Recipes.Repositories;
using System.Collections.Generic;

namespace Recipes.NHibernate.Repositories
{
    public class EmployeeClassificationRepository : IEmployeeClassificationRepository<EmployeeClassification>
    {
        public int Create(EmployeeClassification classification)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Save(classification);
                transaction.Commit();
                return classification.EmployeeClassificationKey;
            }
        }

        public void Delete(int employeeClassificationKey)
        {

            using (ISession session = NHibernateHelper.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var temp = session.Get<EmployeeClassification>(employeeClassificationKey);

                session.Delete(temp);
                transaction.Commit();
            }
        }

        public void Delete(EmployeeClassification classification)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Delete(classification);
                transaction.Commit();
            }
        }

        public IList<EmployeeClassification> GetAll()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session
                    .CreateCriteria(typeof(EmployeeClassification))
                    .List<EmployeeClassification>();
            }
        }

        public EmployeeClassification GetByKey(int employeeClassificationKey)
        {
            using (ISession session = NHibernateHelper.OpenSession())
                return session.Get<EmployeeClassification>(employeeClassificationKey);
        }

        public void Update(EmployeeClassification classification)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Update(classification);
                transaction.Commit();
            }
        }
    }
}

