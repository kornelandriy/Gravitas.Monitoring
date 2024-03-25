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

namespace Gravitas.Monitoring.Pages
{
	public class TestControlsModel : PageModel
	{
		public HelpClasses.clstst ct { get; set; } = new HelpClasses.clstst();

		public void OnGet()
		{

		}

		public void OnPost()
		{

		}
	}
}
