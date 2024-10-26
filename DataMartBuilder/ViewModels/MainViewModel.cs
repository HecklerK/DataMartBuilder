using DataMartBuilder.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DataMartBuilder.ViewModels
{
    public class MainViewModel
    {
        public ObservableCollection<DataMart> DataMarts { get; set; }
        public DataMart SelectedDataMart { get; set; }

        public ICommand AddDataMartCommand { get; }
        public ICommand DeleteDataMartCommand { get; }

        public MainViewModel()
        {
            DataMarts = new ObservableCollection<DataMart>();
            AddDataMartCommand = new RelayCommand(AddDataMart);
        }

        private void AddDataMart()
        {
            var newDataMart = new DataMart { Name = "Новая Витрина" };
            DataMarts.Add(newDataMart);
            SelectedDataMart = newDataMart;
        }

        private void DeleteDataMart(DataMart dataMart)
        {
            DataMarts.Remove(dataMart);
        }
    }
}
