using RainInAustraliaLib.Extensions;

namespace RainInAustraliaLib.Models
{
    public class AussieWeatherInputTestDTO : AussieWeatherInputDTO
    {
        public AussieWeatherInputTestDTO() { }

        public AussieWeatherInputTestDTO(AussieWeatherInputCSV parameters)
            : base()
        {
            //Date = parameters.Date;
            Date = new DateOnly(9999, parameters.Date, 1);
            Location = parameters.Location;
            MinTemp = parameters.MinTemp;
            MaxTemp = parameters.MaxTemp;
            Rainfall = parameters.Rainfall;
            WindGustDir = parameters.WindGustDir;
            WindGustSpeed = parameters.WindGustSpeed;
            WindDir9am = parameters.WindDir9am;
            WindDir3pm = parameters.WindDir3pm;
            WindSpeed9am = parameters.WindSpeed9am;
            WindSpeed3pm = parameters.WindSpeed3pm;
            Humidity9am = parameters.Humidity9am;
            Humidity3pm = parameters.Humidity3pm;
            Pressure9am = parameters.Pressure9am;
            Pressure3pm = parameters.Pressure3pm;
            Temp9am = parameters.Temp9am;
            Temp3pm = parameters.Temp3pm;
            RainToday = parameters.RainToday.ToSampleBool();
            RainTomorrow = parameters.RainTomorrow.ToSampleBool();
        }

        public bool RainTomorrow { get; set; }
    }
}
