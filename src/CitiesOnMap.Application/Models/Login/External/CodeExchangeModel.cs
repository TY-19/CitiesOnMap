namespace CitiesOnMap.Application.Models.Login.External;

public class CodeExchangeModel
{
    public string Code { get; set; } = null!;
    public string CodeVerifier { get; set; } = null!;
}