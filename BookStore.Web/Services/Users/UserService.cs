using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BookStore.Models;
using Newtonsoft.Json;

namespace BookStore.Web.Services.Users
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<User> LoginAsync(User user)
        {
            var serializedUser = JsonConvert.SerializeObject(user);

            var message = new HttpRequestMessage(HttpMethod.Post, "users/login");
            message.Content = new StringContent(serializedUser);
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _httpClient.SendAsync(message);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var userWithToken = JsonConvert.DeserializeObject<UserWithToken>(responseBody);
                return userWithToken;
            }
            else
            {
                return null;
            }
        }

        public async Task<User> RefreshTokenAsync(RefreshRequest refreshRequest)
        {
            var serializedRefreshRequest = JsonConvert.SerializeObject(refreshRequest);

            var message = new HttpRequestMessage(HttpMethod.Post, "users/refreshtoken");
            message.Content = new StringContent(serializedRefreshRequest);
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _httpClient.SendAsync(message);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var userWithToken = JsonConvert.DeserializeObject<UserWithToken>(responseBody);
                return userWithToken;
            }
            else
            {
                return null;
            }
        }


        public async Task<User> RegisterUserAsync(User user)
        {
            user.Password = Utility.Encrypt(user.Password);
            string serializedUser = JsonConvert.SerializeObject(user);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "users/registeruser");
            requestMessage.Content = new StringContent(serializedUser);

            requestMessage.Content.Headers.ContentType
                = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var response = await _httpClient.SendAsync(requestMessage);

            var responseStatusCode = response.StatusCode;
            var responseBody = await response.Content.ReadAsStringAsync();

            var returnedUser = JsonConvert.DeserializeObject<User>(responseBody);

            return await Task.FromResult(returnedUser);
        }


    }
}