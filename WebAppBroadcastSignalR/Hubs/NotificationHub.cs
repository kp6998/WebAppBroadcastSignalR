using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace WebAppBroadcastSignalR.Hubs
{
    [HubName("notificationHub")]
    public class NotificationHub : Hub
    {
        private string broadcastmessage = string.Empty;
        private string chk = string.Empty;
        private string connectionString = ConfigurationManager.ConnectionStrings["BroadCastConnection"].ConnectionString;
        [HubMethodName("sendNotification")]
        public async Task<string> SendNotification()
        {
            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                string query = "select BroadcastId, BroadcastMessage from Broadcasts order by broadcastId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Notification = null;
                    DataTable dt = new DataTable();

                    SqlDependency dependency = new SqlDependency(command);
                    SqlDependency.Start(connectionString);
                    dependency.OnChange += new OnChangeEventHandler(Dependency_OnChange);
                    var reader = command.ExecuteReader();
                    dt.Load(reader);
                    broadcastmessage = JsonConvert.SerializeObject(dt);
                }
            }
            if (chk != broadcastmessage)
            {
                chk = broadcastmessage;
                IHubContext context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                return await context.Clients.All.ReceiveNotification(broadcastmessage);
            }
            else
            {
                return "";
            }
        }

        private static void Dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type != SqlNotificationType.Change)
            {
                NotificationHub nHub = new NotificationHub();
                nHub.SendNotification();
            }
        }
    }
}