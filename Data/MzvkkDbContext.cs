using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Gravitas.Monitoring.Models;

namespace Gravitas.Monitoring.Data
{

	public class MzvkkDbContext : DbContext
	{
		public MzvkkDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}




		public DbSet<Gravitas.Monitoring.Models.Contact> Ticket { get; set; } = default!;
	}




}

