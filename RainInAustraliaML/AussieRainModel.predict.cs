using Microsoft.ML;
using Microsoft.ML.Data;
using RainInAustraliaLib.Extensions;
using RainInAustraliaLib.Models;
using System.Globalization;

namespace RainInAustraliaML
{
    public partial class AussieRainModel
    {
        /// <summary>
        /// Model input class for AussieRainModel.
        /// </summary>
        #region Model input class
        public class ModelInput
        {
            //public float Day { get; set; }
            public float DaySin { get; set; }
            public float DayCos { get; set; }
            public string Location { get; set; }
            public float MinTemp { get; set; }
            public float MaxTemp { get; set; }
            public float Rainfall { get; set; }
            //public string WindGustDir { get; set; }
            public float WindGustDirSin { get; set; }
            public float WindGustDirCos { get; set; }
            public float WindGustSpeed { get; set; }
            //public string WindDir9am { get; set; }
            public float WindDir9amSin { get; set; }
            public float WindDir9amCos { get; set; }
            //public string WindDir3pm { get; set; }
            public float WindDir3pmSin { get; set; }
            public float WindDir3pmCos { get; set; }
            public float WindSpeed9am { get; set; }
            public float WindSpeed3pm { get; set; }
            public float Humidity9am { get; set; }
            public float Humidity3pm { get; set; }
            public float Pressure9am { get; set; }
            public float Pressure3pm { get; set; }
            public float Temp9am { get; set; }
            public float Temp3pm { get; set; }
            public bool RainToday { get; set; }
            public bool RainTomorrow { get; set; } = false;
        }

        #endregion

        /// <summary>
        /// Model output class for AussieRainModel.
        /// </summary>
        #region Model output class
        public class ModelOutput
        {
            [ColumnName(@"PredictedLabel")]
            public bool PredictedLabel { get; set; }

            [ColumnName(@"Score")]
            public float Score { get; set; }

            [ColumnName(@"Probability")]
            public float Probability { get; set; }

        }

        #endregion

        private static readonly string MLNetModelPath = Path.GetFullPath("..\\RainInAustraliaML\\AussieRainModel.mlnet");

        public static readonly Lazy<PredictionEngine<ModelInput, ModelOutput>> PredictEngine = new Lazy<PredictionEngine<ModelInput, ModelOutput>>(() => CreatePredictEngine(), true);

        public static ModelInput CreateInput(AussieWeatherInputDTO dto)
        {
            var WindGustDirValue = float.Parse(dto.WindGustDir, CultureInfo.InvariantCulture);
            var WindDir9amDirValue = float.Parse(dto.WindDir9am, CultureInfo.InvariantCulture);
            var WindDir3pmDirValue = float.Parse(dto.WindDir3pm, CultureInfo.InvariantCulture);

            return new()
            {
                //Day = dto.Date.DayOfYear,
                Location = dto.Location,
                MinTemp = dto.MinTemp,
                MaxTemp = dto.MaxTemp,
                Rainfall = dto.Rainfall,
                //WindGustDir = dto.WindGustDir,
                WindGustDirSin = Convertions.WindDirectionSin(WindGustDirValue),
                WindGustDirCos = Convertions.WindDirectionCos(WindGustDirValue),
                WindGustSpeed = dto.WindGustSpeed,
                //WindDir9am = dto.WindDir9am,
                WindDir9amSin = Convertions.WindDirectionSin(WindDir9amDirValue),
                WindDir9amCos = Convertions.WindDirectionCos(WindDir9amDirValue),
                //WindDir3pm = dto.WindDir3pm,
                WindDir3pmSin = Convertions.WindDirectionSin(WindDir3pmDirValue),
                WindDir3pmCos = Convertions.WindDirectionCos(WindDir3pmDirValue),
                WindSpeed9am = dto.WindSpeed9am,
                WindSpeed3pm = dto.WindSpeed3pm,
                Humidity9am = dto.Humidity9am,
                Humidity3pm = dto.Humidity3pm,
                Pressure9am = dto.Pressure9am,
                Pressure3pm = dto.Pressure3pm,
                Temp9am = dto.Temp9am,
                Temp3pm = dto.Temp3pm,
                RainToday = dto.RainToday
            };
        }

        public static ModelInput CreateInput(AussieWeatherInputCSV csv)
        {
            var WindGustDirValue = float.Parse(csv.WindGustDir, CultureInfo.InvariantCulture);
            var WindDir9amDirValue = float.Parse(csv.WindDir9am, CultureInfo.InvariantCulture);
            var WindDir3pmDirValue = float.Parse(csv.WindDir3pm, CultureInfo.InvariantCulture);

            return new()
            {
                //Day = csv.Date.DayOfYear,
                DaySin = Convertions.DayOfYearSin(csv.Date.DayOfYear),
                DayCos = Convertions.DayOfYearCos(csv.Date.DayOfYear),
                Location = csv.Location,
                MinTemp = csv.MinTemp,
                MaxTemp = csv.MaxTemp,
                Rainfall = csv.Rainfall,
                //WindGustDir = csv.WindGustDir,
                WindGustDirSin = Convertions.WindDirectionSin(WindGustDirValue),
                WindGustDirCos = Convertions.WindDirectionCos(WindGustDirValue),
                WindGustSpeed = csv.WindGustSpeed,
                //WindDir9am = csv.WindDir9am,
                WindDir9amSin = Convertions.WindDirectionSin(WindDir9amDirValue),
                WindDir9amCos = Convertions.WindDirectionCos(WindDir9amDirValue),
                //WindDir3pm = csv.WindDir3pm,
                WindDir3pmSin = Convertions.WindDirectionSin(WindDir3pmDirValue),
                WindDir3pmCos = Convertions.WindDirectionCos(WindDir3pmDirValue),
                WindSpeed9am = csv.WindSpeed9am,
                WindSpeed3pm = csv.WindSpeed3pm,
                Humidity9am = csv.Humidity9am,
                Humidity3pm = csv.Humidity3pm,
                Pressure9am = csv.Pressure9am,
                Pressure3pm = csv.Pressure3pm,
                Temp9am = csv.Temp9am,
                Temp3pm = csv.Temp3pm,
                RainToday = csv.RainToday.ToBool(),
                RainTomorrow = csv.RainTomorrow.ToBool()
            };
        }

        /// <summary>
        /// Use this method to predict on <see cref="ModelInput"/>.
        /// </summary>
        /// <param name="input">Model input.</param>
        /// <returns><seealso cref=" ModelOutput"/></returns>
        public static ModelOutput Predict(ModelInput input)
        {
            var predEngine = PredictEngine.Value;
            return predEngine.Predict(input);
        }

        private static PredictionEngine<ModelInput, ModelOutput> CreatePredictEngine()
        {
            var mlContext = new MLContext();
            ITransformer mlModel = mlContext.Model.Load(MLNetModelPath, out var _);
            return mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);
        }
    }
}
