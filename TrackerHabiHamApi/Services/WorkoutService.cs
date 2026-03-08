using Microsoft.EntityFrameworkCore;
using TrackerHabiHamApi.Data;

namespace TrackerHabiHamApi.Services
{
    public class WorkoutService : IWorkoutService
    {
        private readonly WorkoutDbContext _context;

        public WorkoutService(WorkoutDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CheckConnectionAsync(CancellationToken ct = default)
        {
            try
            {
                return await _context.Database.CanConnectAsync(ct);
            }
            catch
            {
                return false;
            }
        }
    }
}
