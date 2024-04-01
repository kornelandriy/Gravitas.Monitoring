using Gravitas.Monitoring.HelpClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gravitas.Monitoring.Pages
{
	public class DeviceModel : PageModel
	{
		[BindProperty]
		public List<string[]> CurDevice { get; set; } = new List<string[]>();
		[BindProperty]
		public string CurDeviceId { get; set; } = "";
		[BindProperty]
		public string CurDeviceURL { get; set; } = "";
		[BindProperty]
		public string DeviceData { get; set; } = "";
		[BindProperty]
		public string PingResult { get; set; } = "";
		[BindProperty]
		public int CmdSelector { get; set; } = 0;
		[BindProperty]
		public string DeviceIP { get; set; } = "";



		private void PrepareAll()
		{
			CurDeviceId = HttpContext.Request.Query["DeviceId"].ToString();
			//
			List<string[]> dtmp = new List<string[]>();
			if (db.EnterpriseNum == 0) db.GetDataFromDBMSSQL("select * from dbo.Devices where Id = '" + CurDeviceId + "'", ref dtmp);
			CurDevice = dtmp;
			DeviceIP = GetIPFromTag(CurDevice[0][5]);
			if (!IsIP(DeviceIP))
			{
				List<string[]> tmppd = new List<string[]>();
				db.GetDataFromDBMSSQL("select * from dbo.Devices where Id = '" + CurDevice[0][1] + "'", ref tmppd);
				DeviceIP = GetIPFromTag(tmppd[0][5]);
			}
			GetDeviceUrl();
		}

		public void OnGet()
		{
			PrepareAll();
		}

		public void OnPost()
		{
			PrepareAll();

			switch (CmdSelector)
			{
				case 0:
					DeviceData = GetDeviceData(CurDeviceId);
					break;
				case 1:
					PingResult = Pingalka.Test(DeviceIP);
					break;
			}


		}

		private void GetDeviceUrl()
		{

			List<string[]> tmp = new List<string[]>();
			db.GetDataFromDBMSSQL("select * from dbo.Devices where Id = '" + CurDeviceId + "'", ref tmp);


			if (tmp.Count > 0)
				if (tmp[0][1] != "")
					db.GetDataFromDBMSSQL("select * from dbo.Devices where Id = '" + tmp[0][1] + "'", ref tmp);

			string IP = GetIPFromTag(tmp[0][5]);
			if (tmp.Count > 0)
			{
				switch (tmp[0][3])
				{
					case "2":
						CurDeviceURL = "https://" + IP;
						break;
					case "1":
					case "4":
					case "5":
					case "8":
					case "13":
					case "16":
					case "17":
					case "31":
						CurDeviceURL = "http://" + IP;
						break;
					default:
						CurDeviceURL = "http://" + IP;
						break;
				}
			}


			// ###############################################################################################################################################



		}

		private string GetIPFromTag(string tag, string before = "", string after = "")
		{
			try
			{
				int n = tag.IndexOf("IpAddress") + "IpAddress".Length + 3;
				if (tag.Substring(n, 10).ToLower().Contains("http"))
				{
					n = tag.IndexOf("//", n, tag.Length - n) + 2;
				}
				string mask = "0123456789.";
				string result = "";
				for (int i = n; i < tag.Length; i++)
				{
					if (mask.Contains(tag.Substring(i, 1))) { result += tag.Substring(i, 1); } else { break; }
				}
				return before + result + after;
			}
			catch { return ""; }
		}

		public static bool IsIP(string ip)
		{
			string patternIP = "^([01]?\\d\\d?|2[0-4]\\d|25[0-5])\\" +
							   ".([01]?\\d\\d?|2[0-4]\\d|25[0-5])\\" +
							   ".([01]?\\d\\d?|2[0-4]\\d|25[0-5])\\" +
							   ".([01]?\\d\\d?|2[0-4]\\d|25[0-5])$";
			return System.Text.RegularExpressions.Regex.Match(ip, patternIP).Success;
		}


		private string GetDeviceData(string id)
		{
			if (string.IsNullOrEmpty(id)) return "Wrong device id";
			long l = 0;
			try
			{
				l = long.Parse(id);
				return GetDeviceData(l);
			}
			catch { return "Wrong device id"; }
		}

		private string GetDeviceData(long id)
		{
			try
			{
				if (id < 1) return "Invalid id";
				string s = "";
				using (System.Net.WebClient wc = new System.Net.WebClient())
				{
					//byte[] raw = wc.DownloadData("http://localhost:8090/DeviceSync/GetState?deviceId=" + id);
					byte[] raw = wc.DownloadData("http://10.9.176.98:8090/DeviceSync/GetState?deviceId=" + id);
					s = System.Text.Encoding.UTF8.GetString(raw);
				}
				return s;
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}
	}
}
