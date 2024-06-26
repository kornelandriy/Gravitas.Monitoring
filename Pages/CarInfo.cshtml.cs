﻿using System.Drawing;
using Gravitas.Monitoring.HelpClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gravitas.Monitoring.Pages
{
	public class CarInfoModel : PageModel
	{
		public string CurTC = "";
		private string CurT = "";


		[BindProperty]
		public string tc { get; set; } = "";
		[BindProperty]
		public List<string[]> Route { get; set; } = new List<string[]>();
		[BindProperty]
		public string RouteNane { get; set; } = "";
		[BindProperty]
		public string LastRouteItem { get; set; } = "";
		[BindProperty]
		public string LastRouteNode { get; set; } = "";
		[BindProperty]
		public string LastRouteNodeName { get; set; } = "";
		[BindProperty]
		public List<string[]> CarProgress { get; set; } = new List<string[]>();
		[BindProperty]
		public bool FrCarProgress { get; set; } = true;
		[BindProperty]
		public List<string[]> TicketsList { get; set; } = new List<string[]>();
		[BindProperty]
		public string CurFeed { get; set; } = "";
		[BindProperty]
		public string CurCarNo { get; set; } = "";
		[BindProperty]
		public string CurTrailerNo { get; set; } = "";
		[BindProperty]
		public bool ShowLongRangeFlag { get; set; } = false;

		//##################################################################################################################################

		public string sNodes = "";
		public string sTickets = "";
		public string RouteTemplate = "";
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
			DoIt();
		}

		public void OnPost()
		{
			DoIt();
		}

		private void DoIt()
		{
			CurTC = tc;
			GetCrInfo();
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
			}
			catch { CarCards = "Не вдалося отримати дані про картки..."; }
			//####################################################################################
			string LastNode = "";
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
			//
			ShowRoute(RouteTemplate, LastRouteItem, LastNode, tmp[0][0], tc);

			//####################################################################################

			sNodes = GetNodeDataByTC(tc);

			//####################################################################################

			sTickets = GetTickets();

			//####################################################################################
		}

		private void GetCrInfo()
		{
			List<string[]> tmp = new List<string[]>();
			string sql = "";
			if (db.EnterpriseNum == 0)
			{
				sql = "SELECT SingleWindowOpDatas.ProductTitle, (select RegistrationNo from FixedAssets where Id = SingleWindowOpDatas.TransportId) as 'CNo', SingleWindowOpDatas.HiredTransportNumber, (select RegistrationNo from FixedAssets where Id = SingleWindowOpDatas.TrailerId) as 'TNo', SingleWindowOpDatas.HiredTrailerNumber FROM [mhp].[dbo].[SingleWindowOpDatas] where SingleWindowOpDatas.TicketContainerId='" + CurTC + "'";
				db.GetDataFromDBMSSQL(sql, ref tmp);
				if (tmp.Count > 0)
				{
					if (tmp[0][1] != "") CurCarNo = tmp[0][1]; else CurCarNo = tmp[0][2];
					if (tmp[0][3] != "") CurTrailerNo = tmp[0][3]; else CurTrailerNo = tmp[0][4];
					CurFeed = tmp[0][0];
				}
				else
				{
					CurCarNo = "ND";
					CurTrailerNo = "ND";
					CurFeed = "ND";
				}
			}
			else
			{
				CurCarNo = "ND";
				CurTrailerNo = "ND";
				CurFeed = "ND";
			}
		}

		//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
		public void ShowRoute(string RouteTemplateId, string LastRouteItem, string LastNode, string t, string tc)
		{
			if (RouteTemplateId == "") { return; }
			List<string[]> RouteTmp = new List<string[]>();
			List<string[]> tmp = new List<string[]>();
			try
			{
				CurTC = tc;
				CurT = t;
				List<string[]> tmpp = new List<string[]>();
				if (db.EnterpriseNum == 0) db.GetDataFromDBMSSQL("select * from dbo.RouteTemplates where Id = " + RouteTemplateId, ref tmp);
				if (db.EnterpriseNum == 1) db.GetDataFromDBMSSQL("select * from dbo.RouteTemplate where Id = " + RouteTemplateId, ref tmp);
				RouteNane = "[" + RouteTemplateId + "] " + tmp[0][1];
				LastRouteNode = LastNode;
				LastRouteNodeName = GetNodeName(LastNode);
				//
				RouteParser(ref RouteTmp, tmp[0][2]);
				Route = RouteTmp;
			}
			catch (Exception ex) { /* return "Route parse error...<br>\r\n" + "LastNode: " + LastRouteItem + "<br>\r\n" + ex.ToString(); */ }
		}

		private void RouteParser(ref List<string[]> lst, string s)
		{
			List<string[]> tmp = new List<string[]>();
			lst.Clear();
			string r = "";
			int iGroup = 0;
			string i1 = "{\"groupId\"";
			string i2 = "{\"id\"";
			for (int i = 0; i < s.Length - 10; i++)
			{
				if (s.Substring(i, i1.Length) == i1) iGroup++;
				if (s.Substring(i, i2.Length) == i2) r += iGroup + "#" + s.Substring(i + 6, 10).Split(',')[0] + "\r\n";
			}
			//
			RouteParserClear2(ref tmp, r);
			//
			string s0 = "";
			string s1 = "";
			bool fr = true;
			foreach (string[] ss in tmp)
			{
				if (ss[0] != s0)
				{
					if (fr)
					{
						s1 = "";
						fr = false;
					}
					else
					{
						lst.Add(new string[] { s0, s1 });
						s1 = "";
					}
					s0 = ss[0];
				}
				s1 += ss[1] + "<br />";
			}
			lst.Add(new string[] { s0, s1 });
		}

		private void RouteParserClear2(ref List<string[]> lst, string s)
		{
			string oldGroupNum = "#";
			s = s.Replace("\r", "");
			s = s.Substring(0, s.Length - 1);
			var NodeList = s.Split('\n').ToList();
			//
			string s1 = "";
			string s2 = "";
			//
			foreach (string ss in NodeList)
			{
				s1 = ss.Split('#')[0];
				s2 = ss.Split('#')[1];

				if (s1 == LastRouteItem && s2 == LastRouteNode)
				{
					lst.Add(new string[] { s1, "👉[" + s2 + "] " + GetNodeName(s2) + " " + LongRangeRFID(s2) });
				}
				else
				{
					lst.Add(new string[] { s1, "[" + s2 + "] " + GetNodeName(s2) + " " + LongRangeRFID(s2) });
				}

			}
		}

		private string LongRangeRFID(string NodeId)
		{
			string rslt = "";
			if (!ShowLongRangeFlag)
			{
				return "";
			}
			List<string[]> tmp = new List<string[]>();
			if (db.EnterpriseNum == 0)
			{
				if (db.EnterpriseNum == 0) db.GetDataFromDBMSSQL("select Config from dbo.Nodes where Id = " + NodeId, ref tmp);

				List<string[]> tmp2 = new List<string[]>();
				ParseNodeConfig(ref tmp2, tmp[0][0]);
				foreach (string[] s in tmp2)
				{
					if (IsDeviceTypeLongRangeRFIDById(s[0]))
					{
						rslt += "<a href=\"./ReplaceLabel?tc=" + tc + "&AntennaId=" + s[0] +"\" style=\"btn btn-primary\">🛜</a>";
					}
				}
			}
			else
			{
				rslt = "";
			}
			return rslt;
		}

		private void ParseNodeConfig(ref List<string[]> lst, string NodeConfig)
		{
			lst.Clear();
			string find = "\"DeviceId\":";
			int c = 0;
			int n = 0;
			string nums = "0123456789";
			//
			string s = "";
			n = NodeConfig.IndexOf(find, n);
			//
			if (n < find.Length + 5)
			{
				return;
			}
			//
			string sc = "";
			try
			{
				do
				{
					s = "";
					for (int i = n + find.Length; i < n + 20; i++)
					{
						sc += NodeConfig.Substring(i, 1);
						if (nums.Contains(NodeConfig.Substring(i, 1)))
						{
							s += NodeConfig.Substring(i, 1);
						}
						else
						{
							break;
						}
					}
					sc += "\r\n";
					lst.Add(new string[] { s, GetDeviceNameById(s) });
					n = NodeConfig.IndexOf(find, n + find.Length);
					//
					c++;
					//if (c > 50) { /* return new string[] { "Error (50 спроб...)" }.ToList(); */ }
					if (c > 50) { log.Add("ParseNodeConfig: Error: (50 спроб...)"); }
				} while (n > -1);
			}
			catch (Exception ex)
			{
				log.Add("ParseNodeConfig: Error: " + ex.ToString());
			}
		}

		private string GetDeviceNameById(string Id)
		{
			List<string[]> tmp = new List<string[]>();
			db.GetDataFromDBMSSQL(" select Name from dbo.Devices where Id = '" + Id + "'", ref tmp);
			return tmp.Count > 0 ? tmp[0][0] : "Wrong device Id...";
		}

		private bool IsDeviceTypeLongRangeRFIDById(string DeviceId)
		{
			List<string[]> tmp = new List<string[]>();
			db.GetDataFromDBMSSQL("select Typeid from dbo.Devices where Id = '" + DeviceId + "'", ref tmp);
			if (tmp.Count > 0)
			{
				if (tmp[0][0] == "2" || tmp[0][0] == "31")
				{
					return true;
				}
			}
			return false;
		}

		private string GetNodeName(string NodeId, bool FindRfid = false)
		{

			if (NodeId == "") return "#";
			List<string[]> tmp = new List<string[]>();
			List<string[]> tmp2 = new List<string[]>();
			if (db.EnterpriseNum == 0) db.GetDataFromDBMSSQL("select Name from dbo.Nodes where Id = " + NodeId, ref tmp);
			if (db.EnterpriseNum == 1) db.GetDataFromDBMSSQL("select Name from dbo.Node where Id = " + NodeId, ref tmp);

			return tmp[0][0];
		}

		//#########################################################################################################################################

		public string[] StatusNamesForNode = new string[]
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

		public string GetNodeStatus(string id)
		{
			int n = 0;
			try { n = int.Parse(id); if (id == "") return "#"; else return "" + id + " - " + StatusNamesForNode[n]; }
			catch { return "#: " + id; }
		}

		private List<string> GetTList(string tc)
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


		private string sReturn = "";
		public string GetNodeDataByTC(string tc)
		{
			if (tc == "") return "";
			List<string> tmpT = GetTList(tc);
			CarProgress.Clear();
			//CarProgress.Add(new string[] { "Id", "StateId", "NodeId", "CheckInDateTime", "CheckOutDateTime", "Інше", "Інше", "Інше", "Інше" });
			CarProgress.Add(new string[] { "Тип вузла", "Стан", "Вузол", "Початок обробки", "Кінець обробки", "Інше", "Інше", "Інше", "Інше" });

			foreach (string s in tmpT)
			{
				CarProgress.Add(new string[] { "TicketId", s });
				GetNodeDataByT(s);
			}
			return sReturn;
		}

		private void GetNodeDataByT(string t)
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
					//CarProgress.Add(new string[] { TableName, "КПП Заїзд" });
					foreach (string[] s in tmp)
					{
						CarProgress.Add(new string[] { "КПП Заїзд", "<a href=\"./ChangeStateId?id=" + s[0] + "&StateId=" + s[1] + "&tc=" + CurTC + "&TableName=" + TableName + "&NodeName=" + GetNodeName(s[2]) + "\" class=\"btn btn-primary\" style=\"white-space: nowrap;\">" + GetNodeStatus(s[1]) + "</a>", GetNodeName(s[2]), s[3], s[4] });
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
					//CarProgress.Add(new string[] { TableName, "Візіровка", "", "", "", "Засміченість", "Вологість", "П/М", "Коментар" });
					CarProgress.Add(new string[] { "", "", "", "", "", "Засміченість", "Вологість", "П/М", "Коментар" });
					foreach (string[] s in tmp)
					{
						CarProgress.Add(new string[] { "Візіровка", "<a href=\"./ChangeStateId?id=" + s[0] + "&StateId=" + s[1] + "&tc=" + CurTC + "&TableName=" + TableName + "&NodeName=" + GetNodeName(s[2]) + "\" class=\"btn btn-primary\">" + GetNodeStatus(s[1]) + "</a>", GetNodeName(s[2]), s[3], s[4], s[5], s[6], s[7], s[8] });
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
					//CarProgress.Add(new string[] { TableName, "Ваги", "", "", "", "Тип", "Вага Авто", "Вага причепа" });
					CarProgress.Add(new string[] { "", "", "", "", "", "Тип", "Вага Авто", "Вага причепа" });
					string TareOrGross = "HZ";
					foreach (string[] s in tmp)
					{
						TareOrGross = "HZ";
						if (s[5] == "1") TareOrGross = "Тара";
						if (s[5] == "2") TareOrGross = "Бруто";
						//CarProgress.Add(new string[] { s[0], "<a href=\"./ChangeStateId?id=" + s[0] + "&StateId=" + s[1] + "&tc=" + CurTC + "&TableName=" + TableName + "&NodeName=" + GetNodeName(s[2]) + "\" class=\"btn btn-primary\">" + GetNodeStatus(s[1]) + "</a>", GetNodeName(s[2]), s[3], s[4], TareOrGross, s[6], s[7] });
						CarProgress.Add(new string[] { "Ваги", "<a href=\"./ChangeStateId?id=" + s[0] + "&StateId=" + s[1] + "&tc=" + CurTC + "&TableName=" + TableName + "&NodeName=" + GetNodeName(s[2]) + "\" class=\"btn btn-primary\">" + GetNodeStatus(s[1]) + "</a>", GetNodeName(s[2]), s[3], s[4], TareOrGross, s[6], s[7] });
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
					//CarProgress.Add(new string[] { TableName, "Призн... розвантаження", "", "", "", "Вибрана точка розвантаження" });
					CarProgress.Add(new string[] { "", "", "", "", "", "Вибрана точка розвантаження" });
					foreach (string[] s in tmp)
					{
						CarProgress.Add(new string[] { "Призначення розвантаження", "<a href=\"./ChangeStateId?id=" + s[0] + "&StateId=" + s[1] + "&tc=" + CurTC + "&TableName=" + TableName + "&NodeName=" + GetNodeName(s[2]) + "\" class=\"btn btn-primary\">" + GetNodeStatus(s[1]) + "</a>", GetNodeName(s[2]), s[3], s[4], GetNodeName(s[5]) });
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
					//CarProgress.Add(new string[] { TableName, "Точка розвантаження" });
					foreach (string[] s in tmp)
					{
						CarProgress.Add(new string[] { "Точка розвантаження", "<a href=\"./ChangeStateId?id=" + s[0] + "&StateId=" + s[1] + "&tc=" + CurTC + "&TableName=" + TableName + "&NodeName=" + GetNodeName(s[2]) + "\" class=\"btn btn-primary\">" + GetNodeStatus(s[1]) + "</a>", GetNodeName(s[2]), s[3], s[4] });
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
					//CarProgress.Add(new string[] { TableName, "Призн... Завантаження", "", "", "", "Вибрана точка завантаження" });
					CarProgress.Add(new string[] { "", "", "", "", "", "Вибрана точка завантаження" });
					foreach (string[] s in tmp)
					{
						CarProgress.Add(new string[] { "Призначення завантаження", "<a href=\"./ChangeStateId?id=" + s[0] + "&StateId=" + s[1] + "&tc=" + CurTC + "&TableName=" + TableName + "&NodeName=" + GetNodeName(s[2]) + "\" class=\"btn btn-primary\">" + GetNodeStatus(s[1]) + "</a>", GetNodeName(s[2]), s[3], s[4], GetNodeName(s[5]) });
					}
				}
				//---------------------------------------------- LoadPointOpDatas
				Etap = "LoadPointOpDatas";
				tmp.Clear();
				if (db.EnterpriseNum == 0) db.GetDataFromDBMSSQL("select Id, StateId, NodeId, CheckInDateTime, CheckOutDateTime, LoadSiloNames from dbo.LoadPointOpDatas where TicketId = '" + t + "'", ref tmp);
				if (db.EnterpriseNum == 1) db.GetDataFromDBMSSQL("select Id, StateId, NodeId, CheckInDateTime, CheckOutDateTime from opd.LoadPointOpData where TicketId = '" + t + "'", ref tmp);
				if (tmp.Count > 0)
				{
					if (db.EnterpriseNum == 0) TableName = "dbo.LoadPointOpDatas";
					if (db.EnterpriseNum == 1) TableName = "opd.LoadPointOpData";
					//CarProgress.Add(new string[] { TableName, "Точка завантаження", "", "", "", "Силоси" });
					CarProgress.Add(new string[] { "", "", "", "", "", "Силоси" });
					foreach (string[] s in tmp)
					{
						if (db.EnterpriseNum == 0) CarProgress.Add(new string[] { "Точка завантаження", "<a href=\"./ChangeStateId?id=" + s[0] + "&StateId=" + s[1] + "&tc=" + CurTC + "&TableName=" + TableName + "&NodeName=" + GetNodeName(s[2]) + "\" class=\"btn btn-primary\">" + GetNodeStatus(s[1]) + "</a>", GetNodeName(s[2]), s[3], s[4], (string.IsNullOrEmpty(s[5]) ? "###" + s[0] : s[5]) });
						if (db.EnterpriseNum == 1) CarProgress.Add(new string[] { "Точка завантаження", "<a href=\"./ChangeStateId?id=" + s[0] + "&StateId=" + s[1] + "&tc=" + CurTC + "&TableName=" + TableName + "&NodeName=" + GetNodeName(s[2]) + "\" class=\"btn btn-primary\">" + GetNodeStatus(s[1]) + "</a>", GetNodeName(s[2]), s[3], s[4] });
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
					//CarProgress.Add(new string[] { TableName, "Центр... лабораторія" });
					foreach (string[] s in tmp)
					{
						CarProgress.Add(new string[] { "Центр... лабораторія", "<a href=\"./ChangeStateId?id=" + s[0] + "&StateId=" + s[1] + "&tc=" + CurTC + "&TableName=" + TableName + "&NodeName=" + GetNodeName(s[2]) + "\" class=\"btn btn-primary\">" + GetNodeStatus(s[1]) + "</a>", GetNodeName(s[2]), s[3], s[4] });
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
						//CarProgress.Add(new string[] { TableName, "Чепоінти..." });
						foreach (string[] s in tmp)
						{
							CarProgress.Add(new string[] { "Чепоінти", "<a href=\"./ChangeStateId?id=" + s[0] + "&StateId=" + s[1] + "&tc=" + CurTC + "&TableName=" + TableName + "&NodeName=" + GetNodeName(s[2]) + "\" class=\"btn btn-primary\">" + GetNodeStatus(s[1]) + "</a>", GetNodeName(s[2]), s[3], s[4] });
						}
					}
				}
				if (db.EnterpriseNum == 0) // MZVKK
				{
					//---------------------------------------------- Pay Office
					Etap = "Pay Office";
					tmp.Clear();
					db.GetDataFromDBMSSQL("select Id, StateId, NodeId, CheckInDateTime, CheckOutDateTime from dbo.PayOfficeOpDatas where TicketId = '" + t + "'", ref tmp);

					if (tmp.Count > 0)
					{
						TableName = "dbo.PayOfficeOpDatas";
						//CarProgress.Add(new string[] { TableName, "Каса" });
						foreach (string[] s in tmp)
						{
							CarProgress.Add(new string[] { "Каса", "<a href=\"./ChangeStateId?id=" + s[0] + "&StateId=" + s[1] + "&tc=" + CurTC + "&TableName=" + TableName + "&NodeName=" + GetNodeName(s[2]) + "\" class=\"btn btn-primary\">" + GetNodeStatus(s[1]) + "</a>", GetNodeName(s[2]), s[3], s[4] });
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
					//CarProgress.Add(new string[] { TableName, "КПП Виїзд" });
					foreach (string[] s in tmp)
					{
						CarProgress.Add(new string[] { "КПП Виїзд", "<a href=\"./ChangeStateId?id=" + s[0] + "&StateId=" + s[1] + "&tc=" + CurTC + "&TableName=" + TableName + "&NodeName=" + GetNodeName(s[2]) + "\" class=\"btn btn-primary\">" + GetNodeStatus(s[1]) + "</a>", GetNodeName(s[2]), s[3], s[4] });
					}
				}
			}
			catch (Exception ex) { sReturn = "Etap: " + Etap + "\r\n\r\n" + ex.ToString(); }

			//string str = "<table Class=\"yozhstyle1\">\r\n";
			//string st = "";

			//foreach (string[] s in CarProgress)
			//{
			//	st = "<tr>";
			//	foreach (string ss in s)
			//	{
			//		st += "<td class=\"brdr1sb\">" + ss + "</td>";
			//	}
			//	st += "</tr>\r\n";
			//	str += st;
			//}
			//str += "</table>\r\n";
			//sReturn = "<div style=\"overflow: auto;\">" + str + "</div>";
			sReturn = "Done";
		}

		//###################################################################################################################

		public string GetTickets()
		{
			List<string[]> tmp = new List<string[]>();
			string sql = "";

			if (db.EnterpriseNum == 0) sql = "select StatusId, RoutetemplateId, RouteItemIndex, SecondaryRouteTemplateId, SecondaryRouteItemIndex, Id from dbo.Tickets where TicketContainerId = '" + CurTC + "'";
			if (db.EnterpriseNum == 1) sql = "select StatusId, RoutetemplateId, RouteItemIndex, SecondaryRouteTemplateId, SecondaryRouteItemIndex, Id from dbo.Ticket where ContainerId = '" + CurTC + "'";
			db.GetDataFromDBMSSQL(sql, ref tmp);

			TicketsList = tmp;



			//string r = "<br /><br /><b>Тікети(ТТН)(" + tmp.Count + ")</b><table class=\"yozhstyle1\">";
			//r += "<tr><td class=\"brdr1sb\">id</td><td class=\"brdr1sb\">Статус</td><td class=\"brdr1sb\">Маршрут</td><td class=\"brdr1sb\">Етап</td><td class=\"brdr1sb\">Доп. маршрут</td><td class=\"brdr1sb\">Доп. етап</td><td class=\"brdr1sb\">🖍️</td></tr>";

			//foreach (string[] s in tmp)
			//{
			//	r += "<tr><td class=\"brdr1sb\">" + s[5] + "</td><td class=\"brdr1sb\">" + s[0] + "</td><td class=\"brdr1sb\">" + s[1] + "</td><td class=\"brdr1sb\">" + s[2] + "</td><td class=\"brdr1sb\">" + s[3] + "</td><td class=\"brdr1sb\">" + s[4] + "</td><td class=\"brdr1sb\"><a href=\"./TicketEdit?t=" + s[5] + "\" class=\"btn btn-primary\">🖍️</a></td></tr>";
			//}
			//r += "</table>";

			//return r;
			return "Done";
		}


	}
}
