
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace EcommerceShop.DAL
{

using System;
    using System.Collections.Generic;
    
public partial class Tbl_Members
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public Tbl_Members()
    {

        this.Tbl_MemberInfo = new HashSet<Tbl_MemberInfo>();

        this.Tbl_ShippingDetails = new HashSet<Tbl_ShippingDetails>();

        this.Tbl_Product = new HashSet<Tbl_Product>();

        this.Tbl_Category = new HashSet<Tbl_Category>();

        this.Tbl_Brand = new HashSet<Tbl_Brand>();

    }


    public int id { get; set; }

    public string userId { get; set; }

    public Nullable<bool> IsActive { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }

    public Nullable<int> ConfirmPass { get; set; }

    public string EmailId { get; set; }

    public Nullable<bool> IsDelete { get; set; }

    public Nullable<System.DateTime> CreatedOn { get; set; }

    public Nullable<System.DateTime> ModifiedOn { get; set; }

    public Nullable<int> roleId { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Tbl_MemberInfo> Tbl_MemberInfo { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Tbl_ShippingDetails> Tbl_ShippingDetails { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Tbl_Product> Tbl_Product { get; set; }

    public virtual Tbl_Members Tbl_Members1 { get; set; }

    public virtual Tbl_Members Tbl_Members2 { get; set; }

    public virtual Tbl_Roles Tbl_Roles { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Tbl_Category> Tbl_Category { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Tbl_Brand> Tbl_Brand { get; set; }

}

}
