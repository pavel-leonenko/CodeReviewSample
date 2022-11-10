using CodeReviewSample.CodeForReview.Models;
using CodeReviewSample.CodeForReview.Services;

namespace CodeReviewSample.CodeForReview.ViewModels;

public class DayForecastSummaryViewModel : ViewModelBase
{
    private DayForecastSummaryModel Model;
    
    // Assume there are bindings to all these properties on UI
    public double temperature { get; set; }
    public string WeatherTypeIcon { get; set; }

    public async void RefreshData(ForecastScreenViewModel allData)
    {
        var dayNumber = allData.DailyForecastDetails.ToList().IndexOf(this);
        temperature = allData.WeatherForecastData.temperatures[dayNumber];
        OnPropertyChanged("temperature");

        try
        {
            var dto = new DayForecastSummaryModel {Date = DateTime.Today.AddDays(dayNumber)};
            dto = await new HttpService().Execute("weather-forecast/for-day", dto, "Tkkm9iCRWHecw");
            Model = dto;
        }
        catch
        {
            AlertService.Instance.ShowAlert("Oops", "Something went wrong", "OK");
            Model = null;
        }
        finally
        {
            RefreshWeatherType();
        }
    }

    private void RefreshWeatherType()
    {
        if (Model.WeatherType=="cloudy")
        {
            WeatherTypeIcon= "cloud.png";
        }
        if (Model.WeatherType=="windy")
        {
            WeatherTypeIcon= "bad_weather.png";
        }
        if (Model.WeatherType == "sunny")
        {
            WeatherTypeIcon= "sun.jpg";
        }
        OnPropertyChanged("WeatherTypeIcon");
    }
}