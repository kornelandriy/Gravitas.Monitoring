using Gravitas.Monitoring.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Gravitas.Monitoring.Pages
{
    public class ApUsersModel : PageModel
    {
		private readonly Gravitas.Monitoring.Data.ApplicationDbContext _context;
		public ApUsersModel(Gravitas.Monitoring.Data.ApplicationDbContext context) { _context = context; }
		public IList<AspNetUsers> AspNetUsers { get; set; } = default!;

		[BindProperty]
		public string UserName { get; set; }




		public async Task OnGetAsync()
		{
			//AspNetUsers = await _context.AspNetUsers.ToListAsync();

		}
    }
}
