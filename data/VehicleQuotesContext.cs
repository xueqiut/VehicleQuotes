using Microsoft.EntityFrameworkCore;
using VehicleQuotes.Models;

public class VehicleQuotesContext : DbContext
{
		public VehicleQuotesContext (DbContextOptions<VehicleQuotesContext> options)
				: base(options)
		{
		}

		public DbSet<Make> Makes { get; set; }
		public DbSet<Size> Sizes { get; set; }
		public DbSet<BodyType> BodyTypes { get; set; }
}