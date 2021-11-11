using CsvHelper;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Aplication.Stock.Commands;

namespace WebApi.Controllers
{
    public class BatchLotParser : IFileBatchLotParser
    {
        public Task<List<Result<CreateLotByProductExternalCodeCommand>>> Parser(Stream file)
        {
            using var reader = new StreamReader(file);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.TypeConverterCache.AddConverter<DateTime>(new Teste());
            csv.Context.RegisterClassMap<FooMap>();
            var items = ReadItems(csv).ToList();
            return Task.FromResult(items);
        }
        private static IEnumerable<Result<CreateLotByProductExternalCodeCommand>> ReadItems(CsvReader csv)
        {
            var count = 1;
            while (csv.Read())
            {
                CreateLotByProductExternalCodeCommand record = null;
                FieldValidationException error = null;
                try
                {
                    count++;
                    record = csv.GetRecord<CreateLotByProductExternalCodeCommand>();
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

}
