using FluentResults;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WebApi.Aplication.Stock.Commands;

namespace WebApi.Controllers
{
    public interface IFileBatchLotParser
    {
        Task<List<Result<CreateLotByProductExternalCodeCommand>>> Parser(Stream file);
    }

}
