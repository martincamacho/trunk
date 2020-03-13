using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.DataTypes
{
    public class ReferrerInfoDT
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int TotalVisits { get; set; }

        public int TotalSignups { get; set; }

        public double SignupPercentage { get; set; }

        public int FreeTrial { get; set; }

        public int Active { get; set; }

        public int Cancels { get; set; }

        public int Refunds { get; set; }

        public double CancellationPercentage { get; set; }

        public double CancellationsRefundedPercentage { get; set; }

        public double RefundPercentage { get; set; }

        public double RevenueAmount { get; set; }

        public double RefundAmount { get; set; }

        public double BalanceAmount { get; set; }

        public int SignupsToday { get; set; }

        public int SignupsYesterday { get; set; }

        public int SignupsThisWeek	 { get; set; }

        public int SignupsThisMonth { get; set; }

        public int SignupsThisYear { get; set; }

        public int VisitsToday { get; set; }

        public int VisitsYesterday { get; set; }

        public int VisitsThisWeek { get; set; }

        public int VisitsThisMonth { get; set; }

        public int VisitsThisYear { get; set; }

        public int BilledToday { get; set; }

        public decimal BilledTodayAmount { get; set; }

        public int BilledYesterday { get; set; }

        public decimal BilledYesterdayAmount { get; set; }

        public int BilledThisWeek { get; set; }

        public decimal BilledThisWeekAmount { get; set; }

        public int BilledThisMonth { get; set; }

        public decimal BilledThisMonthAmount { get; set; }

        public int BilledThisYear { get; set; }

        public decimal BilledThisYearAmount { get; set; }

        public int BilledTotal { get; set; }

        public int Cancellations_0_30 { get; set; }

        public decimal CancellationsPercentage_0_30 { get; set; }

        public int Cancellations_30_120 { get; set; }

        public double CancellationsPercentage_30_120 { get; set; }

        public int Cancellations_120_210 { get; set; }

        public double CancellationsPercentage_120_210 { get; set; }

        public int Cancellations_210 { get; set; }

        public double CancellationsPercentage_210 { get; set; }

        public int Refunds_30_120 { get; set; }

        public decimal RefundsAmount_30_120 { get; set; }

        public double RefundsPercentage_30_120 { get; set; }

        public int Refunds_120_210 { get; set; }

        public decimal RefundsAmount_120_210 { get; set; }

        public double RefundsPercentage_120_210 { get; set; }

        public int Refunds_210 { get; set; }

        public decimal RefundsAmount_210 { get; set; }

        public double RefundsPercentage_210 { get; set; }
    }
}
