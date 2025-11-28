namespace IncidentsTI.Application.DTOs.Statistics;

/// <summary>
/// DTO principal que contiene todas las estadísticas del dashboard
/// </summary>
public class DashboardStatisticsDto
{
    // KPIs principales
    public int TotalIncidents { get; set; }
    public int OpenIncidents { get; set; }
    public int InProgressIncidents { get; set; }
    public int ResolvedIncidents { get; set; }
    public int ClosedIncidents { get; set; }
    public int EscalatedIncidents { get; set; }
    public int UnassignedIncidents { get; set; }
    
    // Métricas de tiempo
    public double AverageResolutionTimeHours { get; set; }
    public double AverageFirstResponseTimeHours { get; set; }
    
    // Estadísticas por categorías
    public List<StatusStatDto> IncidentsByStatus { get; set; } = new();
    public List<PriorityStatDto> IncidentsByPriority { get; set; } = new();
    public List<ServiceStatDto> IncidentsByService { get; set; } = new();
    public List<TypeStatDto> IncidentsByType { get; set; } = new();
    public List<TechnicianStatDto> IncidentsByTechnician { get; set; } = new();
    
    // Tendencias
    public List<TrendDataDto> DailyTrend { get; set; } = new();
    public List<TrendDataDto> MonthlyTrend { get; set; } = new();
    
    // Período de análisis
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

/// <summary>
/// Estadísticas por estado de incidente
/// </summary>
public class StatusStatDto
{
    public string Status { get; set; } = string.Empty;
    public string StatusDisplay { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Percentage { get; set; }
    public string Color { get; set; } = string.Empty;
}

/// <summary>
/// Estadísticas por prioridad
/// </summary>
public class PriorityStatDto
{
    public string Priority { get; set; } = string.Empty;
    public string PriorityDisplay { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Percentage { get; set; }
    public string Color { get; set; } = string.Empty;
}

/// <summary>
/// Estadísticas por servicio
/// </summary>
public class ServiceStatDto
{
    public int ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int TotalIncidents { get; set; }
    public int OpenIncidents { get; set; }
    public int ResolvedIncidents { get; set; }
    public double AverageResolutionTimeHours { get; set; }
    public decimal Percentage { get; set; }
}

/// <summary>
/// Estadísticas por tipo de incidente
/// </summary>
public class TypeStatDto
{
    public string Type { get; set; } = string.Empty;
    public string TypeDisplay { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Percentage { get; set; }
    public string Color { get; set; } = string.Empty;
}

/// <summary>
/// Estadísticas por técnico
/// </summary>
public class TechnicianStatDto
{
    public string TechnicianId { get; set; } = string.Empty;
    public string TechnicianName { get; set; } = string.Empty;
    public int TotalAssigned { get; set; }
    public int OpenIncidents { get; set; }
    public int InProgressIncidents { get; set; }
    public int ResolvedIncidents { get; set; }
    public int ClosedIncidents { get; set; }
    public double AverageResolutionTimeHours { get; set; }
    public decimal ResolutionRate { get; set; }
}

/// <summary>
/// Datos de tendencia para gráficos de línea/área
/// </summary>
public class TrendDataDto
{
    public DateTime Date { get; set; }
    public string Label { get; set; } = string.Empty;
    public int Created { get; set; }
    public int Resolved { get; set; }
    public int Closed { get; set; }
    public int Open { get; set; }
}
