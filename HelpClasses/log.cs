using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Gravitas.Monitoring.Pages;
using Microsoft.AspNetCore.Identity;

namespace Gravitas.Monitoring.HelpClasses
{
	public static class log
	{
		


		private static string fn = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + "\\Gravitas.Monitoring.log.txt";

		public static void Add(string txt, bool NewLine = false)
		{
			File.AppendAllLines(fn, new string[] { (NewLine == true ? "\r\n" : "") + DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss") + " - " + txt });
		}

		public static void Add(int num, bool NewLine = false)
		{
			Add(num.ToString(), NewLine);
		}
	}
}
