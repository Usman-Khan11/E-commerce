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
    
    public partial class tbl_rating
    {
        public int id { get; set; }
        public string comment { get; set; }
        public Nullable<int> rating { get; set; }
        public Nullable<int> user_id_fk { get; set; }
        public Nullable<int> pro_id_fk { get; set; }
    
        public virtual tbl_product tbl_product { get; set; }
        public virtual tbl_user tbl_user { get; set; }
    }
}
