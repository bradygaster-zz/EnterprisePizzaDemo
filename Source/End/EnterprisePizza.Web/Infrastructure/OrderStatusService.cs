using EnterprisePizza.DataAccess;
using EnterprisePizza.ServiceContracts;
using EnterprisePizza.Web.Hubs;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EnterprisePizza.Web.Infrastructure
{
    public class OrderStatusService : IOrderStatusService
    {
        public void SendOrderStatus(OrderStatusRequest request)
        {
            GlobalHost.ConnectionManager.GetHubContext<OrderStatusHub>()
                .Clients
                    .Group(request.ClientIdentifier.ToString())
                        .statusUpdated(request.Status);
        }
    }
}