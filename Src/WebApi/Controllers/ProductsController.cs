using Domain;
using Domain.Account;
using Domain.Portifolio;
using Domain.Stock;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApi.Aplication;
using WebApi.Aplication.Catalog;
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
        private readonly ILotRepository _batchRepository;
        private readonly IMediator _mediator;
        private readonly ISupplierRepository _supplierRepository;
        private readonly ITagRepository _tagRepository;

        public ProductsController(
            IProductRepository productRepository, IUnitOfWork unitOfWork, ILotRepository batchRepository, IMediator mediator, ISupplierRepository supplierRepository, ITagRepository tagRepository)
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
                EAN = productDTO.BarCode,
                Description = productDTO.Description,
            };
            tags.ForEach(product.AddCategory);
            await _productRepository.Add(product);
            await _unitOfWork.Commit();
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] ProductQuery query)
        {
            var productsPage = await _mediator.Send(query);
            return Ok(productsPage);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var productsPage = await _mediator.Send(new ProductQueryById(id));
            return Ok(productsPage);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] ProductDTO productDTO)
        {
            var tags = await _tagRepository.List(it => productDTO.Categories.Contains(it.Id));
            var product = await _productRepository.GetById(id);
            product.EAN = productDTO.BarCode;
            product.Description = productDTO.Description;
            product.RemoveAllCategories();
            tags.ForEach(product.AddCategory);
            await _productRepository.Update(product);

            await _unitOfWork.Commit();
            return Ok();
        }

        [HttpGet("batch")]
        public async Task<IActionResult> BatchGet()
        {
            var suppliers = await _batchRepository.List();
            return Ok(suppliers.Adapt<IList<BatchDTO>>());
        }
        [HttpPost("batch")]
        public async Task<IActionResult> BatchPost([FromBody] BatchDTO batch)
        {
            var suppliers = await _supplierRepository.List(it => batch.SuppliersId.Contains(it.Id));
            await _batchRepository.Add(new Lot
            {
                CostPrice = batch.CostValue,
                Date = batch.Date,
                Number = batch.Number,
                Quantity = batch.Quantity,
                SalePrice = batch.SaleValue,
                Weight = batch.Weight,
                ProductId = batch.ProductId,
                Suppliers = suppliers
            });
            await _unitOfWork.Commit();
            return Ok();
        }

        [HttpPost("supplier")]
        public async Task<IActionResult> Supplier([FromBody] SupplierDTO productDto)
        {
            var product = productDto.Adapt<Supplier>();
            await _supplierRepository.Add(product);
            await _unitOfWork.Commit();
            return Ok();
        }

        [HttpGet("supplier")]
        public async Task<IActionResult> SupplierGet()
        {
            var suppliers = await _supplierRepository.List();
            return Ok(suppliers);
        }

        [HttpPost("tags")]
        public async Task<IActionResult> Tags([FromBody] TagDTO tagDTO)
        {
            var tag = tagDTO.Adapt<Tag>();
            await _tagRepository.Add(tag);
            await _unitOfWork.Commit();
            return Ok(tag);
        }

        [HttpGet("agents")]
        public async Task<IActionResult> Agents([FromQuery]PaginateRequest request)
        {
            var query = request.Adapt<AgentsQuery>();
            var result = await _mediator.Send(query);
            return result.ToActionResult();
        }

        [HttpPost("catalog")]
        public async Task<IActionResult> Catalog([FromBody] CatalogOpenRequest request)
        {
            var command = request.Adapt<CatalogOpenCommand>();
            command.AccountableId = User.GetId();
            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }

        [HttpPut("catalog")]
        public async Task<IActionResult> CatalogPut([FromBody] CatalogCloseCommand command)
        {
            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }
    }
     public record PaginateRequest
    {
        public string SearchTerm { get; init; }
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 5;
        public Dictionary<string, Sort> OrderBy { get; init; } = null;
    }
    public static class UserExtensions
    {
        public static Guid GetId(this ClaimsPrincipal user)
        {
            var value = user.FindFirstValue(JwtRegisteredClaimNames.Sub);
            return Guid.Parse(value);
        }
    }


    public record CatalogOpenRequest
    {
        public Guid OwnerId { get; init; }
        public IList<CatalogOpenItemRequest> Items { get; init; }
    }

    public record CatalogOpenItemRequest
    {
        public Guid ProductId { get; init; }
        public Guid LotId { get; init; }
        public decimal Quantity { get; init; }
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
