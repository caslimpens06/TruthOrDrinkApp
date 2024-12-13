using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using SQLite;
using Microsoft.Maui.Storage;

namespace TruthOrDrink.Model
{
	public static class Constants
	{
		public const string DB_NAME = "TruthOrDrinkLocalSQLite.db3";

		public const SQLite.SQLiteOpenFlags Flags =

			SQLite.SQLiteOpenFlags.ReadWrite |

			SQLite.SQLiteOpenFlags.Create |

			SQLite.SQLiteOpenFlags.SharedCache;

		public static string DatabasePath
		{
			get
			{
				return Path.Combine(Microsoft.Maui.Storage.FileSystem.AppDataDirectory, DB_NAME);
			}
		}

	}
}
