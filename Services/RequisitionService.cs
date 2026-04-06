using RequisitionManagement.API.DTOs;
using RequisitionManagement.API.Models;
using RequisitionManagement.API.Repositories;

namespace RequisitionManagement.API.Services
{
    public class RequisitionService
    {
        private readonly RequisitionRepository _repository;

        public RequisitionService(RequisitionRepository repository)
        {
            _repository = repository;
        }

        public async Task<Requisition> CreateRequisitionAsync(CreateRequisitionDto dto, int createdBy)
        {
            var creator = await _repository.GetUserByIdAsync(createdBy);
            if (creator == null)
                throw new ArgumentException($"Invalid createdBy user id: {createdBy}");

            var requisition = new Requisition
            {
                Title = dto.Title,
                Department = dto.Department,
                Skillset = dto.Skillset,
                ExperienceLevel = dto.ExperienceLevel,
                NumberOfPositions = dto.NumberOfPositions,
                Location = dto.Location,
                HireByDate = dto.HireByDate,
                CustomerName = dto.CustomerName,
                Comments = dto.Comments,
                JdContent = dto.JdContent,
                CreatedBy = createdBy,
                Creator = creator,
                Status = "Pending",
                CreatedAt = DateTime.Now
            };

            return await _repository.CreateAsync(requisition);
        }

        public async Task<List<Requisition>> GetMyRequisitionsAsync(string username)
        {
            var user = await _repository.GetUserByUsernameAsync(username);
            if (user == null)
                return new List<Requisition>();

            return await _repository.GetByCreatorAsync(user.Id);
        }

        public async Task<Requisition?> GetRequisitionByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Requisition?> CancelRequisitionAsync(int id)
        {
            var requisition = await _repository.GetByIdAsync(id);
            if (requisition == null)
                throw new ArgumentException($"Requisition {id} not found.");

            requisition.Status = "Cancelled";
            requisition.UpdatedAt = DateTime.Now;
            await _repository.UpdateAsync(requisition);
            return requisition;
        }

        public async Task<List<Requisition>> GetCancelledRequisitionsAsync()
        {
            return await _repository.GetCancelledAsync();
        }

        public async Task DeleteRequisitionAsync(int id)
        {
            var requisition = await _repository.GetByIdAsync(id);
            if (requisition == null)
                throw new ArgumentException($"Requisition {id} not found.");

            await _repository.DeleteAsync(id);
        }


        
    }
}