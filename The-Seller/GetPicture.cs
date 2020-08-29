using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Data;

namespace The_Seller
{
    public class GetPicture
    {
        public String GetToolPicture(string ToolID)
        {
            String PicPath = "0";
            try
            {
                SqlDataReader reader;
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand("SELECT PicPath FROM Pictures WHERE ToolID=@ToolID");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@ToolID", ToolID);
                    connection.Open();

                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        PicPath = reader.GetString(0);
                    }
                    reader.Close();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                PicPath = ex.Message;
            }
            return PicPath;
        }

    }
}