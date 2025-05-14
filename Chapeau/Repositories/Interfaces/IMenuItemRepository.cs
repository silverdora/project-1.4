using Chapeau.Models;

namespace Chapeau.Repositories.Interfaces
{
    public interface IMenuItemRepository
    {
        List<MenuItem> GetByCardAndCategory(string card, string category);
        List<string> GetAllCards();
        List<string> GetAllCategories();


    }
}
