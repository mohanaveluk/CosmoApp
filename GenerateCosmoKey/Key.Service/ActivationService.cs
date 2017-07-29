using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GenerateCosmoKey.Models;
using Key.Data;

namespace Key.Service
{
    public class ActivationService
    {
        public int InsertActivationKey(CustomerInfo customerInfo)
        {
            var activationData = new ActivationData();
            return activationData.InsertActivationKey(customerInfo);
        }
    }
}
