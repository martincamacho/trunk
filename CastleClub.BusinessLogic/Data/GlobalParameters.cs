using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CastleClub.BusinessLogic.Data;
using CastleClub.BusinessLogic.Utils;
using CastleClub.DataTypes;
using CastleClub.DataTypes.Enums;

namespace CastleClub.BusinessLogic.Data
{
    public static class GlobalParameters
    {
        #region Backend
        public static int PageSize
        {
            get
            {
                return int.Parse(System.Configuration.ConfigurationManager.AppSettings["PageSize"]);
            }
        }
        public static string ExcelTemplatePath
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["ExcelTemplatePath"];
            }
        }
        public static string ExcelOutPath
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["ExcelOutPath"];
            }
        }
        public static int FailCount
        {
            get
            {
                return int.Parse(System.Configuration.ConfigurationManager.AppSettings["FailCount"]);
            }
        }
        public static bool ProcessInvoice
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["ProcessInvoice"].ToLower() == "true";
            }
        }
        public static string EmailAccount
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["EmailAccount"];
            }
        }
        public static string Smtp
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["Smtp"];
            }
        }
        public static string EmailPassword
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["EmailPassword"];
            }
        }
        public static int DeployYear
        {
            get
            {
                return int.Parse(System.Configuration.ConfigurationManager.AppSettings["DeployYear"]);
            }
        }
        public static int DeployMonth
        {
            get
            {
                return int.Parse(System.Configuration.ConfigurationManager.AppSettings["DeployMonth"]);
            }
        }
        public static int DeployDay
        {
            get
            {
                return int.Parse(System.Configuration.ConfigurationManager.AppSettings["DeployDay"]);
            }
        }
        public static bool TryChargeAgain
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["tryChargeAgain"].ToLower()=="true";
            }
        }
        public static string Encrypt
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["public static Encrypt"];
            }
        }
        public static string Certificate
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["Certificate"];
            }
        }
        public static string LogPath
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["LogPath"];
            }
        }
        public static bool EnableChangeActiveOffer
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["EnableChangeActiveOffer"].ToLower()=="true";
            }
        }
        public static bool Syncronizate
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["Syncronizate"].ToLower()=="true";
            }
        }
        public static bool BillingReport
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["BillingReport"].ToLower()=="true";
            }
        }
        public static bool AuthorizeTransactionCompare
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["AuthorizeTransactionCompare"].ToLower()=="true";
            }
        }
        public static bool TestSendEmail
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["TestSendEmail"].ToLower()=="true";
            }
        }
        public static bool ActiveCustomerReport
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["ActiveCustomerReport"].ToLower()=="true";
            }
        }
        public static bool Create
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["Create"].ToLower() == "true";
            }
        }
        public static bool Procces
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["Procces"].ToLower() == "true";
            }
        }
        public static int YearMin
        {
            get
            {
                return int.Parse(System.Configuration.ConfigurationManager.AppSettings["YearMin"]);
            }
        }
        public static bool MembershipAgeCountRange
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["MembershipAgeCount"].ToLower() == "true";
            }
        }
        public static bool ReportSiteCreditCards
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["ReportSiteCreditCards"].ToLower() == "true";
            }
        }
        #endregion

        #region Frontend
        public static string AccessDActivationURL
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["AccessDActivationURL"];
            }
        }
        public static string PublicEncrypt
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["PublicEncrypt"];
            }
        }
        public static bool EmailWelcome
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["EmailWelcome"].ToLower()=="true";
            }
        }

        public static string AMTActivationURL
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["AMTActivationURL"];
            }
        }

        public static string AMTAccessToken
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["AMTAccessToken"];
            }
        }

        public static bool AMTNewPlatform
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["AMTNewPlatform"].ToLower() == "true";
            }
        }

        public static int DebtorsToCancel
        {
            get
            {
                return int.Parse( System.Configuration.ConfigurationManager.AppSettings["DebtorsToCancel"]);
            }
        }

        public static int CapEmail
        {
            get
            {
                using (CastleClubEntities entities = new CastleClubEntities())
                {
                    Parameter parameter = entities.Parameters.FirstOrDefault();

                    return (int)parameter.CapEmail;
                } 
                //return int.Parse(System.Configuration.ConfigurationManager.AppSettings["CapEmail"]);
            }
        }

        #endregion

        public static string UrlResetPassword { get { return System.Configuration.ConfigurationManager.AppSettings["UrlResetPassword"]; } }
    }
}
