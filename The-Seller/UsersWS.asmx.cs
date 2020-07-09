using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;

namespace The_Seller
{
    /// <summary>
    /// Summary description for UsersWS1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
     [System.Web.Script.Services.ScriptService]
    public class UsersWS1 : System.Web.Services.WebService
    {

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]

        public void Register(string UserName, string Password, string Email,
               string PhoneNumber, string Logtit, string Latitle, System.Int32 Gender) //get list of note
        {
            // intilize the data user
            JavaScriptSerializer ser = new JavaScriptSerializer();
            int IsAdded = 1;
            string Message = "";

            //check if we have this account
            Users myUseres = new Users();
            if (myUseres.IsAvailable(UserName, Email) == 0) // check if username and email are unused
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString()))
                    {
                        SqlCommand cmd = new SqlCommand("INSERT INTO Users (UserName, Password, Email, PhoneNumber, Logtit, Latitle, Gender)" +
                            " VALUES (@UserName, @Password, @Email, @PhoneNumber, @Logtit, @Latitle, @Gender)");
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = connection;
                        cmd.Parameters.AddWithValue("@UserName", UserName);
                        cmd.Parameters.AddWithValue("@Password", Password);
                        cmd.Parameters.AddWithValue("@Email", Email);
                        cmd.Parameters.AddWithValue("@PhoneNumber", PhoneNumber);
                        cmd.Parameters.AddWithValue("@Logtit", Logtit);
                        cmd.Parameters.AddWithValue("@Latitle", Latitle);
                        cmd.Parameters.AddWithValue("@Gender", Gender);
                        connection.Open();
                        cmd.ExecuteNonQuery();
                        connection.Close();
                    }
                    Message = "your account is created succefully";
                }
                catch (Exception ex)
                {
                    IsAdded = 0;
                    Message =ex.Message; // "Cannot add yor information"
                }
            }
            else
            {
                IsAdded = 0;

                Message = "User name or email is reserved";
            }
            var jsonData = new
            {
                IsAdded = IsAdded,
                Message = Message
            };

            HttpContext.Current.Response.Write(ser.Serialize(jsonData));
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        [System.ComponentModel.ToolboxItem(false)]
        public void Login(string UserName, string Password) //get list of note
        {
            // intilize the data user
            JavaScriptSerializer ser = new JavaScriptSerializer();
            int UserID = 0;
            string Message = "";

            try
            {
                SqlDataReader reader;
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand("SELECT UserID FROM Users WHERE UserName=@UserName and Password=@Password");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@UserName", UserName);
                    cmd.Parameters.AddWithValue("@Password", Password);
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        UserID = reader.GetInt32(0);
                    }
                    if (UserID == 0)
                    {
                        Message = "UserName or Password is in-correct";
                    } else
                    {
                        Message = "Successful Login";
                    }
                    reader.Close();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Message =  ex.Message;
            }

            var jsonData = new
            {
                UserID = UserID,
                Message = Message
            };

            HttpContext.Current.Response.Write(ser.Serialize(jsonData));
        }
    }
}
