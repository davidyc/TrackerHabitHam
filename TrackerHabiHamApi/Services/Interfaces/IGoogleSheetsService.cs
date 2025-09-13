using Microsoft.AspNetCore.Mvc;
using TrackerHabiHamApi.Models.Dto;

namespace TrackerHabiHamApi.Services
{
    public interface IGoogleSheetsService
    {
        string WriteNumberToTodayRow(string number);
        IEnumerable<MounthWeight> GetMounth(int year, int mounth);
        bool CredentialExists();
    }
}


