using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tangy_Business.Repository.IRepository;
using Tangy_Common;
using Tangy_DataAccess;
using Tangy_DataAccess.Data;
using Tangy_DataAccess.ViewModel;
using Tangy_Models;

namespace Tangy_Business.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public OrderRepository(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

		public async Task<OrderHdrDTO> CancelOrder(int Id)
		{
            var orderHeader = await _db.OrderHdrs.FindAsync(Id);
            if (orderHeader == null)
            {
                return new OrderHdrDTO();
            }

            if (orderHeader.Status == SD.Status_Pending)
            {
                orderHeader.Status = SD.Status_Cancelled;
                await _db.SaveChangesAsync();
            }
            if (orderHeader.Status == SD.Status_Confirmed)
            {
                //refund
                var options = new Stripe.RefundCreateOptions
                {
                    Reason = Stripe.RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };

                var service = new Stripe.RefundService();
                Stripe.Refund refund = service.Create(options);

                orderHeader.Status = SD.Status_Refunded;
                await _db.SaveChangesAsync();
            }

            return _mapper.Map<OrderHdr, OrderHdrDTO>(orderHeader);
        }

		public async Task<OrderDTO> Create(OrderDTO objDTO)
        {

            try
            {
                Order orderObj = _mapper.Map<OrderDTO, Order>(objDTO);
                if(orderObj.OrderHdr.Id == 0)
                {
                    _db.OrderHdrs.Add(orderObj.OrderHdr);
                    await _db.SaveChangesAsync();

                    foreach(var dtls in orderObj.OrderDtls)
                    {
                        dtls.OrderHeaderId = orderObj.OrderHdr.Id;
                        //_db.OrderDtls.Add(dtls);
                    }
                    _db.OrderDtls.AddRange(orderObj.OrderDtls);
                    await _db.SaveChangesAsync();

                    return new OrderDTO()
                    {
                        OrderHdr = _mapper.Map<OrderHdr, OrderHdrDTO>(orderObj.OrderHdr),
                        OrderDtls = _mapper.Map<IEnumerable<OrderDtl>, IEnumerable<OrderDtlDTO>>(orderObj.OrderDtls).ToList()
                    };
                }

                return objDTO;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public async Task<int> Delete(int id)
        {
            var orderObj = await _db.OrderHdrs.FirstOrDefaultAsync(u => u.Id == id);
            if(orderObj != null)
            {
                IEnumerable<OrderDtl> objDtl = _db.OrderDtls.Where(u => u.OrderHeaderId == id);

                _db.OrderDtls.RemoveRange(objDtl);
                _db.OrderHdrs.Remove(orderObj);
                return _db.SaveChanges();
            }

            return 0;
        }

        public async Task<OrderDTO> Get(int Id)
        {
            Order order = new()
            {
                OrderHdr = await _db.OrderHdrs.FirstOrDefaultAsync(u => u.Id == Id),
                OrderDtls = _db.OrderDtls.Where(u => u.OrderHeaderId == Id)
            };
            if(order != null)
            {
                return _mapper.Map<Order, OrderDTO>(order);
            }

            return new OrderDTO();
        }

        public async Task<IEnumerable<OrderDTO>> GetAll(string? userId = null, string? status = null)
        {
            List<Order> OrderFromDb = new List<Order>();
            IEnumerable<OrderHdr> orderHdrList = _db.OrderHdrs;
            IEnumerable<OrderDtl> orderDtlList = _db.OrderDtls;

            foreach(OrderHdr hdr in orderHdrList)
            {
                Order order = new()
                {
                    OrderHdr = hdr,
                    OrderDtls = _db.OrderDtls.Where(u => u.OrderHeaderId == hdr.Id)
                };

                OrderFromDb.Add(order);
            }
            // Filter #TODI

            return _mapper.Map<IEnumerable<Order>, IEnumerable<OrderDTO>>(OrderFromDb);
        }

        public async Task<OrderHdrDTO> MarkPaymentSuccessful(int id)
        {
            var data = await _db.OrderHdrs.FindAsync(id);
            if(data == null)
            {
                return new OrderHdrDTO();
            }

            if(data.Status == SD.Status_Pending)
            {
                data.Status = SD.Status_Confirmed;
                await _db.SaveChangesAsync();
                return _mapper.Map<OrderHdr, OrderHdrDTO>(data);
            }
            return new OrderHdrDTO();
        }

        public async Task<OrderHdrDTO> UpdateHdr(OrderHdrDTO objDTO)
        {
            if(objDTO != null)
            {
                var orderHeaderFromDb = _db.OrderHdrs.FirstOrDefault(u => u.Id == objDTO.Id);

                UpdateOrderStatus(objDTO.Id, objDTO.Status);

                orderHeaderFromDb.Name = objDTO.Name;
                orderHeaderFromDb.PhoneNumber = objDTO.PhoneNumber;
                orderHeaderFromDb.Carrier = objDTO.Carrier;
                orderHeaderFromDb.Tracking = objDTO.Tracking;
                orderHeaderFromDb.StreetAddress = objDTO.StreetAddress;
                orderHeaderFromDb.City = objDTO.City;
                orderHeaderFromDb.State = objDTO.State;
                orderHeaderFromDb.PostalCode = objDTO.PostalCode;
                orderHeaderFromDb.Status = objDTO.Status;

                await _db.SaveChangesAsync();
                return _mapper.Map<OrderHdr, OrderHdrDTO>(orderHeaderFromDb);
            }
            return new OrderHdrDTO();
        }

        public async Task<bool> UpdateOrderStatus(int orderId, string status)
        {
            var data = await _db.OrderHdrs.FindAsync(orderId);
            if (data == null)
            {
                return false;
            }

            data.Status = status;
            if(status == SD.Status_Shipped)
            {
                data.ShippingDate = DateTime.Now;
            }

            await _db.SaveChangesAsync();
            return true;
        }
    }
}
