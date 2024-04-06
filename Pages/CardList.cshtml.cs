using Gravitas.Monitoring.HelpClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gravitas.Monitoring.Pages
{
	public class CardListModel : PageModel
	{
		[BindProperty]
		public List<string[]> Cards { get; set; } = new List<string[]>();
		[BindProperty]
		public string CardType { get; set; } = "2";
		[BindProperty]
		public string FindType { get; set; } = "1";
		[BindProperty]
		public string FindWhat { get; set; } = "";
		[BindProperty]
		public string tmpResult { get; set; } = "";

		// ### ### ### ### ### ### ### ### ### ### ### ### ### ### ### ### ### ### ### ### ### ### ### ### 







		// ### ### ### ### ### ### ### ### ### ### ### ### ### ### ### ### ### ### ### ### ### ### ### ### 

		public void OnGet()
		{
			string CardIn = HttpContext.Request.Query["CardId"].ToString();
			if (!string.IsNullOrEmpty(CardIn))
			{
				CardType = "2";
				FindType = "2";
				FindWhat = CardIn;
				Makelist();
			}
		}

		public void OnPost()
		{
			Makelist();
		}

		private void Makelist()
		{
			// string CardType = HttpContext.Request.Query["CardType"].ToString();
			//string FindWhat = HttpContext.Request.Query["FindWhat"].ToString();
			//string FindBy = HttpContext.Request.Query["FindBy"].ToString(); //   num    id
			//string FindWhat = HttpContext.Request.Query["FindWhat"].ToString();

			string sql = "";
			string SelectFilter = "Id, No, IsActive, TicketContainerId, EmployeeId";
			if (CardType != "" && FindType != "")
			{
				if (db.EnterpriseNum == 0)
				{
					if (FindType == "1") sql = "select " + SelectFilter + " from dbo.Cards where TypeId = '" + CardType + "' and No like N'%" + FindWhat + "%' order by No";
					if (FindType == "2") sql = "select " + SelectFilter + " from dbo.Cards where Id='" + FindWhat + "' ";
				}
				if (db.EnterpriseNum == 1)
				{
					if (FindType == "1") sql = "select " + SelectFilter + " from dbo.Card where TypeId = '" + CardType + "' and No like N'%" + FindWhat + "%' order by No";
					if (FindType == "2") sql = "select " + SelectFilter + " from dbo.Card where Id='" + FindWhat + "' ";
				}
				List<string[]> lst = new List<string[]>();
				db.GetDataFromDBMSSQL(sql, ref lst);
				Cards = lst;
			}
		}


	}
}
