using CRUD_Application.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CRUD_Application.Data
{
    //inheriting PreDefine DbContext
    public class ApplicationContext: DbContext
    {
        //using Constructor method 
        public ApplicationContext(DbContextOptions options) : base(options) 
        { }
        //Fetching Employee Model
        public DbSet<Employee> Employees { get; set; }
    }
}
