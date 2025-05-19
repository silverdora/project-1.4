using Chapeau.Models;
using Chapeau.Repository.Interface;
using Chapeau.Service.Interface;
using Microsoft.AspNetCore.Identity;

namespace Chapeau.Service
{
    public class TableService : ITableService
    {
   
             private  ITableRepository _tableRepo;

            public TableService(ITableRepository tableRepo)
            {
                _tableRepo = tableRepo;
            }

            public List<Table> GetAllTables() => _tableRepo.GetAllTables();
}
    }




