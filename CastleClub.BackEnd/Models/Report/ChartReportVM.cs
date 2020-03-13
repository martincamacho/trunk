using CastleClub.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CastleClub.BackEnd.Models
{
    public class ChartReportVM
    {
        public List<SiteDT> Sites { get; set; }

        [Required]
        public int SiteID { get; set; }
    }

    public class ReportData
    {
        public string Text { get; set; }
        public List<string> Colors { get; set; }
        public List<string> Categories { get; set; }
        public List<Data> Series { get; set; }
        public string Id { get; set; }
        public ChartTable Table { get; set; }
        public string Type { get; set; }
        public string YAxisTitle { get; set; }
        public string TooltipValueSuffix { get; set; }
    }

    public class ChartTable
    {
        public List<string> Rows { get; set; }
        public List<string> Cols { get; set; }
        /// <summary>
        /// Index 0: Is Revenue.
        /// Index 1: Is Costo.
        /// </summary>
        public string[][] Data { get; set; }
    }
    
    public class Data
    {
        public string name { get; set; }
        public List<double> data { get; set; }
    }

    public class ReportDataChartJS
    {
        public List<string> Labels { get; set; }
        public List<ReportDataSetChartJS> DataSet { get; set; }
    }

    public class ReportDataSetChartJS
    {
        public string Label { get;set; }
        public string FillColor { get;set;}//
        public string StrokeColor { get;set;}//: "rgba(151,187,205,0.8)",
        public string HighlightFill { get;set;}//: "rgba(151,187,205,0.75)",
        public string HighlightStroke { get;set;}//: "rgba(151,187,205,1)",
        public List<int> Data { get; set; }//: [28, 48, 40, 19, 86, 27, 90]
    }

    public class AuthorizeCompareTransactionVM
    {
        [Required]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
        public DateTime From { get; set; }
        [Required]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
        public DateTime To { get; set; }
        [Required]
        public int SitedID { get; set; }
        public List<SiteDT> Sites { get; set; }
    }

    public class AuthorizeCompareTableRequestVM
    {
        public int WebSite { get; set; }
        public string Date { get; set; }
        public string ViewType { get; set; }
    }
}