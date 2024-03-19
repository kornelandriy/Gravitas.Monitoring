using Gravitas.Monitoring.HelpClasses;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Gravitas.Monitoring.HelpClasses
{
	public class LabelFinderHelper
	{
		private List<string[]> RawDeviceList = new List<string[]>();
		public List<string[]> DeviceList = new List<string[]>();

		public LabelFinderHelper() { GetDevices(); }

		public void GetDevices()
		{
			RawDeviceList.Clear();
			db.GetDataFromDBMSSQL("select * from dbo.Devices where TypeId = 2 or TypeId = 3 or TypeId = 31", ref RawDeviceList);
			foreach (string[] s in RawDeviceList)
			{
				if (s[3] == "3")
				{ // label style="padding-right: 7px;"><input style="padding-right: 3px;" type="radio" name="CardType" value="1" @ct[1] /> 1 - Користувач</label>
					DeviceList.Add(new string[] { s[0],"<label><input type=\"radio\" name=\"Device\" value=\"" + s[0] + "\" ", " />" + GetDeviceNameByParentId(s[1]) + s[0].Substring(s[0].Length-1,1) + "</label><br />" });
				}
				if (s[3] == "31")
				{
					DeviceList.Add(new string[] { s[0],"<label><input type=\"radio\" name=\"Device\" value=\"" + s[0] + "\" ", " />" + s[4] + "</label><br />" });
				}
			}
		}

		private string GetDeviceNameByParentId(string ParentId)
		{
			if (db.EnterpriseNum != 0) return "ND";
			string r = "";
			foreach (string[] s in RawDeviceList)
			{
				if (s[0] == ParentId)
				{
					r = s[4];
					break;
				}
			}
			return r;
		}
	}
}
