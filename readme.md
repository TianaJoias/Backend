dotnet tool update --global dotnet-ef
dotnet ef migrations add --project .\Infra\Infra.csproj --startup-project .\WebApi\WebApi.csproj -o EF\Migrations\SqlLite --context TianaJoiasContextDB NOME_DA_MIGRATION


dotnet ef database update 0 --project .\Infra\Infra.csproj --startup-project .\WebApi\WebApi.csproj --context TianaJoiasContextDB

    /// <summary>
    ///  ef migrations add --project .\Src\Infra\Infra.csproj --startup-project .\Src\WebApi\WebApi.csproj -o EF\Migrations\SqlLite --context TianaJoiasContextDB InitialBackend
    /// </summary>