using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using CustomScaffolder.Models;

namespace CustomScafollder.Models
{ 
    public class CustomerRepository : ICustomerRepository
    {
        CustomScafollderContext context = new CustomScafollderContext();

        public IQueryable<Customer> All
        {
            get { return context.Customers; }
        }

        public IQueryable<Customer> AllIncluding(params Expression<Func<Customer, object>>[] includeProperties)
        {
            IQueryable<Customer> query = context.Customers;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public Customer Find(int id)
        {
            return context.Customers.Find(id);
        }

        public void InsertOrUpdate(Customer customer)
        {
            if (customer.Id == default(int)) {
                // New entity
                context.Customers.Add(customer);
            } else {
                // Existing entity
                context.Entry(customer).State = System.Data.Entity.EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var customer = context.Customers.Find(id);
            context.Customers.Remove(customer);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Dispose() 
        {
            context.Dispose();
        }
    }

    public interface ICustomerRepository : IDisposable
    {
        IQueryable<Customer> All { get; }
        IQueryable<Customer> AllIncluding(params Expression<Func<Customer, object>>[] includeProperties);
        Customer Find(int id);
        void InsertOrUpdate(Customer customer);
        void Delete(int id);
        void Save();
    }
}