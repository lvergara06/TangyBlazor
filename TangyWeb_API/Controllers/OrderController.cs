using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tangy_Business.Repository.IRepository;
using Tangy_Models;

namespace TangyWeb_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository OrderRepository)
        {
            _orderRepository = OrderRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _orderRepository.GetAll());
        }

        [HttpGet("GetId")]
        public async Task<IActionResult> Get(int? orderHdrId)
        {
            if(orderHdrId == null || orderHdrId == 0)
            {
                return BadRequest(new ErrorModelDTO()
                {
                    Title = "",
                    ErrorMessage = "Invalid Id",
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }

            var orderHdr = await _orderRepository.Get(orderHdrId.Value);
            if(orderHdr == null)
            {
                return BadRequest(new ErrorModelDTO()
                {
                    Title = "",
                    ErrorMessage = "Invalid Id",
                    StatusCode = StatusCodes.Status404NotFound
                }) ;
            }

            return Ok(orderHdr);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StripePaymentDTO paymentDTO)
        {
            paymentDTO.Order.OrderHdr.OrderDate = DateTime.Now;
            var result = await _orderRepository.Create(paymentDTO.Order);
            return Ok(result);
        }
    }
}
