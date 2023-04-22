using RainInAustraliaLib.Models;

namespace RainInAustraliaBlazor.Services
{
    public interface IAussieWeatherService
    {
        /// <summary>
        /// Predict whether there will be rain tomorrow.
        /// </summary>
        /// <param name="parameters">Input parameters for the ML model.</param>
        /// <returns><see cref="RainTomorrowResult"/> containing whether it will be rainy tomorrow and the probability.</returns>
        public RainTomorrowResult RainTomorrow(AussieWeatherInputDTO parameters);

        /// <summary>
        /// Test prediction for which the correct answer is already known.
        /// </summary>
        /// <param name="testParameters">Input parameters read from a CSV file, including the expected value of the label column to predict.</param>
        /// <returns><see cref="RainTomorrowTestResult"/> containing the expected and actual result from the prediction.</returns>
        public RainTomorrowTestResult RainTomorrowTest(AussieWeatherInputCSV testParameters);
    }
}
