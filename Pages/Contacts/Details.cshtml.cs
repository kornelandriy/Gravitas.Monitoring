using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Gravitas.Monitoring.Data;
using Gravitas.Monitoring.Models;

namespace Gravitas.Monitoring.Pages.Contacts
{
    public class DetailsModel : PageModel
    {
        private readonly Gravitas.Monitoring.Data.ApplicationDbContext _context;

        public DetailsModel(Gravitas.Monitoring.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Contact Contact { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contact.FirstOrDefaultAsync(m => m.ContactId == id);
            if (contact == null)
            {
                return NotFound();
            }
            else
            {
                Contact = contact;
            }
            return Page();
        }
    }
}
