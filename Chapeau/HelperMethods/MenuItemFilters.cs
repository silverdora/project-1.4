namespace Chapeau.HelperMethods
{
    public static class MenuItemFilters
    {
        public static readonly List<string> Cards = new() { "All", "Diner", "Drinks", "Lunch" };
        public static readonly List<string> Categories = new() { "All", "Beer", "Coffe/Tea","Desserts","Entremets", "Mains", "Soft Drinks", "Spirits", "Starters", "Wine" };

        public enum MenuCard
        {
            Lunch,
            Diner,
            Drinks
        }

        public enum MenuCategory
        {
            Starters,
            Mains,
            Desserts,
            Entremets,
            Beer,
            Wine,
            Spirits,
            CoffeeTea,
            SoftDrinks
        }

    }
}
