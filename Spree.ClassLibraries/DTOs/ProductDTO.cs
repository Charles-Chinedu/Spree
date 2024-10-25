using Spree.Libraries.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spree.Libraries.DTOs
{
    public class ProductDTO : Product
    {
        public int SerialNumber { get; set; }
    }
}
