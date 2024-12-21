namespace CitiesOnMap.Application.Interfaces;

public interface IImportService
{
    Task<string> ImportCountriesAsync(Stream csvFileStream, CancellationToken cancellationToken);
    Task<string> ImportCitiesAsync(Stream csvFileStream, CancellationToken cancellationToken);
}