using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebApi.Aplication;
using WebApi.Domain;
using WebApi.Security;

namespace WebApi.Controllers
{
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize()]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public ProductsController(
            IProductRepository productRepository, IUnitOfWork unitOfWork, IMediator mediator)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
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
        public async Task<IActionResult> Get([FromQuery] ProductQueryInDTO query)
        {
            var productsPage = await _mediator.Send(query);
            return Ok(productsPage);
        }
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
        public int[] Thematic { get; set; }
    }
}
