using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace WebAppBroadcastSignalR.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult sendMsg(string strInput)
        {
            string strStatus = string.Empty;
            string strMessage = string.Empty;
            int resultCount = 0;
            string connectionString = ConfigurationManager.ConnectionStrings["BroadCastConnection"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "insert into Broadcasts (BroadcastMessage) values ('" + strInput + "')";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    resultCount = command.ExecuteNonQuery();
                    strStatus = "01";
                    strMessage = "Sended";
                    connection.Close();
                }
            }
            return Json(new { Status = strStatus, Message = strMessage });
        }
    }
}