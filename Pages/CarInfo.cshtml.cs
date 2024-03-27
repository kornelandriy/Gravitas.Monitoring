using System.Drawing;
using Gravitas.Monitoring.HelpClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gravitas.Monitoring.Pages
{
	public class CarInfoModel : PageModel
	{
		public  string CurTC = "";
		private  string CurT = "";

		[BindProperty]
		public string tc { get; set; } = "";

		
		public string sNodes  = "";
		public string sTickets = "";
		public string sRoute = "";




		public string RouteTemplate = "";
		public string LastRouteItem = "";
		public string LastNode = "";


		//string tc = HttpContext.Request.Query["tc"].ToString();


		public string CarCards = "";

		//####################################################################################

		List<string[]> cards = new List<string[]>();

		[BindProperty]
		public List<string[]> lst1 { get; set; } = new List<string[]>(); // 2
		[BindProperty]
		public List<string[]> lst2 { get; set; } = new List<string[]>(); // 3
		[BindProperty]
		public List<string[]> lst3 { get; set; } = new List<string[]>(); // 4

		//#################################################################


		public void OnGet()
		{

			tc = HttpContext.Request.Query["tc"].ToString();



			//this.tc = tc;
			CurTC = tc;

			try
			{
				if (db.EnterpriseNum == 0) db.GetDataFromDBMSSQL("select * from dbo.Cards where TicketContainerId = " + tc, ref cards);
				if (db.EnterpriseNum == 1) db.GetDataFromDBMSSQL("select * from dbo.Card where TicketContainerId = " + tc, ref cards);

				foreach (string[] s in cards)
				{
					switch (s[1])
					{
						case "2":
							lst1.Add(new string[] { s[2], s[0] }); // "Картка водія"
							break;
						case "3":
							lst2.Add(new string[] { s[2], s[0] }); // "Мітка авто"
							break;
						case "4":
							lst3.Add(new string[] { s[2], s[0] }); // "Картка лотка"
							break;
					}
				}


				//CarCards = st;
			}
			catch { CarCards = "Не вдалося отримати дані про картки..."; }

			//####################################################################################

			List<string[]> tmp = new List<string[]>();
			if (db.EnterpriseNum == 0) db.GetDataFromDBMSSQL("select * from dbo.Tickets where TicketContainerId = '" + tc + "'", ref tmp);
			if (db.EnterpriseNum == 1) db.GetDataFromDBMSSQL("select * from dbo.Ticket where ContainerId = '" + tc + "'", ref tmp);
			//
			if (db.EnterpriseNum == 0) RouteTemplate = tmp[0][6];
			if (db.EnterpriseNum == 1) RouteTemplate = tmp[0][4];
			//
			if (db.EnterpriseNum == 0) LastRouteItem = tmp[0][7];
			if (db.EnterpriseNum == 1) LastRouteItem = tmp[0][5];
			//
			if (db.EnterpriseNum == 0) LastNode = tmp[0][9];
			if (db.EnterpriseNum == 1) LastNode = tmp[0][9];

			sRoute = ShowRoute(RouteTemplate, LastRouteItem, LastNode, tmp[0][0], tc) + "<br />";

			//####################################################################################

			sNodes = GetNodeDataByTC(tc);

			//####################################################################################

			sTickets = GetTickets();

			//####################################################################################
		}
		//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
		public  string ShowRoute(string RouteTemplateId, string LastRouteItem, string LastNode, string t, string tc)
		{
			if (RouteTemplateId == "") { return "No route"; }
			try
			{
				CurTC = tc;
				CurT = t;
				List<string[]> tmp = new List<string[]>();
				if (db.EnterpriseNum == 0) db.GetDataFromDBMSSQL("select * from dbo.RouteTemplates where Id = " + RouteTemplateId, ref tmp);
				if (db.EnterpriseNum == 1) db.GetDataFromDBMSSQL("select * from dbo.RouteTemplate where Id = " + RouteTemplateId, ref tmp);
				return "<hr>[" + RouteTemplateId + "] " + tmp[0][1] + "\r\n" + RouteParser(tmp[0][2]) + "<br><br>\r\nОстанній пройдений етап: [" + LastRouteItem + "] <br>\r\nОстанній пройдений вузол: [" + LastNode + "] " + GetNodeName(LastNode) + "<hr>";
			}
			catch (Exception ex) { return "Route parse error...<br>\r\n" + "LastNode: " + LastRouteItem + "<br>\r\n" + ex.ToString(); }
		}

		private  string RouteParser(string s)
		{
			string r = "";
			int iGroup = 0;
			string i1 = "{\"groupId\"";
			string i2 = "{\"id\"";
			for (int i = 0; i < s.Length - 10; i++)
			{
				if (s.Substring(i, i1.Length) == i1) iGroup++;
				if (s.Substring(i, i2.Length) == i2) r += iGroup + "#" + s.Substring(i + 6, 10).Split(',')[0] + "\r\n";
			}

			return RouteParserClear2(r);
		}

		private  string RouteParserClear2(string s)
		{
			string oldGroupNum = "#";
			s = s.Replace("\r", "");
			s = s.Substring(0, s.Length - 1);
			var NodeList = s.Split('\n').ToList();

			string s1 = "";
			string s2 = "";
			string r = "<table class=\"yozhstyle1\">";
			r += "<tr><td class=\"headercolor\">Група</td><td class=\"headercolor\">Вузли</td></tr>";
			bool FirstRun = true;
			int btns = 0;
			foreach (string ss in NodeList)
			{
				s1 = ss.Split('#')[0];
				s2 = ss.Split('#')[1];

				if (s1 != oldGroupNum)
				{
					r += (FirstRun ? "" : "</td></tr>\r\n") + "<tr><td class=\"brdr1sb\">" + s1 + "</td><td class=\"brdr1sb\">";
					btns++;
				}


				r += "[" + s2 + "] " + GetNodeName(s2) + "<br>";



				oldGroupNum = s1;
				FirstRun = false;
			}

			r += "</td></tr>\r\n</table>";

			r += "<br />Поставити авто перед групою:<br />";
			for (int i = 0; i < btns; i++)
			{
				r += "<a href=\"./MoveOnRoute?t=" + CurT + "&RouteItem=" + i + "&tc=" + CurTC + "\" class=\"btn btn-primary\">" + (i + 1) + "</a>&nbsp;";
			}
			r += "Або&nbsp;<a href=\"./MoveOnRoute?t=" + CurT + "&RouteItem=" + btns + "&tc=" + CurTC + "\" class=\"btn btn-primary\">Завершити маршрут</a>";

			return r;
		}


		private  string GetNodeName(string NodeId)
		{
			if (NodeId == "") return "#";
			List<string[]> tmp = new List<string[]>();
			if (db.EnterpriseNum == 0) db.GetDataFromDBMSSQL("select Name from dbo.Nodes where Id = " + NodeId, ref tmp);
			if (db.EnterpriseNum == 1) db.GetDataFromDBMSSQL("select Name from dbo.Node where Id = " + NodeId, ref tmp);
			return tmp[0][0];
		}

		//#########################################################################################################################################


		public  List<string[]> CarProgress = new List<string[]>();

		public  string[] StatusNamesForNode = new string[]
		{
			"",
			"Бланк", // 1
            "В обробці", // 2
            "На погодженні", // 3
            "Погоджено", // 4
            "Відмовлено у погодженні",//5
            "Очікування", // 6
            "", "", "",
			"Виконано", // 10
            "Відмовлено", // 11
            "Скасовано", // 12
            "Часткове завантаження", // 13
            "Часткове розвантаження", // 14
            "Перезавантаження" // 15
		};

		public  string GetNodeStatus(string id)
		{
			int n = 0;
			try { n = int.Parse(id); if (id == "") return "#"; else return "" + id + " - " + StatusNamesForNode[n]; }
			catch { return "#: " + id; }
		}

		private  List<string> GetTList(string tc)
		{
			List<string> result = new List<string>();
			List<string[]> tmp = new List<string[]>();
			if (db.EnterpriseNum == 0) db.GetDataFromDBMSSQL("select Id from dbo.Tickets where TicketContainerId = '" + tc + "'", ref tmp);
			if (db.EnterpriseNum == 1) db.GetDataFromDBMSSQL("select Id from dbo.Ticket where ContainerId = '" + tc + "'", ref tmp);
			if (tmp.Count == 0) return null;
			foreach (string[] s in tmp)
				result.Add(s[0]);
			return result;
		}


		private  string sReturn = "";
		public  string GetNodeDataByTC(string tc)
		{
			if (tc == "") return "";
			List<string> tmpT = GetTList(tc);
			CarProgress.Clear();
			CarProgress.Add(new string[] { "Id", "StateId", "NodeId", "CheckInDateTime", "CheckOutDateTime", "Інше", "Інше", "Інше", "Інше" });

			foreach (string s in tmpT)
			{
				CarProgress.Add(new string[] { "TicketId", s });
				GetNodeDataByT(s);
			}
			return sReturn;
		}

		private  void GetNodeDataByT(string t)
		{
			// 🚗🗺️🏠❤️🔨🍥🧆🧊🗽🗾♨️💈🛗🚽🪠❄️🔥💧☀️🌑🏠🏡🏚️🏘️🏟️🌏🌎🌍🌌🚦🚥📍✏️✒️🖋️🖊️🖌️🖍️📕📗📘📙📚🧮💾🪫🔋🪓🔨⛏️⚒️🛠️🔧♟️♠️♣️♥️♦️🧩🧿🎲💀☠️🔃
			Color CapColor = Color.Lime;
			List<string[]> tmp = new List<string[]>();
			string Etap = "";
			string TableName = "";
			try
			{
				//---------------------------------------------- KPP In
				Etap = "KPP In";
				tmp.Clear();
				if (db.EnterpriseNum == 0) db.GetDataFromDBMSSQL("select Id, StateId, NodeId, CheckInDateTime, CheckOutDateTime from dbo.SecurityCheckInOpDatas where TicketId = '" + t + "'", ref tmp);
				if (db.EnterpriseNum == 1) db.GetDataFromDBMSSQL("select Id, StateId, NodeId, CheckInDateTime, CheckOutDateTime from opd.SecurityCheckInOpData where TicketId = '" + t + "'", ref tmp);
				if (tmp.Count > 0)
				{
					if (db.EnterpriseNum == 0) TableName = "dbo.SecurityCheckInOpDatas";
					if (db.EnterpriseNum == 1) TableName = "opd.SecurityCheckInOpData";
					CarProgress.Add(new string[] { TableName, "КПП Заїзд" });
					foreach (string[] s in tmp)
					{ // 
					  //string sas = "<a href=\".ChangeStateId?id=" + s[0] + "&StateId=" + s[1] +"&tc="+CurTC+"&TableName="+TableName+"&NodeName="+ GetNodeName(s[2]) + "\">"+ GetNodeStatus(s[1]) + "</a>";
						CarProgress.Add(new string[] { s[0], "<a href=\"./ChangeStateId?id=" + s[0] + "&StateId=" + s[1] + "&tc=" + CurTC + "&TableName=" + TableName + "&NodeName=" + GetNodeName(s[2]) + "\" class=\"btn btn-primary\">" + GetNodeStatus(s[1]) + "</a>", GetNodeName(s[2]), s[3], s[4] });
						//CarProgress.Add(new string[] { s[0], "<a href=\"./\" class=\"btn btn-primary\">" + GetNodeStatus(s[1]) + "</a>", GetNodeName(s[2]), s[3], s[4] });
					}
				}
				//---------------------------------------------- Vizir / Lab
				Etap = "Vizir / Lab";
				tmp.Clear();
				if (db.EnterpriseNum == 0) db.GetDataFromDBMSSQL("select Id, StateId, NodeId, CheckInDateTime, CheckOutDateTime, ImpurityValue, HumidityValue, EffectiveValue, Comment from dbo.LabFacelessOpDatas where TicketId = '" + t + "'", ref tmp);
				if (db.EnterpriseNum == 1) db.GetDataFromDBMSSQL("select Id, StateId, NodeId, CheckInDateTime, CheckOutDateTime, ImpurityValue, HumidityValue, EffectiveValue, Comment from opd.LabFacelessOpData where TicketId = '" + t + "'", ref tmp);
				if (tmp.Count > 0)
				{
					if (db.EnterpriseNum == 0) TableName = "dbo.LabFacelessOpDatas";
					if (db.EnterpriseNum == 1) TableName = "opd.LabFacelessOpData";
					CarProgress.Add(new string[] { TableName, "Візіровка", "", "", "", "Засміченість", "Вологість", "П/М", "Коментар" });
					foreach (string[] s in tmp)
					{
						CarProgress.Add(new string[] { s[0], "<a href=\"./ChangeStateId?id=" + s[0] + "&StateId=" + s[1] + "&tc=" + CurTC + "&TableName=" + TableName + "&NodeName=" + GetNodeName(s[2]) + "\" class=\"btn btn-primary\">" + GetNodeStatus(s[1]) + "</a>", GetNodeName(s[2]), s[3], s[4], s[5], s[6], s[7], s[8] });
					}
				}
				//---------------------------------------------- Vagi
				Etap = "Vagi";
				tmp.Clear();
				if (db.EnterpriseNum == 0) db.GetDataFromDBMSSQL("select Id, StateId, NodeId, CheckInDateTime, CheckOutDateTime, TypeId, TruckWeightValue, TrailerWeightValue from dbo.ScaleOpDatas where TicketId = '" + t + "'", ref tmp);
				if (db.EnterpriseNum == 1) db.GetDataFromDBMSSQL("select Id, StateId, NodeId, CheckInDateTime, CheckOutDateTime, TypeId, TruckWeightValue, TrailerWeightValue from opd.ScaleOpData where TicketId = '" + t + "'", ref tmp);
				if (tmp.Count > 0)
				{
					if (db.EnterpriseNum == 0) TableName = "dbo.ScaleOpDatas";
					if (db.EnterpriseNum == 1) TableName = "opd.ScaleOpData";
					CarProgress.Add(new string[] { TableName, "Ваги", "", "", "", "Тип", "Вага Авто", "Вага причепа" });
					string TareOrGross = "HZ";
					foreach (string[] s in tmp)
					{
						TareOrGross = "HZ";
						if (s[5] == "1") TareOrGross = "Тара";
						if (s[5] == "2") TareOrGross = "Бруто";
						CarProgress.Add(new string[] { s[0], "<a href=\"./ChangeStateId?id=" + s[0] + "&StateId=" + s[1] + "&tc=" + CurTC + "&TableName=" + TableName + "&NodeName=" + GetNodeName(s[2]) + "\" class=\"btn btn-primary\">" + GetNodeStatus(s[1]) + "</a>", GetNodeName(s[2]), s[3], s[4], TareOrGross, s[6], s[7] });
					}
				}
				//---------------------------------------------- UnloadGuideOpDatas
				Etap = "UnloadGuideOpDatas";
				tmp.Clear();
				if (db.EnterpriseNum == 0) db.GetDataFromDBMSSQL("select Id, StateId, NodeId, CheckInDateTime, CheckOutDateTime, UnloadPointNodeId from dbo.UnloadGuideOpDatas where TicketId = '" + t + "'", ref tmp);
				if (db.EnterpriseNum == 1) db.GetDataFromDBMSSQL("select Id, StateId, NodeId, CheckInDateTime, CheckOutDateTime, UnloadPointNodeId from opd.UnloadGuideOpData where TicketId = '" + t + "'", ref tmp);
				if (tmp.Count > 0)
				{
					if (db.EnterpriseNum == 0) TableName = "dbo.UnloadGuideOpDatas";
					if (db.EnterpriseNum == 1) TableName = "opd.UnloadGuideOpData";
					CarProgress.Add(new string[] { TableName, "Призн... розвантаження", "", "", "", "Вибрана точка розвантаження" });
					foreach (string[] s in tmp)
					{
						CarProgress.Add(new string[] { s[0], "<a href=\"./ChangeStateId?id=" + s[0] + "&StateId=" + s[1] + "&tc=" + CurTC + "&TableName=" + TableName + "&NodeName=" + GetNodeName(s[2]) + "\" class=\"btn btn-primary\">" + GetNodeStatus(s[1]) + "</a>", GetNodeName(s[2]), s[3], s[4], GetNodeName(s[5]) });
					}
				}
				//---------------------------------------------- UnloadPointOpDatas
				Etap = "UnloadPointOpDatas";
				tmp.Clear();
				if (db.EnterpriseNum == 0) db.GetDataFromDBMSSQL("select Id, StateId, NodeId, CheckInDateTime, CheckOutDateTime from dbo.UnloadPointOpDatas where TicketId = '" + t + "'", ref tmp);
				if (db.EnterpriseNum == 1) db.GetDataFromDBMSSQL("select Id, StateId, NodeId, CheckInDateTime, CheckOutDateTime from opd.UnloadPointOpData where TicketId = '" + t + "'", ref tmp);
				if (tmp.Count > 0)
				{
					if (db.EnterpriseNum == 0) TableName = "dbo.UnloadPointOpDatas";
					if (db.EnterpriseNum == 1) TableName = "opd.UnloadPointOpData";
					CarProgress.Add(new string[] { TableName, "Точка розвантаження" });
					foreach (string[] s in tmp)
					{
						CarProgress.Add(new string[] { s[0], "<a href=\"./ChangeStateId?id=" + s[0] + "&StateId=" + s[1] + "&tc=" + CurTC + "&TableName=" + TableName + "&NodeName=" + GetNodeName(s[2]) + "\" class=\"btn btn-primary\">" + GetNodeStatus(s[1]) + "</a>", GetNodeName(s[2]), s[3], s[4] });
					}
				}
				//---------------------------------------------- LoadGuideOpDatas
				Etap = "LoadGuideOpDatas";
				tmp.Clear();
				if (db.EnterpriseNum == 0) db.GetDataFromDBMSSQL("select Id, StateId, NodeId, CheckInDateTime, CheckOutDateTime, LoadPointNodeId from dbo.LoadGuideOpDatas where TicketId = '" + t + "'", ref tmp);
				if (db.EnterpriseNum == 1) db.GetDataFromDBMSSQL("select Id, StateId, NodeId, CheckInDateTime, CheckOutDateTime, LoadPointNodeId from opd.LoadGuideOpData where TicketId = '" + t + "'", ref tmp);
				if (tmp.Count > 0)
				{
					if (db.EnterpriseNum == 0) TableName = "dbo.LoadGuideOpDatas";
					if (db.EnterpriseNum == 1) TableName = "opd.LoadGuideOpData";
					CarProgress.Add(new string[] { TableName, "Призн... Завантаження", "", "", "", "Вибрана точка завантаження" });
					foreach (string[] s in tmp)
					{
						CarProgress.Add(new string[] { s[0], "<a href=\"./ChangeStateId?id=" + s[0] + "&StateId=" + s[1] + "&tc=" + CurTC + "&TableName=" + TableName + "&NodeName=" + GetNodeName(s[2]) + "\" class=\"btn btn-primary\">" + GetNodeStatus(s[1]) + "</a>", GetNodeName(s[2]), s[3], s[4], GetNodeName(s[5]) });
					}
				}
				//---------------------------------------------- LoadPointOpDatas
				Etap = "LoadPointOpDatas";
				tmp.Clear();
				if (db.EnterpriseNum == 0) db.GetDataFromDBMSSQL("select Id, StateId, NodeId, CheckInDateTime, CheckOutDateTime from dbo.LoadPointOpDatas where TicketId = '" + t + "'", ref tmp);
				if (db.EnterpriseNum == 1) db.GetDataFromDBMSSQL("select Id, StateId, NodeId, CheckInDateTime, CheckOutDateTime from opd.LoadPointOpData where TicketId = '" + t + "'", ref tmp);
				if (tmp.Count > 0)
				{
					if (db.EnterpriseNum == 0) TableName = "dbo.LoadPointOpDatas";
					if (db.EnterpriseNum == 1) TableName = "opd.LoadPointOpData";
					CarProgress.Add(new string[] { TableName, "Точка завантаження" });
					foreach (string[] s in tmp)
					{
						CarProgress.Add(new string[] { s[0], "<a href=\"./ChangeStateId?id=" + s[0] + "&StateId=" + s[1] + "&tc=" + CurTC + "&TableName=" + TableName + "&NodeName=" + GetNodeName(s[2]) + "\" class=\"btn btn-primary\">" + GetNodeStatus(s[1]) + "</a>", GetNodeName(s[2]), s[3], s[4] });
					}
				}
				//---------------------------------------------- CentralLabOpDatas
				Etap = "CentralLabOpDatas";
				tmp.Clear();
				if (db.EnterpriseNum == 0) db.GetDataFromDBMSSQL("select Id, StateId, NodeId, CheckInDateTime, CheckOutDateTime from dbo.CentralLabOpDatas where TicketId = '" + t + "'", ref tmp);
				if (db.EnterpriseNum == 1) db.GetDataFromDBMSSQL("select Id, StateId, NodeId, CheckInDateTime, CheckOutDateTime from opd.CentralLabOpData where TicketId = '" + t + "'", ref tmp);
				if (tmp.Count > 0)
				{
					if (db.EnterpriseNum == 0) TableName = "dbo.CentralLabOpDatas";
					if (db.EnterpriseNum == 1) TableName = "opd.CentralLabOpData";
					CarProgress.Add(new string[] { TableName, "Центр... лабораторія" });
					foreach (string[] s in tmp)
					{
						CarProgress.Add(new string[] { s[0], "<a href=\"./ChangeStateId?id=" + s[0] + "&StateId=" + s[1] + "&tc=" + CurTC + "&TableName=" + TableName + "&NodeName=" + GetNodeName(s[2]) + "\" class=\"btn btn-primary\">" + GetNodeStatus(s[1]) + "</a>", GetNodeName(s[2]), s[3], s[4] });
					}
				}
				//---------------------------------------------- CheckPointOpDatas
				if (db.EnterpriseNum == 0)
				{
					Etap = "CheckPointOpDatas";
					tmp.Clear();
					if (db.EnterpriseNum == 0) db.GetDataFromDBMSSQL("select Id, StateId, NodeId, CheckInDateTime, CheckOutDateTime from dbo.CheckPointOpDatas where TicketId = '" + t + "'", ref tmp);
					if (db.EnterpriseNum == 1) db.GetDataFromDBMSSQL("select Id, StateId, NodeId, CheckInDateTime, CheckOutDateTime from opd.CheckPointOpData where TicketId = '" + t + "'", ref tmp);
					if (tmp.Count > 0)
					{
						if (db.EnterpriseNum == 0) TableName = "dbo.CheckPointOpDatas";
						if (db.EnterpriseNum == 1) TableName = "opd.CheckPointOpData";
						CarProgress.Add(new string[] { TableName, "Чепоінти..." });
						foreach (string[] s in tmp)
						{
							CarProgress.Add(new string[] { s[0], "<a href=\"./ChangeStateId?id=" + s[0] + "&StateId=" + s[1] + "&tc=" + CurTC + "&TableName=" + TableName + "&NodeName=" + GetNodeName(s[2]) + "\" class=\"btn btn-primary\">" + GetNodeStatus(s[1]) + "</a>", GetNodeName(s[2]), s[3], s[4] });
						}
					}
				}
				//---------------------------------------------- KPP Out
				Etap = "KPP Out";
				tmp.Clear();
				if (db.EnterpriseNum == 0) db.GetDataFromDBMSSQL("select Id, StateId, NodeId, CheckInDateTime, CheckOutDateTime from dbo.SecurityCheckOutOpDatas where TicketId = '" + t + "'", ref tmp);
				if (db.EnterpriseNum == 1) db.GetDataFromDBMSSQL("select Id, StateId, NodeId, CheckInDateTime, CheckOutDateTime from opd.SecurityCheckOutOpData where TicketId = '" + t + "'", ref tmp);
				if (tmp.Count > 0)
				{
					if (db.EnterpriseNum == 0) TableName = "dbo.SecurityCheckOutOpDatas";
					if (db.EnterpriseNum == 1) TableName = "opd.SecurityCheckOutOpData";
					CarProgress.Add(new string[] { TableName, "КПП Виїзд" });
					foreach (string[] s in tmp)
					{
						CarProgress.Add(new string[] { s[0], "<a href=\"./ChangeStateId?id=" + s[0] + "&StateId=" + s[1] + "&tc=" + CurTC + "&TableName=" + TableName + "&NodeName=" + GetNodeName(s[2]) + "\" class=\"btn btn-primary\">" + GetNodeStatus(s[1]) + "</a>", GetNodeName(s[2]), s[3], s[4] });
					}
				}
			}
			catch (Exception ex) { sReturn = "Etap: " + Etap + "\r\n\r\n" + ex.ToString(); }

			string str = "<table Class=\"yozhstyle1\">\r\n";
			string st = "";

			foreach (string[] s in CarProgress)
			{
				st = "<tr>";
				foreach (string ss in s)
				{
					st += "<td class=\"brdr1sb\">" + ss + "</td>";
				}
				st += "</tr>\r\n";
				str += st;
			}
			str += "</table>\r\n";
			sReturn = "<div style=\"overflow: auto;\">" + str + "</div>";
		}

		//###################################################################################################################

		public  string GetTickets()
		{
			List<string[]> tmp = new List<string[]>();
			string sql = "";


			if (db.EnterpriseNum == 0) sql = "select StatusId, RoutetemplateId, RouteItemIndex, SecondaryRouteTemplateId, SecondaryRouteItemIndex, Id from dbo.Tickets where TicketContainerId = '" + CurTC + "'";
			if (db.EnterpriseNum == 1) sql = "select StatusId, RoutetemplateId, RouteItemIndex, SecondaryRouteTemplateId, SecondaryRouteItemIndex, Id from dbo.Ticket where ContainerId = '" + CurTC + "'";
			db.GetDataFromDBMSSQL(sql, ref tmp);

			string r = "<br /><hr /><br /><b>Тікети(ТТН)(" + tmp.Count + ")</b><table class=\"yozhstyle1\">";
			r += "<tr><td class=\"brdr1sb\">id</td><td class=\"brdr1sb\">Статус</td><td class=\"brdr1sb\">Маршрут</td><td class=\"brdr1sb\">Етап</td><td class=\"brdr1sb\">Доп. маршрут</td><td class=\"brdr1sb\">Доп. етап</td><td class=\"brdr1sb\">🖍️</td></tr>";

			foreach (string[] s in tmp)
			{
				r += "<tr><td class=\"brdr1sb\">" + s[5] + "</td><td class=\"brdr1sb\">" + s[0] + "</td><td class=\"brdr1sb\">" + s[1] + "</td><td class=\"brdr1sb\">" + s[2] + "</td><td class=\"brdr1sb\">" + s[3] + "</td><td class=\"brdr1sb\">" + s[4] + "</td><td class=\"brdr1sb\"><a href=\"./TicketEdit?t=" + s[5] + "\" class=\"btn btn-primary\">🖍️</a></td></tr>";
			}
			r += "</table>";

			return r;
		}


	}
}
