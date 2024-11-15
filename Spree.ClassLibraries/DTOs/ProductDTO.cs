using Spree.Libraries.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spree.Libraries.DTOs
{
    public class ProductDTO : Product
    {
        [DisplayName("S/N")]
        public int SerialNumber { get; set; }
    }
}
