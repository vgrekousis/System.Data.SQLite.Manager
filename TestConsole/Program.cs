using System;
using System.Collections.Generic;
using System.Data.SQLite.Manager;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
	internal class Program
	{
		static void Main(string[] args)
		{
			SQLiteManager manager = new SQLiteManager("mytestdb");
			Console.WriteLine("Database Created/Exists: {0}", manager.CreateDatabaseFile());
			Console.WriteLine("Table Created/Exists: {0}", manager.CreateTable("MyTable", "ID INT PrimaryKey"));
			Console.WriteLine("Table Created/Exists: {0}", manager.CreateTable("MyTable2", "ID INT PrimaryKey, ForeignID INT"));
			Console.WriteLine("Relationship created: {0}", manager.AddForeignKeyToTable("MyTable2", "ForeignID", "MyTable", "ID"));
			Console.WriteLine("Drop table query succesfull: {0}", manager.DropTable("MyTable"));
		}
	}
}
