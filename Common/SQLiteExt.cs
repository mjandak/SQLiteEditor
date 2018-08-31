using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data;
using System.Configuration;
using System.Security;
using System.Runtime.InteropServices;

namespace Common
{
    public static class SQLiteExt
    {
		/// <summary>
		/// If db is encrypted sets password on db connection before calling regular fill. See P2.
		/// </summary>
		/// <param name="adapter"></param>
		/// <param name="dataTable"></param>
		/// <returns></returns>
        public static int FillEx(this SQLiteDataAdapter adapter, DataTable dataTable)
        {
            if (Globals.Pswd != null)
            {
                Globals.DbConnection.SetPassword(HandleSecureString(Globals.Pswd));
            }
            return adapter.Fill(dataTable);
        }

		/// <summary>
		/// If db is encrypted sets password on db connection before calling regular fill. See P2.
		/// </summary>
		/// <param name="adapter"></param>
		/// <param name="dataTable"></param>
		/// <returns></returns>
		public static int FillEx(this SQLiteDataAdapter adapter, DataSet dataSet)
		{
			if (Globals.Pswd != null)
			{
				Globals.DbConnection.SetPassword(HandleSecureString(Globals.Pswd));
			}
			return adapter.Fill(dataSet);
		}

		/// <summary>
		/// If db is enrypted sets the password on db connection before calling regular update. See P2.
		/// </summary>
		/// <param name="adapter"></param>
		/// <param name="dataTable"></param>
		/// <returns></returns>
		public static int UpdateEx(this SQLiteDataAdapter adapter, DataTable dataTable)
		{
			if (Globals.Pswd != null)
			{
                Globals.DbConnection.SetPassword(HandleSecureString(Globals.Pswd));
			}
			return adapter.Update(dataTable);
		}

        public static void OpenEx(this SQLiteConnection connection)
        {
            if (connection.State != ConnectionState.Closed)
            {
                return;
            }
            if (Globals.Pswd != null)
            {
                connection.SetPassword(HandleSecureString(Globals.Pswd));
            }
            connection.Open();
        }

        public static byte[] HandleSecureString(SecureString value)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                byte[] bytes = new byte[value.Length * 2];
                for (int i = 0; i < value.Length; i++)
                {
                    short unicodeChar = Marshal.ReadInt16(valuePtr, i * 2);
                    byte b1;
                    byte b2;
                    FromShort(unicodeChar, out b1, out b2);
                    bytes[i * 2] = b1;
                    bytes[i * 2 + 1] = b2;
                }
                return bytes;
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }

        public static void FromShort(short number, out byte byte1, out byte byte2)
        {
            byte2 = (byte)(number >> 8);
            byte1 = (byte)(number & 255);
        }
    }
}
