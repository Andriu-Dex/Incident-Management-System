using MediatR;

namespace IncidentsTI.Application.Commands;

/// <summary>
/// Comando para reclamar un incidente (sistema first-come-first-serve)
/// </summary>
public class ClaimIncidentCommand : IRequest<ClaimIncidentResult>
{
    /// <summary>
    /// ID del incidente a reclamar
    /// </summary>
    public int IncidentId { get; set; }
    
    /// <summary>
    /// ID del usuario que reclama el incidente (técnico o pasante)
    /// </summary>
    public string ClaimedByUserId { get; set; } = string.Empty;
}

/// <summary>
/// Resultado del comando de reclamo de incidente
/// </summary>
public class ClaimIncidentResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    
    public static ClaimIncidentResult Ok() => new() { Success = true };
    public static ClaimIncidentResult Fail(string message) => new() { Success = false, ErrorMessage = message };
}
