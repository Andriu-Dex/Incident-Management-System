using IncidentsTI.Application.DTOs;
using MediatR;

namespace IncidentsTI.Application.Queries;

/// <summary>
/// Query para obtener los incidentes disponibles para reclamar.
/// Filtra según el nivel de soporte del usuario:
/// - Incidentes nuevos (sin nivel): visibles para todos
/// - Incidentes escalados: visibles solo si el usuario tiene nivel >= al requerido
/// </summary>
public class GetAvailableIncidentsQuery : IRequest<IEnumerable<IncidentDto>>
{
    /// <summary>
    /// ID del usuario que solicita los incidentes disponibles.
    /// Se usa para filtrar según su nivel de soporte.
    /// </summary>
    public string UserId { get; set; } = string.Empty;
}
