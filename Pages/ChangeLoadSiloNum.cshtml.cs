using Gravitas.Monitoring.HelpClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gravitas.Monitoring.Pages
{
	public class ChangeLoadSiloNumModel : PageModel
	{
		[BindProperty]
		public string tc { get; set; } = "";
		[BindProperty]
		public string Siloes { get; set; } = "";
		[BindProperty]
		public string Result { get; set; } = "";
		[BindProperty]
		public string id { get; set; } = "";



		public void OnGet()
		{
			tc = HttpContext.Request.Query["tc"].ToString();
			id = HttpContext.Request.Query["id"].ToString();
		}

		public void OnPost()
		{
			if (db.EnterpriseNum != 0)
			{
				Result = "Даний функціонал доступний тільки для МЗВКК";
				return;
			}

			if (!string.IsNullOrWhiteSpace(Siloes))
			{
				try
				{
					db.SendRequestToDB("update dbo.LoadPointOpDatas set LoadSiloNames='" + Siloes + "' where Id = '" + id + "'");
					Result = "Збережено...";
				}
				catch (Exception ex)
				{
					Result = ex.ToString();
				}
			}
		}
	}
}
