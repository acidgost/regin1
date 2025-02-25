using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace regin.Classes
{
    internal class WorkingDB
    {
        readonly static string connection = "Server=localhost;database=regin;uid=root;port=3306";
        public static MySqlConnection OpenConnection()
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(connection);
                conn.Open();
                return conn;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }
        public static MySqlDataReader Query(string SQL, MySqlConnection connection)
        {
            MySqlCommand command = new MySqlCommand(SQL, connection);
            return command.ExecuteReader();
        }
        public static void CloseConnection(MySqlConnection connection)
        {
            connection.Close();
            MySqlConnection.ClearPool(connection);
        }
        public static bool OpenConnection(MySqlConnection connection) => connection != null && connection.State == System.Data.ConnectionState.Open;
    }
}
