//https://github.com/dotnet-architecture/eShopOnContainers/tree/dev/src/BuildingBlocks/EventBus

dotnet tool update --global dotnet-ef
dotnet ef migrations add --project .\Infra\Infra.csproj --startup-project .\TianaCli\TianaCli.csproj -o EF\Migrations\SqlLite --context TianaJoiasContextDB NOME_DA_MIGRATION


dotnet ef database update 0 --project .\Infra\Infra.csproj --startup-project .\WebApi\WebApi.csproj --context TianaJoiasContextDB

    /// <summary>
    ///  ef migrations add --project .\Src\Infra\Infra.csproj --startup-project .\Src\WebApi\WebApi.csproj -o EF\Migrations\SqlLite --context TianaJoiasContextDB InitialBackend
    /// </summary>


    // ADD MIGRATION
    /// dotnet ef migrations add --project .\Infra\Infra.csproj --startup-project .\TianaCli\TianaCli.csproj -o  Products\Migrations\SqlLite --context ProductContextDB Initial
    // UPDATE DB 
    /// dotnet ef database update --project .\Infra\Infra.csproj --startup-project .\TianaCli\TianaCli.csproj --context ProductContextDB

    // REMOVE LAST MIGRATION ADDED
    ///  dotnet ef migrations remove --project .\Infra\Infra.csproj --startup-project .\TianaCli\TianaCli.csproj --context ProductContextDB

    /// dotnet ef migrations add --project .\Infra\Infra.csproj --startup-project .\TianaCli\TianaCli.csproj -o  Application\Migrations\SqlLite --context ApplicationDbContext Initial  
    /// dotnet ef database update --project .\Infra\Infra.csproj --startup-project .\TianaCli\TianaCli.csproj  --context ApplicationDbContext  