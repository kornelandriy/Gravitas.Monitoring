using Gravitas.Monitoring.HelpClasses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NuGet.Protocol;

namespace Gravitas.Monitoring.Pages
{
	public class CardInfoModel : PageModel
	{
		[BindProperty]
		public string CardId { get; set; } = "";
		[BindProperty]
		public string tc { get; set; } = "";
		[BindProperty]
		public string UserId { get; set; } = "";
		[BindProperty]
		public string Result { get; set; } = "";
		[BindProperty]
		public bool IsActive { get; set; } = true;



		public void OnGet()
		{
			CardId = HttpContext.Request.Query["CardId"].ToString();
			GetCardData();
		}

		public void OnPost()
		{
			Result = "";
			WriteData();
		}

		private void GetCardData()
		{
			List<string[]> tmp = new List<string[]>();
			if (db.EnterpriseNum == 0) db.GetDataFromDBMSSQL("select * from dbo.Cards where Id = '" + CardId + "'", ref tmp);
			if (db.EnterpriseNum == 1) db.GetDataFromDBMSSQL("select * from dbo.Card where Id = '" + CardId + "'", ref tmp);
			if (db.EnterpriseNum == 0)
			{
				tc = tmp[0][5];
				UserId = tmp[0][4];
				IsActive = tmp[0][3] == "True" ? true : false;
			}
			if (db.EnterpriseNum == 1)
			{
				tc = tmp[0][4];
				UserId = tmp[0][5];
				IsActive = tmp[0][3] == "True" ? true : false;
			}
		}

		private void WriteData()
		{
			string _isactive = IsActive ? "1" : "0";
			string _tc = "";
			string _UserId = "";
			if (string.IsNullOrEmpty(tc)) _tc = "NULL"; else _tc = "'" + tc + "'";
			if (string.IsNullOrEmpty(UserId)) _UserId = "NULL"; else _UserId = "'" + UserId + "'";

			string sql = "";
			if (db.EnterpriseNum == 0) sql = "update dbo.Cards set IsActive=" + _isactive + ", TicketContainerId=" + _tc + ", EmployeeId=" + _UserId + " where Id = '" + CardId + "'";
			if (db.EnterpriseNum == 1) sql = "update dbo.Card set IsActive=" + _isactive + ", TicketContainerId=" + _tc + ", EmployeeId=" + _UserId + " where Id = '" + CardId + "'";
			try
			{
				db.SendRequestToDB(sql);
				log.Add("User: " + User.Identity.Name + " CardEdit new data (IsActive=" + _isactive + " Id: " + CardId + " TicketContainer: " + _tc + " EmployeeId: " + _UserId + ")");
				Result = "Збережено...";

			}
			catch (Exception ex)
			{
				log.Add("User: " + User.Identity.Name + " CardEdit Error:" + ex.ToString());
				Result = "Помилка: " + ex.ToString();
			}
		}
	}
}
