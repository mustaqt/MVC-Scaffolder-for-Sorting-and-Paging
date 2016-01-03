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
    public class CompanyRepository : ICompanyRepository
    {
        CustomScafollderContext context = new CustomScafollderContext();

        public IQueryable<Company> All
        {
            get { return context.Companies; }
        }

        public IQueryable<Company> AllIncluding(params Expression<Func<Company, object>>[] includeProperties)
        {
            IQueryable<Company> query = context.Companies;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public Company Find(int id)
        {
            return context.Companies.Find(id);
        }

        public void InsertOrUpdate(Company company)
        {
            if (company.Id == default(int)) {
                // New entity
                context.Companies.Add(company);
            } else {
                // Existing entity
                context.Entry(company).State = System.Data.Entity.EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var company = context.Companies.Find(id);
            context.Companies.Remove(company);
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

    public interface ICompanyRepository : IDisposable
    {
        IQueryable<Company> All { get; }
        IQueryable<Company> AllIncluding(params Expression<Func<Company, object>>[] includeProperties);
        Company Find(int id);
        void InsertOrUpdate(Company company);
        void Delete(int id);
        void Save();
    }
}