using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Tangy_Common;
using Tangy_Models;
using TangyWeb_Client.Service.IService;

namespace TangyWeb_Client.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _client;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authStateProvider;

        public AuthenticationService(HttpClient client, ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider)
        {
            _client = client;
            _localStorage = localStorage;
            _authStateProvider = authStateProvider;
        }
        public async Task<SignInResponseDTO> Login(SignInRequestDTO signIn)
        {
            var content = JsonConvert.SerializeObject(signIn);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/account/signin", bodyContent);

            // Convert response into readabe 
            var contentTemp = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<SignInResponseDTO>(contentTemp);

            if(response.IsSuccessStatusCode)
            {
                // This token returned from API is the aunthentication needed
                await _localStorage.SetItemAsync(SD.Local_Token, result.Token);
                await _localStorage.SetItemAsync(SD.UserDetails, result.UserDTO);
                ((AuthStateProvider)_authStateProvider).NotifyUserLoggedIn(result.Token);
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.Token);

                return new SignInResponseDTO()
                {
                    IsAuthSuccessful = true
                };
            }
            else
            {
                return result;
            }

        }
         
        public async Task Logout()
        {
            // Clear the token
            await _localStorage.RemoveItemAsync(SD.Local_Token);
            await _localStorage.RemoveItemAsync(SD.UserDetails);
            ((AuthStateProvider)_authStateProvider).NotifyUserLogout();
            _client.DefaultRequestHeaders.Authorization = null;
        }

        public async Task<SignUpResponseDTO> RegisterUser(SignUpRequestDTO singUp)
        {
            var content = JsonConvert.SerializeObject(singUp);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/account/signup", bodyContent);

            // Convert response into readabe 
            var contentTemp = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<SignUpResponseDTO>(contentTemp);

            if (response.IsSuccessStatusCode)
            {
                return new SignUpResponseDTO
                {
                    IsRegisterationSuccessful = true
                };
            }
            else
            {
                return new SignUpResponseDTO
                {
                    IsRegisterationSuccessful = false,
                    Errors = result.Errors
                };
            }
        }
    }
}
