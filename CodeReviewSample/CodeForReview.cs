using System.Text;
using Newtonsoft.Json;

namespace CodeReviewSample;

public class WeatherForecastDto
{
    public int _forecastDurationInDaysFromNow;
    public List<double> temperatures;
    
    public List<double> AtmospherePressure;
}

public class ForecastDetailsViewModel : ViewModelBase
{
    public override void OnAppearing()
    {
        if (WeatherForecastData == null)
        {
            WeatherForecastData = new WeatherForecastDto {_forecastDurationInDaysFromNow = 7};
            DailyForecastDetails = Enumerable.Range(0, 7).Select(_ => new DayForecastSummaryViewModel());
        }
        refreshData();
    }

    async void refreshData()
    {
        WeatherForecastData = await new HttpService().Execute("weather-forecast/summary", WeatherForecastData,
            "application/json", "Tkkm9iCRWHecw");
        if (WeatherForecastData.AtmospherePressure.Count > WeatherForecastData._forecastDurationInDaysFromNow)
        {
            AlertService.Instance.ShowAlert("Oops", "Something went wrong", "OK");
            return;
        }
        OnPropertyChanged("WeatherForecastData");

        foreach (var dailyForecast in DailyForecastDetails)
        {
            dailyForecast.RefreshData(this);
        }

        Cloudy = DailyForecastDetails.Where(day => day.weatherTypeIcon == "cloud.png").Count();
        OnPropertyChanged("Cloudy");
        WindyDays = DailyForecastDetails.Where(day => day.weatherTypeIcon == "bad_weather.png").Count();
        OnPropertyChanged("WindyDays");
        sunny = DailyForecastDetails.Where(day => day.weatherTypeIcon == "sun.jpg").Count();
        OnPropertyChanged("sunny");
    }
    
    public WeatherForecastDto? WeatherForecastData { get; set; }
    
    public IEnumerable<DayForecastSummaryViewModel> DailyForecastDetails { get; set; }
    
    public int Cloudy { get; set; }
    public int WindyDays { get; set; }
    public int sunny { get; set; }
}

public class DayForecastSummaryViewModel : ViewModelBase
{
    class DayForecastModel
    {
        public DateTime Date;
        public string weatherType;
    }

    public async void RefreshData(ForecastDetailsViewModel allData)
    {
        int dayNumber = allData.DailyForecastDetails.ToList().IndexOf(this);
        temperature = allData.WeatherForecastData.temperatures[dayNumber];
        pressure = allData.WeatherForecastData.AtmospherePressure[dayNumber];
        
        
        var dto = new DayForecastModel {Date = DateTime.Today.AddDays(dayNumber)};
        dto = await new HttpService().Execute("weather-forecast/for-day", dto,
            "application/json", "Tkkm9iCRWHecw")
            .ConfigureAwait(false);
        if (dto.Date != DateTime.Today.AddDays(dayNumber))
        {
            AlertService.Instance.ShowAlert("Oops", "Something went wrong", "OK");
            return;
        }

        if (dto.weatherType=="cloudy")
        {
            weatherTypeIcon="cloud.png";
        }
        if (dto.weatherType=="windy")
        {
            weatherTypeIcon  ="bad_weather.png";
        }
        if (dto.weatherType == "sunny")
        {
            weatherTypeIcon = "sun.jpg";
        }
        
        OnPropertyChanged("temperature");
        OnPropertyChanged("pressure");
        OnPropertyChanged("weatherTypeIcon");
    }
    
    public double temperature { get; set; }
    public double pressure { get; set; }
    public string weatherTypeIcon { get; set; }
}

public class HttpService
{
    public async Task<T> Execute<T>(string url, T dto, string jsonContentType, string clientSecret)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, new Uri($"https://some-base-url/{url}"));
        request.Content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, jsonContentType);
        request.Headers.Add("Client-Secret", clientSecret);

        var _response = await new HttpClient().SendAsync(request).GetAwaiter().GetResult().Content.ReadAsStringAsync();
        if (_response == null || _response == "")
        {
            AlertService.Instance.ShowAlert("Oops", "Something went wrong", "OK");
            return default;
        }
        return JsonConvert.DeserializeObject<T>(_response);
    }
}