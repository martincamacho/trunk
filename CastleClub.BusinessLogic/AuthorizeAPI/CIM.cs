using CastleClub.BusinessLogic.AuthorizeService;
using CastleClub.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.BusinessLogic.AuthorizeAPI
{
    public class CIM
    {
        public static bool CreateCustomerProfile(SiteDT site, CustomerDT customer, CreditCardDT cc, out long customerProfileId, out long customerPaymentProfileId)
        {

           // if (1 == 0) { 
                MerchantAuthenticationType merchantAT = new MerchantAuthenticationType();
                merchantAT.name = site.AuthorizeLoginId;
                merchantAT.transactionKey = site.AuthorizeTransactionKey;

                CustomerProfileType customerPT = new CustomerProfileType();
                customerPT.merchantCustomerId = customer.Id.ToString();
                customerPT.email = customer.Email;
                customerPT.paymentProfiles = new CustomerPaymentProfileType[1];

                CustomerPaymentProfileType customerPPT = new CustomerPaymentProfileType();
                customerPPT.customerType = CustomerTypeEnum.individual;
                customerPPT.customerTypeSpecified = true;
                customerPPT.billTo = new CustomerAddressType()
                {
                    firstName= customer.FirstName,
                    lastName= customer.LastName,
                    address= customer.Address,
                    city=customer.City,
                    country="USA",
                    state=customer.StateId,
                    zip=customer.ZipCode
                };

                CreditCardType ccT = new CreditCardType();
                ccT.cardNumber = cc.CardNumber;
                ccT.expirationDate = cc.ExpDate;
                ccT.cardCode = cc.CVV;         

                PaymentType payment = new PaymentType();
                payment.Item = ccT;

                customerPPT.payment = payment;
                customerPT.paymentProfiles[0] = customerPPT;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServiceSoapClient client = new ServiceSoapClient();
                CreateCustomerProfileResponseType resp = client.CreateCustomerProfile(merchantAT, customerPT, ValidationModeEnum.testMode);

                customerProfileId = resp.customerProfileId;
                customerPaymentProfileId = resp.customerPaymentProfileIdList.Length == 1 ? resp.customerPaymentProfileIdList[0] : 0;
                if (resp.resultCode != MessageTypeEnum.Ok)
                {

                    string errorValidate = resp.validationDirectResponseList != null && resp.validationDirectResponseList.Count() > 0 ? resp.validationDirectResponseList.ToList().Aggregate((a, b) => a + " || " + b) : string.Empty;
                    string error = resp.messages!=null && resp.messages.Count()>0 ? resp.messages.ToList().Select(a=>a.text).Aggregate((a, b) => a + " || " + b) : string.Empty;
                    LoggingUtilities.Logger.LogEntry("Resp Error code:" + resp.resultCode.ToString() + ". Errors: "+ error + "Error V: "+errorValidate);
                }

                return resp.resultCode == MessageTypeEnum.Ok;
                
           
           /* }
      
            else
            {
               customerProfileId = 1;
                customerPaymentProfileId = 1;
                return true;
            }*/
        }

        public static bool CreateCustomerProfileTransactionAuthCapture(SiteDT site, InvoiceDT invoice, CustomerDT customer, CreditCardDT creditCard, out long transactionId, out string message, out string code)
        {
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
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServiceSoapClient client = new ServiceSoapClient();
            CreateCustomerProfileTransactionResponseType resp = client.CreateCustomerProfileTransaction(merchantAT, profileTT, String.Empty);
            code = string.Empty;

            if (resp.directResponse != null)
            {
                PaymentGatewayResponse paymentGatewayResponse = new PaymentGatewayResponse(resp.directResponse);
                transactionId = paymentGatewayResponse.TransactionId;
                message = paymentGatewayResponse.ResponseReasonText;

                code = resp.messages!=null && resp.messages.Count()>0 ? resp.messages.Select(x => x.code + " - " + x.text).Aggregate((a, b) => a + "; " + b) : string.Empty;
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

        public static bool CreateCustomerProfileTransactionRefund(SiteDT site, InvoiceDT invoice, TransactionDT transaction, CustomerDT customer, CreditCardDT creditCard, out long transactionId, out string message)
        {
            //This datetime is when system change.
            int deployYear = CastleClub.BusinessLogic.Data.GlobalParameters.DeployYear;
            int deployMonth = CastleClub.BusinessLogic.Data.GlobalParameters.DeployMonth;
            int deployDay = CastleClub.BusinessLogic.Data.GlobalParameters.DeployDay;

            if (invoice.BilledDate >= new DateTime(deployYear, deployMonth, deployDay))
            {
                MerchantAuthenticationType merchantAT = new MerchantAuthenticationType();
                merchantAT.name = site.AuthorizeLoginId;
                merchantAT.transactionKey = site.AuthorizeTransactionKey;

                ProfileTransactionType profileTT = new ProfileTransactionType();
                ProfileTransRefundType profileTRT = new ProfileTransRefundType();

                OrderExType orderET = new OrderExType();
                orderET.invoiceNumber = invoice.Id.ToString();

                profileTRT.amount = invoice.Amount;
                profileTRT.transId = transaction.AuthorizeTransactionId.ToString();
                profileTRT.customerProfileId = customer.AuthorizeProfileId;
                profileTRT.customerProfileIdSpecified = true;
                profileTRT.customerPaymentProfileId = creditCard.AuthorizePaymentProfileId;
                profileTRT.customerPaymentProfileIdSpecified = true;
                profileTRT.order = orderET;


                profileTT.Item = profileTRT;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServiceSoapClient client = new ServiceSoapClient();
                CreateCustomerProfileTransactionResponseType resp = client.CreateCustomerProfileTransaction(merchantAT, profileTT, String.Empty);

                if (resp.directResponse != null)
                {
                    PaymentGatewayResponse paymentGatewayResponse = new PaymentGatewayResponse(resp.directResponse);
                    transactionId = paymentGatewayResponse.TransactionId;
                    message = paymentGatewayResponse.ResponseReasonText;
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
            else
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                using (ServiceSoapClient client = new ServiceSoapClient())
                {
                    MerchantAuthenticationType merchant = new MerchantAuthenticationType()
                    {
                        name = site.AuthorizeLoginId,
                        transactionKey = site.AuthorizeTransactionKey
                    };

                    ProfileTransactionType profileTT = new ProfileTransactionType();
                    ProfileTransRefundType profileTRT = new ProfileTransRefundType();

                    profileTRT.amount = invoice.Amount;
                    profileTRT.transId = transaction.AuthorizeTransactionId.ToString();
                    profileTRT.creditCardNumberMasked = "XXXX" + creditCard.LastFourDigit;

                    profileTT.Item = profileTRT;

                    var resp = client.CreateCustomerProfileTransaction(merchant, profileTT, string.Empty);
                    if (resp.resultCode== MessageTypeEnum.Ok)
                    {
                        PaymentGatewayResponse paymentGatewayResponse = new PaymentGatewayResponse(resp.directResponse);
                        transactionId = paymentGatewayResponse.TransactionId;
                        message = paymentGatewayResponse.ResponseReasonText;
                    }
                    else
                    {
                        transactionId = 0;
                        message = string.Empty;
                        message = resp.messages != null && resp.messages.Count() > 0 ? resp.messages.Select(a => a.text).Aggregate((a, b) => a + " - " + b) : string.Empty;
                    }

                    return (resp.resultCode == MessageTypeEnum.Ok);
                }
            }
        }

        /// <summary>
        /// Get balance for specific transactions.
        /// </summary>
        /// <param name="transactions">Transactions with its sites.</param>
        /// <returns>In index 0 is for charge, and in index 1 is for charge fail.</returns>
        public static decimal[] GetTransactionRange(Dictionary<SiteDT, List<TransactionDT>> transactions)
        {
            using (ServiceSoapClient client= new ServiceSoapClient())
            {
                decimal[] resp= new decimal [2];
                resp[0] = 0;
                resp[1] = 0;

                foreach (var site in transactions.Keys)
                {
                    foreach (var transaction in transactions[site])
                    {
                        var response = client.GetTransactionDetails(new MerchantAuthenticationType()
                            {
                                name=site.AuthorizeLoginId,
                                transactionKey=site.AuthorizeTransactionKey
                            }, transaction.AuthorizeTransactionId.ToString());

                        if (response.resultCode== MessageTypeEnum.Ok)
                        {
                            //Code for successfull
                            if (!string.IsNullOrEmpty(response.transaction.transactionStatus) && response.transaction.transactionStatus.ToLower() == "settledSuccessfully".ToLower())
                            {
                                resp[0] = resp[0] + response.transaction.settleAmount;
                            }
                            else
                            {
                                resp[1] = resp[1] + response.transaction.authAmount;
                            }
                        }
                        else
                        {
                            string message = response.messages != null && response.messages.Count() > 0 ? response.messages.Select(a => a.text).Aggregate((a, b) => a + " - " + b) : string.Empty;
                            Utils.EventViewer.Writte("Test authorize", "Compare", "Response is not Ok. ("+transaction.AuthorizeTransactionId.ToString()+")\n"+message, System.Diagnostics.EventLogEntryType.Error);
                        }
                    }
                }

                return resp;
            }
        }

        public static bool UpdateCustomerPaymentProfile(SiteDT site, CustomerDT customer, CreditCardDT creditCard)
        {
            using (ServiceSoapClient client = new ServiceSoapClient())
            {
                var merchant = new MerchantAuthenticationType()
                    {
                        name = site.AuthorizeLoginId,
                        transactionKey = site.AuthorizeTransactionKey
                    };
                var paymentProfileResponse = client.GetCustomerPaymentProfile(merchant, customer.AuthorizeProfileId, creditCard.AuthorizePaymentProfileId);

                if (paymentProfileResponse.resultCode == MessageTypeEnum.Ok)
                {
                    CreditCardType creditCardType = new CreditCardType()
                    {
                        cardCode = string.Empty,
                        cardNumber = creditCard.CardNumber,
                        expirationDate = creditCard.ExpDate
                    };

                    var customerUpdate = new CustomerPaymentProfileExType()
                        {
                            billTo = paymentProfileResponse.paymentProfile.billTo,
                            customerPaymentProfileId = paymentProfileResponse.paymentProfile.customerPaymentProfileId,
                            payment = new PaymentType() { Item = creditCardType }
                        };
                    

                    var updateResponse = client.UpdateCustomerPaymentProfile(merchant, customer.AuthorizeProfileId, null, ValidationModeEnum.none);
                    if (updateResponse.resultCode== MessageTypeEnum.Ok)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public static bool CreateNewCustomerPaymentProfile(SiteDT site, CustomerDT customer, string cardNumber, string expirationDate, out long paymentProfile, out string message)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            using (ServiceSoapClient client= new ServiceSoapClient())
            {
                paymentProfile = 0;
                message = string.Empty;

                var merchant = new MerchantAuthenticationType()
                {
                    name= site.AuthorizeLoginId,
                    transactionKey=site.AuthorizeTransactionKey
                };

                CustomerPaymentProfileType customerPPT = new CustomerPaymentProfileType();
                customerPPT.customerType = CustomerTypeEnum.individual;
                customerPPT.customerTypeSpecified = true;
                customerPPT.billTo = new CustomerAddressType()
                {
                    firstName = customer.FirstName,
                    lastName = customer.LastName,
                    address = customer.Address,
                    city = customer.City,
                    country = "USA",
                    state = customer.StateId,
                    zip = customer.ZipCode
                };

                CreditCardType ccT = new CreditCardType();
                ccT.cardNumber = cardNumber;
                ccT.expirationDate = expirationDate;
                ccT.cardCode = string.Empty;

                PaymentType payment = new PaymentType();
                payment.Item = ccT;

                customerPPT.payment = payment;

                var resp = client.CreateCustomerPaymentProfile(merchant, customer.AuthorizeProfileId, customerPPT, ValidationModeEnum.testMode);

                if (resp.resultCode== MessageTypeEnum.Ok)
                {
                    paymentProfile = resp.customerPaymentProfileId;
                    return true;
                }
                else
                {
                    message = resp.messages.Select(x => x.code + ": " + x.text).Aggregate((a, b) => a + "\n" + b);
                }

                return false;
            }
        }
    }
}
