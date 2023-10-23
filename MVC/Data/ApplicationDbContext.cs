using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using MVC.Models;

namespace MVC.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<AccountsModel>? AccountsModel { get; set; }

        public DbSet<PostsModel>? PostsModel { get; set; }
    }
}
