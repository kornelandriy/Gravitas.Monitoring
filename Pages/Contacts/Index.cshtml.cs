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
    public class IndexModel : PageModel
    {
        private readonly Gravitas.Monitoring.Data.ApplicationDbContext _context;

        public IndexModel(Gravitas.Monitoring.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Contact> Contact { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Contact = await _context.Contact.ToListAsync();
        }
    }
}
