using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NuGet.Protocol.Plugins;

namespace Gravitas.Monitoring.Pages
{
	public class ChangeStateIdModel : PageModel
	{
		[BindProperty]
		public string id { get; set; } = "";
		[BindProperty]
		public string StateId { get; set; } = "";
		[BindProperty]
		public string tc { get; set; } = "";
		[BindProperty]
		public string TableName { get; set; } = "";
		[BindProperty]
		public string NodeName { get; set; } = "";
		[BindProperty]
		public string Result { get; set; } = "";
		[BindProperty]
		public string NewStateId { get; set; } = "";




		private void PrepareAll()
		{
			id = HttpContext.Request.Query["id"].ToString();
			StateId = HttpContext.Request.Query["StateId"].ToString();
			tc = HttpContext.Request.Query["tc"].ToString();
			TableName = HttpContext.Request.Query["TableName"].ToString();
			NodeName = HttpContext.Request.Query["NodeName"].ToString();
		}

		public void OnGet()
		{
			PrepareAll();
		}

		public void OnPost()
		{
			PrepareAll();
			string sql = "update " + TableName + " set StateId='" + NewStateId + "' where Id = '" + id + "'";
			//Result = sql + " - ";
			try
			{
				HelpClasses.db.SendRequestToDB(sql);
				Result = "Виконано... новий статус - " + GetNodeStatus(NewStateId);
				RedirectToPage("CarInfo?tc=" + tc);
			}
			catch
			{
				Result = "Помилка";
			}
		}
		
		public string[] StatusNamesForNode = new string[]
				{
			"",
			"Бланк", // 1
            "В обробці", // 2
            "На погодженні", // 3
            "Погоджено", // 4
            "Відмовлено у погодженні",//5
            "Очікування", // 6
            "", "", "",
			"Виконано", // 10
            "Відмовлено", // 11
            "Скасовано", // 12
            "Часткове завантаження", // 13
            "Часткове розвантаження", // 14
            "Перезавантаження" // 15
				};

		public string GetNodeStatus(string id)
		{
			int n = 0;
			try { n = int.Parse(id); if (id == "") return "#"; else return "" + id + " - " + StatusNamesForNode[n]; }
			catch { return "#: " + id; }
		}

	}
}
