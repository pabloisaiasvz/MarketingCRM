using ProjectApprovalAPI.Data;
using ProjectApprovalAPI.DTOs;
using ProjectApprovalAPI.Models;
using ProjectApprovalAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ProjectApprovalAPI.DTOs.ProjectApprovalAPI.DTOs;

public class ProjectService : IProjectService
{
    private readonly ProjectApprovalDbContext _context;

    public ProjectService(ProjectApprovalDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> CreateProposalAsync(ProjectProposalDto dto)
    {
        if (await _context.ProjectProposals.AnyAsync(p => p.Title == dto.Title))
            throw new InvalidOperationException("Ya existe un proyecto con ese título.");

        var proposal = new ProjectProposal
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            AreaId = dto.AreaId,
            TypeId = dto.TypeId,
            EstimatedAmount = dto.EstimatedAmount,
            EstimatedDuration = dto.EstimatedDuration,
            StatusId = 1,
            CreateAt = DateTime.UtcNow,
            CreateById = dto.CreateById
        };

        _context.ProjectProposals.Add(proposal);

        var steps = await GenerateApprovalStepsAsync(proposal);
        _context.ProjectApprovalSteps.AddRange(steps);

        await _context.SaveChangesAsync();

        return proposal.Id;
    }

    private async Task<List<ProjectApprovalStep>> GenerateApprovalStepsAsync(ProjectProposal proposal)
    {
        var users = await _context.Users.ToListAsync();

        var rules = await _context.ApprovalRules
            .Where(r =>
                (r.MinAmount == 0 || proposal.EstimatedAmount >= r.MinAmount) &&
                (r.MaxAmount == 0 || proposal.EstimatedAmount <= r.MaxAmount) &&
                (r.AreaId == null || r.AreaId == proposal.AreaId) &&
                (r.TypeId == null || r.TypeId == proposal.TypeId)
            )
            .OrderBy(r => r.StepOrder)
            .ToListAsync();

        var steps = new List<ProjectApprovalStep>();
        var assignedUserIds = new HashSet<int>();

        foreach (var rule in rules)
        {
            var user = users.FirstOrDefault(u => u.RoleId == rule.ApproverRoleId && !assignedUserIds.Contains(u.Id));
            if (user == null)
                continue;

            assignedUserIds.Add(user.Id);

            steps.Add(new ProjectApprovalStep
            {
                ProjectProposalId = proposal.Id,
                StepOrder = rule.StepOrder,
                ApproverRoleId = rule.ApproverRoleId,
                ApproverUserId = user.Id,
                StatusId = 1
            });
        }

        return steps;
    }

    public async Task<List<ProjectProposal>> SearchAsync(string? title, int? statusId, int? creatorId, int? approverId)
    {
        var query = _context.ProjectProposals
            .Include(p => p.ApprovalSteps)
            .AsQueryable();

        if (!string.IsNullOrEmpty(title))
            query = query.Where(p => p.Title.Contains(title));

        if (statusId.HasValue)
            query = query.Where(p => p.StatusId == statusId);

        if (creatorId.HasValue)
            query = query.Where(p => p.CreateById == creatorId);

        if (approverId.HasValue)
            query = query.Where(p => p.ApprovalSteps.Any(a => a.ApproverUserId == approverId));

        return await query.ToListAsync();
    }

    public async Task<bool> UpdateProposalAsync(Guid id, ProjectProposalDto dto)
    {
        var proposal = await _context.ProjectProposals.FindAsync(id);

        if (proposal == null || proposal.StatusId != 4)
            return false;

        proposal.Title = dto.Title;
        proposal.Description = dto.Description;
        proposal.AreaId = dto.AreaId;
        proposal.TypeId = dto.TypeId;
        proposal.EstimatedAmount = dto.EstimatedAmount;
        proposal.EstimatedDuration = dto.EstimatedDuration;

        await _context.SaveChangesAsync();
        return true;
    }

    private async Task UpdateProposalStatusAsync(Guid projectId)
    {
        var steps = await _context.ProjectApprovalSteps
            .Where(s => s.ProjectProposalId == projectId)
            .ToListAsync();

        var proposal = await _context.ProjectProposals.FindAsync(projectId);
        if (proposal == null) return;

        if (steps.All(s => s.StatusId == 2))
            proposal.StatusId = 2;
        else if (steps.Any(s => s.StatusId == 3))
            proposal.StatusId = 3;
        else if (steps.Any(s => s.StatusId == 4))
            proposal.StatusId = 4;
        else
            proposal.StatusId = 1;
    }


    public async Task<bool> ApproveAsync(Guid projectId, int approverUserId)
    {
        var step = await _context.ProjectApprovalSteps
            .FirstOrDefaultAsync(s => s.ProjectProposalId == projectId && s.ApproverUserId == approverUserId);

        if (step == null || step.StatusId != 4)
            return false;

        step.StatusId = 2;
        await UpdateProposalStatusAsync(projectId);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ObserveAsync(Guid projectId, int approverUserId, string observation)
    {
        var step = await _context.ProjectApprovalSteps
            .FirstOrDefaultAsync(s => s.ProjectProposalId == projectId && s.ApproverUserId == approverUserId);

        if (step == null)
            return false;

        step.StatusId = 4;
        step.Observations = observation;
        await _context.SaveChangesAsync();
        await UpdateProposalStatusAsync(projectId);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RejectAsync(Guid projectId, int approverUserId)
    {
        var step = await _context.ProjectApprovalSteps
            .FirstOrDefaultAsync(s => s.ProjectProposalId == projectId && s.ApproverUserId == approverUserId);

        if (step == null)
            return false;

        step.StatusId = 3;
        await UpdateProposalStatusAsync(projectId);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ProjectOptionsDto> GetProjectOptionsAsync()
    {
        return await Task.FromResult(new ProjectOptionsDto
        {
            Areas = new List<AreaDto>(),
            ProjectTypes = new List<ProjectTypeDto>(),
            Roles = new List<RoleDto>(),
            ApprovalStatuses = new List<ApprovalStatusDto>(),
            Users = new List<UserDto>()
        });
    }


    public async Task<IEnumerable<ProjectListItemDto>> GetFilteredAsync(ProjectFilterDto filter)
    {
        var query = _context.ProjectProposals
            .Include(p => p.Status)
            .Include(p => p.CreateBy)
            .AsQueryable();

        if (!string.IsNullOrEmpty(filter.Title))
            query = query.Where(p => p.Title.Contains(filter.Title));

        if (filter.StatusId.HasValue)
            query = query.Where(p => p.StatusId == filter.StatusId.Value);

        if (filter.ApplicantUserId.HasValue)
            query = query.Where(p => p.CreateById == filter.ApplicantUserId.Value);

        if (filter.ApprovalUserId.HasValue)
            query = query.Where(p => p.ApprovalSteps.Any(a => a.ApproverUserId == filter.ApprovalUserId.Value));

        var projects = await query.ToListAsync();

        return projects.Select(p => new ProjectListItemDto
        {
            Id = p.Id,
            Title = p.Title,
            StatusId = p.StatusId,
            StatusName = p.Status != null ? p.Status.Name : string.Empty,
            CreatedAt = p.CreateAt,
            ApplicantUserName = p.CreateBy != null ? p.CreateBy.Name : string.Empty
        });
    }

    public async Task<DecisionOptionsDto> GetDecisionOptionsAsync(Guid projectId)
    {
        var project = await _context.ProjectProposals
            .Include(p => p.Status)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null)
            throw new KeyNotFoundException("Proyecto no encontrado");

        var availableStatuses = await _context.ApprovalStatuses
            .Where(s => s.Id == 2 || s.Id == 3 || s.Id == 4)
            .Select(s => new ApprovalStatusDto
            {
                Id = s.Id,
                Name = s.Name
            })
            .ToListAsync();

        return new DecisionOptionsDto
        {
            AvailableStatuses = availableStatuses,
            CurrentStatus = project.Status.Name
        };
    }


    public async Task<ProjectDetailsDto?> GetByIdAsync(Guid id)
    {
        var proposal = await _context.ProjectProposals
            .Include(p => p.Area)
            .Include(p => p.Type)
            .Include(p => p.Status)
            .Include(p => p.CreateBy)
            .Include(p => p.ApprovalSteps)
                .ThenInclude(s => s.ApproverRole)
            .Include(p => p.ApprovalSteps)
                .ThenInclude(s => s.Status)
            .Include(p => p.ApprovalSteps)
                .ThenInclude(s => s.ApproverUser)
                    .ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (proposal == null)
            return null;

        var dto = new ProjectDetailsDto
        {
            Id = proposal.Id,
            Title = proposal.Title,
            Description = proposal.Description,
            Area = new AreaDto
            {
                Id = proposal.Area.Id,
                Name = proposal.Area.Name
            },
            ProjectType = new ProjectTypeDto
            {
                Id = proposal.Type.Id,
                Name = proposal.Type.Name
            },
            Status = new ApprovalStatusDto
            {
                Id = proposal.Status.Id,
                Name = proposal.Status.Name
            },
            Steps = proposal.ApprovalSteps.Select(step => new ApprovalStepDetailsDto
            {
                Id = step.Id,
                StepOrder = step.StepOrder,
                DecisionDate = step.DecisionDate,
                Observations = step.Observations,
                ApproverRole = new RoleDto
                {
                    Id = step.ApproverRole.Id,
                    Name = step.ApproverRole.Name
                },
                Status = new ApprovalStatusDto
                {
                    Id = step.Status.Id,
                    Name = step.Status.Name
                },
                ApproverUser = step.ApproverUser == null ? null : new UserDto
                {
                    Id = step.ApproverUser.Id,
                    Name = step.ApproverUser.Name,
                    Email = step.ApproverUser.Email,
                    Role = new RoleDto
                    {
                        Id = step.ApproverUser.Role.Id,
                        Name = step.ApproverUser.Role.Name
                    }
                }
            }).ToList()
        };

        return dto;
    }



    public async Task<IEnumerable<ProjectListItemDto>> GetAllAsync(string? title = null, int? statusId = null, int? createdById = null, int? approverUserId = null)
    {
        var query = _context.ProjectProposals
            .AsQueryable();

        if (!string.IsNullOrEmpty(title))
            query = query.Where(p => p.Title.Contains(title));

        if (statusId.HasValue)
            query = query.Where(p => p.StatusId == statusId.Value);

        if (createdById.HasValue)
            query = query.Where(p => p.CreateById == createdById.Value);

        if (approverUserId.HasValue)
            query = query.Where(p => p.ApprovalSteps.Any(s => s.ApproverUserId == approverUserId.Value));

        var projects = await query
            .Select(p => new ProjectListItemDto
            {
                Id = p.Id,
                Title = p.Title,
                CreatedAt = p.CreateAt
            })
            .ToListAsync();

        return projects;
    }


    public async Task<EditProjectOptionsDto> GetEditProjectOptionsAsync(Guid projectId)
    {
        var project = await _context.ProjectProposals
            .Include(p => p.Area)
            .Include(p => p.Type)
            .Include(p => p.Status)
            .Include(p => p.CreateBy)
            .Include(p => p.ApprovalSteps)
            .ThenInclude(s => s.ApproverRole)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null)
            throw new Exception("Proyecto no encontrado");

        var dto = new EditProjectOptionsDto
        {
            Project = new ProjectDto
            {
                Id = project.Id,
                Title = project.Title,
                Description = project.Description,
                AreaId = project.AreaId,
                ProjectTypeId = project.TypeId,
                StatusId = project.StatusId,
                Steps = project.ApprovalSteps.Select(s => new ApprovalStepDto
                {
                    Id = s.Id,
                    ProjectProposalId = s.ProjectProposalId,
                    ApproverUserId = s.ApproverUserId ?? 0,
                    ApproverRole = new RoleDto
                    {
                        Id = s.ApproverRole.Id,
                        Name = s.ApproverRole.Name
                    },
                    Status = new ApprovalStatusDto { Id = s.Status.Id, Name = s.Status.Name }
                }).ToList()
            },
            Areas = await _context.Areas.Select(a => new AreaDto { Id = a.Id, Name = a.Name }).ToListAsync(),
            ProjectTypes = await _context.ProjectTypes.Select(t => new ProjectTypeDto { Id = t.Id, Name = t.Name }).ToListAsync(),
            Roles = await _context.ApproverRoles.Select(r => new RoleDto { Id = r.Id, Name = r.Name }).ToListAsync(),
            ApprovalStatuses = await _context.ApprovalStatuses.Select(s => new ApprovalStatusDto { Id = s.Id, Name = s.Name }).ToListAsync(),
            Users = await _context.Users.Select(u => new UserDto { Id = u.Id, Name = u.Name }).ToListAsync()
        };

        return dto;
    }

    public async Task<IEnumerable<AreaDto>> GetAreasAsync()
    {
        return await _context.Areas
            .Select(a => new AreaDto { Id = a.Id, Name = a.Name })
            .ToListAsync();
    }

    public async Task<IEnumerable<ProjectTypeDto>> GetProjectTypesAsync()
    {
        return await _context.ProjectTypes
            .Select(pt => new ProjectTypeDto { Id = pt.Id, Name = pt.Name })
            .ToListAsync();
    }

    public async Task<IEnumerable<RoleDto>> GetRolesAsync()
    {
        return await _context.ApproverRoles
            .Select(r => new RoleDto { Id = r.Id, Name = r.Name })
            .ToListAsync();
    }

    public async Task<IEnumerable<ApprovalStatusDto>> GetApprovalStatusesAsync()
    {
        return await _context.ApprovalStatuses
            .Select(s => new ApprovalStatusDto { Id = s.Id, Name = s.Name })
            .ToListAsync();
    }

    public async Task<IEnumerable<UserDto>> GetUsersAsync()
    {
        return await _context.Users
            .Include(u => u.Role)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role == null ? null : new RoleDto
                {
                    Id = u.Role.Id,
                    Name = u.Role.Name
                }
            })
            .ToListAsync();
    }


}
