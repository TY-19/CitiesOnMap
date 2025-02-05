using CitiesOnMap.Application.Features.Games.Models;
using CitiesOnMap.Domain.Entities;
using MediatR;

namespace CitiesOnMap.Application.Features.Games.Commands.PatchGameOptions;

public record PatchGameOptionsCommand(GameOptions Options, GameOptionsModel UpdatedOptions) : IRequest;
