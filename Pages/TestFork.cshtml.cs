using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gravitas.Monitoring.Pages
{
	public class TestForkModel : PageModel
	{
		[BindProperty]
		public int fork { get; set; } = 0;
		[BindProperty]
		public string val { get; set; } = "Zero";

		public void OnGet()
		{

		}

		public void OnPost()
		{
			switch (fork)
			{
				case 0:
					val = "Zero";
					break;
				case 1:
					val = "One";
					break;
				case 2:
					val = "Two";
					break;
				case 3:
					val = "Three";
					break;
				case 4:
					val = "Four";
					break;
				default:
					val = "Other";
					break;
			}
		}
	}
}
