using CitiesOnMap.Application.Features.Games.Models;
using CitiesOnMap.Domain.Entities;
using MediatR;

namespace CitiesOnMap.Application.Features.Games.Commands.CheckAnswer;

public record CheckAnswerCommand(Game Game, AnswerModel Answer) : IRequest<AnswerResultModel>;
