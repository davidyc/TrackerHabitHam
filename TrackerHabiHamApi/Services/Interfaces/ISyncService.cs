namespace TrackerHabiHamApi.Services
{
    public interface ISyncService
    {
        Task<int> SyncByYearAsync(int year);
    }
}


