using Gravitas.Monitoring.HelpClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gravitas.Monitoring.Pages
{
	public class ApUserModel : PageModel
	{
		[BindProperty]
		public string UserId { get; set; } = "";
		[BindProperty]
		public List<string[]> Roles { get; set; } = new List<string[]>();
		[BindProperty]
		public List<string> UserRoles { get; set; } = new List<string>();
		[BindProperty]
		public string UserName { get; set; } = "";

		[BindProperty]
		public bool a { get; set; } = false;
		[BindProperty]
		public bool m { get; set; } = false;
		[BindProperty]
		public bool u { get; set; } = false;

		[BindProperty]
		public string Result { get; set; } = "";

		public bool aa { get; set; } = false;
		public bool mm { get; set; } = false;
		public bool uu { get; set; } = false;


		public void OnGet()
		{
			UserId = HttpContext.Request.Query["UserId"].ToString();
			GetUserName();
			GetUseRoles();
			GetRoles();
			ShowRoles();
		}

		public void OnPost()
		{


			SaveRoles();

			GetUserName();
			GetUseRoles();
			GetRoles();
			ShowRoles();
		}

		private void SaveRoles()
		{
			Result = "";
			string sql = "";
			if (a)
			{
				sql = "if not exists (select top(1) 1 from dbo.AspNetUserRoles where UserId='" + UserId + "' and RoleId='8e23487e-f616-47cb-9c16-8d70b8958614')";
				sql += " insert into dbo.AspNetUserRoles (UserId, RoleId) values ('" + UserId + "', '8e23487e-f616-47cb-9c16-8d70b8958614')";
			}
			else
			{
				sql = "if exists (select top(1) 1 from dbo.AspNetUserRoles where UserId='" + UserId + "' and RoleId='8e23487e-f616-47cb-9c16-8d70b8958614')";
				sql += " delete from dbo.AspNetUserRoles where UserId = '" + UserId + "' and RoleId = '8e23487e-f616-47cb-9c16-8d70b8958614'";
			}
			try
			{
				db.SendRequestToDB(db.DBUsersConnStr, sql);
				Result += "Admin Ok<br />";
			}
			catch
			{
				Result += "Admin Error<br />";
			}
			//
			if (m)
			{
				sql = "if not exists (select top(1) 1 from dbo.AspNetUserRoles where UserId='" + UserId + "' and RoleId='59c9adb7-43df-4eb6-923c-5ba89104ab7d')";
				sql += " insert into dbo.AspNetUserRoles (UserId, RoleId) values ('" + UserId + "', '59c9adb7-43df-4eb6-923c-5ba89104ab7d')";
			}
			else
			{
				sql = "if exists (select top(1) 1 from dbo.AspNetUserRoles where UserId='" + UserId + "' and RoleId='59c9adb7-43df-4eb6-923c-5ba89104ab7d')";
				sql += " delete from dbo.AspNetUserRoles where UserId = '" + UserId + "' and RoleId = '59c9adb7-43df-4eb6-923c-5ba89104ab7d'";
			}
			try
			{
				db.SendRequestToDB(db.DBUsersConnStr,sql);
				Result += "Modifier Ok<br />";
			}
			catch
			{
				Result += "Modifier Error<br />";
			}
			//
			if (u)
			{
				sql = "if not exists (select top(1) 1 from dbo.AspNetUserRoles where UserId='" + UserId + "' and RoleId='e7f8a782-eecc-42a2-9594-7e1b77f470db')";
				sql += " insert into dbo.AspNetUserRoles (UserId, RoleId) values ('" + UserId + "', 'e7f8a782-eecc-42a2-9594-7e1b77f470db')";
			}
			else
			{
				sql = "if exists (select top(1) 1 from dbo.AspNetUserRoles where UserId='" + UserId + "' and RoleId='e7f8a782-eecc-42a2-9594-7e1b77f470db')";
				sql += " delete from dbo.AspNetUserRoles where UserId = '" + UserId + "' and RoleId = 'e7f8a782-eecc-42a2-9594-7e1b77f470db'";
			}
			try
			{
				db.SendRequestToDB(db.DBUsersConnStr, sql);
				Result += "User Ok<br />";
			}
			catch
			{
				Result += "User Error<br />";
			}
		}

		private void GetUserName()
		{
			List<string[]> tmp = new List<string[]>();
			db.GetDataFromDBMSSQL(db.DBUsersConnStr, "select UserName from dbo.AspNetUsers where Id = '" + UserId + "'", ref tmp);
			if (tmp.Count > 0)
			{
				UserName = tmp[0][0];
			}
			else
			{
				UserName = "No user name";
			}
		}

		private void ShowRoles()
		{
			foreach (string[] s in Roles)
			{
				if (UserRoles.Contains(s[0]))
				{
					if (s[1] == "Admin") a = aa = true;
					if (s[1] == "Modifier") m = mm = true;
					if (s[1] == "User") u = uu = true;
				}
			}
		}
		private void ShowRoles2()
		{
			foreach (string[] s in Roles)
			{
				if (UserRoles.Contains(s[0]))
				{
					if (s[1] == "Admin") a = true;
					if (s[1] == "Modifier") m = true;
					if (s[1] == "User") u = true;
				}
			}
		}

		private void GetRoles()
		{
			List<string[]> tmp = new List<string[]>();
			db.GetDataFromDBMSSQL(db.DBUsersConnStr, "select Id, Name from dbo.AspNetRoles order by Name", ref tmp);

			foreach (string[] s in tmp)
			{
				Roles.Add(new string[] { s[0], s[1], UserRoles.Contains(s[0]) ? "#" : "" });
			}


			Roles = tmp;
		}

		private void GetUseRoles()
		{
			List<string[]> tmp = new List<string[]>();
			db.GetDataFromDBMSSQL(db.DBUsersConnStr, "select * from dbo.AspNetUserRoles where UserId='" + UserId + "'", ref tmp);
			UserRoles.Clear();
			foreach (string[] s in tmp)
			{
				UserRoles.Add(s[1]);
			}
		}



	}
}
