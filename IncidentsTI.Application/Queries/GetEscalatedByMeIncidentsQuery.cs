using IncidentsTI.Application.DTOs;
using MediatR;

namespace IncidentsTI.Application.Queries;

/// <summary>
/// Query para obtener los incidentes que el usuario escaló.
/// Estos incidentes se muestran en modo solo lectura para seguimiento.
/// </summary>
public class GetEscalatedByMeIncidentsQuery : IRequest<IEnumerable<IncidentDto>>
{
    /// <summary>
    /// ID del usuario que escaló los incidentes.
    /// </summary>
    public string UserId { get; set; } = string.Empty;
}
