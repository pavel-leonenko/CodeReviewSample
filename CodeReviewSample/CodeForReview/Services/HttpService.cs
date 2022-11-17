using Newtonsoft.Json;

namespace CodeReviewSample.CodeForReview.Services;

public class HttpService
{
    public const string BASE_URL = "https://some-base-url/";
    
    public async Task<T> Execute<T>(string url, T dto, string clientSecret)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, new Uri($"{BASE_URL}{url}"));
        request.Content = new StringContent(JsonConvert.SerializeObject(dto));
        request.Headers.Add("Client-Secret", clientSecret);

        var response = new HttpClient().SendAsync(request).GetAwaiter().GetResult();
        var responseContent = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrEmpty(responseContent))
        {
            AlertService.Instance.ShowAlert("Oops", "Something went wrong", "OK");
            return default;
        }
        
        return JsonConvert.DeserializeObject<T>(responseContent);
    }
}