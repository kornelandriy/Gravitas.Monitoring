using Microsoft.Data.SqlClient;

namespace Gravitas.Monitoring.HelpClasses
{
	public static class db
	{

		/// <summary>
		///  [ 0 - MZVKK ] [ 1 - KE ]
		/// </summary>
		public static int EnterpriseNum = 0; // 0 - MZVKK     1 - KE

		public static string[] DBConnStr = new string[] { "server=10.9.176.98;user=sa;database=mhp;password=Qq123123123;TrustServerCertificate=True;", "server=10.9.176.85;user=sa;database=mhp;password=wu*o8Ewediel;TrustServerCertificate=True;" };

		public static void GetTableHeaders2(string TableName, ref List<string> lst)
		{
			string sql = "select COLUMN_NAME from information_schema.columns where table_catalog='mhp' and table_schema='dbo' and table_name = '" + TableName + "'";
			SqlConnection conn = new SqlConnection(DBConnStr[EnterpriseNum]);
			conn.Open();
			SqlDataReader reader = new SqlCommand(sql, conn).ExecuteReader();
			lst.Clear();
			while (reader.Read())
				lst.Add(reader[0].ToString());
			reader.Close();
			conn.Close();
		}


		public static void GetTableHeaders3(string TableShema, string TableName, ref List<string> lst)
		{
			string sql = "select COLUMN_NAME from information_schema.columns where table_catalog='mhp' and table_schema='" + TableShema + "' and table_name = '" + TableName + "'";
			SqlConnection conn = new SqlConnection(DBConnStr[EnterpriseNum]);
			conn.Open();
			SqlDataReader reader = new SqlCommand(sql, conn).ExecuteReader();
			lst.Clear();
			while (reader.Read())
				lst.Add(reader[0].ToString());
			reader.Close();
			conn.Close();
		}

		/// <summary>
		/// Універсальний метод для отримання даних з БД MSSQL
		/// </summary>
		/// <param name="ConnString">Рядок підключення до БД "server=srvIP;user=usrName;database=dbName;password=pass;"</param>
		/// <param name="SelectString">Запит до БД "select * from table_name"</param>
		/// <param name="DataList">Посилання на список для результатів запиту</param>
		public static void GetDataFromDBMSSQL(string SelectString, ref List<string[]> DataList)
		{
			log.Add("GetDataFromDBMSSQL: SQL: " + SelectString);
			SqlConnection conn = new SqlConnection(DBConnStr[EnterpriseNum]);
			conn.Open();
			SqlDataReader reader = new SqlCommand(SelectString, conn).ExecuteReader();
			try
			{
				DataList.Clear();
				while (reader.Read())
				{
					DataList.Add(new string[reader.FieldCount]);
					for (int i = 0; i < reader.FieldCount; i++)
						DataList[DataList.Count - 1][i] = reader[i].ToString();
				}
			}
			catch (Exception ex) { }
			reader.Close();
			reader.Dispose();
			conn.Close();
			conn.Dispose();

			log.Add("GetDataFromDBMSSQL: Returned rows count: " + DataList.Count);

			GC.Collect();
		}

		/// <summary>
		/// Універсальний метод для отримання даних з БД MSSQL
		/// </summary>
		/// <param name="ConnString">Рядок підключення до БД "server=srvIP;user=usrName;database=dbName;password=pass;"</param>
		/// <param name="SelectString">Запит до БД "select * from table_name"</param>
		/// <param name="DataList">Посилання на список для результатів запиту</param>
		public static void GetDataFromDBMSSQL(string ConnString, string SelectString, ref List<string[]> DataList)
		{
			SqlConnection conn = new SqlConnection(ConnString);
			conn.Open();
			SqlDataReader reader = new SqlCommand(SelectString, conn).ExecuteReader();
			try
			{
				DataList.Clear();
				while (reader.Read())
				{
					DataList.Add(new string[reader.FieldCount]);
					for (int i = 0; i < reader.FieldCount; i++)
						DataList[DataList.Count - 1][i] = reader[i].ToString();
				}
			}
			catch (Exception ex) { }
			reader.Close();
			reader.Dispose();
			conn.Close();
			conn.Dispose();
			GC.Collect();
		}



		/// <summary>
		/// Заміна спецсимволів SQL
		/// </summary>
		/// <param name="text">SQL рядок</param>
		/// <returns></returns>
		public static string ParseSQLSymbol(string text)
		{
			string result = "";
			string tmp = "";
			foreach (char c in text.ToArray())
			{
				switch (c)
				{
					case '\'':
						tmp = "\\\'";
						break;
					case '\\':
						tmp = "\\\\";
						break;
					case '"':
						tmp = "\\\"";
						break;
					default:
						tmp = c.ToString();
						break;
				}
				result += tmp;
			}
			return result;
		}

		/// <summary>
		/// відправляє запити MySQL серверу
		/// Приклад: long n = SendRequestToDB("insert into table_name (column_name) values ('value')");
		/// </summary>
		/// <param name="sql">SQL запит</param>
		/// <returns>Повертає id доданого запису або 0 у випадку помилки</returns>
		public static void SendRequestToDB(string sql)
		{
			//MessageBox.Show(sql);
			//Console.WriteLine("SendRequestToDB: " + sql);
			log.Add("SendRequestToDB: SQL: "+sql);
			if (sql.Length == 0) return;
			try
			{
				SqlConnection _conn = new SqlConnection(DBConnStr[EnterpriseNum]);
				_conn.Open();

				using (SqlCommand mysqlcmd = new SqlCommand(sql, _conn))
				{
					mysqlcmd.ExecuteNonQuery();
				}
				_conn.Close();
			}
			catch (Exception ex) { Console.WriteLine("SendRequestToDB Error: " + ex.ToString()); }
		}

		/// <summary>
		/// Пошук значення у списку по іншому значенню
		/// </summary>
		/// <param name="FromList">Ссилка на список в якому шукаємо</param>
		/// <param name="FindText">Значення яке шукаємо</param>
		/// <param name="FindColNum">Номер стовпчика в якому шукаємо</param>
		/// <param name="ReturnColNum">Номер стовпчика з якого отримуємо значення</param>
		/// <returns></returns>
		public static string GetItemData(ref List<string[]> FromList, string FindText, int FindColNum, int ReturnColNum)
		{
			return GetItemData(ref FromList, FindText, FindColNum, ReturnColNum, "");
		}
		/// <summary>
		/// Пошук значення у списку по іншому значенню
		/// </summary>
		/// <param name="FromList">Ссилка на список в якому шукаємо</param>
		/// <param name="FindText">Значення яке шукаємо</param>
		/// <param name="FindColNum">Номер стовпчика в якому шукаємо</param>
		/// <param name="ReturnColNum">Номер стовпчика з якого отримуємо значення</param>
		/// <param name="defaultText">Значення яке отримуємо якщо нічого не знайдено</param>
		/// <returns></returns>
		public static string GetItemData(ref List<string[]> FromList, string FindText, int FindColNum, int ReturnColNum, string defaultText)
		{
			foreach (string[] s in FromList)
				if (FindText == s[FindColNum]) return s[ReturnColNum];
			return defaultText;
		}

		public static List<string> GetIdListByTableNameAndT(string TableName, string t)
		{
			List<string> result = new List<string>();
			List<string[]> tmp = new List<string[]>();
			db.GetDataFromDBMSSQL("select Id from dbo." + TableName + " where TicketId = '" + t + "'", ref tmp);
			foreach (string[] s in tmp)
				result.Add(s[0]);
			return result;
		}

	}
}
