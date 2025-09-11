using Microsoft.EntityFrameworkCore;
using TrackerHabiHamApi.Data;
using TrackerHabiHamApi.Models.Dto;

namespace TrackerHabiHamApi.Services
{
    public class WeightAnalysisService : IWeightAnalysisService
    {
        private readonly ApplicationDbContext _context;

        public WeightAnalysisService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<WeightSummaryDto> GetSummaryAsync(DateTime? start, DateTime? end)
        {
            var (from, to) = NormalizeRange(start, end);

            var query = _context.MounthWeights
                .Where(w => w.Date.Date >= from && w.Date.Date <= to)
                .OrderBy(w => w.Date)
                .Select(w => new { w.Date, w.Weight });

            var list = await query.ToListAsync();

            var parsed = list
                .Select(x => new { x.Date, Value = ParseWeight(x.Weight) })
                .Where(x => x.Value.HasValue)
                .Select(x => new { x.Date, Value = x.Value!.Value })
                .ToList();

            double? min = parsed.Count > 0 ? parsed.Min(p => p.Value) : null;
            double? max = parsed.Count > 0 ? parsed.Max(p => p.Value) : null;
            double? avg = parsed.Count > 0 ? parsed.Average(p => p.Value) : null;
            double? startVal = parsed.Count > 0 ? parsed.First().Value : null;
            double? endVal = parsed.Count > 0 ? parsed.Last().Value : null;

            return new WeightSummaryDto
            {
                Start = from,
                End = to,
                Min = min,
                Max = max,
                Average = avg,
                StartValue = startVal,
                EndValue = endVal,
                Change = (startVal.HasValue && endVal.HasValue) ? endVal - startVal : null
            };
        }

        public async Task<IReadOnlyList<WeightPointDto>> GetSeriesAsync(DateTime? start, DateTime? end)
        {
            var (from, to) = NormalizeRange(start, end);

            var list = await _context.MounthWeights
                .Where(w => w.Date.Date >= from && w.Date.Date <= to)
                .OrderBy(w => w.Date)
                .Select(w => new WeightPointDto
                {
                    Date = w.Date,
                    Value = ParseWeight(w.Weight)
                })
                .ToListAsync();

            return list;
        }

        private static (DateTime from, DateTime to) NormalizeRange(DateTime? start, DateTime? end)
        {
            var today = DateTime.UtcNow.Date;
            var from = (start ?? new DateTime(today.Year, 1, 1)).Date;
            var to = (end ?? today).Date;

            if (to < from)
            {
                (from, to) = (to, from);
            }

            if (to > today)
            {
                to = today;
            }

            return (from, to);
        }

        private static double? ParseWeight(string weight)
        {
            if (string.IsNullOrWhiteSpace(weight))
            {
                return null;
            }

            var normalized = weight.Replace(',', '.');
            if (double.TryParse(normalized, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var value))
            {
                return value;
            }

            return null;
        }
    }
}


