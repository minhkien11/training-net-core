using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrainingAPI.Models
{
    public class TrainingDbContext : DbContext
    {
        public TrainingDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
    }
}
