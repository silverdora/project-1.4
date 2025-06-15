using Chapeau.Models;
using Chapeau.ViewModels;
using Microsoft.Data.SqlClient;

namespace Chapeau.HelperMethods
{
   
        public static class MapTableHelper
        {
            public static TableOrderViewModel MapFromReader(SqlDataReader reader)
            {
                return new TableOrderViewModel
                {
                    TableId = Convert.ToInt32(reader["tableID"]),
                    TableNumber = Convert.ToInt32(reader["table_number"]),
                    IsOccupied = Convert.ToBoolean(reader["isOccupied"]),
                    DrinkStatus = reader["DrinkStatus"] != DBNull.Value
                        && Enum.TryParse(reader["DrinkStatus"].ToString(), out Status drinkStatus)
                            ? drinkStatus : (Status?)null,
                    FoodStatus = reader["FoodStatus"] != DBNull.Value
                        && Enum.TryParse(reader["FoodStatus"].ToString(), out Status foodStatus)
                            ? foodStatus : (Status?)null
                };
            }
        }

    
}
