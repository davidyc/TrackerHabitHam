using TrackerHabiHamApi.Models.Dto;

namespace TrackerHabiHamApi.Services
{
    public interface IWeightService
    {
        Task<IEnumerable<MounthWeight>> GetFromPeriod(DateTime? start, DateTime? end);


        Task<MounthWeight?> UpdateWeightAsync(DateTime date, string weight);

        Task<int> UpsertManyAsync(IEnumerable<MounthWeight> items);
    }
}


