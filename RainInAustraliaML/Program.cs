using RainInAustraliaML;

Console.WriteLine("================= Type 'train' to retrain the model =================");

// Get user confirmation.
//string? input;
//do
//{
//    input = Console.ReadLine();
//
//} while (!String.IsNullOrEmpty(input) && input != "train");

// Retrain model.
AussieRainModel.TrainAsync().GetAwaiter().GetResult();

Console.WriteLine("=============== End of process, hit any key to finish ===============");
Console.ReadKey();
