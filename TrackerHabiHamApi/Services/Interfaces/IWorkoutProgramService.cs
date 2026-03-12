using TrackerHabiHamApi.Models;
using TrackerHabiHamApi.Models.Dto;

namespace TrackerHabiHamApi.Services
{
    public interface IWorkoutProgramService
    {
        Task<IEnumerable<ProgramListDto>> GetProgramsListAsync(CancellationToken ct = default);
        Task<IEnumerable<WorkoutProgram>> GetAllAsync(CancellationToken ct = default);
        Task<WorkoutProgram?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<WorkoutProgram> CreateAsync(WorkoutProgram program, CancellationToken ct = default);
        Task<WorkoutProgram?> UpdateAsync(int id, WorkoutProgram program, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
        Task<WorkoutProgramExercise?> AddExerciseAsync(int programId, int exerciseId, int order, string? comment = null, CancellationToken ct = default);
        Task<bool> RemoveExerciseAsync(int programId, int programExerciseId, CancellationToken ct = default);
    }
}
