using CitiesOnMap.Application.Interfaces.Data;
using CitiesOnMap.Application.Interfaces.Services;
using CitiesOnMap.Domain.Entities;
using CitiesOnMap.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CitiesOnMap.Application.Services;

public class ImportService(IAppDbContext context) : IImportService
{
    public async Task<string> ImportCountriesAsync(Stream csvFileStream, CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(csvFileStream);
        var countries = new Dictionary<string, Country>();

        while (!reader.EndOfStream)
        {
            string? line = await reader.ReadLineAsync(cancellationToken);
            if (TryParseLine(line, out string[]? elements)
                && elements != null
                && !countries.ContainsKey(elements[4]))
            {
                countries.Add(elements[4], new Country
                {
                    Name = elements[4],
                    Iso2 = elements[5],
                    Iso3 = elements[6]
                });
            }
        }

        List<string> names = await context.Countries.Select(c => c.Name).ToListAsync(cancellationToken);
        List<Country> toAdd = countries.Where(kvp => !names.Contains(kvp.Key))
            .Select(kvp => kvp.Value)
            .ToList();
        await context.Countries.AddRangeAsync(toAdd, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return $"{toAdd.Count} countries has been imported";
    }

    public async Task<string> ImportCitiesAsync(Stream csvFileStream, CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(csvFileStream);
        int count = 0;
        while (!reader.EndOfStream)
        {
            string? line = await reader.ReadLineAsync(cancellationToken);
            if (!TryParseLine(line, out string[]? elements) || elements == null)
            {
                continue;
            }

            Country? country =
                await context.Countries.FirstOrDefaultAsync(c => c.Name == elements[4], cancellationToken);
            if (country == null)
            {
                country = new Country
                {
                    Name = elements[4],
                    Iso2 = elements[5],
                    Iso3 = elements[6]
                };
                await context.Countries.AddAsync(country, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);
            }

            var city = new City
            {
                Id = elements[10],
                Name = elements[0],
                NameAscii = elements[1],
                Latitude = decimal.Parse(elements[2]),
                Longitude = decimal.Parse(elements[3]),
                Country = country,
                AdministrationName = elements[7],
                CapitalType = GetCapitalType(elements[8]),
                Population = GetPopulation(elements[9])
            };
            await context.Cities.AddAsync(city, cancellationToken);
            count++;
        }

        await context.SaveChangesAsync(cancellationToken);
        return $"{count} cities has been imported";
    }

    private static bool TryParseLine(string? line, out string[]? elements)
    {
        elements = null;
        if (line == null)
        {
            return false;
        }

        elements = line.Replace("\"", "").Split(',');
        return IsValid(elements) && !IsTitle(elements);
    }

    private static bool IsValid(string[] values)
    {
        return values.Length == 11;
    }

    private static bool IsTitle(string[] values)
    {
        return values[0] == "city";
    }

    private static CapitalType GetCapitalType(string type)
    {
        return type switch
        {
            "primary" => CapitalType.Primary,
            "admin" => CapitalType.Admin,
            "minor" => CapitalType.Minor,
            _ => CapitalType.Undefined
        };
    }

    private static int GetPopulation(string value)
    {
        if (int.TryParse(value, out int result))
        {
            return result;
        }

        if (decimal.TryParse(value, out decimal resultDec))
        {
            return (int)resultDec;
        }

        return 0;
    }
}