namespace CodeReviewSample.CodeForReview.Dtos;

public class ForecastRequestResponseDto
{
    public DateTime ForecastStartDate { get; set; }
    public int ForecastDurationInDays { get; set; }
    
    public List<double> temperatures { get; set; }
}