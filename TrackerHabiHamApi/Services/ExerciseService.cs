using Microsoft.EntityFrameworkCore;
using TrackerHabiHamApi.Data;
using TrackerHabiHamApi.Models;

namespace TrackerHabiHamApi.Services
{
    public class ExerciseService : IExerciseService
    {
        private readonly WorkoutDbContext _context;

        public ExerciseService(WorkoutDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Exercise>> GetAllAsync(int? muscleGroupId = null, CancellationToken ct = default)
        {
            var query = _context.Exercises.Include(e => e.MuscleGroup).AsQueryable();
            if (muscleGroupId.HasValue)
                query = query.Where(e => e.MuscleGroupId == muscleGroupId.Value);
            return await query.OrderBy(e => e.Name).ToListAsync(ct);
        }

        public async Task<Exercise?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _context.Exercises.Include(e => e.MuscleGroup).FirstOrDefaultAsync(e => e.Id == id, ct);
        }

        public async Task<Exercise> CreateAsync(Exercise exercise, CancellationToken ct = default)
        {
            _context.Exercises.Add(exercise);
            await _context.SaveChangesAsync(ct);
            return exercise;
        }

        public async Task<Exercise?> UpdateAsync(int id, Exercise exercise, CancellationToken ct = default)
        {
            var existing = await _context.Exercises.FindAsync([id], ct);
            if (existing == null) return null;

            existing.Name = exercise.Name;
            existing.Description = exercise.Description;
            existing.MuscleGroupId = exercise.MuscleGroupId;
            await _context.SaveChangesAsync(ct);
            return existing;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var existing = await _context.Exercises.FindAsync([id], ct);
            if (existing == null) return false;
            _context.Exercises.Remove(existing);
            await _context.SaveChangesAsync(ct);
            return true;
        }
    }
}
