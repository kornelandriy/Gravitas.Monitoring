using System.Net;
using System.Reflection.Metadata.Ecma335;
using Gravitas.Monitoring.HelpClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gravitas.Monitoring.Pages
{
	public class VKModuleDataModel : PageModel
	{
		[BindProperty]
		public List<string[]> Devices1 { get; set; } = new List<string[]>();
		[BindProperty]
		public List<string[]> Devices2 { get; set; } = new List<string[]>();
		[BindProperty]
		public List<string[]> Devices3 { get; set; } = new List<string[]>();

		[BindProperty]
		public string SelectedDevice { get; set; } = "";
		[BindProperty]
		public string SelectedDeviceIP { get; set; } = "";
		[BindProperty]
		public string DeviceData { get; set; } = "";
		[BindProperty]
		public string ReturnedIP { get; set; } = "";
		public List<string[]> DeviceDataList { get; set; } = new List<string[]>();

		// ### ### ### ### ### ### ### ### ### ### ### ### ### ### ### ### ### ### ### ### ### ### ### ### ### ### ### ###

		public void OnGet()
		{
			MakeDeviceList();
		}

		public void OnPost()
		{
			MakeDeviceList();
			DeviceDataList = ParseDeviceData2(GetVKModuleData(SelectedDeviceIP));
		}

		private List<string[]> ParseDeviceData2(string data)
		{
			List<string> tmp1 = new List<string>();
			List<string[]> tmp2 = new List<string[]>();
			data = data.Replace("\r", "#");
			data = data.Replace("\n", "@");
			//
			data = data.Replace("@", "");
			//
			for (int i = 0; i < 8; i++)
			{
				data = data.Replace("</btn" + i + ">", "");
				data = data.Replace("</led" + i + ">", "");
				//
				data = data.Replace("<btn" + i + ">", "Enter " + i + "$");
				data = data.Replace("<led" + i + ">", "Relay " + i + "$");
				//
				data = data.Replace("</response>", "");
				data = data.Replace("<response>", "");
			}
			data = data.Substring(1, data.Length - 3);
			tmp1 = data.Split('#').ToList();
			foreach(string s in tmp1)
			{
				tmp2.Add(s.Split('$'));
			}
			return tmp2;
		}

		private string ParseDeviceData(string data)
		{
			data = data.Replace("\r", "");
			data = data.Replace("\n", "");
			for (int i = 0; i < 8; i++)
			{
				data = data.Replace("</btn" + i + ">", "</td></tr>");
				data = data.Replace("</led" + i + ">", "</td></tr>");
				//
				data = data.Replace("<btn" + i + ">", "<tr><td class=\"brdr1sb\">Enter " + i + "</td><td class=\"brdr1sb\">");
				data = data.Replace("<led" + i + ">", "<tr><td class=\"brdr1sb\">Relay " + i + "</td><td class=\"brdr1sb\">");
				//
				data = data.Replace("</response>", "");
				data = data.Replace("<response>", "");
			}
			return data;
		}

		private void MakeDeviceList()
		{
			List<string[]> tmp1 = new List<string[]>();
			List<string[]> tmp2 = new List<string[]>();
			List<string[]> tmp3 = new List<string[]>();
			//
			db.GetDataFromDBMSSQL("select Id, Name, ParamJson from dbo.Devices where TypeId = '5'", ref tmp1);
			db.GetDataFromDBMSSQL("select Id, Name, ParamJson from dbo.Devices where TypeId = '4'", ref tmp2);
			db.GetDataFromDBMSSQL("select Id, Name, ParamJson from dbo.Devices where TypeId = '16'", ref tmp3);
			//
			Devices1.Clear();
			Devices2.Clear();
			Devices3.Clear();
			foreach (var d in tmp1)
				Devices1.Add(new string[] { d[0], d[1], GetIPFromTag(d[2]) });
			foreach (var d in tmp2)
				Devices2.Add(new string[] { d[0], d[1], GetIPFromTag(d[2]) });
			foreach (var d in tmp3)
				Devices3.Add(new string[] { d[0], d[1], GetIPFromTag(d[2]) });
			//
		}

		public string GetVKModuleData(string ip)
		{
			ReturnedIP = ip;
			string result = "ND";
			try
			{
				using (WebClient web1 = new WebClient())
				{
					web1.Credentials = new NetworkCredential("admin", "Qq123123123");
					result = web1.DownloadString("http://" + ip + "/protect/status.xml");
				}
			}
			catch { result = "Err"; }
			return result;
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
