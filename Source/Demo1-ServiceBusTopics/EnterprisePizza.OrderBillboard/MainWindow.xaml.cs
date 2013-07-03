using EnterprisePizza.Infrastructure;
using EnterprisePizza.OrderBillboard.ViewModels;
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
                };
        }

        private void AddOrderToView(PizzaOrder order)
        {
            Dispatcher.InvokeAsync(() =>
                {
                    var orderViewModel = OrderItem.ConvertOrderToViewModel(order);

                    this.ViewModel.Orders.Add(orderViewModel);
                });
        }
    }
}
