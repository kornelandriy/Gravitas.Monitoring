using Gravitas.Monitoring.HelpClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gravitas.Monitoring.Pages
{
	public class ClearNodeModel : PageModel
	{
		[BindProperty]
		public string NodeId { get; set; } = "";
		[BindProperty]
		public string NodeName { get; set; } = "";
		[BindProperty]
		public string Result { get; set; } = "";
		[BindProperty]
		public string NodeType { get; set; } = "";

		public void OnGet()
		{
			NodeId = HttpContext.Request.Query["NodeId"].ToString();
			List<string[]> tmp = new List<string[]>();
			string sql = "";
			if (db.EnterpriseNum == 0) sql = "select * from dbo.Nodes where Id = '" + NodeId + "'";
			if (db.EnterpriseNum == 1) sql = "select * from dbo.Node where Id = '" + NodeId + "'";
			db.GetDataFromDBMSSQL(sql, ref tmp);
			NodeName = tmp[0][3];
			NodeType = tmp[0][2];
		}

		public void OnPost()
		{
			ClearNodeNow();
		}

		private string GetBeginValForNode()
		{
			string result = "";
			switch (NodeType)
			{
				case "1":
					result = "101";
					break;
				default:
					result = "Err";
					break;
			}
			return result;
		}

		private void ClearNodeNow()
		{
			string State = GetBeginValForNode();
			string sql = "";
			if (State == "Err")
			{
				Result = "Не вірний вузол";
			}
			else
			{
				if (db.EnterpriseNum == 0) sql += "update dbo.Nodes set Context = '{\"OpRoutineStateId\":" + State + ",\"TicketContainerId\":null,\"TicketId\":null,\"OpDataId\":null,\"OpDataComponentId\":null,\"OpProcessData\":null,\"ResponsibleUserId\":null,\"LastStateChangeTime\":null}' where Id = '" + NodeId + "'";
				if (db.EnterpriseNum == 1) sql += "update dbo.Node set Context = '{\"OpRoutineStateId\":" + State + ",\"TicketContainerId\":null,\"TicketId\":null,\"OpDataId\":null,\"OpDataComponentId\":null,\"OpProcessData\":null,\"LastStateChangeTime\":null}' where Id = '" + NodeId + "'";

				db.SendRequestToDB(sql);

				Result = "Виконано";
			}
		}
	}
}
