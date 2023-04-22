using RainInAustraliaML;

Console.WriteLine("================= Type 'train' to retrain the model =================");

// Get user confirmation.
string? input;
do
{
    input = Console.ReadLine();

} while (!String.IsNullOrEmpty(input) && input != "train");

// Retrain model.
var result = AussieRainModel.TrainAsync().GetAwaiter().GetResult();

Console.WriteLine("============================== Summary ==============================");
Console.WriteLine($"Accuracy:        {result.Metric} ({double.Round(result.Metric * 100, 2)}%)");
Console.WriteLine($"Loss:            {1 + result.Loss} ({double.Round((1 + result.Loss) * 100, 2)}%)");
Console.WriteLine($"Duration:        {result.DurationInMilliseconds} ms ({double.Round(result.DurationInMilliseconds / 1000, 2)} s)");
Console.WriteLine($"Peak CPU usage:  {double.Round((result.PeakCpu ?? 0) * 100, 2)}%");
Console.WriteLine($"Peak mem. usage: {double.Round((result.PeakMemoryInMegaByte ?? 0) * 100, 1)} MB");

Console.WriteLine("=============== End of process, hit any key to finish ===============");
Console.ReadKey();
