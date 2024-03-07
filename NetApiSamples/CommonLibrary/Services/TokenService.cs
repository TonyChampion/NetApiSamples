using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Services
{
    public class TokenService : ITokenService
    {
        private Dictionary<string, string> _tokens = new Dictionary<string, string>();

        public bool AddToken(string id, string token)
        {
            if(!_tokens.ContainsKey(id))
            {
                _tokens.Add(id, token);
            }

            return true;
        }

        public bool ClearAllTokens()
        {
            _tokens.Clear();

            return true;
        }

        public bool IsValid(string id)
        {
            return _tokens.ContainsKey(id);
        }
    }
}
