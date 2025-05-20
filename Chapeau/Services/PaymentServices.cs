using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using Chapeau.Services.Interfaces;

namespace Chapeau.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public List<Payment> GetAllPayments(int orderId)
        {
            return _paymentRepository.GetAllPayments();
        }
        public void ProcessPaymentForOrder(Order order, string paymentType, decimal tip)
        {
            decimal total = 0;
            decimal lowVATAmount = 0;
            decimal highVATAmount = 0;

            foreach (var item in order.OrderItems)
            {
                var price = item.MenuItem.Price * item.Quantity;
                var vatRate = item.MenuItem.VATPercent / 100m;
                var vatAmount = price * vatRate;

                if (item.MenuItem.VATPercent == 9)
                {
                    lowVATAmount += vatAmount;
                }
                else if (item.MenuItem.VATPercent == 21)
                {
                    highVATAmount += vatAmount;
                }

                total += price;
            }

            var payment = new Payment
            {
                orderID = order.OrderID,
                paymentType = paymentType,
                amountPaid = total + tip,
                tipAmount = tip,
                lowVATAmount = lowVATAmount,
                highVATAmount = highVATAmount,
                paymentDAte = DateTime.Now
            };

            _paymentRepository.AddPayment(payment);
        }


    }
}
