using Domain;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Aplication;
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
        private readonly ITagRepository _tagRepository;

        public ProductsController(
            IProductRepository productRepository, IUnitOfWork unitOfWork, IBatchRepository batchRepository, IMediator mediator, ISupplierRepository supplierRepository, ITagRepository tagRepository)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _batchRepository = batchRepository;
            _mediator = mediator;
            _supplierRepository = supplierRepository;
            _tagRepository = tagRepository;
        }
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] ProductDTO productDTO)
        {
            var tags = await _tagRepository.List(it => productDTO.Categories.Contains(it.Id));
            var product = new Product
            {
                BarCode = productDTO.BarCode,
                Description = productDTO.Description,
            };
            tags.ForEach(product.AddCategory);
            await _productRepository.Add(product);
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
        public async Task<IActionResult> Put(Guid id, [FromBody] ProductDTO productDTO)
        {
            var tags = await _tagRepository.List(it => productDTO.Categories.Contains(it.Id));
            var product = await _productRepository.GetById(id);
            product.BarCode = productDTO.BarCode;
            product.Description = productDTO.Description;
            product.RemoveAllCategories();
            tags.ForEach(product.AddCategory);
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

        [HttpPost("/tags")]
        public async Task<IActionResult> Tags([FromBody] TagDTO tagDTO)
        {
            var tag = tagDTO.Adapt<Tag>();
            await _tagRepository.Add(tag);
            await _unitOfWork.Commit();
            return Ok(tag);
        }
    }
    public record TagDTO
    {
        public Guid? Id { get; init; }
        public string Name { get; init;  }
    }
    public record SupplierDTO
    {
        public Guid? Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
    }
    public class SupplierDTOValidation : AbstractValidator<SupplierDTO>
    {
        public SupplierDTOValidation()
        {
            RuleFor(it => it.Name).NotEmpty().MinimumLength(5);
            RuleFor(it => it.Description).NotEmpty().MinimumLength(5);
        }
    }
    public record ProductDTO
    {
        public Guid? Id { get; init; }
        public string BarCode { get; init; }
        public string Description { get; init; }
        public IList<Guid> Categories { get; init; }
    }

    public class ProductDTOValidation : AbstractValidator<ProductDTO>
    {
        public ProductDTOValidation()
        {
            RuleFor(it => it.BarCode).NotEmpty().MinimumLength(5);
            RuleFor(it => it.Description).NotEmpty().MinimumLength(5);
            RuleFor(it => it.Categories).NotEmpty();
        }
    }

    public record BatchDTO
    {
        public Guid? Id { get; init; }
        public Guid ProductId { get; init; }
        public decimal CostValue { get; init; }
        public decimal SaleValue { get; init; }
        public decimal Quantity { get; init; }
        public decimal? Weight { get; init; }
        public IList<Guid> SuppliersId { get; init; }
        public DateTime Date { get; init; }
        public string Number { get; init; }
    }
    public class BatchDTOValidation : AbstractValidator<BatchDTO>
    {
        public BatchDTOValidation()
        {
            RuleFor(it => it.ProductId).NotEqual(Guid.Empty);
            RuleFor(it => it.CostValue).NotEqual(0);
            RuleFor(it => it.SaleValue).NotEqual(0);
            RuleFor(it => it.Quantity).NotEqual(0);
            RuleFor(it => it.Date).NotEqual(DateTime.MinValue);
            RuleFor(it => it.Number).NotEmpty();
            RuleFor(it => it.SuppliersId).NotEmpty();
        }
    }

}
