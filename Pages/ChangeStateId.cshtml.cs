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
				Result = "��������... ����� ������ - " + GetNodeStatus(NewStateId);
				RedirectToPage("CarInfo?tc=" + tc);
			}
			catch
			{
				Result = "�������";
			}
		}
		
		public string[] StatusNamesForNode = new string[]
				{
			"",
			"�����", // 1
            "� �������", // 2
            "�� ���������", // 3
            "���������", // 4
            "³�������� � ���������",//5
            "����������", // 6
            "", "", "",
			"��������", // 10
            "³��������", // 11
            "���������", // 12
            "�������� ������������", // 13
            "�������� �������������", // 14
            "����������������" // 15
				};

		public string GetNodeStatus(string id)
		{
			int n = 0;
			try { n = int.Parse(id); if (id == "") return "#"; else return "" + id + " - " + StatusNamesForNode[n]; }
			catch { return "#: " + id; }
		}

	}
}
