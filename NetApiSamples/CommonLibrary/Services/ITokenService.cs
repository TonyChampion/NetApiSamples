using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Services
{
    public interface ITokenService
    {
        bool AddToken(string id, string token);
        bool ClearAllTokens();
        bool IsValid(string id);
    }
}
