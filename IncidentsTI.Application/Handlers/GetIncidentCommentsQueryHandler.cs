using IncidentsTI.Application.DTOs;
using IncidentsTI.Application.Queries;
using IncidentsTI.Domain.Interfaces;
using MediatR;

namespace IncidentsTI.Application.Handlers;

public class GetIncidentCommentsQueryHandler : IRequestHandler<GetIncidentCommentsQuery, IEnumerable<IncidentCommentDto>>
{
    private readonly IIncidentCommentRepository _commentRepository;
    private readonly IUserRepository _userRepository;

    public GetIncidentCommentsQueryHandler(
        IIncidentCommentRepository commentRepository,
        IUserRepository userRepository)
    {
        _commentRepository = commentRepository;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<IncidentCommentDto>> Handle(GetIncidentCommentsQuery request, CancellationToken cancellationToken)
    {
        var comments = request.IncludeInternal
            ? await _commentRepository.GetByIncidentIdAsync(request.IncidentId)
            : await _commentRepository.GetPublicByIncidentIdAsync(request.IncidentId);

        var result = new List<IncidentCommentDto>();

        foreach (var comment in comments)
        {
            var userRole = await _userRepository.GetUserRoleAsync(comment.UserId);
            
            result.Add(new IncidentCommentDto
            {
                Id = comment.Id,
                IncidentId = comment.IncidentId,
                UserId = comment.UserId,
                UserName = $"{comment.User.FirstName} {comment.User.LastName}",
                UserRole = userRole ?? "Unknown",
                Content = comment.Content,
                IsInternal = comment.IsInternal,
                CreatedAt = comment.CreatedAt
            });
        }

        return result;
    }
}
