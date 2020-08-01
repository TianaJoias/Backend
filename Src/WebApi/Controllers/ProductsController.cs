using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using WebApi.Infra;

namespace WebApi.Controllers
{
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    //  [Authorize(Roles = Policies.Admin)]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProductsController(IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] ProductDTO product)
        {
            await _productRepository.Add(new Product
            {
                BarCode = product.BarCode,
                Categories = product.Category,
                Colors = product.Color,
                CostValue = product.CostPrice,
                Description = product.Description,
                Quantity = product.Quantity,
                SalePrice = product.SalePrice,
                Supplier = product.Supplier,
                Thematics = product.Thematic,
                Typologies = product.Typology,
                Weight = product.Weight
            });
            await _unitOfWork.Commit();
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var products = await _productRepository.List();
            return Ok(products);
        }
    }

    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(IUnitOfWork unitOfWork, TianaJoiasContextDB context) : base(unitOfWork, context)
        { }
    }

    public class ProductDTO
    {
        public string BarCode { get; set; }
        public string Description { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SalePrice { get; set; }
        public decimal Quantity { get; set; }
        public decimal? Weight { get; set; }
        public Guid Supplier { get; set; }
        public int[] Typology { get; set; }
        public int[] Color { get; set; }
        public int[] Category { get; set; }
        public int[] SubCategory { get; set; }
        public int[] Thematic { get; set; }
    }
}
