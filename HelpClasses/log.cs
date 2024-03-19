namespace Gravitas.Monitoring.HelpClasses
{
	public static class log
	{
		private static string fn = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + "\\Gravitas.Monitoring.log.txt";

		public static void Add(string txt, bool NewLine = false)
		{
			System.IO.File.AppendAllLines(fn, new string[] { (NewLine == true ? "\r\n" : "") + DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss") + " - " + txt });
		}
	}
}
