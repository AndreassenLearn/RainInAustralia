using Microsoft.ML;
using CsvHelper;
using RainInAustraliaLib.Models;
using Microsoft.ML.AutoML;

namespace RainInAustraliaML
{
    public partial class AussieRainModel
    {
        private const string _dataPath =  @"C:\vs\RainInAustralia\Data\weather-train.csv";
        private const string _modelPath = @"C:\vs\RainInAustralia\RainInAustraliaML\AussieRainModel.mlnet";

        private const char _retrainSeparatorChar = ',';
        private const bool _retrainHasHeader =  true;

        private const uint _trainTimeSec = 600;

        private const string _featuresColumnName = "Features";

        /// <summary>
        /// Train a new model with the provided dataset.
        /// </summary>
        /// <param name="inputDataFilePath">Path to the data file for training.</param>
        /// <param name="separatorChar">Separator character for delimited training file.</param>
        /// <param name="hasHeader">Boolean if training file has a header.</param>
        /// <returns>Results from the training process.</returns>
        public static async Task<TrialResult> TrainAsync(string inputDataFilePath = _dataPath, char separatorChar = _retrainSeparatorChar, bool hasHeader = _retrainHasHeader)
        {
            var mlContext = new MLContext();

            // Configure logging for experiment trials.
            mlContext.Log += (_, e) => {
                if (e.Source.Equals("AutoMLExperiment"))
                {
                    Console.WriteLine(e.RawMessage);
                }
            };

            // Prepare data and pipeline.
            var data = LoadIDataViewFromFile(mlContext, inputDataFilePath, separatorChar, hasHeader);
            var split = mlContext.Data.TrainTestSplit(data, 0.2); // 20% test data.

            var pipeline = BuildPipeline(mlContext, data);

            // Train model.
            var experiment = mlContext.Auto().CreateExperiment()
                .SetPipeline(pipeline)
                .SetBinaryClassificationMetric(BinaryClassificationMetric.Accuracy, nameof(ModelInput.RainTomorrow), nameof(ModelOutput.PredictedLabel))
                .SetTrainingTimeInSeconds(_trainTimeSec)
                .SetDataset(split.TrainSet, split.TestSet);

            TrialResult experimentResults = await experiment.RunAsync();

            // Save model.
            SaveModel(mlContext, experimentResults.Model, data);

            return experimentResults;
        }

        /// <summary>
        /// Load an IDataView from a file path.
        /// </summary>
        /// <param name="mlContext">The common context for all ML.NET operations.</param>
        /// <param name="path">Path to the data file for training.</param>
        /// <param name="separator">Separator character for delimited training file.</param>
        /// <param name="hasHeader">Boolean if training file has a header.</param>
        /// <returns>IDataView with loaded training data.</returns>
        public static IDataView LoadIDataViewFromFile(MLContext mlContext, string path, char separator, bool hasHeader)
        {
            //return mlContext.Data.LoadFromTextFile<ModelInput>(path, new TextLoader.Options()
            //{
            //    Columns = new[]
            //    {
            //        new TextLoader.Column(nameof(ModelInput.RainToday), DataKind.Boolean, 17),
            //        new TextLoader.Column(nameof(ModelInput.RainTomorrow), DataKind.Boolean, 18)
            //    }
            //});
            List<ModelInput> data = new();

            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
            {
                AussieWeatherInputCSV record = new();
                var records = csv.EnumerateRecords(record);
                foreach (var r in records)
                {
                    data.Add(CreateInput(r));
                }
            }

            return mlContext.Data.LoadFromEnumerable(data);
        }

        /// <summary>
        /// Save a model at the specified path.
        /// </summary>
        /// <param name="mlContext">The common context for all ML.NET operations.</param>
        /// <param name="model">Model to save.</param>
        /// <param name="data">IDataView used to train the model.</param>
        public static void SaveModel(MLContext mlContext, ITransformer model, IDataView data)
        {
            // Pull the data schema from the IDataView used for training the model.
            DataViewSchema dataViewSchema = data.Schema;

            // Save .mlnet file.
            using var fs = File.Create(_modelPath);
            mlContext.Model.Save(model, dataViewSchema, fs);
        }

        /// <summary>
        /// Build the sweepable pipeline.
        /// </summary>
        /// <param name="mlContext">The common context for all ML.NET operations.</param>
        /// <param name="data">IDataView used to train the model.</param>
        /// <returns>Data process configuration, i.e. the pipeline.</returns>
        public static SweepablePipeline BuildPipeline(MLContext mlContext, IDataView data) => 
            mlContext.Auto().Featurizer(data, _featuresColumnName,
                catelogicalColumns: new[]
                {
                    nameof(ModelInput.Location),
                    nameof(ModelInput.RainToday)
                },
                numericColumns: new[]
                {
                    //nameof(ModelInput.Day),
                    nameof(ModelInput.DaySin),
                    nameof(ModelInput.DayCos),
                    nameof(ModelInput.MinTemp),
                    nameof(ModelInput.MaxTemp),
                    nameof(ModelInput.Rainfall),
                    nameof(ModelInput.WindGustDirSin),
                    nameof(ModelInput.WindGustDirCos),
                    nameof(ModelInput.WindGustSpeed),
                    nameof(ModelInput.WindDir9amSin),
                    nameof(ModelInput.WindDir9amCos),
                    nameof(ModelInput.WindDir3pmSin),
                    nameof(ModelInput.WindDir3pmCos),
                    nameof(ModelInput.WindSpeed9am),
                    nameof(ModelInput.WindSpeed3pm),
                    nameof(ModelInput.Humidity9am),
                    nameof(ModelInput.Humidity3pm),
                    nameof(ModelInput.Pressure9am),
                    nameof(ModelInput.Pressure3pm),
                    nameof(ModelInput.Temp9am),
                    nameof(ModelInput.Temp3pm),
                },
                //textColumns: new[]
                //{
                //    nameof(ModelInput.WindGustDir),
                //    nameof(ModelInput.WindDir9am),
                //    nameof(ModelInput.WindDir3pm)
                //},
                excludeColumns: new[]
                {
                    nameof(ModelInput.RainTomorrow)
                })
            .Append(mlContext.Transforms.NormalizeMinMax(_featuresColumnName, _featuresColumnName))
            .Append(mlContext.Auto().BinaryClassification(labelColumnName: nameof(ModelInput.RainTomorrow), featureColumnName: _featuresColumnName));
    }
 }
