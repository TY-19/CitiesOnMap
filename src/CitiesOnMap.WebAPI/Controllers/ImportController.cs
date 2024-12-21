using CitiesOnMap.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CitiesOnMap.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ImportController(IImportService importService) : ControllerBase
{
    [HttpPost("countries")]
    public async Task<ActionResult> ImportCountries(IFormFile csvFile, CancellationToken cancellationToken)
    {
        await using Stream stream = csvFile.OpenReadStream();
        return Ok(await importService.ImportCountriesAsync(stream, cancellationToken));
    }

    [HttpPost("cities")]
    public async Task<ActionResult> ImportCities(IFormFile csvFile, CancellationToken cancellationToken)
    {
        await using Stream stream = csvFile.OpenReadStream();
        return Ok(await importService.ImportCitiesAsync(stream, cancellationToken));
    }
}