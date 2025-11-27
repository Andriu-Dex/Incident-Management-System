using IncidentsTI.Application.Commands;
using IncidentsTI.Application.DTOs;
using IncidentsTI.Application.Services;
using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace IncidentsTI.Application.Handlers;

public class CreateIncidentCommandHandler : IRequestHandler<CreateIncidentCommand, IncidentDto>
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IIncidentHistoryService _historyService;
    private readonly INotificationService _notificationService;

    public CreateIncidentCommandHandler(
        IIncidentRepository incidentRepository,
        IServiceRepository serviceRepository,
        UserManager<ApplicationUser> userManager,
        IIncidentHistoryService historyService,
        INotificationService notificationService)
    {
        _incidentRepository = incidentRepository;
        _serviceRepository = serviceRepository;
        _userManager = userManager;
        _historyService = historyService;
        _notificationService = notificationService;
    }

    public async Task<IncidentDto> Handle(CreateIncidentCommand request, CancellationToken cancellationToken)
    {
        // Generar número de ticket
        var lastTicketNumber = await _incidentRepository.GetLastTicketNumberAsync();
        var ticketNumber = GenerateTicketNumber(lastTicketNumber);

        var incident = new Incident
        {
            TicketNumber = ticketNumber,
            UserId = request.UserId,
            ServiceId = request.ServiceId,
            Type = request.Type,
            Priority = request.Priority,
            Status = Domain.Enums.IncidentStatus.Open,
            Title = request.Title,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        var createdIncident = await _incidentRepository.AddAsync(incident);
        
        // Registrar en el historial
        await _historyService.RecordCreation(createdIncident.Id, request.UserId);
        
        // Cargar relaciones para la notificación
        createdIncident.User = await _userManager.FindByIdAsync(createdIncident.UserId);
        createdIncident.Service = await _serviceRepository.GetByIdAsync(createdIncident.ServiceId);
        
        // Enviar notificación a técnicos y administradores
        await _notificationService.NotifyIncidentCreatedAsync(createdIncident);
        
        // Cargar relaciones para el DTO
        var user = createdIncident.User;
        var service = createdIncident.Service;

        return new IncidentDto
        {
            Id = createdIncident.Id,
            TicketNumber = createdIncident.TicketNumber,
            UserId = createdIncident.UserId,
            UserName = user != null ? $"{user.FirstName} {user.LastName}" : "",
            UserEmail = user?.Email ?? "",
            ServiceId = createdIncident.ServiceId,
            ServiceName = service?.Name ?? "",
            ServiceCategory = service?.Category.ToString() ?? "",
            Type = createdIncident.Type,
            TypeName = GetTypeName(createdIncident.Type),
            Priority = createdIncident.Priority,
            PriorityName = GetPriorityName(createdIncident.Priority),
            Status = createdIncident.Status,
            StatusName = GetStatusName(createdIncident.Status),
            Title = createdIncident.Title,
            Description = createdIncident.Description,
            CreatedAt = createdIncident.CreatedAt,
            UpdatedAt = createdIncident.UpdatedAt
        };
    }

    private string GenerateTicketNumber(string? lastTicketNumber)
    {
        var year = DateTime.UtcNow.Year;
        var prefix = $"INC-{year}-";

        if (string.IsNullOrEmpty(lastTicketNumber) || !lastTicketNumber.StartsWith(prefix))
        {
            return $"{prefix}0001";
        }

        var lastNumberPart = lastTicketNumber.Split('-').Last();
        if (int.TryParse(lastNumberPart, out int lastNumber))
        {
            return $"{prefix}{(lastNumber + 1):D4}";
        }

        return $"{prefix}0001";
    }

    private string GetTypeName(Domain.Enums.IncidentType type)
    {
        return type switch
        {
            Domain.Enums.IncidentType.Failure => "Falla",
            Domain.Enums.IncidentType.Query => "Consulta",
            Domain.Enums.IncidentType.Request => "Requerimiento",
            _ => type.ToString()
        };
    }

    private string GetPriorityName(Domain.Enums.IncidentPriority priority)
    {
        return priority switch
        {
            Domain.Enums.IncidentPriority.Low => "Baja",
            Domain.Enums.IncidentPriority.Medium => "Media",
            Domain.Enums.IncidentPriority.High => "Alta",
            Domain.Enums.IncidentPriority.Critical => "Crítica",
            _ => priority.ToString()
        };
    }

    private string GetStatusName(Domain.Enums.IncidentStatus status)
    {
        return status switch
        {
            Domain.Enums.IncidentStatus.Open => "Abierto",
            Domain.Enums.IncidentStatus.InProgress => "En Progreso",
            Domain.Enums.IncidentStatus.Escalated => "Escalado",
            Domain.Enums.IncidentStatus.Resolved => "Resuelto",
            Domain.Enums.IncidentStatus.Closed => "Cerrado",
            _ => status.ToString()
        };
    }
}
