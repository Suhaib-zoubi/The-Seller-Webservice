using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Data;
namespace The_Seller
{
    public class Tools
    {
        public String GetLastToolAdded(string UserID, DateTime DateAdded)
        {
            String ToolID = "0";
            try
            {
                SqlDataReader reader;
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand("SELECT ToolId FROM Tools WHERE UserID=@UserID and DateAdd=@DateAdded");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    cmd.Parameters.AddWithValue("@DateAdded", DateAdded);
                    connection.Open();

                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ToolID += " nice ";
                        ToolID = Convert.ToString(reader.GetInt64(0));
                    }
                    reader.Close();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
            }
            return ToolID;
        }
    }
}