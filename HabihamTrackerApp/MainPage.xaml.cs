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
        private DateTime _startDate;
        private DateTime _endDate;

        public MainPage()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            
            // Устанавливаем значения по умолчанию
            _startDate = DateTime.Today.AddDays(-30); // 30 дней назад
            _endDate = DateTime.Today; // сегодня
            
            // Инициализируем DatePicker'ы
            StartDatePicker.Date = _startDate;
            EndDatePicker.Date = _endDate;
            
            // Подписываемся на события изменения дат
            StartDatePicker.DateSelected += OnStartDateSelected;
            EndDatePicker.DateSelected += OnEndDateSelected;
        }

        private void OnApiTabClicked(object sender, EventArgs e)
        {
            // Активируем API таб
            ApiTab.BackgroundColor = Color.FromArgb("#4A90E2");
            ApiTab.TextColor = Colors.White;
            
            // Деактивируем Sync таб
            SyncTab.BackgroundColor = Color.FromArgb("#404040");
            SyncTab.TextColor = Color.FromArgb("#CCCCCC");
            
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
            ApiTab.BackgroundColor = Color.FromArgb("#404040");
            ApiTab.TextColor = Color.FromArgb("#CCCCCC");
            
            // Показываем Sync блок, скрываем API блок
            SyncFrame.IsVisible = true;
            ApiFrame.IsVisible = false;
        }

        private void OnStartDateSelected(object sender, DateChangedEventArgs e)
        {
            _startDate = e.NewDate;
            
            // Проверяем, что дата начала не больше даты окончания
            if (_startDate > _endDate)
            {
                _endDate = _startDate;
                EndDatePicker.Date = _endDate;
            }
        }

        private void OnEndDateSelected(object sender, DateChangedEventArgs e)
        {
            _endDate = e.NewDate;
            
            // Проверяем, что дата окончания не меньше даты начала
            if (_endDate < _startDate)
            {
                _startDate = _endDate;
                StartDatePicker.Date = _startDate;
            }
        }

        private async void OnApiButtonClicked(object sender, EventArgs e)
        {
            try
            {
                ApiBtn.Text = "Загрузка...";
                ApiBtn.IsEnabled = false;
                
                // Показываем индикатор загрузки
                ShowLoadingState();

                // Формируем URL с выбранными датами
                var startDateStr = _startDate.ToString("yyyy-MM-dd");
                var endDateStr = _endDate.ToString("yyyy-MM-dd");
                var url = $"https://trackerhabitham.onrender.com/api/WeightAnalysis/summary?start={startDateStr}&end={endDateStr}";
                
                var response = await _httpClient.GetAsync(url);
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
            StartDateLabel.Text = $"Выбранный период начала: {_startDate:dd.MM.yyyy}";
            EndDateLabel.Text = $"Выбранный период окончания: {_endDate:dd.MM.yyyy}";
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
            StartDateLabel.TextColor = Color.FromArgb("#FF6B6B"); // Красный цвет для ошибки
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

            // Сбрасываем цвет текста на обычный
            StartDateLabel.TextColor = Color.FromArgb("#CCCCCC");
            EndDateLabel.TextColor = Color.FromArgb("#CCCCCC");
            MinValueLabel.TextColor = Color.FromArgb("#CCCCCC");
            MaxValueLabel.TextColor = Color.FromArgb("#CCCCCC");
            AverageLabel.TextColor = Color.FromArgb("#CCCCCC");
            StartValueLabel.TextColor = Color.FromArgb("#CCCCCC");
            EndValueLabel.TextColor = Color.FromArgb("#CCCCCC");
            ChangeLabel.TextColor = Color.FromArgb("#CCCCCC");

            // Отображаем выбранные пользователем даты
            StartDateLabel.Text = $"Выбранный период начала: {_startDate:dd.MM.yyyy}";
            EndDateLabel.Text = $"Выбранный период окончания: {_endDate:dd.MM.yyyy}";
            
            // Отображаем данные аналитики
            MinValueLabel.Text = $"Минимальное значение: {data.Min:F1}";
            MaxValueLabel.Text = $"Максимальное значение: {data.Max:F1}";
            AverageLabel.Text = $"Среднее значение: {data.Average:F2}";
            StartValueLabel.Text = $"Начальное значение: {data.StartValue:F1}";
            EndValueLabel.Text = $"Конечное значение: {data.EndValue:F1}";
            
            // Форматируем изменение с знаком и цветом
            var changeText = data.Change >= 0 ? $"+{data.Change:F1}" : data.Change.ToString("F1");
            ChangeLabel.Text = $"Изменение: {changeText}";
            
            // Цвет для изменения: зеленый для положительного, красный для отрицательного
            if (data.Change >= 0)
            {
                ChangeLabel.TextColor = Color.FromArgb("#4CAF50"); // Зеленый
            }
            else
            {
                ChangeLabel.TextColor = Color.FromArgb("#FF6B6B"); // Красный
            }
        }

        private async void OnSyncButtonClicked(object sender, EventArgs e)
        {
            try
            {
                SyncBtn.Text = "Синхронизация...";
                SyncBtn.IsEnabled = false;
                SyncLabel.Text = "Синхронизация...";
                SyncLabel.TextColor = Color.FromArgb("#4A90E2"); // Синий для загрузки

                var response = await _httpClient.GetAsync("http://trackerhabitham.onrender.com/api/Sync?year=2025");
                var content = await response.Content.ReadAsStringAsync();

                SyncLabel.Text = $"Синхронизация завершена!\n{content}";
                SyncLabel.TextColor = Color.FromArgb("#4CAF50"); // Зеленый для успеха
                SyncBtn.Text = "Синхронизировать";
            }
            catch (Exception ex)
            {
                SyncLabel.Text = $"Ошибка синхронизации: {ex.Message}";
                SyncLabel.TextColor = Color.FromArgb("#FF6B6B"); // Красный для ошибки
                SyncBtn.Text = "Синхронизировать";
            }
            finally
            {
                SyncBtn.IsEnabled = true;
            }
        }
    }
}
