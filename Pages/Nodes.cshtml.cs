using Gravitas.Monitoring.HelpClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gravitas.Monitoring.Pages
{
	public class NodesModel : PageModel
	{
		[BindProperty]
		public List<string[]> nodes { get; set; } = new List<string[]>();
		[BindProperty]
		public List<string[]> CurDevices { get; set; } = new List<string[]>();
		[BindProperty]
		public string CurNode { get; set; } = "";
		[BindProperty]
		public string CurNodeName { get; set; } = "";
		[BindProperty]
		public List<string[]> NodeContext { get; set; } = new List<string[]>();
		[BindProperty]
		public List<string> SelectedNode { get; set; } = new List<string>();
		//
		private void GetData()
		{
			List<string[]> tmp = new List<string[]>();
			if (db.EnterpriseNum == 0) db.GetDataFromDBMSSQL("select * from dbo.Nodes", ref tmp);
			nodes = tmp;
		}

		public void OnGet()
		{
			GetData();
		}

		public void OnPost()
		{
			GetData();
			foreach (string[] s in nodes)
			{
				if (s[0] == CurNode)
				{
					SelectedNode = s.ToList();
					break;
				}
			}




			List<string[]> tmp = new List<string[]>();
			tmp = nodes;
			string NodeParams = db.GetItemData(ref tmp, CurNode, 0, 8);
			CurNodeName = db.GetItemData(ref tmp, CurNode, 0, 3);
			//
			List<string[]> tmp2 = new List<string[]>();
			ParseNodeConfig(ref tmp2, NodeParams);
			CurDevices = tmp2;
			//

			string CurContext = db.GetItemData(ref tmp, CurNode, 0, 9);
			List<string[]> tmp3 = new List<string[]>();
			ParseNodeContext(ref tmp3, CurContext);

			List<string[]> tmtList = new List<string[]>();
			if (!string.IsNullOrEmpty(tmp3[3][1]))
			{
				//db.GetDataFromDBMSSQL("select StateId from dbo. ", ref tmp);
			}


			tmp3.Add(new string[] { "Тип вузла", SelectedNode[2] });


			NodeContext = tmp3;
		}


		private void ParseNodeConfig(ref List<string[]> lst, string NodeConfig)
		{
			lst.Clear();
			string find = "\"DeviceId\":";
			int c = 0;
			int n = 0;
			string nums = "0123456789";
			//
			string s = "";
			n = NodeConfig.IndexOf(find, n);
			//
			if (n < find.Length + 5)
			{
				return;
			}
			//
			string sc = "";
			try
			{
				do
				{
					s = "";
					for (int i = n + find.Length; i < n + 20; i++)
					{
						sc += NodeConfig.Substring(i, 1);
						if (nums.Contains(NodeConfig.Substring(i, 1)))
						{
							s += NodeConfig.Substring(i, 1);
						}
						else
						{
							break;
						}
					}
					sc += "\r\n";
					lst.Add(new string[] { s, GetDeviceNameById(s) });
					n = NodeConfig.IndexOf(find, n + find.Length);
					//
					c++;
					//if (c > 50) { /* return new string[] { "Error (50 спроб...)" }.ToList(); */ }
					if (c > 50) { log.Add("ParseNodeConfig: Error: (50 спроб...)"); }
				} while (n > -1);
			}
			catch (Exception ex)
			{
				log.Add("ParseNodeConfig: Error: " + ex.ToString());
			}
		}

		private string GetDeviceNameById(string Id)
		{
			List<string[]> tmp = new List<string[]>();
			db.GetDataFromDBMSSQL(" select Name from dbo.Devices where Id = '" + Id + "'", ref tmp);
			return tmp.Count > 0 ? tmp[0][0] : "Wrong device Id...";
		}

		private void ParseNodeContext(ref List<string[]> lst, string NodeContext)
		{
			lst.Clear();
			NodeContext = NodeContext.Replace("\"", "");

			List<string> tmp = new List<string>();
			tmp = NodeContext.Substring(1, NodeContext.Length - 2).Split(',').ToList();

			foreach (string s in tmp)
			{
				lst.Add(s.Split(":"));
			}
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

	}
}
