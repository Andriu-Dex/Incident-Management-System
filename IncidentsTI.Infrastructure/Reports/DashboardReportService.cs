using ClosedXML.Excel;
using IncidentsTI.Application.Reports.DTOs;
using IncidentsTI.Application.Reports.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace IncidentsTI.Infrastructure.Reports;

/// <summary>
/// Service for generating dashboard reports using QuestPDF
/// </summary>
public class DashboardReportService : IReportService
{
    // Brand colors
    private static readonly string PrimaryColor = "#3B82F6";    // Blue
    private static readonly string SecondaryColor = "#6366F1";  // Indigo
    private static readonly string SuccessColor = "#10B981";    // Green
    private static readonly string WarningColor = "#F59E0B";    // Amber
    private static readonly string DangerColor = "#EF4444";     // Red
    private static readonly string TextColor = "#1F2937";       // Gray-800
    private static readonly string LightGray = "#F3F4F6";       // Gray-100
    private static readonly string MediumGray = "#9CA3AF";      // Gray-400

    public Task<byte[]> GenerateDashboardPdfAsync(DashboardReportDto reportData, CancellationToken cancellationToken = default)
    {
        // Configure QuestPDF license (Community license for open source)
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(10).FontColor(TextColor));

                // Header
                page.Header().Element(c => ComposeHeader(c, reportData));

                // Content
                page.Content().Element(c => ComposeContent(c, reportData));

                // Footer
                page.Footer().Element(c => ComposeFooter(c, reportData));
            });
        });

        var pdfBytes = document.GeneratePdf();
        return Task.FromResult(pdfBytes);
    }

    public Task<byte[]> GenerateDashboardExcelAsync(DashboardReportDto reportData, CancellationToken cancellationToken = default)
    {
        using var workbook = new XLWorkbook();
        var stats = reportData.Statistics;

        // ===== Hoja 1: Resumen =====
        if (reportData.IncludeSections.Summary || reportData.IncludeSections.KpiCards)
        {
            var summarySheet = workbook.Worksheets.Add("Resumen");
            var row = 1;

            // Título
            summarySheet.Cell(row, 1).Value = reportData.OrganizationName;
            summarySheet.Cell(row, 1).Style.Font.Bold = true;
            summarySheet.Cell(row, 1).Style.Font.FontSize = 16;
            summarySheet.Range(row, 1, row, 4).Merge();
            row++;

            summarySheet.Cell(row, 1).Value = reportData.Title;
            summarySheet.Cell(row, 1).Style.Font.Bold = true;
            summarySheet.Cell(row, 1).Style.Font.FontSize = 14;
            summarySheet.Range(row, 1, row, 4).Merge();
            row++;

            summarySheet.Cell(row, 1).Value = $"Período: {reportData.StartDate:dd/MM/yyyy} - {reportData.EndDate:dd/MM/yyyy}";
            summarySheet.Range(row, 1, row, 4).Merge();
            row++;

            summarySheet.Cell(row, 1).Value = $"Generado: {reportData.GeneratedAt:dd/MM/yyyy HH:mm} por {reportData.GeneratedBy}";
            summarySheet.Cell(row, 1).Style.Font.FontColor = XLColor.Gray;
            summarySheet.Range(row, 1, row, 4).Merge();
            row += 2;

            // KPIs
            if (reportData.IncludeSections.KpiCards)
            {
                summarySheet.Cell(row, 1).Value = "Indicadores Clave (KPIs)";
                summarySheet.Cell(row, 1).Style.Font.Bold = true;
                summarySheet.Cell(row, 1).Style.Font.FontSize = 12;
                summarySheet.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#3B82F6");
                summarySheet.Cell(row, 1).Style.Font.FontColor = XLColor.White;
                summarySheet.Range(row, 1, row, 2).Merge();
                row++;

                var kpis = new[]
                {
                    ("Total Incidentes", stats.TotalIncidents),
                    ("Abiertos", stats.OpenIncidents),
                    ("En Progreso", stats.InProgressIncidents),
                    ("Resueltos", stats.ResolvedIncidents),
                    ("Cerrados", stats.ClosedIncidents),
                    ("Escalados", stats.EscalatedIncidents),
                    ("Sin Asignar", stats.UnassignedIncidents)
                };

                foreach (var (label, value) in kpis)
                {
                    summarySheet.Cell(row, 1).Value = label;
                    summarySheet.Cell(row, 2).Value = value;
                    summarySheet.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    row++;
                }
                row++;
            }

            // Métricas de Tiempo
            if (reportData.IncludeSections.TimeMetrics)
            {
                summarySheet.Cell(row, 1).Value = "Métricas de Tiempo";
                summarySheet.Cell(row, 1).Style.Font.Bold = true;
                summarySheet.Cell(row, 1).Style.Font.FontSize = 12;
                summarySheet.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#6366F1");
                summarySheet.Cell(row, 1).Style.Font.FontColor = XLColor.White;
                summarySheet.Range(row, 1, row, 2).Merge();
                row++;

                summarySheet.Cell(row, 1).Value = "Tiempo Promedio de Resolución (horas)";
                summarySheet.Cell(row, 2).Value = Math.Round(stats.AverageResolutionTimeHours, 2);
                row++;

                summarySheet.Cell(row, 1).Value = "Tiempo Promedio Primera Respuesta (horas)";
                summarySheet.Cell(row, 2).Value = Math.Round(stats.AverageFirstResponseTimeHours, 2);
                row++;
            }

            summarySheet.Columns().AdjustToContents();
        }

        // ===== Hoja 2: Por Estado =====
        if (reportData.IncludeSections.StatusChart && stats.IncidentsByStatus?.Any() == true)
        {
            var statusSheet = workbook.Worksheets.Add("Por Estado");
            AddTableWithHeader(statusSheet, "Distribución por Estado", 
                new[] { "Estado", "Cantidad", "Porcentaje" },
                stats.IncidentsByStatus.Select(s => new object[] 
                { 
                    s.StatusDisplay ?? s.Status, 
                    s.Count, 
                    $"{s.Percentage:F1}%" 
                }).ToList());
        }

        // ===== Hoja 3: Por Prioridad =====
        if (reportData.IncludeSections.PriorityChart && stats.IncidentsByPriority?.Any() == true)
        {
            var prioritySheet = workbook.Worksheets.Add("Por Prioridad");
            AddTableWithHeader(prioritySheet, "Distribución por Prioridad",
                new[] { "Prioridad", "Cantidad", "Porcentaje" },
                stats.IncidentsByPriority.Select(p => new object[]
                {
                    p.PriorityDisplay ?? p.Priority,
                    p.Count,
                    $"{p.Percentage:F1}%"
                }).ToList());
        }

        // ===== Hoja 4: Por Técnico =====
        if (reportData.IncludeSections.TechnicianTable && stats.IncidentsByTechnician?.Any() == true)
        {
            var techSheet = workbook.Worksheets.Add("Por Técnico");
            AddTableWithHeader(techSheet, "Rendimiento por Técnico",
                new[] { "Técnico", "Asignados", "Resueltos", "Tiempo Prom. (h)", "Tasa Resolución" },
                stats.IncidentsByTechnician.Select(t => new object[]
                {
                    t.TechnicianName,
                    t.TotalAssigned,
                    t.ResolvedIncidents,
                    Math.Round(t.AverageResolutionTimeHours, 2),
                    $"{t.ResolutionRate:F1}%"
                }).ToList());
        }

        // ===== Hoja 5: Por Servicio =====
        if (reportData.IncludeSections.ServiceTable && stats.IncidentsByService?.Any() == true)
        {
            var serviceSheet = workbook.Worksheets.Add("Por Servicio");
            AddTableWithHeader(serviceSheet, "Incidentes por Servicio",
                new[] { "Servicio", "Categoría", "Total", "Abiertos", "Resueltos", "Tiempo Prom. (h)" },
                stats.IncidentsByService.Select(s => new object[]
                {
                    s.ServiceName,
                    s.Category,
                    s.TotalIncidents,
                    s.OpenIncidents,
                    s.ResolvedIncidents,
                    Math.Round(s.AverageResolutionTimeHours, 2)
                }).ToList());
        }

        // ===== Hoja 6: Por Tipo =====
        if (reportData.IncludeSections.TypeTable && stats.IncidentsByType?.Any() == true)
        {
            var typeSheet = workbook.Worksheets.Add("Por Tipo");
            AddTableWithHeader(typeSheet, "Incidentes por Tipo",
                new[] { "Tipo", "Cantidad", "Porcentaje" },
                stats.IncidentsByType.Select(t => new object[]
                {
                    t.TypeDisplay ?? t.Type,
                    t.Count,
                    $"{t.Percentage:F1}%"
                }).ToList());
        }

        // ===== Hoja 7: Tendencias =====
        if (reportData.IncludeSections.TrendChart)
        {
            var trendSheet = workbook.Worksheets.Add("Tendencias");
            var trendData = stats.DailyTrend?.Any() == true ? stats.DailyTrend : stats.MonthlyTrend;
            
            if (trendData?.Any() == true)
            {
                AddTableWithHeader(trendSheet, "Tendencia de Incidentes",
                    new[] { "Fecha", "Creados", "Resueltos", "Cerrados", "Abiertos" },
                    trendData.Select(t => new object[]
                    {
                        t.Date.ToString("dd/MM/yyyy"),
                        t.Created,
                        t.Resolved,
                        t.Closed,
                        t.Open
                    }).ToList());
            }
        }

        // Guardar a memoria
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return Task.FromResult(stream.ToArray());
    }

    private void AddTableWithHeader(IXLWorksheet sheet, string title, string[] headers, List<object[]> data)
    {
        var row = 1;

        // Título
        sheet.Cell(row, 1).Value = title;
        sheet.Cell(row, 1).Style.Font.Bold = true;
        sheet.Cell(row, 1).Style.Font.FontSize = 14;
        sheet.Range(row, 1, row, headers.Length).Merge();
        row += 2;

        // Encabezados
        for (int i = 0; i < headers.Length; i++)
        {
            sheet.Cell(row, i + 1).Value = headers[i];
            sheet.Cell(row, i + 1).Style.Font.Bold = true;
            sheet.Cell(row, i + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#3B82F6");
            sheet.Cell(row, i + 1).Style.Font.FontColor = XLColor.White;
            sheet.Cell(row, i + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        }
        row++;

        // Datos
        foreach (var dataRow in data)
        {
            for (int i = 0; i < dataRow.Length; i++)
            {
                sheet.Cell(row, i + 1).Value = dataRow[i]?.ToString() ?? "";
                sheet.Cell(row, i + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                
                // Alinear números a la derecha
                if (dataRow[i] is int or double or decimal)
                {
                    sheet.Cell(row, i + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }
            }
            row++;
        }

        sheet.Columns().AdjustToContents();
    }

    private void ComposeHeader(IContainer container, DashboardReportDto report)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item()
                    .Text(report.OrganizationName)
                    .FontSize(20)
                    .Bold()
                    .FontColor(PrimaryColor);

                column.Item()
                    .Text(report.Title)
                    .FontSize(16)
                    .SemiBold()
                    .FontColor(TextColor);

                if (!string.IsNullOrEmpty(report.Subtitle))
                {
                    column.Item()
                        .Text(report.Subtitle)
                        .FontSize(11)
                        .FontColor(MediumGray);
                }

                column.Item()
                    .PaddingTop(5)
                    .Text($"Período: {report.StartDate:dd/MM/yyyy} - {report.EndDate:dd/MM/yyyy}")
                    .FontSize(10)
                    .FontColor(MediumGray);
            });

            row.ConstantItem(100).Column(column =>
            {
                column.Item()
                    .AlignRight()
                    .Text(report.GeneratedAt.ToString("dd/MM/yyyy"))
                    .FontSize(10)
                    .FontColor(MediumGray);

                column.Item()
                    .AlignRight()
                    .Text(report.GeneratedAt.ToString("HH:mm"))
                    .FontSize(9)
                    .FontColor(MediumGray);
            });
        });

        container.PaddingBottom(15);
    }

    private void ComposeContent(IContainer container, DashboardReportDto report)
    {
        container.Column(column =>
        {
            // Executive Summary
            if (report.IncludeSections.Summary)
            {
                column.Item().Element(c => ComposeSummarySection(c, report));
            }

            // KPI Cards
            if (report.IncludeSections.KpiCards)
            {
                column.Item().Element(c => ComposeKpiSection(c, report));
            }

            // Time Metrics
            if (report.IncludeSections.TimeMetrics)
            {
                column.Item().Element(c => ComposeTimeMetricsSection(c, report));
            }

            // Status Distribution
            if (report.IncludeSections.StatusChart)
            {
                column.Item().Element(c => ComposeStatusSection(c, report));
            }

            // Priority Distribution
            if (report.IncludeSections.PriorityChart)
            {
                column.Item().Element(c => ComposePrioritySection(c, report));
            }

            // Technician Performance
            if (report.IncludeSections.TechnicianTable)
            {
                column.Item().Element(c => ComposeTechnicianSection(c, report));
            }

            // Services
            if (report.IncludeSections.ServiceTable)
            {
                column.Item().Element(c => ComposeServiceSection(c, report));
            }

            // Incident Types
            if (report.IncludeSections.TypeTable)
            {
                column.Item().Element(c => ComposeTypeSection(c, report));
            }
        });
    }

    private void ComposeSummarySection(IContainer container, DashboardReportDto report)
    {
        var stats = report.Statistics;
        var resolutionRate = stats.TotalIncidents > 0
            ? Math.Round((double)(stats.ResolvedIncidents + stats.ClosedIncidents) / stats.TotalIncidents * 100, 1)
            : 0;

        container.Padding(10).Column(column =>
        {
            column.Item().Text("Resumen Ejecutivo").FontSize(14).Bold().FontColor(PrimaryColor);
            column.Item().PaddingTop(10).BorderBottom(1).BorderColor(LightGray);

            column.Item().PaddingTop(10).Text(text =>
            {
                text.Span("Durante el período seleccionado, el sistema registró un total de ");
                text.Span($"{stats.TotalIncidents} incidentes").Bold();
                text.Span(", de los cuales ");
                text.Span($"{stats.ResolvedIncidents + stats.ClosedIncidents} ({resolutionRate}%)").Bold().FontColor(SuccessColor);
                text.Span(" fueron resueltos exitosamente. ");
                
                if (stats.EscalatedIncidents > 0)
                {
                    text.Span($"Se escalaron {stats.EscalatedIncidents} incidentes").FontColor(WarningColor);
                    text.Span(". ");
                }
                
                if (stats.UnassignedIncidents > 0)
                {
                    text.Span($"Actualmente hay {stats.UnassignedIncidents} incidentes sin asignar").FontColor(DangerColor);
                    text.Span(".");
                }
            });

            column.Item().PaddingTop(5).Text(text =>
            {
                text.Span("El tiempo promedio de resolución fue de ");
                text.Span($"{stats.AverageResolutionTimeHours:F1} horas").Bold();
                text.Span(" y el tiempo promedio de primera respuesta fue de ");
                text.Span($"{stats.AverageFirstResponseTimeHours:F1} horas").Bold();
                text.Span(".");
            });
        });
    }

    private void ComposeKpiSection(IContainer container, DashboardReportDto report)
    {
        var stats = report.Statistics;

        container.Padding(10).Column(column =>
        {
            column.Item().PaddingTop(15).Text("Indicadores Clave (KPIs)").FontSize(14).Bold().FontColor(PrimaryColor);
            column.Item().PaddingTop(10).BorderBottom(1).BorderColor(LightGray);

            column.Item().PaddingTop(10).Row(row =>
            {
                row.RelativeItem().Element(c => ComposeKpiCard(c, "Total Incidentes", stats.TotalIncidents.ToString(), PrimaryColor));
                row.ConstantItem(10);
                row.RelativeItem().Element(c => ComposeKpiCard(c, "Abiertos", stats.OpenIncidents.ToString(), WarningColor));
                row.ConstantItem(10);
                row.RelativeItem().Element(c => ComposeKpiCard(c, "En Progreso", stats.InProgressIncidents.ToString(), SecondaryColor));
                row.ConstantItem(10);
                row.RelativeItem().Element(c => ComposeKpiCard(c, "Resueltos", stats.ResolvedIncidents.ToString(), SuccessColor));
            });

            column.Item().PaddingTop(10).Row(row =>
            {
                row.RelativeItem().Element(c => ComposeKpiCard(c, "Cerrados", stats.ClosedIncidents.ToString(), "#6B7280"));
                row.ConstantItem(10);
                row.RelativeItem().Element(c => ComposeKpiCard(c, "Escalados", stats.EscalatedIncidents.ToString(), DangerColor));
                row.ConstantItem(10);
                row.RelativeItem().Element(c => ComposeKpiCard(c, "Sin Asignar", stats.UnassignedIncidents.ToString(), "#DC2626"));
                row.ConstantItem(10);
                row.RelativeItem(); // Empty space for alignment
            });
        });
    }

    private void ComposeKpiCard(IContainer container, string title, string value, string color)
    {
        container
            .Border(1)
            .BorderColor(LightGray)
            .Background(Colors.White)
            .Padding(10)
            .Column(column =>
            {
                column.Item()
                    .Text(value)
                    .FontSize(24)
                    .Bold()
                    .FontColor(color);

                column.Item()
                    .Text(title)
                    .FontSize(9)
                    .FontColor(MediumGray);
            });
    }

    private void ComposeTimeMetricsSection(IContainer container, DashboardReportDto report)
    {
        var stats = report.Statistics;

        container.Padding(10).Column(column =>
        {
            column.Item().PaddingTop(15).Text("Métricas de Tiempo").FontSize(14).Bold().FontColor(PrimaryColor);
            column.Item().PaddingTop(10).BorderBottom(1).BorderColor(LightGray);

            column.Item().PaddingTop(10).Row(row =>
            {
                row.RelativeItem()
                    .Border(1)
                    .BorderColor(LightGray)
                    .Padding(15)
                    .Column(col =>
                    {
                        col.Item().Text("Tiempo Promedio de Resolución").FontSize(10).FontColor(MediumGray);
                        col.Item().PaddingTop(5).Row(r =>
                        {
                            r.AutoItem().Text($"{stats.AverageResolutionTimeHours:F1}").FontSize(28).Bold().FontColor(PrimaryColor);
                            r.AutoItem().PaddingLeft(5).AlignBottom().Text("horas").FontSize(12).FontColor(MediumGray);
                        });
                    });

                row.ConstantItem(15);

                row.RelativeItem()
                    .Border(1)
                    .BorderColor(LightGray)
                    .Padding(15)
                    .Column(col =>
                    {
                        col.Item().Text("Tiempo Promedio Primera Respuesta").FontSize(10).FontColor(MediumGray);
                        col.Item().PaddingTop(5).Row(r =>
                        {
                            r.AutoItem().Text($"{stats.AverageFirstResponseTimeHours:F1}").FontSize(28).Bold().FontColor(SecondaryColor);
                            r.AutoItem().PaddingLeft(5).AlignBottom().Text("horas").FontSize(12).FontColor(MediumGray);
                        });
                    });
            });
        });
    }

    private void ComposeStatusSection(IContainer container, DashboardReportDto report)
    {
        var stats = report.Statistics;
        if (stats.IncidentsByStatus == null || !stats.IncidentsByStatus.Any()) return;

        container.Padding(10).Column(column =>
        {
            column.Item().PaddingTop(15).Text("Distribución por Estado").FontSize(14).Bold().FontColor(PrimaryColor);
            column.Item().PaddingTop(10).BorderBottom(1).BorderColor(LightGray);

            column.Item().PaddingTop(10).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    header.Cell().Background(LightGray).Padding(8).Text("Estado").Bold();
                    header.Cell().Background(LightGray).Padding(8).AlignRight().Text("Cantidad").Bold();
                    header.Cell().Background(LightGray).Padding(8).AlignRight().Text("Porcentaje").Bold();
                });

                foreach (var status in stats.IncidentsByStatus.OrderByDescending(s => s.Count))
                {
                    var percentage = stats.TotalIncidents > 0
                        ? Math.Round((double)status.Count / stats.TotalIncidents * 100, 1)
                        : 0;

                    table.Cell().BorderBottom(1).BorderColor(LightGray).Padding(8).Text(status.Status);
                    table.Cell().BorderBottom(1).BorderColor(LightGray).Padding(8).AlignRight().Text(status.Count.ToString());
                    table.Cell().BorderBottom(1).BorderColor(LightGray).Padding(8).AlignRight().Text($"{percentage}%");
                }
            });
        });
    }

    private void ComposePrioritySection(IContainer container, DashboardReportDto report)
    {
        var stats = report.Statistics;
        if (stats.IncidentsByPriority == null || !stats.IncidentsByPriority.Any()) return;

        container.Padding(10).Column(column =>
        {
            column.Item().PaddingTop(15).Text("Distribución por Prioridad").FontSize(14).Bold().FontColor(PrimaryColor);
            column.Item().PaddingTop(10).BorderBottom(1).BorderColor(LightGray);

            column.Item().PaddingTop(10).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    header.Cell().Background(LightGray).Padding(8).Text("Prioridad").Bold();
                    header.Cell().Background(LightGray).Padding(8).AlignRight().Text("Cantidad").Bold();
                    header.Cell().Background(LightGray).Padding(8).AlignRight().Text("Porcentaje").Bold();
                });

                var priorityOrder = new[] { "Crítica", "Alta", "Media", "Baja" };
                var orderedPriorities = stats.IncidentsByPriority
                    .OrderBy(p => Array.IndexOf(priorityOrder, p.Priority) >= 0 
                        ? Array.IndexOf(priorityOrder, p.Priority) 
                        : 99);

                foreach (var priority in orderedPriorities)
                {
                    var percentage = stats.TotalIncidents > 0
                        ? Math.Round((double)priority.Count / stats.TotalIncidents * 100, 1)
                        : 0;

                    var color = priority.Priority switch
                    {
                        "Crítica" => DangerColor,
                        "Alta" => WarningColor,
                        "Media" => SecondaryColor,
                        "Baja" => SuccessColor,
                        _ => TextColor
                    };

                    table.Cell().BorderBottom(1).BorderColor(LightGray).Padding(8).Text(priority.Priority).FontColor(color);
                    table.Cell().BorderBottom(1).BorderColor(LightGray).Padding(8).AlignRight().Text(priority.Count.ToString());
                    table.Cell().BorderBottom(1).BorderColor(LightGray).Padding(8).AlignRight().Text($"{percentage}%");
                }
            });
        });
    }

    private void ComposeTechnicianSection(IContainer container, DashboardReportDto report)
    {
        var stats = report.Statistics;
        if (stats.IncidentsByTechnician == null || !stats.IncidentsByTechnician.Any()) return;

        container.Padding(10).Column(column =>
        {
            column.Item().PaddingTop(15).Text("Rendimiento por Técnico").FontSize(14).Bold().FontColor(PrimaryColor);
            column.Item().PaddingTop(10).BorderBottom(1).BorderColor(LightGray);

            column.Item().PaddingTop(10).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    header.Cell().Background(LightGray).Padding(8).Text("Técnico").Bold();
                    header.Cell().Background(LightGray).Padding(8).AlignRight().Text("Asignados").Bold();
                    header.Cell().Background(LightGray).Padding(8).AlignRight().Text("Resueltos").Bold();
                    header.Cell().Background(LightGray).Padding(8).AlignRight().Text("Tasa Resolución").Bold();
                });

                foreach (var tech in stats.IncidentsByTechnician.OrderByDescending(t => t.ResolvedIncidents))
                {
                    var rateColor = (double)tech.ResolutionRate >= 80 ? SuccessColor :
                                   (double)tech.ResolutionRate >= 50 ? WarningColor : DangerColor;

                    table.Cell().BorderBottom(1).BorderColor(LightGray).Padding(8).Text(tech.TechnicianName);
                    table.Cell().BorderBottom(1).BorderColor(LightGray).Padding(8).AlignRight().Text(tech.TotalAssigned.ToString());
                    table.Cell().BorderBottom(1).BorderColor(LightGray).Padding(8).AlignRight().Text(tech.ResolvedIncidents.ToString());
                    table.Cell().BorderBottom(1).BorderColor(LightGray).Padding(8).AlignRight().Text($"{tech.ResolutionRate:F1}%").FontColor(rateColor);
                }
            });
        });
    }

    private void ComposeServiceSection(IContainer container, DashboardReportDto report)
    {
        var stats = report.Statistics;
        if (stats.IncidentsByService == null || !stats.IncidentsByService.Any()) return;

        container.Padding(10).Column(column =>
        {
            column.Item().PaddingTop(15).Text("Incidentes por Servicio").FontSize(14).Bold().FontColor(PrimaryColor);
            column.Item().PaddingTop(10).BorderBottom(1).BorderColor(LightGray);

            column.Item().PaddingTop(10).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    header.Cell().Background(LightGray).Padding(8).Text("Servicio").Bold();
                    header.Cell().Background(LightGray).Padding(8).AlignRight().Text("Cantidad").Bold();
                    header.Cell().Background(LightGray).Padding(8).AlignRight().Text("Porcentaje").Bold();
                });

                foreach (var service in stats.IncidentsByService.OrderByDescending(s => s.TotalIncidents).Take(10))
                {
                    var percentage = stats.TotalIncidents > 0
                        ? Math.Round((double)service.TotalIncidents / stats.TotalIncidents * 100, 1)
                        : 0;

                    table.Cell().BorderBottom(1).BorderColor(LightGray).Padding(8).Text(service.ServiceName);
                    table.Cell().BorderBottom(1).BorderColor(LightGray).Padding(8).AlignRight().Text(service.TotalIncidents.ToString());
                    table.Cell().BorderBottom(1).BorderColor(LightGray).Padding(8).AlignRight().Text($"{percentage}%");
                }
            });
        });
    }

    private void ComposeTypeSection(IContainer container, DashboardReportDto report)
    {
        var stats = report.Statistics;
        if (stats.IncidentsByType == null || !stats.IncidentsByType.Any()) return;

        container.Padding(10).Column(column =>
        {
            column.Item().PaddingTop(15).Text("Incidentes por Tipo").FontSize(14).Bold().FontColor(PrimaryColor);
            column.Item().PaddingTop(10).BorderBottom(1).BorderColor(LightGray);

            column.Item().PaddingTop(10).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    header.Cell().Background(LightGray).Padding(8).Text("Tipo").Bold();
                    header.Cell().Background(LightGray).Padding(8).AlignRight().Text("Cantidad").Bold();
                    header.Cell().Background(LightGray).Padding(8).AlignRight().Text("Porcentaje").Bold();
                });

                foreach (var type in stats.IncidentsByType.OrderByDescending(t => t.Count).Take(10))
                {
                    var percentage = stats.TotalIncidents > 0
                        ? Math.Round((double)type.Count / stats.TotalIncidents * 100, 1)
                        : 0;

                    table.Cell().BorderBottom(1).BorderColor(LightGray).Padding(8).Text(type.TypeDisplay ?? type.Type);
                    table.Cell().BorderBottom(1).BorderColor(LightGray).Padding(8).AlignRight().Text(type.Count.ToString());
                    table.Cell().BorderBottom(1).BorderColor(LightGray).Padding(8).AlignRight().Text($"{percentage}%");
                }
            });
        });
    }

    private void ComposeFooter(IContainer container, DashboardReportDto report)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item()
                    .Text($"Generado por: {report.GeneratedBy}")
                    .FontSize(8)
                    .FontColor(MediumGray);

                column.Item()
                    .Text($"Fecha de generación: {report.GeneratedAt:dd/MM/yyyy HH:mm:ss}")
                    .FontSize(8)
                    .FontColor(MediumGray);
            });

            row.RelativeItem().AlignRight().Column(column =>
            {
                column.Item()
                    .AlignRight()
                    .Text(text =>
                    {
                        text.Span("Página ").FontSize(8).FontColor(MediumGray);
                        text.CurrentPageNumber().FontSize(8).FontColor(MediumGray);
                        text.Span(" de ").FontSize(8).FontColor(MediumGray);
                        text.TotalPages().FontSize(8).FontColor(MediumGray);
                    });
            });
        });
    }
}
