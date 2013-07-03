using EnterprisePizza.Infrastructure;
using EnterprisePizza.OrderBillboard.ViewModels;
using EnterprisePizza.ServiceContracts;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Controls.Primitives;
using EnterprisePizza.Domain;
using System.Threading;
using System;

namespace EnterprisePizza.OrderBillboard
{
    public partial class MainWindow : Window
    {
        private IOrderStatusService _orderStatusService;
        private IInventoryService _corporateInventoryService;
        private IInventoryService _storeInventoryService;
        private ChannelFactory<IInventoryService> _webSiteInventoryChannel;
        private int _retryCount;
        
        public MainPageViewModel ViewModel { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            base.Loaded += (sender, args) =>
                {
                    this.ViewModel = new MainPageViewModel();
                    this.DataContext = this.ViewModel;

                    ServiceBusTopicHelper.Setup(SubscriptionInitializer.Initialize())
                        .Subscribe<PizzaOrder>((order) => AddOrderToView(order)
                            ,"(IsOrdered = true) AND (IsReceivedByStore = false)"
                            ,"OrdersSentToStore"
                        );

                    this.ConnectToServices(this.GetStoreInventory);
                };
        }

        private void ConnectToServices(Action onConnectionCallback)
        {
            if (_retryCount < 4)
            {
                try
                {
                    _orderStatusService =
                        new ChannelFactory<IOrderStatusService>("OrderStatusService").CreateChannel();

                    _corporateInventoryService =
                        new ChannelFactory<IInventoryService>("CorporateOfficeInventoryService").CreateChannel();

                    _webSiteInventoryChannel = 
                        new ChannelFactory<IInventoryService>("WebSiteInventoryService");

                    _storeInventoryService = _webSiteInventoryChannel.CreateChannel();

                    _retryCount = 0;

                    if (onConnectionCallback != null)
                        onConnectionCallback();
                }
                catch
                {
                    _retryCount += 1;
                    Thread.Sleep(2000);
                }
            }
        }

        private void GetStoreInventory()
        {
            if (_retryCount < 4)
            {
                try
                {
                    if (_webSiteInventoryChannel.State == CommunicationState.Opened)
                    {
                        var ingredients = _storeInventoryService.GetIngredientList();

                        this.ViewModel.Ingredients.Clear();

                        ingredients.ToList().ForEach(ing =>
                            this.ViewModel.Ingredients.Add(ing)
                            );
                    }
                    else
                    {
                        Thread.Sleep(2000);

                        GetStoreInventory();
                    }
                }
                catch
                {
                    _retryCount += 1;

                    Thread.Sleep(2000);

                    GetStoreInventory();
                }
            }
        }

        private void UpdateInventoryServices(ToggleIngredientAvailabilityRequest request)
        {
            Dispatcher.InvokeAsync(() =>
                {
                    _storeInventoryService.ToggleIngredientAvailability(request);
                    _corporateInventoryService.ToggleIngredientAvailability(request);
                });
        }

        private void AddOrderToView(PizzaOrder order)
        {
            Dispatcher.InvokeAsync(() =>
                {
                    var orderViewModel = OrderItem.ConvertOrderToViewModel(order, 
                        this.ViewModel.Ingredients);

                    this.ViewModel.Orders.Add(orderViewModel);

                    SendOrderStatus(orderViewModel, "Your order has been received by our store.");
                });
        }

        private void SendOrderStatus(OrderItem order, string status)
        {
            _orderStatusService.SendOrderStatus(new OrderStatusRequest
            {
                ClientIdentifier = order.ClientIdentifier,
                OrderId = order.OrderId,
                Status = status
            });
        }

        OrderItem GetOrderFromButtonClick(object sender)
        {
            var order = ((Button)sender).CommandParameter as OrderItem;
            ((Button)sender).IsEnabled = false;
            return order;
        }

        private void markPizzaPreparedButton_Click(object sender, RoutedEventArgs e)
        {
            var order = GetOrderFromButtonClick(sender);
            SendOrderStatus(order, "Your order has been prepared and is about to be baked.");
        }

        private void markPizzaCookedButton_Click(object sender, RoutedEventArgs e)
        {
            var order = GetOrderFromButtonClick(sender);
            SendOrderStatus(order, "Your order has been cooked and is getting ready to come home.");
        }

        private void markPizzaLeavingBuildingButton_Click(object sender, RoutedEventArgs e)
        {
            var order = GetOrderFromButtonClick(sender);
            SendOrderStatus(order, "Your order has left the building!");
            this.ViewModel.Orders.Remove(order);
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            var ingredient = ((CheckBox)sender).DataContext
                as IngredientAvailability;

            this.UpdateInventoryServices(new ToggleIngredientAvailabilityRequest
            {
                IngredientName = ingredient.Name,
                IsAvailable = true
            });
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            var ingredient = ((CheckBox)sender).DataContext
                as IngredientAvailability;

            this.UpdateInventoryServices(new ToggleIngredientAvailabilityRequest
            {
                IngredientName = ingredient.Name,
                IsAvailable = false
            });
        }
    }
}
