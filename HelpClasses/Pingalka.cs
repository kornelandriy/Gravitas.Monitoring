using System.Net.NetworkInformation;
using System.Text;

namespace Gravitas.Monitoring.HelpClasses
{
	public static class Pingalka
	{
		private static int CurTimeout = 1000;

		public static string Test(string ip) { return Test(ip, 1000, 4); }
		public static string Test(string ip, int Timeout, int Count)
		{

			if (!IsIP(ip)) return "Wrong IP";

			CurTimeout = Timeout;
			string st = "<h4>Результат PING</h4>IP - "+ip+"<br />";
			long tmp = -10;
			for (int i = 0; i < Count; i++)
			{
				tmp = PingOne(ip);
				if (tmp > -1)
				{
					st += "Спроба №" + (i + 1) + " - " + tmp + "ms<br />";
				}
				else if (tmp == -1)
				{
					st += "Спроба №" + (i + 1) + " - Time out<br />";
					//st += "To|"; // TimeOut
				}
				else if (tmp == -2)
				{
					st += "Спроба №" + (i + 1) + " - Ping error<br />";
					//st += "Pe|"; // Ping error
				}
				else
				{
					st += "Спроба №" + (i + 1) + " - Other error<br />";
					//st += "Oe|"; // Other error
				}
			}
			return st;
		}

		private static long PingOne(string ip)
		{
			long result = -1;
			Ping pingSender = new Ping();
			PingOptions options = new PingOptions();
			options.DontFragment = true;
			string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
			byte[] buffer = Encoding.ASCII.GetBytes(data);
			int timeout = CurTimeout; // 1000;
			PingReply reply;

			try
			{
				reply = pingSender.Send(ip, timeout, buffer, options);
			}
			catch
			{
				return -2; // Ping Error
			}

			if (reply.Status == IPStatus.Success)
			{
				result = reply.RoundtripTime;
			}
			else
			{
				result = -1; // Timeout
			}

			return result;
		}

		public static bool IsIP(string ip)
		{
			string patternIP = "^([01]?\\d\\d?|2[0-4]\\d|25[0-5])\\" +
							   ".([01]?\\d\\d?|2[0-4]\\d|25[0-5])\\" +
							   ".([01]?\\d\\d?|2[0-4]\\d|25[0-5])\\" +
							   ".([01]?\\d\\d?|2[0-4]\\d|25[0-5])$";
			return System.Text.RegularExpressions.Regex.Match(ip, patternIP).Success;
		}
	}
}
