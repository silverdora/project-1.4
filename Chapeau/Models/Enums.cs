namespace Chapeau.Models
{
    public enum MenuCard //as enum starts with 0 and in my database lunch = 1
    {
        Lunch = 1,
        Dinner = 2,
        Drinks = 3
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
        Coffee,
        Tea,
        SoftDrink
    }


    public enum PaymentType
    {
        Cash,
        DebitCard,
        CreditCard
    }
}

