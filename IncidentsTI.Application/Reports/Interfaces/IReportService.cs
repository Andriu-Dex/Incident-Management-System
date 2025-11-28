using IncidentsTI.Application.Reports.DTOs;

namespace IncidentsTI.Application.Reports.Interfaces;

/// <summary>
/// Interface for report generation services
/// </summary>
public interface IReportService
{
    /// <summary>
    /// Generates a PDF report for the dashboard statistics
    /// </summary>
    /// <param name="reportData">The data to include in the report</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>PDF file as byte array</returns>
    Task<byte[]> GenerateDashboardPdfAsync(DashboardReportDto reportData, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Generates an Excel report for the dashboard statistics
    /// </summary>
    /// <param name="reportData">The data to include in the report</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Excel file as byte array</returns>
    Task<byte[]> GenerateDashboardExcelAsync(DashboardReportDto reportData, CancellationToken cancellationToken = default);
}
