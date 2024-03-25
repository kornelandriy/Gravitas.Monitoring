using Gravitas.Monitoring.HelpClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gravitas.Monitoring.Pages
{
	public class ReplaceLabelModel : PageModel
	{
		[BindProperty]
		private string tc { get; set; } = "";
		[BindProperty]
		public List<string[]> AntennaList { get; set; } = new List<string[]>();
		[BindProperty]
		public string CurAntenna { get; set; } = "";
		[BindProperty]
		public string Result { get; set; } = "";
		[BindProperty]
		public string CurLabel { get; set; } = "";
		[BindProperty]
		public string ParsedData { get; set; } = "";
		[BindProperty]
		public List<string> ParsedLabels { get; set; } = new List<string>();
		[BindProperty]
		public string DeviceRawData { get; set; } = "\"{\\\"InData\\\":{\\\"TagList\\\":{\\\"E200001D1717016518108F3A\\\":\\\"2024-03-25T23:41:34.4819965+02:00\\\",\\\"E200001D9912015319507E00\\\":\\\"2024-03-25T23:41:34.4819965+02:00\\\"}},\\\"OutData\\\":null,\\\"LastUpdate\\\":\\\"2024-03-25T23:41:34.4819965+02:00\\\",\\\"ErrorCode\\\":0,\\\"Id\\\":0}\"";





		public void OnGet()
		{

			tc = HttpContext.Request.Query["tc"].ToString();
			MakeAntennaList();

			List<string[]> tmp = new List<string[]>();
			db.GetDataFromDBMSSQL("select  top(1) Id from dbo.Cards where TypeId = 3 and TicketContainerId = '" + tc + "'", ref tmp);

			if (tmp.Count > 0)
			{
				CurLabel = tmp[0][0];
			}
			else
			{
				CurLabel = "ND";
			}
		}

		public void OnPost()
		{
			MakeAntennaList();
			Result = CurAntenna;
			GetDeviceData(); // tst !!!
			List<string> tmp = new List<string>();
			ParsedData = ZebraDataParser(DeviceRawData, ref tmp);
			ParsedLabels = tmp;
		}

		private void GetDeviceData()
		{

		}

		private void MakeAntennaList()
		{
			AntennaList.Clear();
			string sql = "";
			List<string[]> tmp = new List<string[]>();
			if (db.EnterpriseNum == 0) sql += "SELECT case when TypeId = 2 then 'Zebra' when TypeId = 31 then 'ZKTeco' end as 'Type', [Name],[Id] FROM [mhp].[dbo].[Devices] where TypeId = 2 or TypeId=31 order by TypeId";
			if (db.EnterpriseNum == 1) sql += "SELECT case when TypeId = 2 then 'Zebra' when TypeId = 31 then 'ZKTeco' end as 'Type', [Name],[Id] FROM [mhp].[dbo].[Devices] where TypeId = 2 order by TypeId";
			db.GetDataFromDBMSSQL(sql, ref tmp);
			AntennaList = tmp;
		}

		private string ZebraDataParser(string s, ref List<string> lst)
		{
			int n1 = -1;
			try { n1 = s.IndexOf("TagList"); } catch { }
			//
			if (n1 == -1) return "Parse error";
			//
			int n2 = -1;
			int n3 = -1;
			//
			n2 = s.IndexOf("{\\\"", n1);
			if (n2 < 1) return "Parse error";
			//
			n3 = s.IndexOf("}", n2);
			if (n3 < 1) return "Parse error";
			//
			return ZebrePartOfDataParser(s.Substring(n2, n3 - n2 + 1), ref lst) + "\r\n";
		}

		private string ZebrePartOfDataParser(string s, ref List<string> lst)
		{
			s = s.Replace("\\", "").Replace("\"", "").Replace("{", "").Replace("}", "");
			string r = "";
			int n1 = 0;
			int n2 = -1;
			List<string> tmp = new List<string>();
			tmp = s.Split(',').ToList();
			string rr = "";
			foreach (string ss in tmp)
			{

				n1 = 0;
				n2 = ss.IndexOf(":");
				r = ss.Substring(n1, n2 - n1);
				lst.Add(r);
				n1 = ss.IndexOf("T") - 10;
				n2 = ss.IndexOf("T") + 8;
				r = "[ " + ss.Substring(n1, n2 - n1 + 1) + " ]   -   " + r;
				r = r.Replace("T", " ]   [ ");
				rr += r + "\r\n";
			}
			return rr;
		}
	}
}
