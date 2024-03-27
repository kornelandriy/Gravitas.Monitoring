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
		[BindProperty]
		public string s { get; set; } = "";




		public void Copy()
		{
			try
			{
				log.Add("Copy:");
				s = "Copy: Ok";
			}
			catch (Exception ex)
			{
				s = "Copy: Err: " + ex.ToString();
			}



		}

		public void OnGet()
		{
			string sAction= HttpContext.Request.Query["action"].ToString();

			if (!sAction.IsNullOrEmpty())
			{

				log.Add("Copy ???");

				Redirect("/TestControls");
			}
		}

		public void OnPost()
		{

		}
	}
}
