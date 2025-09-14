using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HabihamTrackerApp
{
    public class AnalyticsData
    {
        [JsonPropertyName("start")]
        public string Start { get; set; }
        
        [JsonPropertyName("end")]
        public string End { get; set; }
        
        [JsonPropertyName("min")]
        public double Min { get; set; }
        
        [JsonPropertyName("max")]
        public double Max { get; set; }
        
        [JsonPropertyName("average")]
        public double Average { get; set; }
        
        [JsonPropertyName("startValue")]
        public double StartValue { get; set; }
        
        [JsonPropertyName("endValue")]
        public double EndValue { get; set; }
        
        [JsonPropertyName("change")]
        public double Change { get; set; }
    }

    public partial class MainPage : ContentPage
    {
        private readonly HttpClient _httpClient;

        public MainPage()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
        }

        private void OnApiTabClicked(object sender, EventArgs e)
        {
            // Активируем API таб
            ApiTab.BackgroundColor = Color.FromArgb("#4A90E2");
            ApiTab.TextColor = Colors.White;
            
            // Деактивируем Sync таб
            SyncTab.BackgroundColor = Color.FromArgb("#E0E0E0");
            SyncTab.TextColor = Color.FromArgb("#666666");
            
            // Показываем API блок, скрываем Sync блок
            ApiFrame.IsVisible = true;
            SyncFrame.IsVisible = false;
        }

        private void OnSyncTabClicked(object sender, EventArgs e)
        {
            // Активируем Sync таб
            SyncTab.BackgroundColor = Color.FromArgb("#50C878");
            SyncTab.TextColor = Colors.White;
            
            // Деактивируем API таб
            ApiTab.BackgroundColor = Color.FromArgb("#E0E0E0");
            ApiTab.TextColor = Color.FromArgb("#666666");
            
            // Показываем Sync блок, скрываем API блок
            SyncFrame.IsVisible = true;
            ApiFrame.IsVisible = false;
        }

        private async void OnApiButtonClicked(object sender, EventArgs e)
        {
            try
            {
                ApiBtn.Text = "Загрузка...";
                ApiBtn.IsEnabled = false;
                
                // Показываем индикатор загрузки
                ShowLoadingState();

                var response = await _httpClient.GetAsync("https://trackerhabitham.onrender.com/api/WeightAnalysis/summary");
                var content = await response.Content.ReadAsStringAsync();

                // Отладочная информация
                System.Diagnostics.Debug.WriteLine($"JSON Response: {content}");

                // Парсим JSON ответ
                var analyticsData = JsonSerializer.Deserialize<AnalyticsData>(content);
                
                // Отладочная информация
                System.Diagnostics.Debug.WriteLine($"Parsed data - Min: {analyticsData?.Min}, Max: {analyticsData?.Max}");
                
                // Обновляем поля
                UpdateAnalyticsFields(analyticsData);
                
                ApiBtn.Text = "Получить аналитику";
            }
            catch (Exception ex)
            {
                ShowErrorState($"Ошибка: {ex.Message}");
                ApiBtn.Text = "Получить аналитику";
            }
            finally
            {
                ApiBtn.IsEnabled = true;
            }
        }

        private void ShowLoadingState()
        {
            StartDateLabel.Text = "Период начала: Загрузка...";
            EndDateLabel.Text = "Период окончания: Загрузка...";
            MinValueLabel.Text = "Минимальное значение: Загрузка...";
            MaxValueLabel.Text = "Максимальное значение: Загрузка...";
            AverageLabel.Text = "Среднее значение: Загрузка...";
            StartValueLabel.Text = "Начальное значение: Загрузка...";
            EndValueLabel.Text = "Конечное значение: Загрузка...";
            ChangeLabel.Text = "Изменение: Загрузка...";
        }

        private void ShowErrorState(string errorMessage)
        {
            StartDateLabel.Text = $"Ошибка: {errorMessage}";
            EndDateLabel.Text = "";
            MinValueLabel.Text = "";
            MaxValueLabel.Text = "";
            AverageLabel.Text = "";
            StartValueLabel.Text = "";
            EndValueLabel.Text = "";
            ChangeLabel.Text = "";
        }

        private void UpdateAnalyticsFields(AnalyticsData data)
        {
            if (data == null)
            {
                ShowErrorState("Нет данных");
                return;
            }

            // Форматируем даты
            var startDate = DateTime.TryParse(data.Start, out var start) ? start.ToString("dd.MM.yyyy HH:mm") : data.Start;
            var endDate = DateTime.TryParse(data.End, out var end) ? end.ToString("dd.MM.yyyy HH:mm") : data.End;

            StartDateLabel.Text = $"Период начала: {startDate}";
            EndDateLabel.Text = $"Период окончания: {endDate}";
            MinValueLabel.Text = $"Минимальное значение: {data.Min:F1}";
            MaxValueLabel.Text = $"Максимальное значение: {data.Max:F1}";
            AverageLabel.Text = $"Среднее значение: {data.Average:F2}";
            StartValueLabel.Text = $"Начальное значение: {data.StartValue:F1}";
            EndValueLabel.Text = $"Конечное значение: {data.EndValue:F1}";
            
            // Форматируем изменение с знаком
            var changeText = data.Change >= 0 ? $"+{data.Change:F1}" : data.Change.ToString("F1");
            ChangeLabel.Text = $"Изменение: {changeText}";
        }

        private async void OnSyncButtonClicked(object sender, EventArgs e)
        {
            try
            {
                SyncBtn.Text = "Синхронизация...";
                SyncBtn.IsEnabled = false;
                SyncLabel.Text = "Синхронизация...";
                SyncLabel.TextColor = Colors.Blue;

                var response = await _httpClient.PostAsync("http://trackerhabitham.onrender.com/api/Sync", null);
                var content = await response.Content.ReadAsStringAsync();

                SyncLabel.Text = $"Синхронизация завершена!\n{content}";
                SyncLabel.TextColor = Colors.Green;
                SyncBtn.Text = "Синхронизировать";
            }
            catch (Exception ex)
            {
                SyncLabel.Text = $"Ошибка синхронизации: {ex.Message}";
                SyncLabel.TextColor = Colors.Red;
                SyncBtn.Text = "Синхронизировать";
            }
            finally
            {
                SyncBtn.IsEnabled = true;
            }
        }
    }
}
