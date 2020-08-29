using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
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

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
        [System.ComponentModel.ToolboxItem(false)]
        public void UploadImage(String image, String TempToolID)
        {
            // intilize the data user
            JavaScriptSerializer ser = new JavaScriptSerializer();
            int IsAdded = 0;
            string PicPath = "";

            //upload image
            Image convertedImage = Base64ToImage(image);
            string type= GetImageFormat(convertedImage).ToString();
            ImageFormat imageFormat = GetImageFormat(convertedImage);


            if (convertedImage != null)
            {
                Random ran = new Random();
                String rand = Convert.ToString(ran.Next(7000000) + 5000);
                PicPath = TempToolID + "H" + StringGeneration.getString() +"."+imageFormat.ToString() ;

                try
                {
                    //save image in server
                    convertedImage.Save(Server.MapPath("/Images/" + PicPath), imageFormat);
   
                    //save image info
                    try
                    {
                        using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString()))
                        {
                            SqlCommand cmd = new SqlCommand("INSERT INTO Pictures (ToolID, PicPath)" +
                           " VALUES (@TempToolID, @PicPath)");
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = connection;
                            cmd.Parameters.AddWithValue("@TempToolID", TempToolID);
                            cmd.Parameters.AddWithValue("@PicPath", PicPath);
                            connection.Open();
                            cmd.ExecuteNonQuery();
                            connection.Close();
                        }
                        IsAdded = 1;
                    }
                    catch (Exception ex)
                    {
                        PicPath = ex.Message;
                    }
                }
                catch (Exception ex)
                {
                    PicPath = ex.Message;
                } // if there is data

            }
            var jsonData = new
            {
                IsAdded = IsAdded,
                PicPath = PicPath,
                type = type
            };

            HttpContext.Current.Response.Write(ser.Serialize(jsonData));
        }

        public static System.Drawing.Imaging.ImageFormat GetImageFormat( System.Drawing.Image img)
        {
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
                return System.Drawing.Imaging.ImageFormat.Jpeg;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Bmp))
                return System.Drawing.Imaging.ImageFormat.Bmp;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Png))
                return System.Drawing.Imaging.ImageFormat.Png;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Emf))
                return System.Drawing.Imaging.ImageFormat.Emf;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Exif))
                return System.Drawing.Imaging.ImageFormat.Exif;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Gif))
                return System.Drawing.Imaging.ImageFormat.Gif;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Icon))
                return System.Drawing.Imaging.ImageFormat.Icon;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.MemoryBmp))
                return System.Drawing.Imaging.ImageFormat.MemoryBmp;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Tiff))
                return System.Drawing.Imaging.ImageFormat.Tiff;
            else
                return System.Drawing.Imaging.ImageFormat.Wmf;
        }

        public Image Base64ToImage(string base64String)
        {
            try
            {
                //convert Base64 sString to byte[]
                byte[] imageBytes = Convert.FromBase64String(base64String);
                using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
                {
                    // convert byte[] to Image
                    ms.Write(imageBytes, 0, imageBytes.Length);
                    Image image = Image.FromStream(ms, true);
                    return image;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        [System.ComponentModel.ToolboxItem(false)]
        public void AddTools(string UserID, string ToolName, string ToolDes, string ToolPrice
        , string ToolTypeID, string TempToolID) //get list of note
        {
            // intilize the data user
            JavaScriptSerializer ser = new JavaScriptSerializer();
            int IsAdded = 1;
            string Message = "";
            DateTime DateAdded = DateTime.Now;

            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO Tools (ToolName, ToolDes, ToolPrice, ToolTypeID, UserID, DateAdd)" +
                        " VALUES (@ToolName, @ToolDes, @ToolPrice, @ToolTypeID, @UserID, @DateAdd)");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@ToolName", ToolName);
                    cmd.Parameters.AddWithValue("@ToolDes", ToolDes);
                    cmd.Parameters.AddWithValue("@ToolPrice", ToolPrice);
                    cmd.Parameters.AddWithValue("@ToolTypeID", ToolTypeID);
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    cmd.Parameters.AddWithValue("@DateAdd", DateAdded);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
          
                //uploade the temp image id with this tool id
                Tools myUser = new Tools();
                String ToolID = myUser.GetLastToolAdded(UserID, DateAdded);

                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand("UPDATE pictures set ToolID=@ToolID where ToolID=@TempToolID");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@ToolID", ToolID);
                    cmd.Parameters.AddWithValue("@TempToolID", TempToolID);
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }

                Message = "Tool is Added";
            }
            catch (Exception ex)
            {
                IsAdded = 0;
                Message = ex.Message; // "Cannot add yor information"
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
        public void GetToolType() //get list of note
        {
            // intilize the data user
            JavaScriptSerializer ser = new JavaScriptSerializer();
            ToolType[] ToolData = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString()))
                {
                    connection.Open();
                    SqlDataAdapter adpt = new SqlDataAdapter("SELECT * FROM ToolType", connection);
                    DataTable dataTable = new DataTable();
                    adpt.Fill(dataTable);

                    ToolData = new ToolType[dataTable.Rows.Count];
                    int Count = 0;
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        ToolData[Count] = new ToolType();
                        ToolData[Count].ToolTypeID = Convert.ToInt32(dataTable.Rows[i]["ToolTypeID"]);
                        ToolData[Count].ToolTypeName = Convert.ToString(dataTable.Rows[i]["ToolTypeName"]);
                        Count++;
                    }
                    dataTable.Clear();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {

            }

            var jsonData = new
            {
                ToolData = ToolData
            };

            HttpContext.Current.Response.Write(ser.Serialize(jsonData));
        }



        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        [System.ComponentModel.ToolboxItem(false)]
        public void GetToolListing(string UserID, string Distance, string From, string to
        , string ToolTypeID, string ToolID, string q) //get list of note
        {
            if (q.Equals("@")) q = "%"; // android canot call link with %
            else q = "%" + q + "%";

            // inistailze the data Users
            JavaScriptSerializer ser = new JavaScriptSerializer();

            ToolListing[] ToolData = null;
            int HasTool = 0;
            string message = "ok";

            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "ToolListing";
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    cmd.Parameters.AddWithValue("@Distance", Distance);
                    cmd.Parameters.AddWithValue("@From", From);
                    cmd.Parameters.AddWithValue("@to", to);
                    cmd.Parameters.AddWithValue("@ToolTypeID", ToolTypeID);
                    cmd.Parameters.AddWithValue("@ToolID", ToolID);
                    cmd.Parameters.AddWithValue("@q", q);
                    connection.Open();
                    SqlDataAdapter adpt = new SqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adpt.Fill(dataTable);

                    ToolData = new ToolListing[dataTable.Rows.Count];

                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        ToolData[i] = new ToolListing();
                        ToolData[i].ToolID = Convert.ToString(dataTable.Rows[i]["ToolID"]);
                        ToolData[i].ToolName = Convert.ToString(dataTable.Rows[i]["ToolName"]);
                        ToolData[i].ToolDes = Convert.ToString(dataTable.Rows[i]["ToolDes"]);
                        ToolData[i].ToolPrice = Convert.ToString(dataTable.Rows[i]["ToolPrice"]);
                        ToolData[i].DateAdd = Convert.ToString(dataTable.Rows[i]["DateAdd"]);
                        ToolData[i].PictureLink = Convert.ToString(dataTable.Rows[i]["PictureLink"]);
                    }
                    if (dataTable.Rows.Count > 0)
                        HasTool = 1;
                    dataTable.Clear();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                HasTool = 0;
                message = ex.Message;
            }
            var jsonData = new
            {
                ToolData = ToolData,
                HasTool = HasTool,
                message = message
            };

            HttpContext.Current.Response.Write(ser.Serialize(jsonData));

        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        [System.ComponentModel.ToolboxItem(false)]
        public void GetMyTool(string UserID) //get list of note
        {
            // inistailze the data Users
            JavaScriptSerializer ser = new JavaScriptSerializer();

            ToolListing[] ToolData = null;
            int HasTool = 0;
            string message = "ok";

            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Tools WHERE UserID=@UserID");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    connection.Open();
                    SqlDataAdapter adpt = new SqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adpt.Fill(dataTable);

                    ToolData = new ToolListing[dataTable.Rows.Count];
                    GetPicture getPicture = new GetPicture();

                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        ToolData[i] = new ToolListing();
                        ToolData[i].ToolID = Convert.ToString(dataTable.Rows[i]["ToolID"]);
                        ToolData[i].ToolName = Convert.ToString(dataTable.Rows[i]["ToolName"]);
                        ToolData[i].ToolDes = Convert.ToString(dataTable.Rows[i]["ToolDes"]);
                        ToolData[i].ToolPrice = Convert.ToString(dataTable.Rows[i]["ToolPrice"]);
                        ToolData[i].DateAdd = Convert.ToString(dataTable.Rows[i]["DateAdd"]);
                        ToolData[i].PictureLink = getPicture.GetToolPicture(ToolData[i].ToolID);
                    }
                    if (dataTable.Rows.Count > 0)
                        HasTool = 1;
                    dataTable.Clear();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                HasTool = 0;
                message = ex.Message;
            }
            var jsonData = new
            {
                ToolData = ToolData,
                HasTool = HasTool,
                message = message
            };

            HttpContext.Current.Response.Write(ser.Serialize(jsonData));

        }



        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        [System.ComponentModel.ToolboxItem(false)]
        public void UpdateTool(string ToolID, string ToolName, string ToolDes, string ToolPrice
        , string ToolTypeID) //get list of note
        {
            // intilize the data user
            JavaScriptSerializer ser = new JavaScriptSerializer();
            int IsUpdate = 1;
            string Message = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand("UPDATE Tools SET ToolName=@ToolName, ToolDes=@ToolDes, ToolPrice=@ToolPrice, ToolTypeID=@ToolTypeID" +
                        " WHERE ToolID=@ToolID");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@ToolName", ToolName);
                    cmd.Parameters.AddWithValue("@ToolDes", ToolDes);
                    cmd.Parameters.AddWithValue("@ToolPrice", ToolPrice);
                    cmd.Parameters.AddWithValue("@ToolTypeID", ToolTypeID);
                    cmd.Parameters.AddWithValue("@ToolID", ToolID);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }


                Message = "Tool is Update";
            }
            catch (Exception ex)
            {
                IsUpdate = 0;
                Message = ex.Message; // "Cannot add yor information"
            }

            var jsonData = new
            {
                IsUpdate = IsUpdate,
                Message = Message
            };

            HttpContext.Current.Response.Write(ser.Serialize(jsonData));
        }


    }
}