using Newtonsoft.Json;
using System.Text;
using Tangy_Models;
using TangyWeb_Client.Service.IService;

namespace TangyWeb_Client.Service
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private string BaseServerUrl;

        public OrderService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            BaseServerUrl = configuration.GetValue<string>("BaseServerUrl");
        }

        public async Task<OrderDTO> Create(StripePaymentDTO paymentDTO)
        {
            var content = JsonConvert.SerializeObject(paymentDTO);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/order/create", bodyContent);

            string responseResult = response.Content.ReadAsStringAsync().Result;
            if(response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<OrderDTO>(responseResult);
            }
            return new OrderDTO();
        }

        public async Task<OrderDTO> Get(int orderHdrId)
        {
            var response = await _httpClient.GetAsync($"/api/order/GetId?OrderHdrId={orderHdrId}");
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var order = JsonConvert.DeserializeObject<OrderDTO>(content);
                return order;
            }
            else
            {
                var errorModel = JsonConvert.DeserializeObject<ErrorModelDTO>(content);
                throw new Exception(errorModel.ErrorMessage);
            }
        }


        public async Task<IEnumerable<OrderDTO>> GetAll(string? userId = null )
        {
            var response = await _httpClient.GetAsync("/api/order");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var orders = JsonConvert.DeserializeObject<IEnumerable<OrderDTO>>(content);

                return orders;
            }
            return new List<OrderDTO>();
        }
}
}
