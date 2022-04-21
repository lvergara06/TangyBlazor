using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Tangy_Business.Repository.IRepository;
using Tangy_Models;

namespace TangyWeb_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IEmailSender _emailSender;

        public OrderController(IOrderRepository OrderRepository, IEmailSender emailSender)
        {
            _orderRepository = OrderRepository;
            _emailSender = emailSender;
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

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] StripePaymentDTO paymentDTO)
        {
            paymentDTO.Order.OrderHdr.OrderDate = DateTime.Now;
            var result = await _orderRepository.Create(paymentDTO.Order);
            return Ok(result);
        }

        [HttpPost("paymentsuccessful")]
        public async Task<IActionResult> PaymentSuccessful([FromBody] OrderHdrDTO orderHdrDTO)
        {
            var service = new SessionService();
            var sessionDetails = service.Get(orderHdrDTO.SessionId);
            if(sessionDetails.PaymentStatus == "paid")
            {
                var result = await _orderRepository.MarkPaymentSuccessful(orderHdrDTO.Id);
                await _emailSender.SendEmailAsync(orderHdrDTO.Email, "Tangy Order Confirmation",
                    "New Order has been created:" + orderHdrDTO.Id);
                if(result == null)
                {
                    return BadRequest(new ErrorModelDTO()
                    {
                        ErrorMessage = "Can not mark payment as successful"
                    });
                }
                return Ok(result);
            }
            return BadRequest();
        }
    }
}
