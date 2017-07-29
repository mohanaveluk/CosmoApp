using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Web;
using GenerateCosmoKey.Data;
using GenerateCosmoKey.Models;

namespace GenerateCosmoKey.Service
{
    public class ActivationService
    {
        #region Update ActivationKey
        public string InsertActivationKey(CustomerInfo customerInfo)
        {
            int result;
            var activationData = new ActivationData();
            result = activationData.InsertActivationKey(customerInfo);
            if (result > 0)
                return "This activation key was generated earlier";
            return "Success";
        }
        #endregion Update ActivationKey

        #region Get all ActivationKeyList

        public List<CustomerInfo> GetActivationList(string internalKey, int id)
        {
            var activationData = new ActivationData();
            return activationData.GetActivationList(internalKey, id);
        }

        #endregion Get all ActivationKeyList
    }
}