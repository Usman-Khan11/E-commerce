//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Shop_4_8_21.Models
{
    using System;
    using System.Collections.Generic;
    using System.Web;

    public partial class tbl_product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbl_product()
        {
            this.tbl_rating = new HashSet<tbl_rating>();
        }
    
        public int p_id { get; set; }
        public string p_name { get; set; }
        public string p_discription { get; set; }
        public Nullable<int> p_price { get; set; }
        public Nullable<int> cat_fk { get; set; }
        public string featured { get; set; }
        public string images { get; set; }

        public HttpPostedFileBase ImageFile { get; set; }

        public virtual tbl_category tbl_category { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_rating> tbl_rating { get; set; }
    }
}