using Microsoft.EntityFrameworkCore;
using TrackerHabiHamApi.Data;
using TrackerHabiHamApi.Models.Dto;

namespace TrackerHabiHamApi.Services
{
    public class WeightService : IWeightService
    {
        private readonly ApplicationDbContext _context;

        public WeightService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MounthWeight>> GetFromPeriod(DateTime? start, DateTime? end)
        {
            if (!start.HasValue)
            {
                start = new DateTime(DateTime.UtcNow.Year, 1, 1);
            }

            var now = DateTime.UtcNow.Date;

            if (!end.HasValue || end > start || end > now)
            {
                end = now;
            }

            return await _context.MounthWeights
                   .Where(w => w.Date.Date >= start.Value.Date && w.Date.Date <= end.Value.Date)
                   .OrderBy(w => w.Date)
                   .ToListAsync();
        }

        public async Task<MounthWeight?> UpdateWeightAsync(DateTime date, string weight)
        {
            var existingWeight = await _context.MounthWeights
                .FirstOrDefaultAsync(w => w.Date.Date == date.Date);

            if (existingWeight == null)
            {
                var weightRecord = new MounthWeight
                {
                    Date = date.Date,
                    Weight = weight
                };

                _context.MounthWeights.Add(weightRecord);
                await _context.SaveChangesAsync();
                return weightRecord;
            }
              
            existingWeight.Weight = weight;
            await _context.SaveChangesAsync();
            return existingWeight;
        }

        public async Task<int> UpsertManyAsync(IEnumerable<MounthWeight> items)
        {
            var itemList = items
                .Select(i => new MounthWeight { Date = i.Date.Date, Weight = i.Weight })
                .ToList();

            if (itemList.Count == 0)
            {
                return 0;
            }

            var dates = itemList.Select(i => i.Date.Date).ToList();
            var existing = await _context.MounthWeights
                .Where(w => dates.Contains(w.Date.Date))
                .ToListAsync();

            var existingByDate = existing.ToDictionary(w => w.Date.Date, w => w);

            var inserts = new List<MounthWeight>();
            var updates = new List<MounthWeight>();

            foreach (var item in itemList)
            {
                if (!existingByDate.TryGetValue(item.Date.Date, out var found))
                {
                    inserts.Add(item);
                }
                else if (!string.Equals(found.Weight, item.Weight, StringComparison.OrdinalIgnoreCase))
                {
                    found.Weight = item.Weight;
                    updates.Add(found);
                }
            }

            if (inserts.Count > 0)
            {
                await _context.MounthWeights.AddRangeAsync(inserts);
            }

            // Updates are tracked already; SaveChanges will persist modifications
            var affected = inserts.Count + updates.Count;
            if (affected > 0)
            {
                await _context.SaveChangesAsync();
            }

            return affected;
        }

    }
}


