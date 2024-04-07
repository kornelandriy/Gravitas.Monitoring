using Gravitas.Monitoring.HelpClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mono.TextTemplating;

namespace Gravitas.Monitoring.Pages
{
	public class TicketEditModel : PageModel
	{
		[BindProperty]
		public string t { get; set; } = "";
		[BindProperty]
		public string SI { get; set; } = "";
		[BindProperty]
		public string RTI { get; set; } = "";
		[BindProperty]
		public string RII { get; set; } = "";
		[BindProperty]
		public string SRTI { get; set; } = "";
		[BindProperty]
		public string SRII { get; set; } = "";
		[BindProperty]
		public string TC { get; set; } = "";
		[BindProperty]
		public string Result { get; set; } = "";

		public void OnGet()
		{
			t = HttpContext.Request.Query["t"].ToString();
			List<string[]> tmp = new List<string[]>();
			string sql = "";
			if (db.EnterpriseNum == 0) sql = "select StatusId, RoutetemplateId, RouteItemIndex, SecondaryRouteTemplateId, SecondaryRouteItemIndex, TicketContainerId from dbo.Tickets where Id = '" + t + "'";
			if (db.EnterpriseNum == 1) sql = "select StatusId, RoutetemplateId, RouteItemIndex, SecondaryRouteTemplateId, SecondaryRouteItemIndex, ContainerId from dbo.Ticket where Id = '" + t + "'";
			db.GetDataFromDBMSSQL(sql, ref tmp);
			//
			SI = tmp[0][0];
			RTI = tmp[0][1];
			RII = tmp[0][2];
			SRTI = tmp[0][3];
			SRII = tmp[0][4];
			TC = tmp[0][5];
		}

		public void OnPost()
		{ // StatusId, RouteTtemplateId, RouteItemIndex, SecondaryRouteTemplateId, SecondaryRouteItemIndex, TicketContainerId
			string tmpSRTI = "";
			if (string.IsNullOrEmpty(SRTI)) tmpSRTI = "NULL"; else tmpSRTI = "'" + SRTI + "'";
			string sql = "";
			if (db.EnterpriseNum == 0) sql = "update dbo.Tickets set StatusId='" + SI + "', RouteTemplateId='" + RTI + "', RouteItemIndex='" + RII + "', SecondaryRouteTemplateId=" + tmpSRTI + ", SecondaryRouteItemIndex='" + SRII + "' where Id = '" + t + "'";
			if (db.EnterpriseNum == 1) sql = "update dbo.Ticket set  StatusId='" + SI + "', RouteTemplateId='" + RTI + "', RouteItemIndex='" + RII + "', SecondaryRouteTemplateId=" + tmpSRTI + ", SecondaryRouteItemIndex='" + SRII + "' where Id = '" + t + "'";
			try
			{
				db.SendRequestToDB(sql);
				log.Add("User: " + User.Identity.Name + " TicketEdit new data (Id: " + t + " StatusId: " + SI + " RouteTemplateId: " + RTI + " RouteItemIndex: " + RII + " SecondaryRouteTemplateId: " + tmpSRTI + " SecondaryRouteItemIndex: " + SRII + ")");
				Result = "Зміни збережено...";
			}
			catch (Exception ex)
			{
				log.Add("User: " + User.Identity.Name + " TicketEdit Error: " + ex.ToString());
				Result = "Помилка: " + ex.ToString();
			}
		}
	}
}
