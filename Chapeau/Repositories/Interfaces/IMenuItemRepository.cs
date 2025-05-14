using Chapeau.Models;

namespace Chapeau.Repositories.Interfaces
{
    public interface IMenuItemRepository
    {
        List<MenuItem> GetAll();
       // List<MenuItem> GetFiltered(string card, string category);
    }
}
