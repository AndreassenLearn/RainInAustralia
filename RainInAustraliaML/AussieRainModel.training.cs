using Microsoft.ML.Trainers.LightGbm;
using Microsoft.ML.Transforms;
using Microsoft.ML;
using CsvHelper;
using RainInAustraliaLib.Models;
using Microsoft.ML.AutoML;
using Microsoft.ML.Data;

namespace RainInAustraliaML
{
    public partial class AussieRainModel
    {
        //private const string _dataPath =  @"C:\vs\RainInAustralia\Data\weather-train.csv";
        private const string _dataPath =  @"C:\vs\RainInAustralia\Data\weather-validation.csv";
        private const string _modelPath = @"C:\vs\RainInAustralia\RainInAustraliaML\AussieRainModel.mlnet";

        private const uint _trainTimeSec = 50;

        private const char _retrainSeparatorChar = ',';
        private const bool _retrainHasHeader =  true;

        /// <summary>
        /// Train a new model with the provided dataset.
        /// </summary>
        /// <param name="inputDataFilePath">Path to the data file for training.</param>
        /// <param name="separatorChar">Separator character for delimited training file.</param>
        /// <param name="hasHeader">Boolean if training file has a header.</param>
        public static async Task TrainAsync(string inputDataFilePath = _dataPath, char separatorChar = _retrainSeparatorChar, bool hasHeader = _retrainHasHeader)
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
            //var experiment = mlContext.Auto().CreateBinaryClassificationExperiment(_trainTimeSec);
            var experiment = mlContext.Auto().CreateExperiment()
                .SetPipeline(pipeline)
                .SetBinaryClassificationMetric(BinaryClassificationMetric.Accuracy, nameof(ModelInput.RainTomorrow), nameof(ModelOutput.RainTomorrow))
                .SetTrainingTimeInSeconds(_trainTimeSec)
                .SetDataset(split.TrainSet, split.TestSet);

            TrialResult experimentResults = await experiment.RunAsync();

            // Save model.
            SaveModel(mlContext, experimentResults.Model, data);

            //var model = pipeline.Fit(split.TrainSet);

            // Test model and calculate accuarcy.
            //var predictions = model.Transform(split.TestSet);

            //var preview = predictions.Preview();
            //var metrics = mlContext.Regression.Evaluate(predictions, nameof(ModelOutput.Probability), nameof(ModelOutput.Score));

            //SaveModel(mlContext, model, data);
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

            using (var fs = File.Create(_modelPath))
            {
                mlContext.Model.Save(model, dataViewSchema, fs);
            }
        }

        public static SweepablePipeline BuildPipeline(MLContext mlContext, IDataView data) => 
            mlContext.Auto().Featurizer(data,
                //catelogicalColumns: new[]
                //{
                //    nameof(ModelInput.Location),
                //    nameof(ModelInput.RainToday)
                //},
                //numericColumns: new[]
                //{
                //    nameof(ModelInput.Date),
                //    nameof(ModelInput.MinTemp),
                //    nameof(ModelInput.MaxTemp),
                //    nameof(ModelInput.Rainfall),
                //    nameof(ModelInput.WindGustSpeed),
                //    nameof(ModelInput.WindSpeed9am),
                //    nameof(ModelInput.WindSpeed3pm),
                //    nameof(ModelInput.Humidity9am),
                //    nameof(ModelInput.Humidity3pm),
                //    nameof(ModelInput.Pressure9am),
                //    nameof(ModelInput.Pressure3pm),
                //    nameof(ModelInput.Temp9am),
                //    nameof(ModelInput.Temp3pm),
                //},
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
            .Append(mlContext.Auto().BinaryClassification(labelColumnName: nameof(ModelInput.RainTomorrow), featureColumnName: "Features"));

        /// <summary>
        /// Build the pipeline that is used from model builder. Use this function to retrain model.
        /// </summary>
        /// <param name="mlContext"></param>
        /// <returns>Data process configuration with pipeline data transformations.</returns>
        public static IEstimator<ITransformer> BuildPipeline(MLContext mlContext) =>
            mlContext.Transforms.Categorical.OneHotEncoding(@"Location", @"Location", outputKind: OneHotEncodingEstimator.OutputKind.Indicator)
                //.Append(mlContext.Transforms.Conversion.MapValue(@"RainToday", LookupMaps.BooleanMap))
                //.Append(mlContext.Transforms.Conversion.MapValue(@"RainTomorrow", LookupMaps.BooleanMap))
                .Append(mlContext.Transforms.Conversion.ConvertType(@"RainToday", @"RainToday"))
                //.Append(mlContext.Transforms.Conversion.ConvertType(@"RainTomorrow", @"RainTomorrow"))
                //.Append(mlContext.Transforms.NormalizeMinMax(@"MinTemp"))
                //.Append(mlContext.Transforms.NormalizeMinMax(@"MaxTemp"))
                //.Append(mlContext.Transforms.NormalizeMinMax(@"Rainfall"))
                //.Append(mlContext.Transforms.NormalizeMinMax(@"WindGustSpeed"))
                //.Append(mlContext.Transforms.NormalizeMinMax(@"WindSpeed9am"))
                //.Append(mlContext.Transforms.NormalizeMinMax(@"WindSpeed3pm"))
                //.Append(mlContext.Transforms.NormalizeMinMax(@"Humidity9am"))
                //.Append(mlContext.Transforms.NormalizeMinMax(@"Humidity3pm"))
                //.Append(mlContext.Transforms.NormalizeMinMax(@"Pressure9am"))
                //.Append(mlContext.Transforms.NormalizeMinMax(@"Pressure3pm"))
                //.Append(mlContext.Transforms.NormalizeMinMax(@"Temp9am"))
                //.Append(mlContext.Transforms.NormalizeMinMax(@"Temp3pm"))
                .Append(mlContext.Transforms.ReplaceMissingValues(new []{new InputOutputColumnPair(@"Date", @"Date"),new InputOutputColumnPair(@"MinTemp", @"MinTemp"),new InputOutputColumnPair(@"MaxTemp", @"MaxTemp"),new InputOutputColumnPair(@"Rainfall", @"Rainfall"),new InputOutputColumnPair(@"WindGustSpeed", @"WindGustSpeed"),new InputOutputColumnPair(@"WindSpeed9am", @"WindSpeed9am"),new InputOutputColumnPair(@"WindSpeed3pm", @"WindSpeed3pm"),new InputOutputColumnPair(@"Humidity9am", @"Humidity9am"),new InputOutputColumnPair(@"Humidity3pm", @"Humidity3pm"),new InputOutputColumnPair(@"Pressure9am", @"Pressure9am"),new InputOutputColumnPair(@"Pressure3pm", @"Pressure3pm"),new InputOutputColumnPair(@"Temp9am", @"Temp9am"),new InputOutputColumnPair(@"Temp3pm", @"Temp3pm")}))      
                .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName:@"WindGustDir",outputColumnName:@"WindGustDir"))      
                .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName:@"WindDir9am",outputColumnName:@"WindDir9am"))      
                .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName:@"WindDir3pm",outputColumnName:@"WindDir3pm"))      
                .Append(mlContext.Transforms.Concatenate("Features", new[]
                {
                    @"Location",
                    @"Date",
                    @"MinTemp",
                    @"MaxTemp",
                    @"Rainfall",
                    @"WindGustSpeed",
                    @"WindSpeed9am",
                    @"WindSpeed3pm",
                    @"Humidity9am",
                    @"Humidity3pm",
                    @"Pressure9am",
                    @"Pressure3pm",
                    @"Temp9am",
                    @"Temp3pm",
                    @"RainToday",
                    @"WindGustDir",
                    @"WindDir9am",
                    @"WindDir3pm"
                }))
                .Append(mlContext.BinaryClassification.Trainers.LightGbm(new LightGbmBinaryTrainer.Options()
                {
                    NumberOfLeaves=7928,
                    NumberOfIterations=4326,
                    MinimumExampleCountPerLeaf=23,
                    LearningRate=0.10475501273235,
                    LabelColumnName= nameof(ModelInput.RainTomorrow),
                    FeatureColumnName="Features",
                    ExampleWeightColumnName=null,
                    Booster=new GradientBooster.Options()
                    {
                        SubsampleFraction=0.139915823613145,
                        FeatureFraction=0.772247637950334,
                        L1Regularization=4.39325070309715E-10,
                        L2Regularization=0.999999776672986
                    },
                    MaximumBinCountPerFeature=233
                }));
    }
 }
