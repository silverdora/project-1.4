using System.ComponentModel.DataAnnotations;

namespace Chapeau.ViewModels
{
    public class PaymentViewModel
    {
       
            public int OrderID { get; set; }
            public decimal AmountToPay { get; set; }
            public string PaymentType { get; set; } // e.g. Cash, Card
        

    }
}
