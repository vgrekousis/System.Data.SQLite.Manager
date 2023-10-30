using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
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
			//Console.WriteLine("Drop table query succesfull: {0}", manager.DropTable("Lists"));
			Console.WriteLine("Drop table query succesfull: {0}", manager.DropTable("ListValues"));

			Console.WriteLine("Database Created/Exists: {0}", manager.CreateDatabaseFile());

			Console.WriteLine("Table Created/Exists: {0}", manager.CreateTable(
				"Lists", "ListID INTEGER Primary Key AUTOINCREMENT, ListCd varchar(50) NOT NULL, ListDescr_PRM varchar(80), ListDescr_ALT varchar(80)"));

			Console.WriteLine("Table Created/Exists: {0}", manager.CreateTable(
				"ListValues", "ListValueID INTEGER PRIMARY KEY AUTOINCREMENT, ListID_FK INTEGER NOT NULL, Value_PRM varchar(80), Value_ALT varchar(80), Value_EXT varchar(80)"));

			Console.WriteLine("Relationship created: {0}", manager.AddForeignKeyToTable("ListValues", "ListID_FK", "Lists", "ListID"));

			Console.WriteLine(manager.ColumnAcceptsNull("ListValues", "ListID_FK"));


			Console.WriteLine("List inserted: {0}", manager.Insert("Lists",
				"ListCd, ListDescr_PRM, ListDescr_ALT",
				"'MAILLIST', 'MAILLIST', 'MAILLIST'"));

			foreach (DataRow row in manager.Select("Lists", "*").Rows)
				Console.WriteLine($"{row["ListCd"]}, {row["ListID"]}");


			foreach (var col in manager.GetForeignKeyColumnNames("ListValues"))
				Console.WriteLine(col.ToString());


			Console.WriteLine("\nAES Encryption");
			AesEncryption aes = new AesEncryption();
			Console.WriteLine($"KEY: {aes.FromBytes(aes.GenerateKey())}");
			Console.WriteLine($"IV: {aes.FromBytes(aes.GenerateIV())}");
			string encrypted = aes.Encrypt("Hello World");
			Console.WriteLine(encrypted);
			Console.WriteLine(aes.Decrypt(encrypted));


		}
	}
}
