using DataMartBuilder.Interfaces;
using DataMartBuilder.Models;
using DataMartBuilder.Services;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DataMartBuilder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<DataMart> DataMarts { get; set; }
        public DataMart SelectedDataMart { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataMarts = new ObservableCollection<DataMart>();
            DataMartList.ItemsSource = DataMarts;
        }

        private void AddDataMart_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(DataMartNameTextBox.Text))
            {
                MessageBox.Show("Нужно ввести имя витрины данных");
                return;
            }

            var newDataMart = new DataMart { Name = DataMartNameTextBox.Text };
            DataMarts.Add(newDataMart);
            DataMartList.SelectedItem = newDataMart;
        }

        private void DeleteDataMart_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedDataMart != null)
            {
                DataMarts.Remove(SelectedDataMart);
            }
        }

        private void DataMartList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedDataMart = DataMartList.SelectedItem as DataMart;
            EditDataMartTab.IsEnabled = SelectedDataMart != null;
            BindDataMartDetails();
        }

        private void BindDataMartDetails()
        {
            if (SelectedDataMart != null)
            {
                DatabaseConnectionsList.ItemsSource = SelectedDataMart.DatabaseConnections;
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EditDataMartTab.IsSelected && DataMartList.SelectedItem == null)
            {
                EditDataMartTab.IsSelected = false;
                ListDataMartTab.IsSelected = true;
            }
        }

        private void DatabaseConnectionsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void AddDatabaseConnection_Click(object sender, RoutedEventArgs e)
        {
            var nameConnection = DbConnectionName.Text;
            var connectionString = DbConnectionString.Text;

            if (nameConnection != null && connectionString != null)
            {
                var connectionType = ConnectionType.SelectedItem as ComboBoxItem;
                switch (connectionType?.Uid)
                {
                    case "SqlServer":
                        SelectedDataMart.DatabaseConnections.Add(new SqlServerConnector(nameConnection, connectionString));
                        break;
                    default:
                        break;
                }
            }
        }

        private void DeleteDatabaseConnection_Click(object sender, RoutedEventArgs e)
        {
            if (DatabaseConnectionsList.SelectedItem != null)
            {
                SelectedDataMart.DatabaseConnections.Remove(DatabaseConnectionsList.SelectedItem as IDatabaseConnection);
            }
        }

        private void CheckDbConnection_Click(object sender, RoutedEventArgs e)
        {
            // Логика проверки подключения к базе данных
        }

        private void ConnectToTargetDatabase_Click(object sender, RoutedEventArgs e)
        {
            // Логика подключения к целевой базе данных
        }

        private void AddTable_Click(object sender, RoutedEventArgs e)
        {
            // Логика добавления таблицы в целевую БД
        }

        private void RemoveTable_Click(object sender, RoutedEventArgs e)
        {
            // Логика удаления таблицы из целевой БД
        }

        private void AddLink_Click(object sender, RoutedEventArgs e)
        {
            // Логика добавления связи между таблицами
        }

        private void RemoveLink_Click(object sender, RoutedEventArgs e)
        {
            // Логика удаления связи между таблицами
        }
    }
}