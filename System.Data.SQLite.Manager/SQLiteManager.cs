using System.Data.SQLite;
using System;
using System.Data;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace System.Data.SQLite.Manager
{
	public class SQLiteManager
	{
		private string _connectionString;
		private string _dbFilePath;
		private bool _dbFileCreated;

		public SQLiteManager(string dbFilePath, string version = "3")
		{
			_connectionString = $"Data Source={dbFilePath}Version={version};";
			_dbFilePath = dbFilePath ;
			SQLiteFunction.RegisterFunction(typeof(ConcatenateFunction)); // Register your custom SQLite function
			SQLiteFunction.RegisterFunction(typeof(GetDateFunction));
			SQLiteFunction.RegisterFunction(typeof(GetUTCFunction));
			SQLiteFunction.RegisterFunction(typeof(NewIDFunction));
		}

		public bool CreateDatabaseFile()
		{
			if (!File.Exists(_dbFilePath))
			{
				try
				{
					SQLiteConnection.CreateFile(_dbFilePath);
					using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
					{
						connection.Open();
						_dbFileCreated = true;
						return true;
					}
				}
				catch (Exception ex)
				{
					_dbFileCreated = false;
					Console.WriteLine($"SQLite Error: {ex.Message}");
					return false;
				}
			}
			else
			{
				Console.WriteLine("Database file already exists.");
				return _dbFileCreated = true;
			}
		}


		public bool ExecuteNonQuery(string query)
		{
			try
			{
				using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
				{
					connection.Open();
					using (SQLiteCommand cmd = new SQLiteCommand(query, connection))
					{
						cmd.ExecuteNonQuery();
					}
					return true;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"SQLite Error: {ex.Message}");
				return false;
			}
		}

		public DataTable ExecuteQuery(string query)
		{
			try
			{
				using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
				{
					connection.Open();
					using (SQLiteCommand cmd = new SQLiteCommand(query, connection))
					{
						using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
						{
							DataTable dt = new DataTable();
							adapter.Fill(dt);
							return dt;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"SQLite Error: {ex.Message}");
				return null;
			}
		}

		public DataTable Select(string tableName, string columns, string condition = null, string order = null)
		{
			string selectQuery = $"SELECT {columns} FROM {tableName}";

			if (!string.IsNullOrEmpty(condition))
			{
				selectQuery += $" WHERE {condition}";
			}

			if (!string.IsNullOrEmpty(order))
			{
				selectQuery += $" ORDER BY {order}";
			}

			return ExecuteQuery(selectQuery);
		}

		public bool CreateTable(string tableName, string columns)
		{
			string createTableQuery = $"CREATE TABLE IF NOT EXISTS {tableName} ({columns});";
			return ExecuteNonQuery(createTableQuery);
		}

		public bool DropTable(string tableName)
		{
			string dropTableQuery = $"DROP TABLE IF EXISTS {tableName}";
			return ExecuteNonQuery(dropTableQuery);
		}

		public bool Insert(string tableName, string columns, string values)
		{
			string insertQuery = $"INSERT INTO {tableName} ({columns}) VALUES ({values});";
			return ExecuteNonQuery(insertQuery);
		}

		public bool Update(string tableName, string set, string condition)
		{
			string updateQuery = $"UPDATE {tableName} SET {set} WHERE {condition};";
			return ExecuteNonQuery(updateQuery);
		}

		public bool Delete(string tableName, string condition)
		{
			string deleteQuery = $"DELETE FROM {tableName} WHERE {condition};";
			return ExecuteNonQuery(deleteQuery);
		}
	}

	// Custom Concatenate Function
	[SQLiteFunction(Name = "concat", FuncType = FunctionType.Scalar)]
	internal class ConcatenateFunction : SQLiteFunction
	{
		public override object Invoke(object[] args)
		{
			if (args == null || args.Length == 0)
				return null;

			StringBuilder result = new StringBuilder();
			foreach (object arg in args)
			{
				if (arg != null)
					result.Append(arg == null ? string.Empty : arg);
			}

			return result.ToString();
		}
	}

	[SQLiteFunction(Name = "getdate", FuncType = FunctionType.Scalar)]
	internal class GetDateFunction : SQLiteFunction
	{
		public override object Invoke(object[] args)
		{
			return DateTime.Now;
		}
	}

	[SQLiteFunction(Name = "getutc", FuncType = FunctionType.Scalar)]
	internal class GetUTCFunction : SQLiteFunction
	{
		public override object Invoke(object[] args)
		{
			return DateTime.UtcNow;
		}
	}

	[SQLiteFunction(Name = "newid", FuncType = FunctionType.Scalar)]
	internal class NewIDFunction : SQLiteFunction
	{
		public override object Invoke(object[] args)
		{
			return Guid.NewGuid().ToString();
		}
	}
}
