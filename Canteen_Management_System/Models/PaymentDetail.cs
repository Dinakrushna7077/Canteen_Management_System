//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Canteen_Management_System.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class PaymentDetail
    {
        public string Alias { get; set; }
        public Nullable<double> Total_Amt { get; set; }
        public Nullable<double> Paid_Amt { get; set; }
        public System.DateTime Date { get; set; }
        public string Received_By { get; set; }
        public string Payment_Mode { get; set; }
        public Nullable<double> Total_Price { get; set; }
        public int Id { get; set; }
    }
}
