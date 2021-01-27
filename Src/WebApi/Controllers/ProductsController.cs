using Domain;
using Domain.Account;
using Domain.Portifolio;
using Domain.Stock;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApi.Aplication;
using WebApi.Aplication.Stock;
using WebApi.Security;

namespace WebApi.Controllers
{
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    [AuthorizeEnum(Roles.ADMIN)]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILotRepository _lotsRepository;
        private readonly IMediator _mediator;
        private readonly ISupplierRepository _supplierRepository;
        private readonly ITagRepository _tagRepository;

        public ProductsController(
            IProductRepository productRepository, IUnitOfWork unitOfWork, ILotRepository batchRepository, IMediator mediator, ISupplierRepository supplierRepository, ITagRepository tagRepository)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _lotsRepository = batchRepository;
            _mediator = mediator;
            _supplierRepository = supplierRepository;
            _tagRepository = tagRepository;
        }
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] ProductDTO productDTO)
        {
            var tags = await _tagRepository.List(it => productDTO.Categories.Contains(it.Id));
            var product = new Product(productDTO.Sku, productDTO.Description);
            tags.ForEach(product.AddCategory);
            await _productRepository.Add(product);
            await _unitOfWork.Commit();
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] ProductQuery query)
        {
            var productsPage = await _mediator.Send(query);
            return productsPage.ToActionResult();
        }
        [HttpGet("lots/ean/{ean}")]
        public async Task<IActionResult> GetSearch(string ean)
        {
            var result = await _mediator.Send(new LotSearchQuery(ean));
            return result.ToActionResult();
        }
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var productsPage = await _mediator.Send(new ProductQueryById(id));
            return Ok(productsPage.ValueOrDefault);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] ProductDTO productDTO)
        {
            var tags = await _tagRepository.List(it => productDTO.Categories.Contains(it.Id));
            var product = await _productRepository.GetById(id);
            product.SKU = productDTO.Sku;
            product.Description = productDTO.Description;
            product.RemoveAllCategories();
            tags.ForEach(product.AddCategory);
            await _productRepository.Update(product);

            await _unitOfWork.Commit();
            return Ok();
        }

        [HttpGet("{id:guid}/lots")]
        public async Task<IActionResult> LotsGet(Guid id)
        {
            var lots = await _lotsRepository.List(it => it.ProductId == id);
            return Ok(lots.Adapt<IList<LotResponse>>());
        }

        [HttpPost("{id:guid}/lots")]
        public async Task<IActionResult> LotsPost(Guid id, [FromBody] LotRequest batch)
        {
            var command = new LotCreateCommand(batch.ProductId, batch.CostValue, batch.SaleValue, batch.Quantity, batch.Number, batch.SuppliersId);
            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }

        [HttpPost("suppliers")]
        public async Task<IActionResult> Supplier([FromBody] SupplierDTO productDto)
        {
            var product = productDto.Adapt<Supplier>();
            await _supplierRepository.Add(product);
            await _unitOfWork.Commit();
            return Ok();
        }

        [HttpGet("suppliers")]
        public async Task<IActionResult> SupplierGet()
        {
            var suppliers = await _supplierRepository.List();
            return Ok(suppliers.Adapt<IList<SupplierResponse>>());
        }

        [HttpPost("tags")]
        public async Task<IActionResult> Tags([FromBody] TagDTO tagDTO)
        {
            var tag = tagDTO.Adapt<Tag>();
            await _tagRepository.Add(tag);
            await _unitOfWork.Commit();
            return Ok(tag);
        }

        [HttpGet("tags")]
        public async Task<IActionResult> Tags()
        {
            var tags = await _tagRepository.List();
            return Ok(tags);
        }
    }

    public static class UserExtensions
    {
        public static Guid GetId(this ClaimsPrincipal user)
        {
            var value = user.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (value is null)
                throw new UnauthorizedAccessException("User Not Found.");
            return Guid.Parse(value);
        }
    }

    public record TagDTO
    {
        public Guid? Id { get; init; }
        public string Name { get; init; }
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
        public string Sku { get; init; }
        public string Description { get; init; }
        public IList<Guid> Categories { get; init; }
    }

    public class ProductDTOValidation : AbstractValidator<ProductDTO>
    {
        public ProductDTOValidation()
        {
            RuleFor(it => it.Sku).NotEmpty().MinimumLength(5);
            RuleFor(it => it.Description).NotEmpty().MinimumLength(5);
            RuleFor(it => it.Categories).NotEmpty();
        }
    }
    public record LotResponse
    {
        public Guid Id { get; init; }
        public Guid ProductId { get; init; }
        public decimal CostPrice { get; init; }
        public decimal SalePrice { get; init; }
        public decimal CurrentyQuantity { get; init; }
        public decimal Quantity { get; init; }
        public decimal ReservedQuantity { get; init; }
        public decimal? Weight { get; init; }
        public IList<SupplierResponse> Suppliers { get; init; }
        public DateTime CreatedAt { get; init; }
        public string Number { get; init; }
        public string EAN { get; init; }
    }
    public record SupplierResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public record LotRequest
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


    public class BatchDTOValidation : AbstractValidator<LotRequest>
    {
        public BatchDTOValidation()
        {
            RuleFor(it => it.ProductId).NotEqual(Guid.Empty);
            RuleFor(it => it.CostValue).GreaterThan(0);
            RuleFor(it => it.SaleValue).GreaterThan(it => it.CostValue);
            RuleFor(it => it.Quantity).GreaterThan(0);
            RuleFor(it => it.Number).NotEmpty();
            RuleFor(it => it.SuppliersId).NotEmpty();
        }
    }

}
