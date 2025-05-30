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
    using System.Web;

    public partial class Customer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Customer()
        {
            this.Amounts = new HashSet<Amount>();
            this.Meals = new HashSet<Meal>();
            this.Attendances = new HashSet<Attendance>();
            this.OnlinePayments = new HashSet<OnlinePayment>();
            this.Monthly_Due = new HashSet<Monthly_Due>();
        }
    
        public string Alias { get; set; }
        public string Name { get; set; }
        public long Mobile { get; set; }
        public string Email { get; set; }
        public string Honours { get; set; }
        public string College_Roll { get; set; }
        public string Address { get; set; }
        public long Guardian_Mobile { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string Meal_Status { get; set; }
        public HttpPostedFileBase File {  get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Amount> Amounts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Meal> Meals { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Attendance> Attendances { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OnlinePayment> OnlinePayments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Monthly_Due> Monthly_Due { get; set; }
    }
}
