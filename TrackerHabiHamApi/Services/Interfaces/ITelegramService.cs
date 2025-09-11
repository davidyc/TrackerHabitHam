namespace TrackerHabiHamApi.Services
{
    public interface ITelegramService
    {
        Task SendMessageAsync(long chatId, string message);

        bool IsValidNumber(string message);

        Task SendValidationErrorAsync(long chatId, string invalidMessage);

        Task SendMenuAsync(long chatId, string message, IEnumerable<string> buttonsInSingleRow);

        Task SetMyCommandsAsync(IEnumerable<(string command, string description)> commands);
    }
}


