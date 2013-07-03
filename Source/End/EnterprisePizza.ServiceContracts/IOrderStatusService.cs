using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace EnterprisePizza.ServiceContracts
{
    [ServiceContract]
    public interface IOrderStatusService
    {
        [OperationContract]
        void SendOrderStatus(OrderStatusRequest request);
    }

    public class OrderStatusRequest
    {
        public Guid ClientIdentifier { get; set; }
        public int OrderId { get; set; }
        public string Status { get; set; }
    }
}
