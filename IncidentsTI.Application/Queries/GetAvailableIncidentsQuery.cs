using IncidentsTI.Application.DTOs;
using MediatR;

namespace IncidentsTI.Application.Queries;

/// <summary>
/// Query para obtener los incidentes disponibles para reclamar
/// (estado Open y sin asignar)
/// </summary>
public class GetAvailableIncidentsQuery : IRequest<IEnumerable<IncidentDto>>
{
}
