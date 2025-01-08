using CitiesOnMap.Application.Commands.Games.SaveGame;
using CitiesOnMap.Application.Common.Results;
using CitiesOnMap.Application.Models.Game;
using CitiesOnMap.Application.Queries.Games.GetGame;
using CitiesOnMap.Application.Queries.Games.GetNextCity;
using CitiesOnMap.Application.Services;
using CitiesOnMap.Domain.Entities;
using CitiesOnMap.Domain.Enums;
using MediatR;
using NSubstitute;

namespace CitiesOnMap.Application.Tests.Services;

public class GameServiceTests
{
    [Fact]
    public async Task StartNewGameAsync_ShouldWork()
    {
        GameService service = GetGameService();

        OperationResult<GameModel> result = await service.StartNewGameAsync("1234", CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.Payload);
        Assert.True(result.Succeeded);
        Assert.Multiple(
            () => Assert.NotNull(result.Payload.Id),
            () => Assert.NotNull(result.Payload.PlayerId),
            () => Assert.Equal(0, result.Payload.Points),
            () => Assert.Null(result.Payload.CurrentCityName)
        );
    }

    [Fact]
    public async Task GetNextQuestionAsync_ShouldWork()
    {
        Game game = new()
        {
            Id = "1234",
            PlayerId = "player"
        };
        City city = new()
        {
            Name = "Test"
        };
        var mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<GetGameRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Game?>(game));
        mediator.Send(Arg.Any<GetNextCityRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<City?>(city));
        mediator.Send(Arg.Any<SaveGameCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        GameService service = GetGameService(mediator);

        OperationResult<GameModel> result = await service.GetNextQuestionAsync("1234", CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.Payload);
        Assert.True(result.Succeeded);
        Assert.Multiple(
            () => Assert.NotNull(result.Payload.Id),
            () => Assert.NotNull(result.Payload.PlayerId),
            () => Assert.Equal(0, result.Payload.Points),
            () => Assert.NotNull(result.Payload.CurrentCityName)
        );
    }

    [Fact]
    public async Task ProcessAnswerAsync_ShouldWork()
    {
        Game game = new()
        {
            Id = "1234",
            PlayerId = "player",
            Points = 100,
            CurrentCity = new City
            {
                Name = "Test",
                Latitude = 0,
                Longitude = 0,
                CapitalType = CapitalType.Primary,
                Country = new Country
                {
                    Id = 1001,
                    Iso2 = "TT",
                    Iso3 = "TTT",
                    Name = "Test Country"
                }
            }
        };
        var mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<GetGameRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Game?>(game));
        mediator.Send(Arg.Any<SaveGameCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        GameService service = GetGameService(mediator);
        var answer = new AnswerModel
        {
            GameId = "1234",
            SelectedLatitude = 10,
            SelectedLongitude = 10
        };

        OperationResult<AnswerResultModel> result = await service.ProcessAnswerAsync(answer, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.Payload);
        Assert.True(result.Succeeded);
        Assert.Multiple(
            () => Assert.NotNull(result.Payload.City),
            () => Assert.NotNull(result.Payload.City.Country),
            () => Assert.NotNull(result.Payload.Answer),
            () => Assert.Equal(3432, result.Payload.Points),
            () => Assert.InRange(result.Payload.Distance, 1568, 1569)
        );
    }

    private static GameService GetGameService(IMediator? mediator = null)
    {
        if (mediator != null)
        {
            return new GameService(mediator);
        }

        mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<SaveGameCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        return new GameService(mediator);
    }
}