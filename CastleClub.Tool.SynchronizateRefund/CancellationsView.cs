//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CastleClub.Tool.SynchronizateRefund
{
    using System;
    using System.Collections.Generic;
    
    public partial class CancellationsView
    {
        public int Id { get; set; }
        public int SiteId { get; set; }
        public int ReferrerId { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public Nullable<System.DateTime> CreatedAt30 { get; set; }
        public Nullable<System.DateTime> CreatedAt120 { get; set; }
        public Nullable<System.DateTime> CreatedAt210 { get; set; }
        public Nullable<System.DateTime> CancelledDate { get; set; }
    }
}