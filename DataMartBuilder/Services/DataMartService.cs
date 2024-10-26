using DataMartBuilder.Interfaces;
using DataMartBuilder.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DataMartBuilder.Services
{
    public class DataMartService
    {
        private readonly DataMartDbContext _context;

        public DataMartService()
        {
            _context = new DataMartDbContext();
        }

        public List<DataMart> GetDataMarts() => _context.DataMarts.ToList();

        public void AddDataMart(DataMart dataMart)
        {
            _context.DataMarts.Add(dataMart);
            _context.SaveChanges();
        }

        public void RemoveDataMart(DataMart dataMart)
        {
            _context.DataMarts.Remove(dataMart);
            _context.SaveChanges();
        }
    }
}
