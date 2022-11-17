using CodeReviewSample.CodeForReview.Dtos;
using CodeReviewSample.CodeForReview.Services;

namespace CodeReviewSample.CodeForReview.ViewModels;

public class ForecastScreenViewModel : ViewModelBase
{
    // Assume there are bindings to all these properties on UI
    public IEnumerable<DayForecastSummaryViewModel> DailyForecastDetails { get; set; }
    public ForecastRequestResponseDto WeatherForecastData { get; set; }
    public int CloudyDaysCount { get; set; }
    public int Sunny { get; set; }
    
    public override async void OnAppearing()
    {
        if (WeatherForecastData == null)
        {
            WeatherForecastData = new ForecastRequestResponseDto
            {
                ForecastStartDate = DateTime.Today,
                ForecastDurationInDays = 7
            };
            DailyForecastDetails = Enumerable.Range(0, 7).Select(_ => new DayForecastSummaryViewModel());
        }
        
        WeatherForecastData = await new HttpService()
            .Execute("weather-forecast/summary", WeatherForecastData, "Tkkm9iCRWHecw")
            .ConfigureAwait(false);
        OnPropertyChanged("WeatherForecastData");

        foreach (var dailyForecast in DailyForecastDetails)
        {
            dailyForecast.RefreshData(this);
        }

        CloudyDaysCount = DailyForecastDetails.Where(day => day.WeatherTypeIcon == "cloud.png").Count();
        OnPropertyChanged("CloudyDaysCount");
        Sunny = DailyForecastDetails.Where(day => day.WeatherTypeIcon == "sun.jpg").Count();
        OnPropertyChanged("sunny");
    }
}