using Microsoft.EntityFrameworkCore;
using TrackerHabiHamApi.Data;
using TrackerHabiHamApi.Models;

namespace TrackerHabiHamApi.Services
{
    public class WorkoutProgramService : IWorkoutProgramService
    {
        private readonly WorkoutDbContext _context;

        public WorkoutProgramService(WorkoutDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WorkoutProgram>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.WorkoutPrograms
                .Include(p => p.Exercises)
                .ThenInclude(pe => pe.Exercise)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task<WorkoutProgram?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _context.WorkoutPrograms
                .Include(p => p.Exercises.OrderBy(pe => pe.Order))
                .ThenInclude(pe => pe.Exercise)
                .ThenInclude(e => e!.MuscleGroup)
                .FirstOrDefaultAsync(p => p.Id == id, ct);
        }

        public async Task<WorkoutProgram> CreateAsync(WorkoutProgram program, CancellationToken ct = default)
        {
            program.CreatedAt = DateTime.UtcNow;
            _context.WorkoutPrograms.Add(program);
            await _context.SaveChangesAsync(ct);
            return program;
        }

        public async Task<WorkoutProgram?> UpdateAsync(int id, WorkoutProgram program, CancellationToken ct = default)
        {
            var existing = await _context.WorkoutPrograms.FindAsync([id], ct);
            if (existing == null) return null;

            existing.Name = program.Name;
            existing.Description = program.Description;
            await _context.SaveChangesAsync(ct);
            return existing;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var existing = await _context.WorkoutPrograms.FindAsync([id], ct);
            if (existing == null) return false;
            _context.WorkoutPrograms.Remove(existing);
            await _context.SaveChangesAsync(ct);
            return true;
        }

        public async Task<WorkoutProgramExercise?> AddExerciseAsync(int programId, int exerciseId, int order, CancellationToken ct = default)
        {
            var program = await _context.WorkoutPrograms.FindAsync([programId], ct);
            var exercise = await _context.Exercises.FindAsync([exerciseId], ct);
            if (program == null || exercise == null) return null;

            var item = new WorkoutProgramExercise
            {
                WorkoutProgramId = programId,
                ExerciseId = exerciseId,
                Order = order
            };
            _context.WorkoutProgramExercises.Add(item);
            await _context.SaveChangesAsync(ct);
            return item;
        }

        public async Task<bool> RemoveExerciseAsync(int programId, int programExerciseId, CancellationToken ct = default)
        {
            var item = await _context.WorkoutProgramExercises
                .FirstOrDefaultAsync(pe => pe.Id == programExerciseId && pe.WorkoutProgramId == programId, ct);
            if (item == null) return false;
            _context.WorkoutProgramExercises.Remove(item);
            await _context.SaveChangesAsync(ct);
            return true;
        }
    }
}
