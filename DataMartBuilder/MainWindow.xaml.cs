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
        public ObservableCollection<SelectedTable> AvailableTables = new ObservableCollection<SelectedTable>();

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

            if (DataMarts.Any(x => x.Name == DataMartNameTextBox.Text))
            {
                MessageBox.Show("Имя БД должно быть уникально");
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
                TargetTablesList.ItemsSource = AvailableTables;

                TargetDatabases = SelectedDataMart.DatabaseConnections.Select(x => GetDataConnection(x)).ToList();
                CurrentDatebase = SelectedDataMart.CurrantDatabase != null ? GetDataConnection(SelectedDataMart.CurrantDatabase) : null;

                if (SelectedDataMart.CurrantDatabase != null)
                {
                    CurrentConnectionString.Text = SelectedDataMart.CurrantDatabase.ConnectionString;
                    SelectedTablesList.ItemsSource = SelectedDataMart.SelectedTables;
                }

                UpdateAvailableTables();
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

                StatusConnectsDb.Background = Brushes.Yellow;
            }
        }

        private void DeleteDatabaseConnection_Click(object sender, RoutedEventArgs e)
        {
            if (DatabaseConnectionsList.SelectedItem != null)
            {
                SelectedDataMart.DatabaseConnections.Remove(DatabaseConnectionsList.SelectedItem as DbConnection);

                StatusConnectsDb.Background = Brushes.Yellow;
            }

            BindDataMartDetails();
        }

        private void CheckDbConnection_Click(object sender, RoutedEventArgs e)
        {
            CheckTargetConnections();
        }

        private void ConnectToTargetDatabase_Click(object sender, RoutedEventArgs e)
        {
            ConnectToTargetDatabase();
        }

        private void ConnectToTargetDatabase()
        {
            if (SelectedDataMart.CurrantDatabase == null || string.IsNullOrEmpty(SelectedDataMart.CurrantDatabase.ConnectionString))
            {
                if (!string.IsNullOrEmpty(CurrentConnectionString.Text))
                {
                    if (SelectedDataMart.CurrantDatabase == null)
                    {
                        if (CurrentConnectionType.SelectedItem as ComboBoxItem == null)
                        {
                            MessageBox.Show("Выбирите тип подключения");
                            return;
                        }

                        var connectionType = CurrentConnectionType.SelectedItem as ComboBoxItem;
                        switch (connectionType?.Uid)
                        {
                            case "SqlServer":
                                SelectedDataMart.CurrantDatabase = new DbConnection("DataMart", CurrentConnectionString.Text, ConnectionTypes.SqlServer);
                                break;
                            default:
                                break;
                        }

                        BindDataMartDetails();
                    }
                    else
                    {
                        SelectedDataMart.CurrantDatabase.ConnectionString = CurrentConnectionString.Text;
                    }
                }
                else
                {
                    MessageBox.Show("Заполните поле строки подключения к Базе данных витрины");
                    return;
                }
            }

            if (CurrentConnectionString.Text != SelectedDataMart.CurrantDatabase.ConnectionString)
                SelectedDataMart.CurrantDatabase.ConnectionString = CurrentConnectionString.Text;

            var isError = false;
            var errors = new StringBuilder();

            var message = CurrentDatebase.TestConnection();

            if (!string.IsNullOrEmpty(message))
            {
                isError = true;
                errors.AppendLine($"Не удалось подключиться, ошибка: {message}");
            }

            if (isError)
            {
                StatusConnectsCurrentDb.Background = Brushes.Red;
                MessageBox.Show(errors.ToString());
                return;
            }
            else
            {
                StatusConnectsCurrentDb.Background = Brushes.Green;
            }

            var error = CurrentDatebase.GetData();

            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show("При получении данных вохникла ошибка: " + error.ToString());
            }
        }

        private void AddTable_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedDataMart.CurrantDatabase == null)
            {
                MessageBox.Show("Подключитесь к целевой БД");
                return;
            }

            if (TargetTablesList.SelectedItem != null)
                SelectedDataMart.SelectedTables.Add(TargetTablesList.SelectedItem as SelectedTable);

            UpdateAvailableTables();
        }

        private void RemoveTable_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedTablesList.SelectedItem != null)
                SelectedDataMart.SelectedTables.Remove(SelectedTablesList.SelectedItem as SelectedTable);
            UpdateAvailableTables();
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
                    DbConnectionString.Text = "Server=адрес_сервера;Database=имя_базы_данных;Trusted_Connection=True;TrustServerCertificate=true;";
                    break;
                default:
                    break;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            SaveData();
            ClearScripts();
        }

        private void ClearScripts()
        {
            var folder = "./scripts";

            if (Directory.Exists(folder))
                Directory.Delete(folder, true);
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
                    continue;
                }

                var error = db.GetData();

                if (!string.IsNullOrEmpty(error))
                {
                    isError = true;
                    errors.AppendLine($"При получении данных из БД {db.Name} возникла ошибка: {error}");
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

        private void TargetConnectionString_TextChanged(object sender, TextChangedEventArgs e)
        {
            StatusConnectsCurrentDb.Background = Brushes.Yellow;
        }

        private void UpdateAvailableTables_Click(object sender, RoutedEventArgs e)
        {
            UpdateAvailableTables();
        }

        private void UpdateAvailableTables()
        {
            AvailableTables.Clear();

            foreach (var db in SelectedDataMart.DatabaseConnections)
            {
                foreach (var table in db.DbTables)
                {
                    var selectedTable = new SelectedTable(table, db);
                    if (!SelectedDataMart.SelectedTables.Any(x => x.TableName == selectedTable.TableName))
                        AvailableTables.Add(selectedTable);
                }
            }
        }

        private void CurrentConnectionType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var connectionType = CurrentConnectionType.SelectedItem as ComboBoxItem;
            switch (connectionType?.Uid)
            {
                case "SqlServer":
                        CurrentConnectionString.Text = "Server=адрес_сервера;Database=имя_базы_данных;Trusted_Connection=True;TrustServerCertificate=true;";
                    break;
                default:
                    break;
            }
        }

        private void SaveData_Click(object sender, RoutedEventArgs e)
        {
            SaveData();
        }

        private async void TransferData_Click(object sender, RoutedEventArgs e)
        {
            SaveData();

            MainGrid.IsEnabled = false;
            TransferDataButton.IsEnabled = false;

            try
            {
                var transferData = new TransferData(CurrentDatebase, TargetDatabases, SelectedDataMart.SelectedTables.ToList());
                var result = await Task.Run(transferData.Execute);

                if (!string.IsNullOrEmpty(result))
                    MessageBox.Show("Данные перенесены.");
                else
                    MessageBox.Show("Возникли ошибки. Ошибки: " + result);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Возникла ошибка при переносе данных: " + ex.Message);
            }
            finally
            {
                MainGrid.IsEnabled = true;
                TransferDataButton.IsEnabled = true;
            }
        }
    }
}