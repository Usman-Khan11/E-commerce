using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shop_4_8_21.Models
{
    public class Cart
    {
        public List<tbl_product> MyCart { get; set; }
        public List<int>  ProductId { get; set; }
    }

    public class Favourite
    {
        public List<tbl_product> MyFav { get; set; }
        public List<int> ProductId { get; set; }
    }

}