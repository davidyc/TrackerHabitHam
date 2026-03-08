using Microsoft.EntityFrameworkCore;
using TrackerHabiHamApi.Data;
using TrackerHabiHamApi.Models;

namespace TrackerHabiHamApi.Services
{
    public class MuscleGroupService : IMuscleGroupService
    {
        private readonly WorkoutDbContext _context;

        public MuscleGroupService(WorkoutDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MuscleGroup>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.MuscleGroups.OrderBy(m => m.Name).ToListAsync(ct);
        }

        public async Task<MuscleGroup?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _context.MuscleGroups.FindAsync([id], ct);
        }
    }
}
