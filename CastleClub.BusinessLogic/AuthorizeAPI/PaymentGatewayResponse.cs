using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.BusinessLogic.AuthorizeAPI
{
    public class PaymentGatewayResponse
    {
        #region Keys
        /*
         * Subtract 1 for the array index
         * 
        result.Add(1, "Response Code");
        result.Add(2, "Response Subcode");
        result.Add(3, "Response Reason Code");
        result.Add(4, "Response Reason Text");
        result.Add(5, "Authorization Code");
        result.Add(6, "AVS Response");
        result.Add(7, "Transaction ID");
        result.Add(8, "Invoice Number");
        result.Add(9, "Description");
        result.Add(10, "Amount");
        result.Add(11, "Method");
        result.Add(12, "Transaction Type");
        result.Add(13, "Customer ID");
        result.Add(14, "First Name");
        result.Add(15, "Last Name");
        result.Add(16, "Company");
        result.Add(17, "Address");
        result.Add(18, "City");
        result.Add(19, "State");
        result.Add(20, "ZIP Code");
        result.Add(21, "Country");
        result.Add(22, "Phone");
        result.Add(23, "Fax");
        result.Add(24, "Email Address");
        result.Add(25, "Ship To First Name");
        result.Add(26, "Ship To Last Name");
        result.Add(27, "Ship To Company");
        result.Add(28, "Ship To Address");
        result.Add(29, "Ship To City");
        result.Add(30, "Ship To State");
        result.Add(31, "Ship To ZIP Code");
        result.Add(32, "Ship To Country");
        result.Add(33, "Tax");
        result.Add(34, "Duty");
        result.Add(35, "Freight");
        result.Add(36, "Tax Exempt");
        result.Add(37, "Purchase Order Number");
        result.Add(38, "MD5 Hash");
        result.Add(39, "Card Code Response");
        result.Add(40, "Cardholder Authentication Verification Response");
        result.Add(41, "Account Number");
        result.Add(42, "Card Type");
        result.Add(43, "Split Tender ID");
        result.Add(44, "Requested Amount");
        result.Add(45, "Balance On Card");
        */
        #endregion

        private string[] RawResponse = new string[0];

        public PaymentGatewayResponse()
        {
        }

        public PaymentGatewayResponse(string rawResponse)
        {
            if (rawResponse != null)
            {
                RawResponse = rawResponse.Split(',');
            }
        }

        #region Parsers

        private int ParseInt(int index)
        {
            int result = 0;
            if (RawResponse.Length > index)
                int.TryParse(RawResponse[index].ToString(), out result);
            return result;
        }

        private decimal ParseDecimal(int index)
        {
            decimal result = 0;
            if (RawResponse.Length > index)
                decimal.TryParse(RawResponse[index].ToString(), out result);
            return result;
        }

        private long ParseLong(int index)
        {
            long result = 0;
            if (RawResponse.Length > index)
                long.TryParse(RawResponse[index].ToString(), out result);
            return result;
        }

        private string ParseResponse(int index)
        {
            var result = "";
            if (RawResponse.Length > index)
            {
                result = RawResponse[index].ToString();
            }
            return result;
        }

        #endregion

        public long TransactionId
        {
            get
            {
                return ParseLong(6);
            }
        }

        public string ResponseReasonText
        {
            get
            {
                return ParseResponse(3);
            }
        }
    }
}
