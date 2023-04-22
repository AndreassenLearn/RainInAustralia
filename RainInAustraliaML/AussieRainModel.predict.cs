using Microsoft.ML;
using Microsoft.ML.Data;
using RainInAustraliaLib.Models;

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
            [LoadColumn(0)]
            public float Day { get; set; }

            [LoadColumn(1)]
            public string Location { get; set; }

            [LoadColumn(2)]
            public float MinTemp { get; set; }

            [LoadColumn(3)]
            public float MaxTemp { get; set; }

            [LoadColumn(4)]
            public float Rainfall { get; set; }

            [LoadColumn(5)]
            public string WindGustDir { get; set; }

            [LoadColumn(6)]
            public float WindGustSpeed { get; set; }

            [LoadColumn(7)]
            public string WindDir9am { get; set; }

            [LoadColumn(8)]
            public string WindDir3pm { get; set; }

            [LoadColumn(9)]
            public float WindSpeed9am { get; set; }

            [LoadColumn(10)]
            public float WindSpeed3pm { get; set; }

            [LoadColumn(11)]
            public float Humidity9am { get; set; }

            [LoadColumn(12)]
            public float Humidity3pm { get; set; }

            [LoadColumn(13)]
            public float Pressure9am { get; set; }

            [LoadColumn(14)]
            public float Pressure3pm { get; set; }

            [LoadColumn(15)]
            public float Temp9am { get; set; }

            [LoadColumn(16)]
            public float Temp3pm { get; set; }

            [LoadColumn(17)]
            public bool RainToday { get; set; }

            [LoadColumn(18)]
            public bool RainTomorrow { get; set; } = false;
        }

        #endregion

        /// <summary>
        /// Model output class for AussieRainModel.
        /// </summary>
        #region Model output class
        public class ModelOutput
        {
            //[ColumnName(@"Date")]
            //public float Date { get; set; }

            //[ColumnName(@"Location")]
            //public float[] Location { get; set; }

            //[ColumnName(@"MinTemp")]
            //public float MinTemp { get; set; }

            //[ColumnName(@"MaxTemp")]
            //public float MaxTemp { get; set; }

            //[ColumnName(@"Rainfall")]
            //public float Rainfall { get; set; }

            //[ColumnName(@"WindGustDir")]
            //public float[] WindGustDir { get; set; }

            //[ColumnName(@"WindGustSpeed")]
            //public float WindGustSpeed { get; set; }

            //[ColumnName(@"WindDir9am")]
            //public float[] WindDir9am { get; set; }

            //[ColumnName(@"WindDir3pm")]
            //public float[] WindDir3pm { get; set; }

            //[ColumnName(@"WindSpeed9am")]
            //public float WindSpeed9am { get; set; }

            //[ColumnName(@"WindSpeed3pm")]
            //public float WindSpeed3pm { get; set; }

            //[ColumnName(@"Humidity9am")]
            //public float Humidity9am { get; set; }

            //[ColumnName(@"Humidity3pm")]
            //public float Humidity3pm { get; set; }

            //[ColumnName(@"Pressure9am")]
            //public float Pressure9am { get; set; }

            //[ColumnName(@"Pressure3pm")]
            //public float Pressure3pm { get; set; }

            //[ColumnName(@"Temp9am")]
            //public float Temp9am { get; set; }

            //[ColumnName(@"Temp3pm")]
            //public float Temp3pm { get; set; }

            //[ColumnName(@"RainToday")]
            //public float RainToday { get; set; }

            [ColumnName(@"RainTomorrow")]
            public bool RainTomorrow { get; set; }

            //[ColumnName(@"Features")]
            //public float[] Features { get; set; }

            //[ColumnName(@"PredictedLabel")]
            //public bool PredictedLabel { get; set; }

            [ColumnName(@"Score")]
            public float Score { get; set; }

            [ColumnName(@"Probability")]
            public float Probability { get; set; }

        }

        #endregion

        private static readonly string MLNetModelPath = Path.GetFullPath("..\\RainInAustraliaML\\AussieRainModel.mlnet");
        //private static readonly string MLNetModelPath = Path.GetFullPath("..\\RainInAustraliaModel\\AussieRainModel.mlnet");

        public static readonly Lazy<PredictionEngine<ModelInput, ModelOutput>> PredictEngine = new Lazy<PredictionEngine<ModelInput, ModelOutput>>(() => CreatePredictEngine(), true);

        public static ModelInput CreateInput(AussieWeatherInputDTO dto) => new()
        {
            Day = dto.Date.DayOfYear,
            Location = dto.Location,
            MinTemp = dto.MinTemp,
            MaxTemp = dto.MaxTemp,
            Rainfall = dto.Rainfall,
            WindGustDir = dto.WindGustDir,
            WindGustSpeed = dto.WindGustSpeed,
            WindDir9am = dto.WindDir9am,
            WindDir3pm = dto.WindDir3pm,
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

        public static ModelInput CreateInput(AussieWeatherInputCSV csv) => new()
        {
            Day = csv.Date.DayOfYear,
            Location = csv.Location,
            MinTemp = csv.MinTemp,
            MaxTemp = csv.MaxTemp,
            Rainfall = csv.Rainfall,
            WindGustDir = csv.WindGustDir,
            WindGustSpeed = csv.WindGustSpeed,
            WindDir9am = csv.WindDir9am,
            WindDir3pm = csv.WindDir3pm,
            WindSpeed9am = csv.WindSpeed9am,
            WindSpeed3pm = csv.WindSpeed3pm,
            Humidity9am = csv.Humidity9am,
            Humidity3pm = csv.Humidity3pm,
            Pressure9am = csv.Pressure9am,
            Pressure3pm = csv.Pressure3pm,
            Temp9am = csv.Temp9am,
            Temp3pm = csv.Temp3pm,
            RainToday = csv.RainToday == "true",
            RainTomorrow = csv.RainTomorrow == "true"
        };

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
