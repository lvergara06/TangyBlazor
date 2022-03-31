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
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public ProductRepository (ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<ProductDTO> Create(ProductDTO objDTO)
        {
            Product Product = _mapper.Map<ProductDTO, Product>(objDTO);
            _db.Products.Add(Product);
            await _db.SaveChangesAsync();

            return _mapper.Map<Product, ProductDTO>(Product);
        }

        public async Task<int> Delete(int id)
        {
            Product Product = await _db.Products.FirstOrDefaultAsync(c => c.Id == id);

            if(Product != null)
            {
                _db.Products.Remove(Product);
                return await _db.SaveChangesAsync();
            }
            return 0;
            
        }

        public async Task<ProductDTO> Get(int id)
        {
            Product Product = await _db.Products.Include(u=>u.Category).Include(u => u.Prices).FirstOrDefaultAsync(c => c.Id == id);
            if (Product != null)
            {
                return _mapper.Map<Product, ProductDTO>(Product);
            }
            return new ProductDTO();

        }

        public async Task<IEnumerable<ProductDTO>> GetAll()
        {
            return _mapper.Map<IEnumerable<Product>, IEnumerable<ProductDTO>>(_db.Products.Include(u => u.Category)
                                                                                           .Include(u => u.Prices));
        }

        public async Task<ProductDTO> Update(ProductDTO objDTO)
        {
            Product Product = await _db.Products.FirstOrDefaultAsync(c => c.Id == objDTO.Id);
            if(Product != null)
            {
                Product.Name = objDTO.Name;
                Product.Description = objDTO.Description;
                Product.ImageUrl = objDTO.ImageUrl;
                Product.CategoryId = objDTO.CategoryId;
                Product.Color = objDTO.Color;
                Product.ShopFavorites = objDTO.ShopFavorites;
                Product.CustomerFavorites = objDTO.CustomerFavorites;

                _db.Products.Update(Product);
                await _db.SaveChangesAsync();
                return _mapper.Map<Product, ProductDTO>(Product);
            }
            return objDTO;
        }
    }
}
