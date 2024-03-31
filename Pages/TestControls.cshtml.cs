using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Gravitas.Monitoring.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Gravitas.Monitoring.Models;
using Microsoft.AspNetCore.Components.Web;
using Gravitas.Monitoring.HelpClasses;
using Microsoft.IdentityModel.Tokens;

namespace Gravitas.Monitoring.Pages
{
	public class TestControlsModel : PageModel
	{

		private readonly Gravitas.Monitoring.Data.MzvkkDbContext _context;

		public TestControlsModel(Gravitas.Monitoring.Data.MzvkkDbContext context)
		{
			_context = context;
		}

		public IActionResult OnGet()
		{
			//TicketsList = _context.Tickets.Where(t => t.Id > 100557 && t.SecondaryRouteTemplateId != null).ToList();
			TicketsList = _context.Tickets.Where(t => t.Id > 100000 && t.Id < 100009).ToList();
			return Page();
		}

		//[BindProperty]
		//public Tickets Tickets { get; set; } = default!;
		[BindProperty]
		public List<Tickets> TicketsList { get; set; } = new List<Tickets>();

		// To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
		public async Task<IActionResult> OnPostAsync()
		{



			//if (!ModelState.IsValid) { return Page(); }
			//_context.Tickets.Add(Tickets);
			//await _context.SaveChangesAsync();
			//return RedirectToPage("./TestControls");
			return Page();

		}
	}
}
