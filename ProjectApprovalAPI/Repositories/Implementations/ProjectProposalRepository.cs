using Microsoft.EntityFrameworkCore;
using ProjectApprovalAPI.Data;
using ProjectApprovalAPI.Models;
using ProjectApprovalAPI.Repositories.Interfaces;

namespace ProjectApprovalAPI.Repositories.Implementations
{
    public class ProjectProposalRepository : IProjectProposalRepository
    {
        private readonly ProjectApprovalDbContext _context;

        public ProjectProposalRepository(ProjectApprovalDbContext context)
        {
            _context = context;
        }

        public async Task<ProjectProposal> GetByIdAsync(Guid id)
        {
            return await _context.ProjectProposals
                .Include(p => p.Status)
                .Include(p => p.Area)
                .Include(p => p.Type)
                .Include(p => p.CreateBy)
                .Include(p => p.ApprovalSteps)
                    .ThenInclude(s => s.Status)
                .Include(p => p.ApprovalSteps)
                    .ThenInclude(s => s.ApproverRole)
                .Include(p => p.ApprovalSteps)
                    .ThenInclude(s => s.ApproverUser)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<ProjectProposal>> GetAllAsync()
        {
            return await _context.ProjectProposals
                .Include(p => p.Status)
                .Include(p => p.Area)
                .Include(p => p.Type)
                .Include(p => p.CreateBy)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProjectProposal>> SearchAsync(string title = null, int? statusId = null, int? createdById = null, int? approverId = null)
        {
            IQueryable<ProjectProposal> query = _context.ProjectProposals
                .Include(p => p.Status)
                .Include(p => p.Area)
                .Include(p => p.Type)
                .Include(p => p.CreateBy);

            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(p => p.Title.Contains(title));
            }

            if (statusId.HasValue)
            {
                query = query.Where(p => p.StatusId == statusId.Value);
            }

            if (createdById.HasValue)
            {
                query = query.Where(p => p.CreateById == createdById.Value);
            }

            if (approverId.HasValue)
            {
                query = query.Where(p => p.ApprovalSteps.Any(s => s.ApproverRoleId == approverId.Value));
            }

            return await query.ToListAsync();
        }

        public async Task<ProjectProposal> CreateAsync(ProjectProposal projectProposal)
        {
            _context.ProjectProposals.Add(projectProposal);
            await _context.SaveChangesAsync();
            return projectProposal;
        }

        public async Task<ProjectProposal> UpdateAsync(ProjectProposal projectProposal)
        {
            _context.Entry(projectProposal).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return projectProposal;
        }

        public async Task<bool> ExistsTitleAsync(string title)
        {
            return await _context.ProjectProposals.AnyAsync(p => p.Title == title);
        }

        public async Task<bool> ExistsTitleExceptIdAsync(string title, Guid id)
        {
            return await _context.ProjectProposals.AnyAsync(p => p.Title == title && p.Id != id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
