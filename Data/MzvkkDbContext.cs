using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Gravitas.Monitoring.Models;

namespace Gravitas.Monitoring.Data
{
	public class MzvkkDbContext : DbContext
	{
		public MzvkkDbContext(DbContextOptions<MzvkkDbContext> options)
			: base(options)
		{
		}

		public DbSet<Gravitas.Monitoring.Models.Tickets> Tickets { get; set; } = default!;

	}




}

