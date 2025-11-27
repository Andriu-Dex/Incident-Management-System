using IncidentsTI.Application.DTOs.Notifications;
using IncidentsTI.Application.Queries;
using IncidentsTI.Domain.Enums;
using IncidentsTI.Domain.Interfaces;
using MediatR;

namespace IncidentsTI.Application.Handlers;

public class GetUserNotificationsQueryHandler : IRequestHandler<GetUserNotificationsQuery, List<NotificationDto>>
{
    private readonly INotificationRepository _notificationRepository;

    public GetUserNotificationsQueryHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<List<NotificationDto>> Handle(GetUserNotificationsQuery request, CancellationToken cancellationToken)
    {
        var (notifications, _) = await _notificationRepository.GetPaginatedAsync(request.UserId, 1, request.PageSize);

        return notifications.Select(n => new NotificationDto
        {
            Id = n.Id,
            Title = n.Title,
            Message = n.Message,
            TypeName = GetTypeName(n.Type),
            TypeIcon = GetTypeIcon(n.Type),
            TypeClass = GetTypeClass(n.Type),
            IsRead = n.IsRead,
            TimeAgo = GetTimeAgo(n.CreatedAt),
            ActionUrl = n.ActionUrl,
            CreatedAt = n.CreatedAt
        }).ToList();
    }

    private static string GetTypeName(NotificationType type)
    {
        return type switch
        {
            NotificationType.IncidentCreated => "Nuevo Incidente",
            NotificationType.StatusChanged => "Cambio de Estado",
            NotificationType.IncidentAssigned => "Asignación",
            NotificationType.IncidentReassigned => "Reasignación",
            NotificationType.IncidentEscalated => "Escalamiento",
            NotificationType.IncidentResolved => "Resolución",
            NotificationType.IncidentClosed => "Cierre",
            NotificationType.CommentAdded => "Comentario",
            NotificationType.ArticleLinked => "Artículo Vinculado",
            NotificationType.SystemMessage => "Sistema",
            _ => "Notificación"
        };
    }

    private static string GetTypeIcon(NotificationType type)
    {
        return type switch
        {
            NotificationType.IncidentCreated => "bi-plus-circle-fill",
            NotificationType.StatusChanged => "bi-arrow-repeat",
            NotificationType.IncidentAssigned => "bi-person-fill-add",
            NotificationType.IncidentReassigned => "bi-person-fill-gear",
            NotificationType.IncidentEscalated => "bi-arrow-up-circle-fill",
            NotificationType.IncidentResolved => "bi-check-circle-fill",
            NotificationType.IncidentClosed => "bi-x-circle-fill",
            NotificationType.CommentAdded => "bi-chat-left-text-fill",
            NotificationType.ArticleLinked => "bi-journal-bookmark-fill",
            NotificationType.SystemMessage => "bi-gear-fill",
            _ => "bi-bell-fill"
        };
    }

    private static string GetTypeClass(NotificationType type)
    {
        return type switch
        {
            NotificationType.IncidentCreated => "text-primary",
            NotificationType.StatusChanged => "text-info",
            NotificationType.IncidentAssigned => "text-success",
            NotificationType.IncidentReassigned => "text-warning",
            NotificationType.IncidentEscalated => "text-danger",
            NotificationType.IncidentResolved => "text-success",
            NotificationType.IncidentClosed => "text-secondary",
            NotificationType.CommentAdded => "text-info",
            NotificationType.ArticleLinked => "text-purple",
            NotificationType.SystemMessage => "text-dark",
            _ => "text-primary"
        };
    }

    private static string GetTimeAgo(DateTime dateTime)
    {
        var timeSpan = DateTime.UtcNow - dateTime;

        if (timeSpan.TotalMinutes < 1)
            return "Ahora";
        if (timeSpan.TotalMinutes < 60)
            return $"Hace {(int)timeSpan.TotalMinutes} min";
        if (timeSpan.TotalHours < 24)
            return $"Hace {(int)timeSpan.TotalHours} h";
        if (timeSpan.TotalDays < 7)
            return $"Hace {(int)timeSpan.TotalDays} d";
        if (timeSpan.TotalDays < 30)
            return $"Hace {(int)(timeSpan.TotalDays / 7)} sem";

        return dateTime.ToString("dd/MM/yyyy");
    }
}
