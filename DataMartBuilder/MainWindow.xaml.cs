using DataMartBuilder.Interfaces;
using DataMartBuilder.Models;
using DataMartBuilder.Services;
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
using System.IO;
using System.Xml.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DataMartBuilder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<DataMart> DataMarts { get; set; }
        public DataMart SelectedDataMart { get; set; }
        private List<IDatabaseConnection> TargetDatabases {  get; set; }
        private IDatabaseConnection CurrentDatebase { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataMarts = LoadData();
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

                TargetDatabases = SelectedDataMart.DatabaseConnections.Select(x => GetDataConnection(x)).ToList();
                CurrentDatebase = SelectedDataMart.TargetDatabase != null ? GetDataConnection(SelectedDataMart.TargetDatabase) : null;
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
                        SelectedDataMart.DatabaseConnections.Add(new DbConnection(nameConnection, connectionString, ConnectionTypes.SqlServer));
                        break;
                    default:
                        break;
                }

                DbConnectionName.Text = string.Empty;
                DbConnectionString.Text = string.Empty;
                ConnectionType.SelectedIndex = -1;

                BindDataMartDetails();
            }
        }

        private void DeleteDatabaseConnection_Click(object sender, RoutedEventArgs e)
        {
            if (DatabaseConnectionsList.SelectedItem != null)
            {
                SelectedDataMart.DatabaseConnections.Remove(DatabaseConnectionsList.SelectedItem as DbConnection);
            }

            BindDataMartDetails();
        }

        private void CheckDbConnection_Click(object sender, RoutedEventArgs e)
        {
            CheckTargetConnections();
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

        private void ConnectionType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var connectionType = ConnectionType.SelectedItem as ComboBoxItem;
            switch (connectionType?.Uid)
            {
                case "SqlServer":
                    DbConnectionString.Text = "Server=адрес_сервера;Database=имя_базы_данных;Trusted_Connection=True";
                    break;
                default:
                    break;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            SaveData();
        }

        private ObservableCollection<DataMart> LoadData()
        {
            var fileName = "./data.json";
            var result = new ObservableCollection<DataMart>();

            if (File.Exists(fileName))
            {
                var json = File.ReadAllText(fileName);

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Converters =
                    {
                        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                    }
                };

                result = JsonSerializer.Deserialize<ObservableCollection<DataMart>>(json, options);
            }

            return result;
        }

        private void SaveData()
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Converters =
                    {
                        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                    }
                };

                string jsonString = JsonSerializer.Serialize(DataMarts, options);

                File.WriteAllText("./data.json", jsonString);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Возникла ошибка при сохранении данных: " + ex.Message);
            }
        }

        private IDatabaseConnection GetDataConnection(DbConnection connection)
        {
            var connectionType = connection.ConnectionType;

            switch (connectionType)
            {
                case ConnectionTypes.SqlServer:
                    return new SqlServerConnector(connection);

                default:
                    return null;
            }
        }

        private void CheckTargetConnections()
        {
            var isError = false;
            var errors = new StringBuilder();

            foreach (var db in TargetDatabases)
            {
                var message = db.TestConnection();

                if (!string.IsNullOrEmpty(message))
                {
                    isError = true;
                    errors.AppendLine($"Не удалось подключиться к {db.Name}, ошибка: {message}");
                }
            }

            if (isError)
            {
                StatusConnectsDb.Background = Brushes.Red;
                MessageBox.Show(errors.ToString());
            }
            else
            {
                StatusConnectsDb.Background = Brushes.Green;
            }
        }
    }
}