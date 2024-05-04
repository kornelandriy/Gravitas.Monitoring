using Gravitas.Monitoring.HelpClasses;
using Gravitas.Monitoring.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Gravitas.Monitoring.Pages
{
	public class ApUsersModel : PageModel
	{
		[BindProperty]
		public List<string[]> ApUsers { get; set; } = new List<string[]>();

		public void OnGet()
		{
			GetUsers();
		}

		public void OnPost()
		{
			GetUsers();
		}

		private void GetUsers()
		{
			List<string[]> tmpR = new List<string[]>();
			db.GetDataFromDBMSSQL(db.DBUsersConnStr, "SELECT [UserId] ,AspNetRoles.Name FROM [monitoring].[dbo].[AspNetUserRoles] join AspNetRoles on AspNetRoles.Id = AspNetUserRoles.RoleId order by UserId, AspNetRoles.Name", ref tmpR);
			//
			List<string[]> tmp = new List<string[]>();
			db.GetDataFromDBMSSQL(db.DBUsersConnStr, "select Id, UserName from AspNetUsers", ref tmp);
			//
			ApUsers.Clear();
			string st = "";
			foreach (string[] s in tmp)
			{
				st = "";
				foreach(string[] sR in tmpR)
				{
					if (s[0] == sR[0])
						st+= sR[1] + "<br>";
				}
			ApUsers.Add(new string[] { s[0], s[1],st }) ;
			}
		}
	}
}
