using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Gunner.DataAccess.Data;
using Gunner.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
namespace Gunner.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {

        private readonly ApplicationDbContext _db;

        internal DbSet<T> dbset;
        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbset = _db.Set<T>();
            _db.products.Include(u => u.Category).Include(u => u.CategoryID);
        }

        public void Add(T entity)
        {
            dbset.Add(entity);  


        }

        public T Get(Expression<Func<T, bool>> Filter, string? includeProperties = null, bool tracked = false)
        {

            if (tracked)
            {

                IQueryable<T> query = dbset;
                if (!string.IsNullOrEmpty(includeProperties))
                {
                    foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(includeProperty);
                    }
                }
                query = query.Where(Filter);

                return query.FirstOrDefault();
            }
            else
            {
                IQueryable<T> query = dbset.AsNoTracking();
                if (!string.IsNullOrEmpty(includeProperties))
                {
                    foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(includeProperty);
                    }
                }
                query = query.AsNoTracking().Where(Filter);
                return query.FirstOrDefault();
            }
        }
        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? Filter, string? includeProperties = null)
        {
            IQueryable<T> query = dbset;
            if (Filter != null)
            {
                query = query.Where(Filter);
            }

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }

            }

            return query.ToList();


        }

        public void Remove(T entity)
        {

            dbset.Remove(entity);


        }

        public void RemoveRange(IEnumerable<T> entity)
        {

            dbset.RemoveRange(entity);  

        }
    }






}

