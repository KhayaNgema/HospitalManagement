
using HospitalManagement.Models;

namespace HospitalManagement.Interfaces
{
    public interface IPaymentService
    {
        bool ValidatePayment(Payment payment);
        bool ProcessPayment(Payment payment);
    }

}