
using HospitalManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalManagement.Interfaces
{
    public interface IPaymentService
    {
        bool ValidatePayment(Payment payment);
        bool ProcessPayment(Payment payment);
    }

}