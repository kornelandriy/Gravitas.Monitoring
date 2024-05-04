using Gravitas.Monitoring.HelpClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gravitas.Monitoring.Pages
{
	public class QueueModel : PageModel
	{
		[BindProperty]
		public List<string[]> qList { get; set; } = new List<string[]>();
		[BindProperty]
		public string qId { get; set; } = "";
		[BindProperty]
		public string Result { get; set; } = "";


		public void OnGet()
		{
			GetData();
		}

		public void OnPost()
		{
			string sql = "update QueueRegisters set IsAllowedToEnterTerritory='1',IsSMSSend='1',SMSTimeAllowed='" + DateTime.Now.ToString("yyyy-MM-dd HH:dd:mm.fff") + "' where Id='" + qId + "'";
			Result = sql;




			GetData();
		}

		private void GetData()
		{
			List<string[]> tmp = new List<string[]>();
			db.GetDataFromDBMSSQL("SELECT dbo.QueueRegisters.TicketContainerId,dbo.QueueRegisters.RegisterTime,dbo.QueueRegisters.TruckPlate,dbo.QueueRegisters.PhoneNumber,dbo.QueueRegisters.IsAllowedToEnterTerritory,dbo.QueueRegisters.IsSMSSend,dbo.QueueRegisters.SMSTimeAllowed,dbo.RouteTemplates.Name as 'RouteName', dbo.QueueRegisters.Id FROM [mhp].[dbo].[QueueRegisters] join dbo.RouteTemplates on dbo.RouteTemplates.Id = dbo.QueueRegisters.RouteTemplateId", ref tmp);
			qList = tmp;
		}


	}
}
