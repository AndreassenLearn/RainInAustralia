namespace RainInAustraliaLib.Models
{
    public class AussieWeatherInputCSV
    {
        public DateOnly Date { get; set; }
        public string Location { get; set; }
        public float MinTemp { get; set; }
        public float MaxTemp { get; set; }
        public float Rainfall { get; set; }
        public string WindGustDir { get; set; }
        public ushort WindGustSpeed { get; set; }
        public string WindDir9am { get; set; }
        public string WindDir3pm { get; set; }
        public ushort WindSpeed9am { get; set; }
        public ushort WindSpeed3pm { get; set; }
        public ushort Humidity9am { get; set; }
        public ushort Humidity3pm { get; set; }
        public float Pressure9am { get; set; }
        public float Pressure3pm { get; set; }
        public float Temp9am { get; set; }
        public float Temp3pm { get; set; }
        public string RainToday { get; set; }
        public string RainTomorrow { get; set; }
    }
}
