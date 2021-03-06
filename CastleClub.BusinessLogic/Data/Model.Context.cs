﻿

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace CastleClub.BusinessLogic.Data
{

using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

using System.Data.Entity.Core.Objects;
using System.Linq;


public partial class CastleClubEntities : DbContext
{
    public CastleClubEntities()
        : base("name=CastleClubEntities")
    {

    }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        throw new UnintentionalCodeFirstException();
    }


    public virtual DbSet<Site> Sites { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<CreditCard> CreditCards { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Visit> Visits { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<Referrer> Referrers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<CustomersView> CustomersViews { get; set; }

    public virtual DbSet<InvoicesView> InvoicesViews { get; set; }

    public virtual DbSet<CancellationsView> CancellationsViews { get; set; }

    public virtual DbSet<RefundsView> RefundsViews { get; set; }

    public virtual DbSet<Note> Notes { get; set; }

    public virtual DbSet<NotificationProcess> NotificationProcess { get; set; }

    public virtual DbSet<IISResetLog> IISResetLogs { get; set; }

    public virtual DbSet<Parameter> Parameters { get; set; }


    public virtual ObjectResult<Nullable<int>> VisitsPerSiteDateRange(Nullable<int> siteId, Nullable<System.DateTime> from, Nullable<System.DateTime> to)
    {

        var siteIdParameter = siteId.HasValue ?
            new ObjectParameter("SiteId", siteId) :
            new ObjectParameter("SiteId", typeof(int));


        var fromParameter = from.HasValue ?
            new ObjectParameter("From", from) :
            new ObjectParameter("From", typeof(System.DateTime));


        var toParameter = to.HasValue ?
            new ObjectParameter("To", to) :
            new ObjectParameter("To", typeof(System.DateTime));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("VisitsPerSiteDateRange", siteIdParameter, fromParameter, toParameter);
    }


    public virtual ObjectResult<CostDay_Result> CostDay(Nullable<System.DateTime> from, Nullable<System.DateTime> to, Nullable<int> siteID)
    {

        var fromParameter = from.HasValue ?
            new ObjectParameter("from", from) :
            new ObjectParameter("from", typeof(System.DateTime));


        var toParameter = to.HasValue ?
            new ObjectParameter("to", to) :
            new ObjectParameter("to", typeof(System.DateTime));


        var siteIDParameter = siteID.HasValue ?
            new ObjectParameter("siteID", siteID) :
            new ObjectParameter("siteID", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<CostDay_Result>("CostDay", fromParameter, toParameter, siteIDParameter);
    }


    public virtual ObjectResult<ReportDay_Result> ReportDay(Nullable<System.DateTime> from, Nullable<System.DateTime> to, Nullable<int> siteID)
    {

        var fromParameter = from.HasValue ?
            new ObjectParameter("from", from) :
            new ObjectParameter("from", typeof(System.DateTime));


        var toParameter = to.HasValue ?
            new ObjectParameter("to", to) :
            new ObjectParameter("to", typeof(System.DateTime));


        var siteIDParameter = siteID.HasValue ?
            new ObjectParameter("siteID", siteID) :
            new ObjectParameter("siteID", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ReportDay_Result>("ReportDay", fromParameter, toParameter, siteIDParameter);
    }


    public virtual ObjectResult<Testing2_Result> Testing2(Nullable<System.DateTime> from, Nullable<System.DateTime> to, Nullable<int> siteID)
    {

        var fromParameter = from.HasValue ?
            new ObjectParameter("from", from) :
            new ObjectParameter("from", typeof(System.DateTime));


        var toParameter = to.HasValue ?
            new ObjectParameter("to", to) :
            new ObjectParameter("to", typeof(System.DateTime));


        var siteIDParameter = siteID.HasValue ?
            new ObjectParameter("siteID", siteID) :
            new ObjectParameter("siteID", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Testing2_Result>("Testing2", fromParameter, toParameter, siteIDParameter);
    }


    public virtual ObjectResult<Testing_Result> Testing(Nullable<System.DateTime> from, Nullable<System.DateTime> to, Nullable<int> siteID)
    {

        var fromParameter = from.HasValue ?
            new ObjectParameter("from", from) :
            new ObjectParameter("from", typeof(System.DateTime));


        var toParameter = to.HasValue ?
            new ObjectParameter("to", to) :
            new ObjectParameter("to", typeof(System.DateTime));


        var siteIDParameter = siteID.HasValue ?
            new ObjectParameter("siteID", siteID) :
            new ObjectParameter("siteID", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Testing_Result>("Testing", fromParameter, toParameter, siteIDParameter);
    }


    public virtual ObjectResult<Testing21_Result> Testing21(Nullable<System.DateTime> from, Nullable<System.DateTime> to, Nullable<int> siteID)
    {

        var fromParameter = from.HasValue ?
            new ObjectParameter("from", from) :
            new ObjectParameter("from", typeof(System.DateTime));


        var toParameter = to.HasValue ?
            new ObjectParameter("to", to) :
            new ObjectParameter("to", typeof(System.DateTime));


        var siteIDParameter = siteID.HasValue ?
            new ObjectParameter("siteID", siteID) :
            new ObjectParameter("siteID", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Testing21_Result>("Testing21", fromParameter, toParameter, siteIDParameter);
    }


    public virtual ObjectResult<ReportCostDay_Result> ReportCostDay(Nullable<System.DateTime> from, Nullable<System.DateTime> to, Nullable<int> siteID)
    {

        var fromParameter = from.HasValue ?
            new ObjectParameter("from", from) :
            new ObjectParameter("from", typeof(System.DateTime));


        var toParameter = to.HasValue ?
            new ObjectParameter("to", to) :
            new ObjectParameter("to", typeof(System.DateTime));


        var siteIDParameter = siteID.HasValue ?
            new ObjectParameter("siteID", siteID) :
            new ObjectParameter("siteID", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ReportCostDay_Result>("ReportCostDay", fromParameter, toParameter, siteIDParameter);
    }


    public virtual ObjectResult<ReportRevenueDay_Result> ReportRevenueDay(Nullable<System.DateTime> from, Nullable<System.DateTime> to, Nullable<int> siteID)
    {

        var fromParameter = from.HasValue ?
            new ObjectParameter("from", from) :
            new ObjectParameter("from", typeof(System.DateTime));


        var toParameter = to.HasValue ?
            new ObjectParameter("to", to) :
            new ObjectParameter("to", typeof(System.DateTime));


        var siteIDParameter = siteID.HasValue ?
            new ObjectParameter("siteID", siteID) :
            new ObjectParameter("siteID", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ReportRevenueDay_Result>("ReportRevenueDay", fromParameter, toParameter, siteIDParameter);
    }


    public virtual ObjectResult<ReportTheLast4Days_Result> ReportTheLast4Days(Nullable<System.DateTime> date1, Nullable<System.DateTime> date2, Nullable<System.DateTime> date3, Nullable<System.DateTime> date4, Nullable<int> siteID)
    {

        var date1Parameter = date1.HasValue ?
            new ObjectParameter("date1", date1) :
            new ObjectParameter("date1", typeof(System.DateTime));


        var date2Parameter = date2.HasValue ?
            new ObjectParameter("date2", date2) :
            new ObjectParameter("date2", typeof(System.DateTime));


        var date3Parameter = date3.HasValue ?
            new ObjectParameter("date3", date3) :
            new ObjectParameter("date3", typeof(System.DateTime));


        var date4Parameter = date4.HasValue ?
            new ObjectParameter("date4", date4) :
            new ObjectParameter("date4", typeof(System.DateTime));


        var siteIDParameter = siteID.HasValue ?
            new ObjectParameter("siteID", siteID) :
            new ObjectParameter("siteID", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ReportTheLast4Days_Result>("ReportTheLast4Days", date1Parameter, date2Parameter, date3Parameter, date4Parameter, siteIDParameter);
    }


    public virtual ObjectResult<ReportTheLastFourDaysCost_Result> ReportTheLastFourDaysCost(Nullable<System.DateTime> date1, Nullable<System.DateTime> date2, Nullable<System.DateTime> date3, Nullable<System.DateTime> date4, Nullable<int> siteID)
    {

        var date1Parameter = date1.HasValue ?
            new ObjectParameter("date1", date1) :
            new ObjectParameter("date1", typeof(System.DateTime));


        var date2Parameter = date2.HasValue ?
            new ObjectParameter("date2", date2) :
            new ObjectParameter("date2", typeof(System.DateTime));


        var date3Parameter = date3.HasValue ?
            new ObjectParameter("date3", date3) :
            new ObjectParameter("date3", typeof(System.DateTime));


        var date4Parameter = date4.HasValue ?
            new ObjectParameter("date4", date4) :
            new ObjectParameter("date4", typeof(System.DateTime));


        var siteIDParameter = siteID.HasValue ?
            new ObjectParameter("siteID", siteID) :
            new ObjectParameter("siteID", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ReportTheLastFourDaysCost_Result>("ReportTheLastFourDaysCost", date1Parameter, date2Parameter, date3Parameter, date4Parameter, siteIDParameter);
    }


    public virtual ObjectResult<ReportTheLastFourDaysRevenue_Result> ReportTheLastFourDaysRevenue(Nullable<System.DateTime> date1, Nullable<System.DateTime> date2, Nullable<System.DateTime> date3, Nullable<System.DateTime> date4, Nullable<int> siteID)
    {

        var date1Parameter = date1.HasValue ?
            new ObjectParameter("date1", date1) :
            new ObjectParameter("date1", typeof(System.DateTime));


        var date2Parameter = date2.HasValue ?
            new ObjectParameter("date2", date2) :
            new ObjectParameter("date2", typeof(System.DateTime));


        var date3Parameter = date3.HasValue ?
            new ObjectParameter("date3", date3) :
            new ObjectParameter("date3", typeof(System.DateTime));


        var date4Parameter = date4.HasValue ?
            new ObjectParameter("date4", date4) :
            new ObjectParameter("date4", typeof(System.DateTime));


        var siteIDParameter = siteID.HasValue ?
            new ObjectParameter("siteID", siteID) :
            new ObjectParameter("siteID", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ReportTheLastFourDaysRevenue_Result>("ReportTheLastFourDaysRevenue", date1Parameter, date2Parameter, date3Parameter, date4Parameter, siteIDParameter);
    }


    public virtual ObjectResult<ReportTheLastFourMonthCost_Result> ReportTheLastFourMonthCost(Nullable<System.DateTime> date1, Nullable<System.DateTime> date2, Nullable<System.DateTime> date3, Nullable<System.DateTime> date4, Nullable<System.DateTime> date5, Nullable<System.DateTime> date6, Nullable<System.DateTime> date7, Nullable<System.DateTime> date8, Nullable<int> siteID)
    {

        var date1Parameter = date1.HasValue ?
            new ObjectParameter("date1", date1) :
            new ObjectParameter("date1", typeof(System.DateTime));


        var date2Parameter = date2.HasValue ?
            new ObjectParameter("date2", date2) :
            new ObjectParameter("date2", typeof(System.DateTime));


        var date3Parameter = date3.HasValue ?
            new ObjectParameter("date3", date3) :
            new ObjectParameter("date3", typeof(System.DateTime));


        var date4Parameter = date4.HasValue ?
            new ObjectParameter("date4", date4) :
            new ObjectParameter("date4", typeof(System.DateTime));


        var date5Parameter = date5.HasValue ?
            new ObjectParameter("date5", date5) :
            new ObjectParameter("date5", typeof(System.DateTime));


        var date6Parameter = date6.HasValue ?
            new ObjectParameter("date6", date6) :
            new ObjectParameter("date6", typeof(System.DateTime));


        var date7Parameter = date7.HasValue ?
            new ObjectParameter("date7", date7) :
            new ObjectParameter("date7", typeof(System.DateTime));


        var date8Parameter = date8.HasValue ?
            new ObjectParameter("date8", date8) :
            new ObjectParameter("date8", typeof(System.DateTime));


        var siteIDParameter = siteID.HasValue ?
            new ObjectParameter("siteID", siteID) :
            new ObjectParameter("siteID", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ReportTheLastFourMonthCost_Result>("ReportTheLastFourMonthCost", date1Parameter, date2Parameter, date3Parameter, date4Parameter, date5Parameter, date6Parameter, date7Parameter, date8Parameter, siteIDParameter);
    }


    public virtual ObjectResult<ReportTheLastFourMonthRevenue_Result> ReportTheLastFourMonthRevenue(Nullable<System.DateTime> date1, Nullable<System.DateTime> date2, Nullable<System.DateTime> date3, Nullable<System.DateTime> date4, Nullable<System.DateTime> date5, Nullable<System.DateTime> date6, Nullable<System.DateTime> date7, Nullable<System.DateTime> date8, Nullable<int> siteID)
    {

        var date1Parameter = date1.HasValue ?
            new ObjectParameter("date1", date1) :
            new ObjectParameter("date1", typeof(System.DateTime));


        var date2Parameter = date2.HasValue ?
            new ObjectParameter("date2", date2) :
            new ObjectParameter("date2", typeof(System.DateTime));


        var date3Parameter = date3.HasValue ?
            new ObjectParameter("date3", date3) :
            new ObjectParameter("date3", typeof(System.DateTime));


        var date4Parameter = date4.HasValue ?
            new ObjectParameter("date4", date4) :
            new ObjectParameter("date4", typeof(System.DateTime));


        var date5Parameter = date5.HasValue ?
            new ObjectParameter("date5", date5) :
            new ObjectParameter("date5", typeof(System.DateTime));


        var date6Parameter = date6.HasValue ?
            new ObjectParameter("date6", date6) :
            new ObjectParameter("date6", typeof(System.DateTime));


        var date7Parameter = date7.HasValue ?
            new ObjectParameter("date7", date7) :
            new ObjectParameter("date7", typeof(System.DateTime));


        var date8Parameter = date8.HasValue ?
            new ObjectParameter("date8", date8) :
            new ObjectParameter("date8", typeof(System.DateTime));


        var siteIDParameter = siteID.HasValue ?
            new ObjectParameter("siteID", siteID) :
            new ObjectParameter("siteID", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ReportTheLastFourMonthRevenue_Result>("ReportTheLastFourMonthRevenue", date1Parameter, date2Parameter, date3Parameter, date4Parameter, date5Parameter, date6Parameter, date7Parameter, date8Parameter, siteIDParameter);
    }


    public virtual ObjectResult<ReportTheLastFifteenDaysCost_Result> ReportTheLastFifteenDaysCost(Nullable<System.DateTime> date1, Nullable<System.DateTime> date2, Nullable<int> siteID)
    {

        var date1Parameter = date1.HasValue ?
            new ObjectParameter("date1", date1) :
            new ObjectParameter("date1", typeof(System.DateTime));


        var date2Parameter = date2.HasValue ?
            new ObjectParameter("date2", date2) :
            new ObjectParameter("date2", typeof(System.DateTime));


        var siteIDParameter = siteID.HasValue ?
            new ObjectParameter("siteID", siteID) :
            new ObjectParameter("siteID", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ReportTheLastFifteenDaysCost_Result>("ReportTheLastFifteenDaysCost", date1Parameter, date2Parameter, siteIDParameter);
    }


    public virtual ObjectResult<ReportTheLastFifteenDaysRevenue_Result> ReportTheLastFifteenDaysRevenue(Nullable<System.DateTime> date1, Nullable<System.DateTime> date2, Nullable<int> siteID)
    {

        var date1Parameter = date1.HasValue ?
            new ObjectParameter("date1", date1) :
            new ObjectParameter("date1", typeof(System.DateTime));


        var date2Parameter = date2.HasValue ?
            new ObjectParameter("date2", date2) :
            new ObjectParameter("date2", typeof(System.DateTime));


        var siteIDParameter = siteID.HasValue ?
            new ObjectParameter("siteID", siteID) :
            new ObjectParameter("siteID", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ReportTheLastFifteenDaysRevenue_Result>("ReportTheLastFifteenDaysRevenue", date1Parameter, date2Parameter, siteIDParameter);
    }


    public virtual ObjectResult<ReportBilledPerDays_Result> ReportBilledPerDays(Nullable<System.DateTime> date1, Nullable<System.DateTime> date2, Nullable<int> siteID)
    {

        var date1Parameter = date1.HasValue ?
            new ObjectParameter("date1", date1) :
            new ObjectParameter("date1", typeof(System.DateTime));


        var date2Parameter = date2.HasValue ?
            new ObjectParameter("date2", date2) :
            new ObjectParameter("date2", typeof(System.DateTime));


        var siteIDParameter = siteID.HasValue ?
            new ObjectParameter("siteID", siteID) :
            new ObjectParameter("siteID", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ReportBilledPerDays_Result>("ReportBilledPerDays", date1Parameter, date2Parameter, siteIDParameter);
    }


    public virtual ObjectResult<ReportCancelationsPerDays_Result> ReportCancelationsPerDays(Nullable<System.DateTime> date1, Nullable<System.DateTime> date2, Nullable<int> siteID)
    {

        var date1Parameter = date1.HasValue ?
            new ObjectParameter("date1", date1) :
            new ObjectParameter("date1", typeof(System.DateTime));


        var date2Parameter = date2.HasValue ?
            new ObjectParameter("date2", date2) :
            new ObjectParameter("date2", typeof(System.DateTime));


        var siteIDParameter = siteID.HasValue ?
            new ObjectParameter("siteID", siteID) :
            new ObjectParameter("siteID", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ReportCancelationsPerDays_Result>("ReportCancelationsPerDays", date1Parameter, date2Parameter, siteIDParameter);
    }


    public virtual ObjectResult<ReportsignupsPerDays_Result> ReportsignupsPerDays(Nullable<System.DateTime> date1, Nullable<System.DateTime> date2, Nullable<int> siteID)
    {

        var date1Parameter = date1.HasValue ?
            new ObjectParameter("date1", date1) :
            new ObjectParameter("date1", typeof(System.DateTime));


        var date2Parameter = date2.HasValue ?
            new ObjectParameter("date2", date2) :
            new ObjectParameter("date2", typeof(System.DateTime));


        var siteIDParameter = siteID.HasValue ?
            new ObjectParameter("siteID", siteID) :
            new ObjectParameter("siteID", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ReportsignupsPerDays_Result>("ReportsignupsPerDays", date1Parameter, date2Parameter, siteIDParameter);
    }


    public virtual ObjectResult<ReportVisitsPerDays_Result> ReportVisitsPerDays(Nullable<System.DateTime> date1, Nullable<System.DateTime> date2, Nullable<int> siteID)
    {

        var date1Parameter = date1.HasValue ?
            new ObjectParameter("date1", date1) :
            new ObjectParameter("date1", typeof(System.DateTime));


        var date2Parameter = date2.HasValue ?
            new ObjectParameter("date2", date2) :
            new ObjectParameter("date2", typeof(System.DateTime));


        var siteIDParameter = siteID.HasValue ?
            new ObjectParameter("siteID", siteID) :
            new ObjectParameter("siteID", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ReportVisitsPerDays_Result>("ReportVisitsPerDays", date1Parameter, date2Parameter, siteIDParameter);
    }

}

}

