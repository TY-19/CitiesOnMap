using CitiesOnMap.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CitiesOnMap.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ImportController(
    IImportService importService
) : ControllerBase
{
    [HttpPost("countries")]
    public async Task<ActionResult> ImportCountries(IFormFile csvFile, CancellationToken cancellationToken)
    {
        await using Stream stream = csvFile.OpenReadStream();
        string result = await importService.ImportCountriesAsync(stream, cancellationToken);
        return Ok(new { result });
    }

    [HttpPost("cities")]
    public async Task<ActionResult> ImportCities(IFormFile csvFile, CancellationToken cancellationToken)
    {
        await using Stream stream = csvFile.OpenReadStream();
        string result = await importService.ImportCitiesAsync(stream, cancellationToken);
        return Ok(new { result });
    }
}