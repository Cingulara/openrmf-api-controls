using Microsoft.EntityFrameworkCore;
using openstig_api_controls.Models;

namespace openstig_api_controls.Database
{
    public class ControlsDBContext : DbContext  
    {  
        public ControlsDBContext(DbContextOptions<ControlsDBContext> context): base(context)  
        {  
    
        }  

        public DbSet<ControlSet> ControlSets { get; set; }
    }  
}