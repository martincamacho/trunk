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
    
    public partial class Visit
    {
        public int Id { get; set; }
        public int SiteId { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public string IPAddress { get; set; }
        public string UserAgent { get; set; }
        public System.DateTime CreatedAt { get; set; }
    
        public virtual Customer Customer { get; set; }
        public virtual Site Site { get; set; }
    }
}
