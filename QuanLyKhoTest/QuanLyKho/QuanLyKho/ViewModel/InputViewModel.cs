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
    public class InputViewModel : BaseViewModel
    {
        private ObservableCollection<InputInfo> _List;
        public ObservableCollection<InputInfo> List { get => _List; set { _List = value; OnPropertyChanged(); } }

        private ObservableCollection<Model.Object> _Object;
        public ObservableCollection<Model.Object> Object { get => _Object; set { _Object = value; OnPropertyChanged(); } }

        private ObservableCollection<Model.Input> _Input;
        public ObservableCollection<Model.Input> Input { get => _Input; set { _Input = value; OnPropertyChanged(); } }

        private InputInfo _SelectedItem;
        public InputInfo SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    SelectedObject = SelectedItem.Object;
                    SelectedInput = SelectedItem.Input;
                    ObjectDisplayName = SelectedObject.DisplayName;
                    DateInput = SelectedInput.DateInput;
                    Count = SelectedItem.Count ?? 0;
                    PriceInput = SelectedItem.InputPrice ?? 0;
                    PriceOutput = SelectedItem.OutputPrice ?? 0;
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

        private Model.Input _SelectedInput;
        public Model.Input SelectedInput
        {
            get => _SelectedInput;
            set
            {
                _SelectedInput = value;
                OnPropertyChanged();
            }
        }

        private string _ObjectDisplayName;
        public string ObjectDisplayName { get => _ObjectDisplayName; set { _ObjectDisplayName = value; OnPropertyChanged(); } }

        private int _Count;
        public int Count { get => _Count; set { _Count = value; OnPropertyChanged(); } }

        private double _PriceInput;
        public double PriceInput { get => _PriceInput; set { _PriceInput = value; OnPropertyChanged(); } }

        private double _PriceOutput;
        public double PriceOutput { get => _PriceOutput; set { _PriceOutput = value; OnPropertyChanged(); } }

        private string _Status;
        public string Status { get => _Status; set { _Status = value; OnPropertyChanged(); } }

        private DateTime? _DateInput;
        public DateTime? DateInput { get => _DateInput; set { _DateInput = value; OnPropertyChanged(); } }

        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public InputViewModel()
        {
            List = new ObservableCollection<InputInfo>(DataProvider.Ins.DB.InputInfoes);
            Object = new ObservableCollection<Model.Object>(DataProvider.Ins.DB.Objects);
            Input = new ObservableCollection<Input>(DataProvider.Ins.DB.Inputs);

            AddCommand = new RelayCommand<object>((p) =>
            {
                return true;
            },
            (p) =>
            {
            var inputInfo = new InputInfo() { Id = Guid.NewGuid().ToString() };
            DataProvider.Ins.DB.InputInfoes.Add(inputInfo);
            DataProvider.Ins.DB.SaveChanges();

            List.Add(inputInfo);

                UpdateOutputList();
            });

            void UpdateOutputList()
            {
                var outputList = DataProvider.Ins.DB.OutputInfoes.Where(x => x.IdObject == SelectedObject.Id).ToList();
                foreach (var outputInfo in outputList)
                {
                    outputInfo.Object = SelectedObject;
                }
                DataProvider.Ins.DB.SaveChanges();
            }



            EditCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedItem == null)
                    return false;

                var displayList = DataProvider.Ins.DB.InputInfoes.Where(x => x.Id == SelectedItem.IdObject);
                if (displayList != null && displayList.Count() != 0)
                    return true;

                return false;
            }, (p) =>
            {
                var inputInfo = DataProvider.Ins.DB.InputInfoes.Where((x) => x.Id == SelectedItem.Id).SingleOrDefault();
                if (inputInfo != null)
                {
                    inputInfo.IdObject = SelectedObject.Id;
                    inputInfo.Count = Count;
                    inputInfo.InputPrice = PriceInput;
                    inputInfo.OutputPrice = PriceOutput;
                    inputInfo.Status = Status;

                    DataProvider.Ins.DB.SaveChanges();
                }

                // Cập nhật các thuộc tính của SelectedItem
                SelectedItem.Object = SelectedObject;
                SelectedItem.Input = SelectedInput;
                SelectedItem.Count = Count;
                SelectedItem.InputPrice = PriceInput;
                SelectedItem.OutputPrice = PriceOutput;
                SelectedItem.Status = Status;
            });

            DeleteCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedItem == null)
                    return false;

                return true;
            }, (p) =>
            {
                var inputInfo = DataProvider.Ins.DB.InputInfoes.Where((x) => x.Id == SelectedItem.Id).SingleOrDefault();

                DataProvider.Ins.DB.InputInfoes.Remove(inputInfo);
                DataProvider.Ins.DB.SaveChanges();

                List.Remove(inputInfo);
            });



        }

    }
}
