using TrackerHabiHamApi.Models.Dto;

namespace TrackerHabiHamApi.Services
{
    public interface IWeightAnalysisService
    {
        Task<WeightSummaryDto> GetSummaryAsync(DateTime? start, DateTime? end);

        Task<IReadOnlyList<WeightPointDto>> GetSeriesAsync(DateTime? start, DateTime? end);
    }
}


