namespace CitiesOnMap.Application.Features.Authorization.Models.External;

public class CodeExchangeModel
{
    public string Code { get; set; } = null!;
    public string CodeVerifier { get; set; } = null!;
}
