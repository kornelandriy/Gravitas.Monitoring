using Gravitas.Monitoring.HelpClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;




namespace Gravitas.Monitoring.Pages
{
	public class CarListModel : PageModel
	{
		[BindProperty]
		public List<string[]> lstData { get; set; } = new List<string[]>();
		//################################################################################
		[BindProperty]
		public string CarNum { get; set; } = "";
		[BindProperty]
		public bool f1 { get; set; } = true;
		[BindProperty]
		public bool f2 { get; set; } = true;
		[BindProperty]
		public bool f3 { get; set; } = true;
		[BindProperty]
		public bool f4 { get; set; } = true;
		[BindProperty]
		public bool f5 { get; set; } = true;
		[BindProperty]
		public bool f6 { get; set; } = false;
		[BindProperty]
		public bool f10 { get; set; } = false;
		[BindProperty]
		public DateTime fDate { get; set; } = DateTime.Now.Date;
		[BindProperty]
		public bool fOneDay { get; set; } = false;
		//################################################################################
		[BindProperty]
		public string SQLString { get; set; } = "";
		[BindProperty]
		public string cnt { get; set; } = "";
		[BindProperty]
		public string FindTC { get; set; } = "";

		//======================================================================

		public void OnGet() { }

		public void OnPost()
		{
			if (CarNum.IsNullOrEmpty()) CarNum = "";
			List<string[]> lst = new List<string[]>();
			//
			if (!f1 && !f2 && !f3 && !f4 && !f5 && !f6 && !f10)
			{
				f1 = f2 = f3 = f4 = f5 = true;
				f6 = f10 = false;
			}
			//
			string StateFilter = "";
			string ssFilter = "";
			if (f1) { ssFilter += "Tickets.StatusId=1 or "; }
			if (f2) { ssFilter += "Tickets.StatusId=2 or "; }
			if (f3) { ssFilter += "Tickets.StatusId=3 or "; }
			if (f4) { ssFilter += "Tickets.StatusId=4 or "; }
			if (f5) { ssFilter += "Tickets.StatusId=5 or "; }
			if (f6) { ssFilter += "Tickets.StatusId=6 or "; }
			if (f10) { ssFilter += "Tickets.StatusId=10 or "; }

			if (StateFilter == "") StateFilter = "1,2,3,4,5,";
			if (StateFilter == "") ssFilter = "Tickets.StatusId=1 or Tickets.StatusId=2 or Tickets.StatusId=3 or Tickets.StatusId=4 or Tickets.StatusId=5 or ";
			if (db.EnterpriseNum == 1) ssFilter = ssFilter.Replace("ets", "et");


			StateFilter = StateFilter.Substring(0, StateFilter.Length - 1);
			ssFilter = ssFilter.Substring(0, ssFilter.Length - 4);

			DateTime dt = fDate;
			DateTime dt2 = dt;
			DateTime dt1 = dt.AddDays(-1); // Доробить 1 або 2 доби...

			int d = dt1.Day;
			int m = dt1.Month;
			int y = dt1.Year;
			//
			int d2 = dt2.Day;
			int m2 = dt2.Month;
			int y2 = dt2.Year;
			//
			// Date filter in MS SQL
			string dateFilter = "( " + (db.EnterpriseNum == 1 ? "opd." : "") + "singleWindowOpDatas.RegistrationDateTime between '" + y.ToString("0000") + "-" + m.ToString("00") + "-" + d.ToString("00") + " 00:00:00.000'" +
													" and '" + y2.ToString("0000") + "-" + m2.ToString("00") + "-" + d2.ToString("00") + " 23:59:59.999')";
			//
			string sql = "";
			if (string.IsNullOrEmpty(FindTC))
			{
				if (db.EnterpriseNum == 0)
					sql = "SELECT TOP (2000) (select No from [mhp].[dbo].[Cards] where TypeId=2 and TicketContainerId=Tickets.TicketContainerId) as 'Card No', " +
					"Nodes.Name, FixedAssets.RegistrationNo, SingleWindowOpDatas.HiredTransportNumber, SingleWindowOpDatas.ProductTitle, RouteTemplates.Name, " +
					"SingleWindowOpDatas.TicketId, SingleWindowOpDatas.TicketContainerId, Tickets.StatusId, (select COUNT(id) from " +
					"mhp.dbo.Tickets where Tickets.TicketContainerId = SingleWindowOpDatas.TicketContainerId) as 'tc', SingleWindowOpDatas.RegistrationDateTime, " +
					"SingleWindowOpDatas.CheckOutDateTime, dbo.Tickets.RouteItemIndex FROM [mhp].[dbo].[Tickets] join [mhp].[dbo].[SingleWindowOpDatas] on SingleWindowOpDatas.TicketContainerId = Tickets.TicketContainerId " +
					"left join [mhp].[dbo].[FixedAssets] on FixedAssets.Id = SingleWindowOpDatas.TransportId join [mhp].[dbo].[Nodes] on Nodes.Id = SingleWindowOpDatas.NodeId join [mhp].[dbo].[RouteTemplates] on " +
					"RouteTemplates.Id = Tickets.RouteTemplateId where (" + ssFilter + ") and " + dateFilter + " and SingleWindowOpDatas.StateId = 10 " +
					"order by SingleWindowOpDatas.RegistrationDateTime";

				if (db.EnterpriseNum == 1)
					sql = "SELECT TOP (2000) (select No from Card where TypeId=2 and TicketContainerId=Ticket.ContainerId) as 'Card No', " +
					"Node.Name, FixedAsset.RegistrationNo, opd.SingleWindowOpData.HiredTransportNumber, opd.SingleWindowOpData.ProductTitle, RouteTemplate.Name, " +
					"opd.SingleWindowOpData.TicketId, opd.SingleWindowOpData.TicketContainerId, Ticket.StatusId, (select COUNT(id) from " +
					"Ticket where Ticket.ContainerId = opd.SingleWindowOpData.TicketContainerId) as 'tc', opd.SingleWindowOpData.RegistrationDateTime, " +
					"opd.SingleWindowOpData.CheckOutDateTime, dbo.Tickets.RouteItemIndex FROM [mhp].[dbo].[Ticket] join opd.SingleWindowOpData on opd.SingleWindowOpData.TicketContainerId = Ticket.TicketContainerId " +
					"left join FixedAsset on FixedAsset.Id = opd.SingleWindowOpData.TransportId join Node on Node.Id = opd.SingleWindowOpData.NodeId join RouteTemplate on " +
					"RouteTemplate.Id = Ticket.RouteTemplateId where (" + ssFilter + ") and " + dateFilter + " and opd.SingleWindowOpData.StateId = 10 " +
					"order by opd.SingleWindowOpData.RegistrationDateTime";
			}
			else
			{
				if (db.EnterpriseNum == 0)
					sql = "SELECT TOP (2000) (select No from [mhp].[dbo].[Cards] where TypeId=2 and TicketContainerId=Tickets.TicketContainerId) as 'Card No', " +
					"Nodes.Name, FixedAssets.RegistrationNo, SingleWindowOpDatas.HiredTransportNumber, SingleWindowOpDatas.ProductTitle, RouteTemplates.Name, " +
					"SingleWindowOpDatas.TicketId, SingleWindowOpDatas.TicketContainerId, Tickets.StatusId, (select COUNT(id) from " +
					"mhp.dbo.Tickets where Tickets.TicketContainerId = SingleWindowOpDatas.TicketContainerId) as 'tc', SingleWindowOpDatas.RegistrationDateTime, " +
					"SingleWindowOpDatas.CheckOutDateTime, dbo.Tickets.RouteItemIndex FROM [mhp].[dbo].[Tickets] join [mhp].[dbo].[SingleWindowOpDatas] on SingleWindowOpDatas.TicketContainerId = Tickets.TicketContainerId " +
					"left join [mhp].[dbo].[FixedAssets] on FixedAssets.Id = SingleWindowOpDatas.TransportId join [mhp].[dbo].[Nodes] on Nodes.Id = SingleWindowOpDatas.NodeId join [mhp].[dbo].[RouteTemplates] on " +
					"RouteTemplates.Id = Tickets.RouteTemplateId where SingleWindowOpDatas.TicketContainerId = '" + FindTC + "' " +
					"order by SingleWindowOpDatas.RegistrationDateTime";

				if (db.EnterpriseNum == 1)
					sql = "SELECT TOP (2000) (select No from Card where TypeId=2 and TicketContainerId=Ticket.ContainerId) as 'Card No', " +
					"Node.Name, FixedAsset.RegistrationNo, opd.SingleWindowOpData.HiredTransportNumber, opd.SingleWindowOpData.ProductTitle, RouteTemplate.Name, " +
					"opd.SingleWindowOpData.TicketId, opd.SingleWindowOpData.TicketContainerId, Ticket.StatusId, (select COUNT(id) from " +
					"Ticket where Ticket.ContainerId = opd.SingleWindowOpData.TicketContainerId) as 'tc', opd.SingleWindowOpData.RegistrationDateTime, " +
					"opd.SingleWindowOpData.CheckOutDateTime, dbo.Tickets.RouteItemIndex FROM [mhp].[dbo].[Ticket] join opd.SingleWindowOpData on opd.SingleWindowOpData.TicketContainerId = Ticket.TicketContainerId " +
					"left join FixedAsset on FixedAsset.Id = opd.SingleWindowOpData.TransportId join Node on Node.Id = opd.SingleWindowOpData.NodeId join RouteTemplate on " +
					"RouteTemplate.Id = Ticket.RouteTemplateId where opd.SingleWindowOpData.TicketContainerId = '" + FindTC + "' " +
					"order by opd.SingleWindowOpData.RegistrationDateTime";
			}
			SQLString = sql;
			db.GetDataFromDBMSSQL(sql, ref lst);
			List<string[]> NewData = new List<string[]>();
			cnt = "";
			string tmpCarNum = "";
			if (db.EnterpriseNum == 0) // MZVKK
			{
				foreach (string[] s in lst)
				{
					cnt += "mzvkk<br>";
					if (s[6] == "") break;

					tmpCarNum = s[2] == "" ? s[3] : s[2];

					if (tmpCarNum.ToLower().Contains(CarNum))
					{
						lstData.Add(new string[] { /*0*/ s[0],
                                                   /*1*/ s[1],
                                                   /*2*/ tmpCarNum,
                                                   /*3*/ s[4],
                                                   /*4*/ s[5],
                                                   /*5*/ GetStatusNamebyId(s[8]),
                                                   /*6*/ s[9],
                                                   /*7*/ s[7],
                                                   /*8*/ DateTime.Parse(s[10]).ToString("dd.MM.yyyy HH:mm:ss"),
                                                   /*9*/ DateTime.Parse(s[11]).ToString("dd.MM.yyyy HH:mm:ss"),
								      	  	      /*10*/ s[12] });
						//

					}

				}
			}
			if (db.EnterpriseNum == 1) // KE
			{
				foreach (string[] s in lst)
				{
					cnt += "mzvkk<br>";
					if (s[87] == "") break;
					//if (sMas.Contains(fromdb.GetTicketStatus(s[87])))
					{
						tmpCarNum = s[20] != "" ? fromdb.GetCarNumberByCarId(s[20]) : "ND";
						if (tmpCarNum == "ND")
						{
							tmpCarNum = s[97] == "" ? "нема номера" : s[97];
						}
						//
						if (tmpCarNum.ToLower().Contains(CarNum))
						{
							lstData.Add(new string[] {  /*0*/ fromdb.GetCardNumberByTC(s[88]),
                                                        /*1*/ fromdb.GetWndNameById(s[86]),
                                                        /*2*/ tmpCarNum,
                                                        /*3*/ s[106],
                                                        /*4*/ fromdb.GetRouteNameByT(s[87]),
                                                        /*5*/ fromdb.GetWayPointByT(s[87]),
                                                        /*6*/ fromdb.GetTicketCountByTC(s[88]),
                                                        /*7*/ s[88],
                                                        /*8*/ DateTime.Parse(s[34]).ToString("dd.MM.yyyy HH:mm:ss"),
                                                        /*9*/ DateTime.Parse(s[90]).ToString("dd.MM.yyyy HH:mm:ss") });
							//

						}
					}
				}
			}
		}

		private string GetStatusNamebyId(string id)
		{
			string r = "Wrong StateId";
			switch (id.Trim())
			{
				case "1":
					r = "Новий";
					break;
				case "2":
					r = "В обробці";
					break;
				case "3":
					r = "Доопрацювання";
					break;
				case "4":
					r = "В роботі";
					break;
				case "5":
					r = "Завершено";
					break;
				case "6":
					r = "Проведено";
					break;
				case "10":
					r = "Відхилено";
					break;
			}
			return r;
		}
	}
}
