using Microsoft.EntityFrameworkCore;
using TrackerHabiHamApi.Data;
using TrackerHabiHamApi.Models;
using TrackerHabiHamApi.Models.Dto;

namespace TrackerHabiHamApi.Services
{
    public class WorkoutSessionService : IWorkoutSessionService
    {
        private readonly WorkoutDbContext _context;

        public WorkoutSessionService(WorkoutDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Workout>> GetAllAsync(int? programId = null, DateOnly? from = null, DateOnly? to = null, CancellationToken ct = default)
        {
            var query = _context.Workouts
                .Include(w => w.WorkoutProgram)
                .Include(w => w.Sets.OrderBy(s => s.Order))
                .ThenInclude(s => s.Exercise)
                .AsQueryable();

            if (programId.HasValue)
                query = query.Where(w => w.WorkoutProgramId == programId.Value);
            if (from.HasValue)
                query = query.Where(w => w.Date >= from.Value);
            if (to.HasValue)
                query = query.Where(w => w.Date <= to.Value);

            return await query.OrderByDescending(w => w.Date).ToListAsync(ct);
        }

        public async Task<Workout?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _context.Workouts
                .Include(w => w.WorkoutProgram)
                .Include(w => w.Sets.OrderBy(s => s.Order))
                .ThenInclude(s => s.Exercise)
                .ThenInclude(e => e!.MuscleGroup)
                .FirstOrDefaultAsync(w => w.Id == id, ct);
        }

        public async Task<Workout?> CreateFromProgramAsync(CreateWorkoutRequest request, CancellationToken ct = default)
        {
            var program = await _context.WorkoutPrograms.FindAsync([request.WorkoutProgramId], ct);
            if (program == null) return null;

            var workout = new Workout
            {
                WorkoutProgramId = request.WorkoutProgramId,
                Date = request.Date ?? DateOnly.FromDateTime(DateTime.UtcNow),
                Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim()
            };
            _context.Workouts.Add(workout);
            await _context.SaveChangesAsync(ct);
            return workout;
        }

        public async Task<Workout?> UpdateAsync(int id, Workout workout, CancellationToken ct = default)
        {
            var existing = await _context.Workouts.FindAsync([id], ct);
            if (existing == null) return null;

            existing.Date = workout.Date;
            existing.Notes = workout.Notes;
            await _context.SaveChangesAsync(ct);
            return existing;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var existing = await _context.Workouts.FindAsync([id], ct);
            if (existing == null) return false;
            _context.Workouts.Remove(existing);
            await _context.SaveChangesAsync(ct);
            return true;
        }

        public async Task<Set?> AddSetAsync(int workoutId, Set set, CancellationToken ct = default)
        {
            var workout = await _context.Workouts.FindAsync([workoutId], ct);
            var exercise = await _context.Exercises.FindAsync([set.ExerciseId], ct);
            if (workout == null || exercise == null) return null;

            set.WorkoutId = workoutId;
            _context.Sets.Add(set);
            await _context.SaveChangesAsync(ct);
            return set;
        }

        public async Task<Set?> UpdateSetAsync(int workoutId, int setId, Set set, CancellationToken ct = default)
        {
            var existing = await _context.Sets
                .FirstOrDefaultAsync(s => s.Id == setId && s.WorkoutId == workoutId, ct);
            if (existing == null) return null;

            existing.ExerciseId = set.ExerciseId;
            existing.Order = set.Order;
            existing.Reps = set.Reps;
            existing.WeightKg = set.WeightKg;
            await _context.SaveChangesAsync(ct);
            return existing;
        }

        public async Task<bool> RemoveSetAsync(int workoutId, int setId, CancellationToken ct = default)
        {
            var existing = await _context.Sets
                .FirstOrDefaultAsync(s => s.Id == setId && s.WorkoutId == workoutId, ct);
            if (existing == null) return false;
            _context.Sets.Remove(existing);
            await _context.SaveChangesAsync(ct);
            return true;
        }
    }
}
