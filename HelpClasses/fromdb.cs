namespace Gravitas.Monitoring.HelpClasses
{
    public static class fromdb
    {
        public static string GetCarNumberByCarId(string CarId)
        {
            if (CarId == "") return "ND";
            string sql = "";
            List<string[]> tmp = new List<string[]>();
            if (db.EnterpriseNum == 0) sql = "select RegistrationNo from dbo.FixedAssets where Id = '" + CarId + "'";
            if (db.EnterpriseNum == 1) sql = "select RegistrationNo from ext.FixedAsset where Id = '" + CarId + "'";
            db.GetDataFromDBMSSQL(sql, ref tmp);
            return tmp.Count == 0 ? "ND" : tmp[0][0];
        }


        public static string GetTicketCountByTC(string tc)
        {
            List<string[]> tmp = new List<string[]>();
            if (db.EnterpriseNum == 0) db.GetDataFromDBMSSQL("select Id from dbo.Tickets where TicketContainerId = " + tc, ref tmp);
            if (db.EnterpriseNum == 1) db.GetDataFromDBMSSQL("select Id from dbo.Ticket where ContainerId = " + tc, ref tmp);
            return tmp.Count.ToString();
        }

        public static string GetWayPointByT(string t)
        {
            List<string[]> tmp = new List<string[]>();
            if (db.EnterpriseNum == 0) db.GetDataFromDBMSSQL("select RouteTemplateId,RouteItemIndex from dbo.Tickets where Id = " + t, ref tmp);
            if (db.EnterpriseNum == 1) db.GetDataFromDBMSSQL("select RouteTemplateId,RouteItemIndex from dbo.Ticket where Id = " + t, ref tmp);
            return tmp[0][1] + " / " + GetRouteLastBlockNumByRouteId(tmp[0][0]);
        }

        public static string GetRouteNameByT(string t)
        {
            List<string[]> tmp = new List<string[]>();
            if (db.EnterpriseNum == 0) db.GetDataFromDBMSSQL("select RouteTemplateId from dbo.Tickets where Id = " + t, ref tmp);
            if (db.EnterpriseNum == 1) db.GetDataFromDBMSSQL("select RouteTemplateId from dbo.Ticket where Id = " + t, ref tmp);

            return tmp.Count > 0 ? GetRouteNameByNumber(tmp[0][0]) : "Wrong Route Id";
        }

        public static string GetWndNameById(string Id)
        {
            if (db.EnterpriseNum == 1) return Id;

            string[] sId = { "10101", "10103", "10201", "10202" };
            string[] sName = { "Вікно 1.1", "Вікно 1.2", "Вікно 2.1", "Вікно 2.2" };
            string result = "Wrong Id";
            for (int i = 0; i < 4; i++)
            {
                if (sId[i] == Id)
                {
                    result = sName[i];
                    break;
                }
            }
            return result;
        }

        public static string GetTicketStatus(string TicketId)
        {
            List<string[]> tmp = new List<string[]>();
            if (db.EnterpriseNum == 0) db.GetDataFromDBMSSQL("select StatusId from dbo.Tickets where Id = '" + TicketId + "'", ref tmp);
            if (db.EnterpriseNum == 1) db.GetDataFromDBMSSQL("select StatusId from dbo.Ticket where Id = '" + TicketId + "'", ref tmp);
            if (tmp.Count == 0) return ""; else return tmp[0][0];
        }


        public static string GetRouteLastBlockNumByRouteId(string sRouteTemplateId)
        {
            try
            {
                List<string[]> tmp = new List<string[]>();
                if (db.EnterpriseNum == 0) db.GetDataFromDBMSSQL("select * from dbo.RouteTemplates where Id = " + sRouteTemplateId, ref tmp);
                if (db.EnterpriseNum == 1) db.GetDataFromDBMSSQL("select * from dbo.RouteTemplate where Id = " + sRouteTemplateId, ref tmp);
                int iGroup = 0;
                string i1 = "{\"groupId\"";
                for (int i = 0; i < tmp[0][2].Length - 10; i++)
                    if (tmp[0][2].Substring(i, i1.Length) == i1)
                        iGroup++;
                return iGroup.ToString();
            }
            catch (Exception ex)
            {
                //log.Add(ex.ToString());
                return "#";
            }
        }

        public static string GetCardNumberByTC(string tc)
        {
            List<string[]> tmp = new List<string[]>();
            if (db.EnterpriseNum == 0) db.GetDataFromDBMSSQL("select No from dbo.Cards where TicketContainerId = '" + tc + "' and TypeId = '2'", ref tmp);
            if (db.EnterpriseNum == 1) db.GetDataFromDBMSSQL("select No from dbo.Card where TicketContainerId = '" + tc + "' and TypeId = '2'", ref tmp);
            return tmp.Count > 0 ? tmp[0][0] : "ND";
        }

        public static string GetFeedNameByTC(string tc)
        {
            List<string[]> tmp = new List<string[]>();
            db.GetDataFromDBMSSQL("select ProductTitle from dbo.SingleWindowOpDatas where TicketContainerId = '" + tc + "'", ref tmp);
            return tmp.Count > 0 ? tmp[0][0] : "ND";
        }

        public static string GetRouteNameByNumber(string num)
        {
            List<string[]> tmp = new List<string[]>();
            if (db.EnterpriseNum == 0) db.GetDataFromDBMSSQL("select Name from dbo.RouteTemplates where Id = '" + num + "'", ref tmp);
            if (db.EnterpriseNum == 1) db.GetDataFromDBMSSQL("select Name from dbo.RouteTemplate where Id = '" + num + "'", ref tmp);
            return tmp.Count > 0 ? tmp[0][0] : "ND";
        }
    }
}
