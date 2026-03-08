using TrackerHabiHamApi.Models;

namespace TrackerHabiHamApi.Services
{
    public interface IMuscleGroupService
    {
        Task<IEnumerable<MuscleGroup>> GetAllAsync(CancellationToken ct = default);
        Task<MuscleGroup?> GetByIdAsync(int id, CancellationToken ct = default);
    }
}
