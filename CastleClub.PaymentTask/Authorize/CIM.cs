using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CastleClub.Tasks.PaymentTask;
using System.Net;

namespace CastleClub.Tasks.PaymentTask.Authorize
{
    public class CIM
    {
        public static bool CreateCustomerProfileTransactionAuthCapture(Site site, Invoice invoice, Customer customer, CreditCard creditCard, out long transactionId, out string message, out bool cancel)
        {
            cancel = false;
            MerchantAuthenticationType merchantAT = new MerchantAuthenticationType();
            merchantAT.name = site.AuthorizeLoginId;
            merchantAT.transactionKey = site.AuthorizeTransactionKey;

            ProfileTransactionType profileTT = new ProfileTransactionType();
            ProfileTransAuthCaptureType profileTACT = new ProfileTransAuthCaptureType();

            OrderExType orderET = new OrderExType();
            orderET.invoiceNumber = invoice.Id.ToString();

            profileTACT.amount = invoice.Amount;
            profileTACT.customerProfileId = customer.AuthorizeProfileId;
            profileTACT.customerPaymentProfileId = creditCard.AuthorizePaymentProfileId;
            profileTACT.order = orderET;

            profileTT.Item = profileTACT;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;// comparable to modern browsers
            ServiceSoapClient client = new ServiceSoapClient();
            CreateCustomerProfileTransactionResponseType resp = client.CreateCustomerProfileTransaction(merchantAT, profileTT, String.Empty);

            if (resp.directResponse != null)
            {
                PaymentGatewayResponse paymentGatewayResponse = new PaymentGatewayResponse(resp.directResponse);
                transactionId = paymentGatewayResponse.TransactionId;
                message = paymentGatewayResponse.ResponseReasonText;

                if (!string.IsNullOrEmpty(paymentGatewayResponse.ResponseReasonText))
                {
                    cancel = paymentGatewayResponse.ResponseReasonText.ToLower().Contains("the credit card number is invalid")
                    || paymentGatewayResponse.ResponseReasonText.ToLower().Contains("the credit card expiration date is invalid")
                    || paymentGatewayResponse.ResponseReasonText.ToLower().Contains("the credit card has expired");
                }
                
            }
            else
            {
                transactionId = 0;
                if (resp.messages.Length > 0)
                {
                    message = resp.messages[0].text;
                }
                else
                {
                    message = "";
                }
            }

            return resp.resultCode == MessageTypeEnum.Ok;
        }
    }
}
