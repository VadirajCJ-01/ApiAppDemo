using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;

namespace ApiAppDemo.Models

{
    public class DevicesContext : DbContext
    {
        public DevicesContext(DbContextOptions<DevicesContext> options) : base(options) { 
        
        }
        public DbSet<Device> Devices { get; set; } = null!;
    }
}
