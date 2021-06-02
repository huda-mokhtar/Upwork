using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upwork.Data;
using Upwork.Models;

namespace Upwork.services
{
    public interface ICategories
    {
        List<Category> ListOfCategory();
        Category Create(Category categories);
    }
    public class CategoriesDB:ICategories
    {
        private ApplicationDbContext db;
        public Category Create(Category category)
        {
            db.Categories.Add(category);
            db.SaveChanges();
            return category;
        }
        public List<Category> ListOfCategory()
        {
            return db.Categories.ToList();
        }

    }
}

