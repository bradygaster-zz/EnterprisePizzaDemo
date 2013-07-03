using EnterprisePizza.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterprisePizza.OrderBillboard.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public MainPageViewModel()
        {
            this.Orders = new ObservableCollection<OrderItem>();
            this.Ingredients = new ObservableCollection<IngredientAvailability>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        ObservableCollection<OrderItem> _Orders;
        public ObservableCollection<OrderItem> Orders 
        {
            get
            {
                return _Orders;
            }
            set
            {
                _Orders = value;
                OnPropertyChanged("Orders");
            }
        }

        ObservableCollection<IngredientAvailability> _Ingredients;
        public ObservableCollection<IngredientAvailability> Ingredients
        {
            get
            {
                return _Ingredients;
            }
            set
            {
                _Ingredients = value;
                OnPropertyChanged("Ingredients");
            }
        }
    }
}
