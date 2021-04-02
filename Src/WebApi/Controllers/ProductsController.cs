using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Domain;
using Domain.Account;
using Domain.Portifolio;
using Domain.Stock;
using FluentResults;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApi.Aplication;
using WebApi.Aplication.Stock.Commands;
using WebApi.Aplication.Stock.Queries;
using WebApi.Aplication.Stock.Queries.ProductSuppliers;
using WebApi.Security;

namespace WebApi.Controllers
{
    public interface IFileBatchLotParser
    {
        Task<List<Result<BatchLotCreateItemCommand>>> Parser(Stream file);
    }
    public class BatchLotParser : IFileBatchLotParser
    {
        public Task<List<Result<BatchLotCreateItemCommand>>> Parser(Stream file)
        {
            using var reader = new StreamReader(file);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.TypeConverterCache.AddConverter<DateTime>(new Teste());
            csv.Context.RegisterClassMap<FooMap>();
            var items = ReadItems(csv).ToList();
            return Task.FromResult(items);
        }
        private static IEnumerable<Result<BatchLotCreateItemCommand>> ReadItems(CsvReader csv)
        {
            var count = 1;
            while (csv.Read())
            {
                BatchLotCreateItemCommand record = null;
                FieldValidationException error = null;
                try
                {
                    count++;
                    record = csv.GetRecord<BatchLotCreateItemCommand>();
                }
                catch (FieldValidationException ex)
                {
                    error = ex;
                }
                if (record is null)
                    yield return Result.Fail(error.Field);
                else
                    yield return Result.Ok(record);
            }
        }
    }
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
        private readonly IFileBatchLotParser _fileBatchLotParser;

        public ProductsController(
            IProductRepository productRepository, IUnitOfWork unitOfWork, ILotRepository batchRepository, IMediator mediator, ISupplierRepository supplierRepository,
            ITagRepository tagRepository, IFileBatchLotParser fileBatchLotParser)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _lotsRepository = batchRepository;
            _mediator = mediator;
            _supplierRepository = supplierRepository;
            _tagRepository = tagRepository;
            _fileBatchLotParser = fileBatchLotParser;
        }
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] ProductDTO productDTO)
        {
            var tags = await _tagRepository.List(it => productDTO.Tags.Contains(it.Id));
            var product = new Product(productDTO.Sku, productDTO.Description);
            tags.ForEach(product.AddTag);
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
            return productsPage.ToActionResult();
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] ProductDTO productDTO)
        {
            var tags = await _tagRepository.List(it => productDTO.Tags.Contains(it.Id));
            var product = await _productRepository.GetById(id);
            product.SKU = productDTO.Sku;
            product.Description = productDTO.Description;
            product.ClearTags();
            tags.ForEach(product.AddTag);
            await _productRepository.Update(product);

            await _unitOfWork.Commit();
            return Ok();
        }

        [HttpGet("{productId:guid}/lots")]
        public async Task<IActionResult> LotsGet(Guid productId, [FromQuery] FilterPaged query)
        {
            var command = query.Adapt<LotsByProductIdQuery>();
            command.ProductId = productId;
            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }

        [HttpGet("{productId:guid}/lots/{lotId:guid}")]
        public async Task<IActionResult> LotsGet(Guid productId, Guid lotId)
        {
            var lots = await _lotsRepository.GetByQuery(it => it.ProductId == productId && it.Id == lotId);
            return Ok(lots.Adapt<LotResponse>());
        }
        [HttpPut("{productId:guid}/lots/{lotId:guid}")]
        public async Task<IActionResult> UpdateLot(Guid productId, Guid lotId, [FromBody] LotRequest lot)
        {
            var command = new LotUpdateCommand(lotId, productId, lot.CostPrice, lot.SalePrice, lot.Quantity, lot.Number, lot.Suppliers, lot.Weight, lot.Date);
            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }

        [HttpPost("{id:guid}/lots")]
        public async Task<IActionResult> LotsPost(Guid id, [FromBody] LotRequest lot)
        {
            var command = new LotCreateCommand(lot.ProductId, lot.CostPrice, lot.SalePrice, lot.Quantity, lot.Number, lot.Suppliers, lot.Weight, lot.Date);
            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }

        [HttpPost("lots/batch")]
        public async Task<IActionResult> OnPostUploadAsync([FromForm] IFormFile file)
        {
            var items = await _fileBatchLotParser.Parser(file.OpenReadStream());
            var validItems = items.Where(it => it.IsSuccess).Select(it => it.Value);
            var command = new BatchLotCreateCommand(validItems.ToList());
            var result = await _mediator.Send(command);
            return Ok();
        }

        [HttpGet("{productId:guid}/suppliers")]
        public async Task<IActionResult> SuppliersGet(Guid productId, [FromQuery] FilterPaged filter)
        {
            var query = filter.Adapt<ProductSupplierQuery>();
            query.ProductId = productId;
            var result = await _mediator.Send(query);
            return result.ToActionResult();
        }

        [HttpPost("{productId:guid}/suppliers")]
        public async Task<IActionResult> SuppliersPost(Guid productId, [FromBody] ProductSupplierRequest request)
        {
            var command = new ProductSupplierCommand(productId, request.SupplierId, request.Code);
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


    public class Teste : ITypeConverter
    {
        public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            return DateTime.ParseExact(text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        }

        public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            return ((DateTime)value).ToString("dd/MM/yyyy");
        }
    }
    public class FooMap : ClassMap<BatchLotCreateItemCommand>
    {
        public FooMap()
        {
            Map(m => m.SuppliersId).Name("Codigo Fornecedor");
            Map(m => m.ProductCode).Name("Codigo Produto");
            Map(m => m.SaleValue).Name("Valor de Venda");
            Map(m => m.CostValue).Name("Valor de Custo");
            Map(m => m.Quantity).Name("Quantidade");
            Map(m => m.Weight).Name("Peso");
            Map(m => m.Number).Name("Numero Lote");
            Map(m => m.Date).Name("Data Lote").Validate(field => DateTime.TryParse(field.Field, out _));
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
        public IList<Guid> Tags { get; init; }
    }

    public class ProductDTOValidation : AbstractValidator<ProductDTO>
    {
        public ProductDTOValidation()
        {
            RuleFor(it => it.Sku).NotEmpty().MinimumLength(5);
            RuleFor(it => it.Description).NotEmpty().MinimumLength(5);
            RuleFor(it => it.Tags).NotEmpty();
        }
    }
    public record ProductSupplierRequest
    {
        public string Code { get; set; }
        public Guid SupplierId { get; set; }
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
        public DateTime Date { get; init; }
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
        public decimal CostPrice { get; init; }
        public decimal SalePrice { get; init; }
        public decimal Quantity { get; init; }
        public decimal? Weight { get; init; }
        public IList<Guid> Suppliers { get; init; }
        public DateTime Date { get; init; }
        public string Number { get; init; }
    }


    public class BatchDTOValidation : AbstractValidator<LotRequest>
    {
        public BatchDTOValidation()
        {
            RuleFor(it => it.ProductId).NotEqual(Guid.Empty);
            RuleFor(it => it.CostPrice).GreaterThan(0);
            RuleFor(it => it.SalePrice).GreaterThan(it => it.CostPrice);
            RuleFor(it => it.Quantity).GreaterThan(0);
            RuleFor(it => it.Number).NotEmpty();
            RuleFor(it => it.Suppliers).NotEmpty();
        }
    }

}
