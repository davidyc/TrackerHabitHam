using TrackerHabiHamApi.Models.Dto;

namespace TrackerHabiHamApi.Services
{
    public interface IWeightAnalysisService
    {
        Task<WeightSummaryDto> GetSummaryAsync(DateOnly? start, DateOnly? end);

        Task<IReadOnlyList<WeightPointDto>> GetSeriesAsync(DateOnly? start, DateOnly? end);
    }
}


