using Gravitas.Monitoring.HelpClasses;
using System.Drawing;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static System.Net.Mime.MediaTypeNames;

namespace Gravitas.Monitoring.Pages
{
	public class OneCTestModel : PageModel
	{
		[BindProperty]
		public List<string[]> OneCData { get; set; } = new List<string[]>();
		[BindProperty]
		public string tc { get; set; } = "";

		public void OnGet()
		{
			tc = HttpContext.Request.Query["tc"].ToString();
			string sData = GetOneCDataByTC(tc);
			if (sData.Contains("Error"))
			{
				OneCData.Add(new string[] { "Error", sData });
				return;
			}
			sData = sData.Replace("\r", "");
			sData = sData.Replace("\n", "");
			sData = sData.Replace("{", "");
			sData = sData.Replace("}", "");
			List<string> lstData = sData.Split(',').ToList();
			for (int i = 0; i < lstData.Count; i++)
			{
				OneCData.Add(new string[] { lstData[i].Split(':')[0], lstData[i].Split(':')[1] });
			}
		}

		private string GetOneCDataByTC(string tc)
		{
			string result = "";
			try
			{
				List<string[]> tmp = new List<string[]>();
				if (db.EnterpriseNum == 0) db.GetDataFromDBMSSQL("select DeliveryBillId from dbo.SingleWindowOpDatas where TicketContainerId = '" + tc + "'", ref tmp);
				if (db.EnterpriseNum == 1) db.GetDataFromDBMSSQL("select DeliveryBillId from opd.SingleWindowOpData where TicketContainerId = '" + tc + "'", ref tmp);
				if (tmp.Count == 0) // Error
				{
					return "";
				}
				var url = "https://cbapi.srv.mhp.com.ua/1c/hs/bulat/GetDeliveryBillViaId?ID=" + tmp[0][0];
				var request = WebRequest.Create(url);
				request.Method = "GET";
				request.Credentials = new NetworkCredential("srv_mzvkk_bulat", "srv_mzvkk_bulat");
				request.Timeout = 9000;
				using (var webResponse = request.GetResponse())
				{
					using (var webStream = webResponse.GetResponseStream())
					{
						using (var reader = new StreamReader(webStream))
						{
							var data = reader.ReadToEnd();
							result = data;
						}
					}
				}
				return result;
			}
			catch (Exception ex)
			{
				return "Error: " + ex.ToString();
			}
		}
	}
}
