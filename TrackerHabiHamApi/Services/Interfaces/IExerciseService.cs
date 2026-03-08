using TrackerHabiHamApi.Models;

namespace TrackerHabiHamApi.Services
{
    public interface IExerciseService
    {
        Task<IEnumerable<Exercise>> GetAllAsync(int? muscleGroupId = null, CancellationToken ct = default);
        Task<Exercise?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Exercise> CreateAsync(Exercise exercise, CancellationToken ct = default);
        Task<Exercise?> UpdateAsync(int id, Exercise exercise, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
