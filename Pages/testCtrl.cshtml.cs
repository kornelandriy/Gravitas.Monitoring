using Gravitas.Monitoring.HelpClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;



namespace Gravitas.Monitoring.Pages
{
	public class testCtrlModel : PageModel
	{
		[BindProperty]
		public string tc { get; set; } = "";


		public void OnGet()
		{
			log.Add("testCtrl: OnGet: tc: " + tc);
		}


		public async void OnPost()
		{
			log.Add("testCtrl: OnPost: tc: " + tc);
		}

		[HttpPost]
		public IActionResult Index(string tc)
		{
			this.tc = tc;

			log.Add("IActionResult: tc: " + tc);
			return Page();
		}
	}
}
