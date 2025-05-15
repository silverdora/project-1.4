using Chapeau.Models;

namespace Chapeau.Services
{
    public interface IMenuItemService
    {
        List<MenuItem> GetByCardAndCategory(string card, string category);
       


    }
}
