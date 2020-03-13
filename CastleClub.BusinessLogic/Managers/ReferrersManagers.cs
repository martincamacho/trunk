using CastleClub.BusinessLogic.Data;
using CastleClub.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.BusinessLogic.Managers
{
    public class ReferrersManagers
    {
        /// <summary>
        /// Get a specific referrer based in identifier string.
        /// </summary>
        /// <param name="siteIdentifier">Idientifier string.</param>
        /// <returns>Return the referrer.</returns>
        public static Referrer GetReferrer(string siteIdentifier)
        {
            using(Data.CastleClubEntities entities= new CastleClubEntities())
            {
                var referrer = entities.Referrers.FirstOrDefault(x => x.SiteIdentifier.ToLower() == siteIdentifier.ToLower());
                if (referrer==null)
                {
                    throw new ExecutionEngineException();
                }
                return referrer;
            }
        }

        public static Referrer GetReferrerByName(string Name)
        {
            using (Data.CastleClubEntities entities = new CastleClubEntities())
            {
                var referrer = entities.Referrers.FirstOrDefault(x => x.Name.ToLower() == Name.ToLower());
                if (referrer == null)
                {
                    throw new ExecutionEngineException();
                }
                return referrer;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="referrerID">0 for all referrers</param>
        /// <param name="onlyActive">Only active customer</param>
        /// <returns>Customer list of referrer</returns>
        public static List<CustomerDT> GetCustomers(int referrerID, bool onlyActive)
        {
            using (CastleClubEntities entitie = new CastleClubEntities())
            {
                List<CustomerDT> response = new List<CustomerDT>();
                List<Customer> customerList = referrerID == 0 ?
                    entitie.Customers.Where(x => ((!onlyActive) || (x.CancelledDate == null))).ToList()
                    : entitie.Referrers.FirstOrDefault(x => x.Id==referrerID).Customers.Where(x => ((!onlyActive) || (x.CancelledDate == null))).ToList();

                foreach (var customer in customerList)
                {
                    response.Add(new CustomerDT()
                        {
                            Address=customer.Address,
                            AuthorizeProfileId=customer.AuthorizeProfileId,
                            BadLoginCount=customer.BadLoginCount,
                            CancelledDate=customer.CancelledDate,
                            City=customer.City,
                            CreatedAt=customer.CreatedAt,
                            Email=customer.Email,
                            FirstName=customer.FirstName,
                            Id=customer.Id,
                            LastBillDate=customer.LastBillDate.HasValue ? customer.LastBillDate.Value : new DateTime(),
                            LastName=customer.LastName,
                            NcId=customer.NcId,
                            NextBillDate=customer.NextBillDate,
                            Phone=customer.Phone,
                            Referrer= customer.Referrer!=null ? customer.Referrer.Name : string.Empty,
                            Refunded=customer.Refunded,
                            Site=customer.Site.Name,
                            SiteId=customer.SiteId,
                            StateId=customer.StateId,
                            Status=customer.Status,
                            ZipCode=customer.ZipCode,
                            EmailForm=customer.EmailForm
                        });
                }

                return response;
            }
        }

        /// <summary>
        /// Return the customer's referrer.
        /// </summary>
        /// <param name="customerID">Customer Id.</param>
        /// <returns>Return referrer.</returns>
        public static ReferrerDT GetReferrerCustomer(int customerID)
        {
            using (CastleClubEntities entities= new CastleClubEntities())
            {
                return entities.Customers.FirstOrDefault(x => x.Id == customerID).Referrer.GetDT();
            }
        }
    }
}
