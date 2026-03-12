using Microsoft.EntityFrameworkCore;
using TrackerHabiHamApi.Data;
using TrackerHabiHamApi.Models;
using TrackerHabiHamApi.Models.Dto;

namespace TrackerHabiHamApi.Services
{
    public class WorkoutProgramService : IWorkoutProgramService
    {
        private readonly WorkoutDbContext _context;

        public WorkoutProgramService(WorkoutDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProgramListDto>> GetProgramsListAsync(CancellationToken ct = default)
        {
            return await _context.WorkoutPrograms
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new ProgramListDto(
                    p.Id,
                    p.Name,
                    p.Description,
                    p.CreatedAt,
                    p.Exercises
                        .OrderBy(pe => pe.Order)
                        .Select(pe => new ProgramExerciseDto(
                            pe.Id,
                            pe.Order,
                            pe.Comment,
                            new ExerciseBriefDto(
                                pe.Exercise.Id,
                                pe.Exercise.Name,
                                pe.Exercise.Description,
                                pe.Exercise.MuscleGroup == null
                                    ? null
                                    : new MuscleGroupBriefDto(pe.Exercise.MuscleGroup.Id, pe.Exercise.MuscleGroup.Name))))
                        .ToList()))
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<WorkoutProgram>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.WorkoutPrograms
                .Include(p => p.Exercises)
                .ThenInclude(pe => pe.Exercise)
                .ThenInclude(e => e!.MuscleGroup)
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

        public async Task<WorkoutProgramExercise?> AddExerciseAsync(int programId, int exerciseId, int order, string? comment = null, CancellationToken ct = default)
        {
            var program = await _context.WorkoutPrograms.FindAsync([programId], ct);
            var exercise = await _context.Exercises.FindAsync([exerciseId], ct);
            if (program == null || exercise == null) return null;

            var item = new WorkoutProgramExercise
            {
                WorkoutProgramId = programId,
                ExerciseId = exerciseId,
                Order = order,
                Comment = comment
            };
            _context.WorkoutProgramExercises.Add(item);
            await _context.SaveChangesAsync(ct);

            return await _context.WorkoutProgramExercises
                .AsNoTracking()
                .Include(pe => pe.Exercise)
                .ThenInclude(e => e!.MuscleGroup)
                .FirstOrDefaultAsync(pe => pe.Id == item.Id, ct);
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
