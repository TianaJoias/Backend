using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Aplication;
using WebApi.Domain;
using WebApi.Security;

namespace WebApi.Controllers
{
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(Roles = Policies.Admin)]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBatchRepository _batchRepository;
        private readonly IMediator _mediator;
        private readonly ISupplierRepository _supplierRepository;

        public ProductsController(
            IProductRepository productRepository, IUnitOfWork unitOfWork, IBatchRepository batchRepository, IMediator mediator, ISupplierRepository supplierRepository)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _batchRepository = batchRepository;
            _mediator = mediator;
            _supplierRepository = supplierRepository;
        }
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] ProductDTO product)
        {
            await _productRepository.Add(new Product
            {
                BarCode = product.BarCode,
                Categories = product.Categories,
                Colors = product.Colors,
                Description = product.Description,
                Thematics = product.Thematics,
                Typologies = product.Typologies
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

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var productsPage = await _mediator.Send(new ProductQueryByIdRequest { Id = id });
            return Ok(productsPage);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] ProductDTO productDto)
        {
            var product = await _productRepository.GetById(id);
            product.BarCode = productDto.BarCode;
            product.Categories = productDto.Categories;
            product.Colors = productDto.Colors;
            product.Description = productDto.Description;
            product.Thematics = productDto.Thematics;
            product.Typologies = productDto.Typologies;
            await _productRepository.Update(product);

            await _unitOfWork.Commit();
            return Ok();
        }

        [HttpGet("/batch")]
        public async Task<IActionResult> BatchGet()
        {
            var suppliers = await _batchRepository.List();
            return Ok(suppliers.Adapt<IList<BatchDTO>>());
        }
        [HttpPost("/batch")]
        public async Task<IActionResult> BatchPost([FromBody] BatchDTO batch)
        {
            var product = await _productRepository.GetById(batch.ProductId);
            var suppliers = await _supplierRepository.List(it => batch.SuppliersId.Contains(it.Id));
            await _batchRepository.Add(new Batch
            {
                CostValue = batch.CostValue,
                Date = batch.Date,
                Number = batch.Number,
                Quantity = batch.Quantity,
                SaleValue = batch.SaleValue,
                Weight = batch.Weight,
                Product = product,
                Suppliers = suppliers

            });
            await _unitOfWork.Commit();
            return Ok();
        }

        [HttpPost("/supplier")]
        public async Task<IActionResult> Supplier([FromBody] SupplierDTO productDto)
        {
            var product = productDto.Adapt<Supplier>();
            await _supplierRepository.Add(product);
            await _unitOfWork.Commit();
            return Ok();
        }

        [HttpGet("/supplier")]
        public async Task<IActionResult> SupplierGet()
        {
            var suppliers = await _supplierRepository.List();
            return Ok(suppliers);
        }
    }
    public class SupplierDTO
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public class ProductDTO
    {
        public Guid? Id { get; set; }
        public string BarCode { get; set; }
        public string Description { get; set; }
        public IList<int> Typologies { get; set; }
        public IList<int> Colors { get; set; }
        public IList<int> Categories { get; set; }
        public IList<int> Thematics { get; set; }
    }

    public class BatchDTO
    {
        public Guid? Id { get; set; }
        public Guid ProductId { get; set; }
        public decimal CostValue { get; set; }
        public decimal SaleValue { get; set; }
        public decimal Quantity { get; set; }
        public decimal? Weight { get; set; }
        public IList<Guid> SuppliersId { get; set; }
        public DateTime Date { get; set; }
        public string Number { get; set; }
    }
}
