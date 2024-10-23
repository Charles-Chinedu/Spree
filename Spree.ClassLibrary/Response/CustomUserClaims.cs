using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spree.Library.Response
{
    public record CustomUserClaims(string Name = null!, string Email = null!, string Role = null!);
}
