using ProjectApprovalAPI.Models;
namespace ProjectApprovalAPI.Repositories.Interfaces
{
    public interface IProjectProposalRepository
    {
        Task<ProjectProposal> GetByIdAsync(Guid id);
        Task<IEnumerable<ProjectProposal>> GetAllAsync();
        Task<IEnumerable<ProjectProposal>> SearchAsync(string title = null, int? statusId = null, int? createdById = null, int? approverId = null);
        Task<ProjectProposal> CreateAsync(ProjectProposal projectProposal);
        Task<ProjectProposal> UpdateAsync(ProjectProposal projectProposal);
        Task<bool> ExistsTitleAsync(string title);
        Task<bool> ExistsTitleExceptIdAsync(string title, Guid id);
        Task SaveChangesAsync();
    }
}
