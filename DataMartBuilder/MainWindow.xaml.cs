using DataMartBuilder.Models;
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
        private ObservableCollection<DataMart> dataMarts = new ObservableCollection<DataMart>();
        private ObservableCollection<DatabaseConnection> databaseConnections = new ObservableCollection<DatabaseConnection>();
        private ObservableCollection<string> targetTables = new ObservableCollection<string>(); // Таблицы целевой БД
        private DataMart currentDataMart;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddDataMart_Click(object sender, RoutedEventArgs e)
        {
            // Добавление новой витрины данных
            var dataMart = new DataMart { Name = DataMartNameTextBox.Text };
            dataMarts.Add(dataMart);
        }

        private void DeleteDataMart_Click(object sender, RoutedEventArgs e)
        {
            // Удаление выбранной витрины данных
        }

        private void DataMartList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataMartList.SelectedItem is DataMart selectedDataMart)
            {
                // Загрузка данных витрины
                DataMartNameTextBox.Text = selectedDataMart.Name;
                EditDataMartTab.IsEnabled = true;
            }
            else
            {
                EditDataMartTab.IsEnabled = false;
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EditDataMartTab.IsSelected && DataMartList.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите витрину данных для редактирования.");
                EditDataMartTab.IsSelected = false;
            }
        }

        private void DatabaseConnectionsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Логика обработки выбора базы данных
            if (DatabaseConnectionsList.SelectedItem is DatabaseConnection selectedConnection)
            {
                DbConnectionName.Text = selectedConnection.Name;
                DbConnectionString.Text = selectedConnection.ConnectionString;
            }
            else
            {
                DbConnectionName.Text = string.Empty;
                DbConnectionString.Text = string.Empty;
            }
        }

        private void AddDatabaseConnection_Click(object sender, RoutedEventArgs e)
        {
            // Логика добавления подключения к базе данных
        }

        private void DeleteDatabaseConnection_Click(object sender, RoutedEventArgs e)
        {
            // Логика удаления подключения к базе данных
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