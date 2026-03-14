using TrackerHabiHamApi.Models;
using TrackerHabiHamApi.Models.Dto;

namespace TrackerHabiHamApi.Services
{
    public interface IWorkoutSessionService
    {
        Task<IEnumerable<Workout>> GetAllAsync(int? programId = null, DateOnly? from = null, DateOnly? to = null, CancellationToken ct = default);
        Task<Workout?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Workout?> CreateFromProgramAsync(CreateWorkoutRequest request, CancellationToken ct = default);
        Task<Workout?> UpdateAsync(int id, Workout workout, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
        Task<Set?> AddSetAsync(int workoutId, Set set, CancellationToken ct = default);
        Task<Set?> UpdateSetAsync(int workoutId, int setId, Set set, CancellationToken ct = default);
        Task<bool> RemoveSetAsync(int workoutId, int setId, CancellationToken ct = default);
    }
}
