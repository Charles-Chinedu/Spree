using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spree.Libraries.Response
{
    public class CustomResponses
    {
        public record class RegistrationResponse(bool Flag, string Message = null!);

        public record class LoginResponse(bool Flag, string Message = null!, string JWToken = null!);

        public record class ServiceResponse(bool Flag, string Message = null!);

        //public record PagedResult<TResult>(TResult[] Records, int TotalCount);
    }
}
