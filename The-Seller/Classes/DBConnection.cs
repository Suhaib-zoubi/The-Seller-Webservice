using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Web.Configuration;
using System.Data.SqlClient;

namespace The_Seller
{
    public class DBConnection
    {
        
        public static String ConnectionString()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource= "suhaib.database.windows.net";
            builder.UserID = "suhaib";
            builder.Password = "Successful.1";
            builder.InitialCatalog = "TheSeller";
            return builder.ConnectionString;
        }

    }
}