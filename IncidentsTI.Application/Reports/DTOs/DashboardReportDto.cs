using IncidentsTI.Application.Common;
using IncidentsTI.Application.DTOs.Statistics;

namespace IncidentsTI.Application.Reports.DTOs;

/// <summary>
/// DTO for dashboard report generation containing all statistics and metadata
/// </summary>
public class DashboardReportDto
{
    /// <summary>
    /// Report title
    /// </summary>
    public string Title { get; set; } = "Reporte de Estadísticas del Dashboard";
    
    /// <summary>
    /// Report subtitle or description
    /// </summary>
    public string Subtitle { get; set; } = string.Empty;
    
    /// <summary>
    /// Organization or company name
    /// </summary>
    public string OrganizationName { get; set; } = "Sistema de Gestión de Incidentes TI";
    
    /// <summary>
    /// Date range start for the report
    /// </summary>
    public DateTime StartDate { get; set; }
    
    /// <summary>
    /// Date range end for the report
    /// </summary>
    public DateTime EndDate { get; set; }
    
    /// <summary>
    /// When the report was generated (Ecuador timezone - UTC-5)
    /// </summary>
    public DateTime GeneratedAt { get; set; } = EcuadorTimeZone.Now;
    
    /// <summary>
    /// User who generated the report
    /// </summary>
    public string GeneratedBy { get; set; } = string.Empty;
    
    /// <summary>
    /// Dashboard statistics data
    /// </summary>
    public DashboardStatisticsDto Statistics { get; set; } = new();
    
    /// <summary>
    /// Sections to include in the report
    /// </summary>
    public ReportSections IncludeSections { get; set; } = new();
    
    /// <summary>
    /// Chart images as base64 encoded strings (optional, for visual charts)
    /// </summary>
    public Dictionary<string, string> ChartImages { get; set; } = new();
}

/// <summary>
/// Configuration for which sections to include in the report
/// </summary>
public class ReportSections
{
    public bool Summary { get; set; } = true;
    public bool KpiCards { get; set; } = true;
    public bool StatusChart { get; set; } = true;
    public bool PriorityChart { get; set; } = true;
    public bool TrendChart { get; set; } = true;
    public bool TechnicianTable { get; set; } = true;
    public bool ServiceTable { get; set; } = true;
    public bool TypeTable { get; set; } = true;
    public bool TimeMetrics { get; set; } = true;
}

/// <summary>
/// Request DTO for generating a dashboard report
/// </summary>
public class GenerateReportRequest
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string GeneratedBy { get; set; } = string.Empty;
    public ReportSections? IncludeSections { get; set; }
    public string Format { get; set; } = "pdf"; // pdf, excel
}
