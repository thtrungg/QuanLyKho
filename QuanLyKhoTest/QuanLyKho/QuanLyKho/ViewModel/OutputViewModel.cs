using QuanLyKho.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace QuanLyKho.ViewModel
{
    public class OutputViewModel : BaseViewModel
    {
        private ObservableCollection<OutputInfo> _List;
        public ObservableCollection<OutputInfo> List { get => _List; set { _List = value; OnPropertyChanged(); } }

        private ObservableCollection<Model.Object> _Object;
        public ObservableCollection<Model.Object> Object { get => _Object; set { _Object = value; OnPropertyChanged(); } }

        private ObservableCollection<Customer> _Customer;
        public ObservableCollection<Customer> Customer { get => _Customer; set { _Customer = value; OnPropertyChanged(); } }

        private OutputInfo _SelectedItem;
        public OutputInfo SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    SelectedObject = SelectedItem.Object;
                    SelectedCustomer = SelectedItem.Customer;
                    ObjectDisplayName = SelectedObject.DisplayName;
                    Count = SelectedItem.Count ?? 0;
                    Status = SelectedItem.Status;
                }
            }
        }

        private Model.Object _SelectedObject;
        public Model.Object SelectedObject
        {
            get => _SelectedObject;
            set
            {
                _SelectedObject = value;
                OnPropertyChanged();
            }
        }

        private Customer _SelectedCustomer;
        public Customer SelectedCustomer
        {
            get => _SelectedCustomer;
            set
            {
                _SelectedCustomer = value;
                OnPropertyChanged();
            }
        }

        private string _ObjectDisplayName;
        public string ObjectDisplayName { get => _ObjectDisplayName; set { _ObjectDisplayName = value; OnPropertyChanged(); } }

        private int _Count;
        public int Count { get => _Count; set { _Count = value; OnPropertyChanged(); } }

        private string _Status;
        public string Status { get => _Status; set { _Status = value; OnPropertyChanged(); } }

        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public OutputViewModel()
        {
            List = new ObservableCollection<OutputInfo>(DataProvider.Ins.DB.OutputInfoes);
            Object = new ObservableCollection<Model.Object>(DataProvider.Ins.DB.Objects);
            Customer = new ObservableCollection<Customer>(DataProvider.Ins.DB.Customers);

            AddCommand = new RelayCommand<object>((p) =>
            {
                return true;
            },
            (p) =>
            {
                var outputInfo = new OutputInfo() { Id = Guid.NewGuid().ToString() };
                outputInfo.Object = SelectedObject;
                outputInfo.Customer = SelectedCustomer;
                outputInfo.Count = Count;
                outputInfo.Status = Status;

                DataProvider.Ins.DB.OutputInfoes.Add(outputInfo);
                DataProvider.Ins.DB.SaveChanges();

                List.Add(outputInfo);
            });

            EditCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedItem == null)
                    return false;

                var displayList = DataProvider.Ins.DB.OutputInfoes.Where(x => x.Id == SelectedItem.IdObject);
                if (displayList != null && displayList.Count() != 0)
                    return true;

                return false;
            }, (p) =>
            {
                var outputInfo = DataProvider.Ins.DB.OutputInfoes.Where((x) => x.Id == SelectedItem.Id).SingleOrDefault();
                if (outputInfo != null)
                {
                    outputInfo.Object = SelectedObject;
                    outputInfo.Customer = SelectedCustomer;
                    outputInfo.Count = Count;
                    outputInfo.Status = Status;

                    DataProvider.Ins.DB.SaveChanges();
                }

                SelectedItem.Object = SelectedObject;
                SelectedItem.Customer = SelectedCustomer;
                SelectedItem.Count = Count;
                SelectedItem.Status = Status;
            });

            DeleteCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedItem == null)
                    return false;

                return true;
            }, (p) =>
            {
                var outputInfo = DataProvider.Ins.DB.OutputInfoes.Where((x) => x.Id == SelectedItem.Id).SingleOrDefault();

                DataProvider.Ins.DB.OutputInfoes.Remove(outputInfo);
                DataProvider.Ins.DB.SaveChanges();

                List.Remove(outputInfo);
            });
        }
    }
}
