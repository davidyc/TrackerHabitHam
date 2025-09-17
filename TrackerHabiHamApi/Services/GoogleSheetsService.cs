using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using TrackerHabiHamApi.Models.Dto;

namespace TrackerHabiHamApi.Services
{
    public class GoogleSheetsService : IGoogleSheetsService
    {
        private readonly Dictionary<int, string> _monthMap = new Dictionary<int, string>()
        {
            {1,  "B"},
            {2,  "C"},
            {3,  "D"},
            {4,  "E"},
            {5,  "F"},
            {6,  "G"},
            {7,  "H"},
            {8,  "I"},
            {9,  "J"},
            {10, "K"},
            {11, "L"},
            {12, "M"}
        };
        private readonly string _credentialsPath;
        private readonly SheetsService _service;
        private readonly string _spreadsheetId;
        private readonly ILogger<GoogleSheetsService> _logger;

        public GoogleSheetsService(IConfiguration configuration, ILogger<GoogleSheetsService> logger)
        {
            _logger = logger;
            _credentialsPath = configuration["GoogleSheets:CredentialsFilePath"] ?? throw new InvalidOperationException("Google Sheets credentials file path not configured");            
            _service = GetSheetsService(_credentialsPath, "GoogleSheetsServiceApp");
            _spreadsheetId = configuration["GoogleSheets:SpreadsheetId"] ?? throw new InvalidOperationException("Google Sheets spreadsheet ID not configured"); ;
        }

        public string WriteNumberToTodayRow(string number)
        {
            try
            {
                (string year, string range) = GetCellForToday();
                SheetCheck(year);

                var valueRange = new ValueRange
                {
                    Values = [[number]]
                };
                var updateRequest = _service.Spreadsheets.Values.Update(valueRange, _spreadsheetId, range);
                updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
                var updateResponse = updateRequest.Execute();

                var responseForSheets = $"Значение {number} записано в {range}";
                _logger.LogInformation(responseForSheets);
                return responseForSheets;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error writing to Google Sheets");
                throw;
            }
        }

        private SheetsService GetSheetsService(string credentialsPath, string serviceName)
        {
            var credential = GoogleCredential
                .FromFile(credentialsPath)
                .CreateScoped(SheetsService.Scope.Spreadsheets);

            return new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = serviceName,
            });
        }

        private void SheetCheck(string year)
        {
            var ss = _service.Spreadsheets.Get(_spreadsheetId).Execute();
            var targetSheet = ss.Sheets.FirstOrDefault(s =>
                string.Equals(s.Properties.Title, year, StringComparison.CurrentCultureIgnoreCase));

            if (targetSheet == null)
                throw new InvalidOperationException($"Лист '{year}' не найден.");
        }

        private (string year, string range) GetCellForToday()
        {
            var year = DateTime.Now.ToString("yyyy");
            var month = DateTime.Now.Month;
            var day = DateTime.Now.Day;
            var colName = _monthMap[month];
            var targetCell = $"{colName}{day + 1}";
            var range = $"{year}!{targetCell}";             
            return (year, range);
        }

        public IEnumerable<MounthWeight> GetMounth(int year, int mounth)
        {
            if (!_monthMap.TryGetValue(mounth, out var colName))
                throw new ArgumentOutOfRangeException(nameof(mounth), "Unknown month -> column mapping.");

            var daysInMonth = DateTime.DaysInMonth(year, mounth);
            var startRow = 2;
            var endRow = daysInMonth + 1;
            var range = $"{year}!{colName}{startRow}:{colName}{endRow}";

            var request = _service.Spreadsheets.Values.Get(_spreadsheetId, range);
            var response = request.Execute();

            var rows = response?.Values ?? new List<IList<object>>();

            var result = new List<MounthWeight>(daysInMonth);

            for (int day = 1; day <= daysInMonth; day++)
            {
                var rowIndex = day - 1;

                string weight =
                    (rowIndex < rows.Count && rows[rowIndex] is { Count: > 0 })
                        ? rows[rowIndex][0]?.ToString() ?? string.Empty
                        : string.Empty;

                result.Add(new MounthWeight
                {
                    Date = DateOnly.FromDateTime(new DateTime(year, mounth, day)),
                    Weight = weight
                });
            }

            return result;
        }

        public bool CredentialExists()
        {
            return File.Exists(_credentialsPath);
        }
    }
}


