using Chapeau.Models;
using System.Collections.Generic;


namespace Chapeau.Repository.Interface
{
    public interface ITableRepository
    {
        List<Table> GetAllTables();

    }
}
