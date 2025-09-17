using TrackerHabiHamApi.Models.Dto;

namespace TrackerHabiHamApi.Services
{
    public interface IWeightService
    {
        Task<IEnumerable<MounthWeight>> GetFromPeriod(DateOnly? start, DateOnly? end);

        Task<MounthWeight?> UpdateWeightAsync(DateOnly date, string weight);

        Task<int> UpsertManyAsync(IEnumerable<MounthWeight> items);
    }
}


