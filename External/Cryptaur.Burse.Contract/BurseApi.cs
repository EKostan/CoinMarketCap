using System;
using System.Threading.Tasks;
using Cryptaur.Burse.Contract.Model;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Options;
using Nordavind.Infrastructure.Models.Api;

namespace Cryptaur.Burse.Contract
{
    public class BurseApi
    {
        private readonly BurseApiSettings _settings;
        private AccessTokenOutput _currentToken;

        public BurseApi(IOptions<BurseApiSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task<OrderOutput> PlaceOrder(string token, PlaceOrderInput input)
        {
            var response = await _settings.BaseUrl.AppendPathSegment("Market/PlaceOrder")
                .WithOAuthBearerToken(token)
                .PostJsonAsync(input)
                .ReceiveJson<ApiResponse<OrderOutput>>();
            return response.Data;
        }
        
        public async Task<Stat24HResult> GetStat24H(string token, GetStat24HInput input)
        {
            var response = await _settings.BaseUrl.AppendPathSegment("Market/GetStat24H")
                .WithOAuthBearerToken(token)
                .SetQueryParams(input)
                .GetJsonAsync<ApiResponse<Stat24HResult>>();
            return response.Data;
        }
        
        public async Task<Stat24HResult> GetStat24H(GetStat24HInput input)
        {
            await CheckToken();
            return await GetStat24H(_currentToken.AccessToken, input);
        }

        private async Task CheckToken()
        {
            if (_currentToken == null
                || DateTime.UtcNow >= _currentToken.Expires)
            {
                _currentToken = await AccessToken(new AccessTokenInput()
                {
                    Key = _settings.Key
                });
            }
        }

        public async Task<AccessTokenOutput> AccessToken(AccessTokenInput input)
        {
            var response = await _settings.BaseUrl.AppendPathSegment("User/AccessToken")
                .SetQueryParams(input)
                .GetJsonAsync<ApiResponse<AccessTokenOutput>>();
            return response.Data;
        }
    }
}
