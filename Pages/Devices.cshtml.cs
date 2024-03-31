using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gravitas.Monitoring.Pages
{
	public class DevicesModel : PageModel
	{

		public List<string[]> devices { get; set; } = new List<string[]>();


		public void OnGet()
		{
			List<string[]> tmp = new List<string[]>();
			if (HelpClasses.db.EnterpriseNum == 0) HelpClasses.db.GetDataFromDBMSSQL("select * from dbo.Devices order by ParentDeviceId, TypeId", ref tmp);
			if (HelpClasses.db.EnterpriseNum == 1) HelpClasses.db.GetDataFromDBMSSQL("select * from dbo.Device order by ParentDeviceId, TypeId", ref tmp);

			Makelist();
		}

		private void Makelist()
		{

		}


		public string GetDevices()
		{

			return "";
		}
	}
}
