using Greenspot.Configuration;
using Greenspot.Stall.Models.Common;
using Greenspot.Stall.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Newtonsoft.Json;

namespace Greenspot.Stall.Models
{
    public class Accountant
    {
        //PxPay
        #region PxPay
        private static PxPay _pxPay = new PxPay(GreenspotConfiguration.AccessAccounts["pxpay"].Id,
                                                GreenspotConfiguration.AccessAccounts["pxpay"].Secret);

        private static string GeneratePxPayRequestURL(decimal amount, string reference, string transactionID,
            string txnData, string urlFail, string urlSuccess, bool enableAddBillCard = true, string dpsBillingId = null)
        {
            RequestInput reqInput = new RequestInput();

            //if (!string.IsNullOrEmpty(dpsBillingId))
            //{
            //    reqInput.DpsBillingId = dpsBillingId;
            //}
            //else if (enableAddBillCard)
            //{
            //    reqInput.EnableAddBillCard = "1";
            //}

            reqInput.AmountInput = amount.ToString("F2");
            reqInput.CurrencyInput = CurrencyType.NZD;
            reqInput.MerchantReference = reference;
            reqInput.TxnId = transactionID;
            reqInput.TxnData1 = txnData;
            reqInput.TxnType = "Purchase";
            reqInput.UrlFail = urlFail;
            reqInput.UrlSuccess = urlSuccess;

            RequestOutput output = _pxPay.GenerateRequest(reqInput);
            if (output.valid == "1" && output.Url != null)
            {
                // Redirect user to payment page
                return output.Url;
            }
            else
            {
                return null;
            }
        }

        private static string GeneratePxPayRequestURL(decimal amount, string reference, string transactionID,
                                string urlFail, string urlSuccess, bool enableAddBillCard = true, string dpsBillingId = null)
        {
            return GeneratePxPayRequestURL(amount, reference, transactionID, null, urlFail, urlSuccess, enableAddBillCard, dpsBillingId);
        }

        #endregion
        public static string GeneratePayURL(Payment payment, string urlFail, string urlSuccess)
        {
            string payUrl = null;
            string refStr = string.Format("Joodoon {0}", payment.Id);
            string tranIdStr = string.Format("{0}-{1:fff}", payment.Id, DateTime.Now);
            payUrl = GeneratePxPayRequestURL(payment.Amount, refStr, tranIdStr, payment.OrderIds, urlFail, urlSuccess);
            return payUrl;
        }


        /// <summary>
        /// verify px payment match transaction
        /// if pay success set transaction as paid
        /// </summary>
        /// <param name="resultId"></param>
        /// <param name="isSuccess"></param>
        /// <returns></returns>
        public static bool VerifyPxPayPayment(string resultId, bool isSuccess, out int outPaymentId)
        {
            outPaymentId = 0;
            try
            {
                //check response
                StallApplication.BizInfoFormat("[ACCOUNTANT-PXPAY]start to get pxpay payment result={0}", resultId);
                ResponseOutput output = _pxPay.ProcessResponse(resultId);
                if (output == null)
                {
                    StallApplication.BizError("[ACCOUNTANT-PXPAY]can not get pxpay payment result - resposne is null");
                    return false;
                }
                if (!(isSuccess ? "1" : "0").Equals(output.Success))
                {
                    StallApplication.BizErrorFormat("[ACCOUNTANT-PXPAY]payment result not match except {0} - actual {1}", output.Success, isSuccess);
                    StallApplication.BizErrorFormat("[ACCOUNTANT-PXPAY]{0}", output);
                    return false;
                }

                //set payment
                int paymentId = int.Parse(output.TxnId.Split('-')[0]);
                decimal amount = decimal.Parse(output.AmountSettlement);
                using (StallEntities db = new StallEntities())
                {
                    Payment payment = db.Payments.FirstOrDefault(o => o.Id == paymentId);
                    if (payment == null)
                    {
                        StallApplication.BizErrorFormat("[ACCOUNTANT-PXPAY]payment {0} can not mactch payment {1}", resultId, outPaymentId);
                        StallApplication.BizErrorFormat("[ACCOUNTANT-PXPAY]{0}", output);
                        return false;
                    }

                    payment.ResponseTime = DateTime.Now;
                    outPaymentId = paymentId;
                    bool result = true;
                    if (!isSuccess)
                    {
                        //pay fail
                        StallApplication.BizErrorFormat("[ACCOUNT-PAPAY]pay failed payment id={0}", payment.Id);
                    }
                    else
                    {
                        //pay success
                        if (payment.Amount != amount)
                        {
                            StallApplication.BizErrorFormat("[ACCOUNT-PAPAY]pxpay amount {0} <> transaction amount {1}", amount, payment.Amount);
                            result = false;
                        }
                        else
                        {
                            payment.HasPaid = true;
                        }

                    }
                    payment.PxPayResponse = output.ToString();
                    db.SaveChanges();
                    return result;
                }
            }
            catch (Exception ex)
            {
                StallApplication.SysError("can not get pxpay payment result", ex);
                return false;
            }
        }
    }
}