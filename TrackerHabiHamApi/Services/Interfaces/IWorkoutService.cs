namespace TrackerHabiHamApi.Services
{
    public interface IWorkoutService
    {
        Task<bool> CheckConnectionAsync(CancellationToken ct = default);
    }
}
