﻿using DataMartBuilder.Models;

namespace DataMartBuilder.Interfaces
{
    public interface IDatabaseConnection
    {
        string Name { get; set; }
        string ConnectionString { get; set; }
        List<Table> DbTables { get; set; }
        bool Connect();
        bool Disconnect();
        string TestConnection();
        string GetData();
    }
}
