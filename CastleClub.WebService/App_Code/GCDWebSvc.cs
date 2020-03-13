using CastleClub.BusinessLogic.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Services;

/// <summary>
/// Summary description for GCDWebSvc
/// </summary>
[WebService(Namespace = "castleclub/GCDSvc/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[ToolboxItem(false)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class GCDWebSvc : System.Web.Services.WebService
{
    [WebMethod(Description = "Create customer")]
    public string CreateCustomer(string customerData, bool sendEmail)
    {
        return GCDWebSvcImpl.CreateCustomer(customerData, sendEmail);
    }

    [WebMethod(Description = "Cancel Customer")]
    public bool CancelCustomer(string CustomerData)
    {
        return GCDWebSvcImpl.CancelCustomer(CustomerData);
    }

    [WebMethod(Description = "Updates User Password")]
    public string UpdatePassword(string PasswordData)
    {
        return GCDWebSvcImpl.UpdatePassword(PasswordData);
    }

    [WebMethod(Description = "Resets GOLDCLUB User Password")]
    public string ResetPassword(string ResetData, bool SendMail)
    {
        return ResetPasswordByGroup(ResetData, "1181", SendMail);
    }

    [WebMethod(Description = "Resets User Password")]
    public string ResetPasswordByGroup(string ResetData, string GroupID, bool SendMail)
    {
        return GCDWebSvcImpl.ResetPasswordByGroup(ResetData, GroupID, SendMail);
    }

    [WebMethod(Description = "Updates Customer Info")]
    public string UpdateCustomer(string CustomerData, bool sendEmail)
    {
        return GCDWebSvcImpl.UpdateCustomer(CustomerData, sendEmail);
    }

    [WebMethod(Description = "Validates GOLDCLUB User, returns Valid User Info or Error")]
    public string LogonSecure(string LogonData)
    {
        return LogonSecureByGroup(LogonData, "1181");
    }

    [WebMethod(Description = "Validates User, returns Valid User Info or Error")]
    public string LogonSecureByGroup(string LogonData, string GroupID)
    {
        return GCDWebSvcImpl.LogonSecureByGroup(LogonData, GroupID);
    }
}
