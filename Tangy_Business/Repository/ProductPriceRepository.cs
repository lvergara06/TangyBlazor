using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tangy_Business.Repository.IRepository;
using Tangy_DataAccess;
using Tangy_DataAccess.Data;
using Tangy_Models;

namespace Tangy_Business.Repository
{
    public class ProductPriceRepository : IProductPriceRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public ProductPriceRepository (ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<ProductPriceDTO> Create(ProductPriceDTO objDTO)
        {
            ProductPrice ProductPrice = _mapper.Map<ProductPriceDTO, ProductPrice>(objDTO);
            _db.ProductPrices.Add(ProductPrice);
            await _db.SaveChangesAsync();

            return _mapper.Map<ProductPrice , ProductPriceDTO>(ProductPrice );
        }

        public async Task<int> Delete(int id)
        {
            ProductPrice ProductPrice = await _db.ProductPrices.FirstOrDefaultAsync(c => c.Id == id);

            if(ProductPrice != null)
            {
                _db.ProductPrices.Remove(ProductPrice );
                return await _db.SaveChangesAsync();
            }
            return 0;
            
        }

        public async Task<ProductPriceDTO> Get(int id)
        {
            ProductPrice ProductPrice = await _db.ProductPrices.Include(u=>u.Product).FirstOrDefaultAsync(c => c.Id == id);
            if (ProductPrice != null)
            {
                return _mapper.Map<ProductPrice , ProductPriceDTO>(ProductPrice );
            }
            return new ProductPriceDTO();

        }

        public async Task<IEnumerable<ProductPriceDTO>> GetAll(int? Id = null)
        {
            if(Id != null)
            {
                return _mapper.Map<IEnumerable<ProductPrice>, IEnumerable<ProductPriceDTO>>(_db.ProductPrices.Include(u => u.Product)
                                                                                                          .Where(u => u.ProductId == Id));
            }
            else
            {
                return _mapper.Map<IEnumerable<ProductPrice>, IEnumerable<ProductPriceDTO>>(_db.ProductPrices.Include(u => u.Product));
            }
            ;
        }

        public async Task<ProductPriceDTO> Update(ProductPriceDTO objDTO)
        {
            ProductPrice ProductPrice = await _db.ProductPrices.FirstOrDefaultAsync(c => c.Id == objDTO.Id);
            if(ProductPrice != null)
            {
                ProductPrice.ProductId = objDTO.ProductId;
                ProductPrice.Size = objDTO.Size;
                ProductPrice.Price = objDTO.Price;

                _db.ProductPrices.Update(ProductPrice);
                await _db.SaveChangesAsync();
                return _mapper.Map<ProductPrice, ProductPriceDTO>(ProductPrice);
            }
            return objDTO;
        }
    }
}
