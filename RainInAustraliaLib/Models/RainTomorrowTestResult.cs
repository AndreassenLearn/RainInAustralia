namespace RainInAustraliaLib.Models
{
    public class RainTomorrowTestResult
    {
        public AussieWeatherInputTestDTO Input { get; set; }
        public bool Prediction { get; set; }
        public float Probability { get; set; }
        public bool Success
        {
            get => Input.RainTomorrow == Prediction;
        }
    }
}
