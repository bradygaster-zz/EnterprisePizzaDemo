using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EnterprisePizza.Web.Hubs
{
    [HubName("orderStatus")]
    public class OrderStatusHub : Hub
    {
        public void ListenForUpdates(string clientId)
        {
            Groups.Add(base.Context.ConnectionId, clientId);
        }
    }
}