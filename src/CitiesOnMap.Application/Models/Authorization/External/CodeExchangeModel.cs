namespace CitiesOnMap.Application.Models.Authorization.External;

public class CodeExchangeModel
{
    public string Code { get; set; } = null!;
    public string CodeVerifier { get; set; } = null!;
}