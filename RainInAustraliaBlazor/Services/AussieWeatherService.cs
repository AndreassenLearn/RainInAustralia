using RainInAustraliaML;
using RainInAustraliaLib.Models;

namespace RainInAustraliaBlazor.Services
{
    public class AussieWeatherService : IAussieWeatherService
    {
        /// <inheritdoc/>
        public RainTomorrowResult RainTomorrow(AussieWeatherInputDTO parameters)
        {
            // Load model and predict output.
            AussieRainModel.ModelOutput result = AussieRainModel.Predict(AussieRainModel.CreateInput(parameters));

            return new RainTomorrowResult()
            {
                Prediction = result.PredictedLabel,
                Probability = result.Probability
            };
        }

        /// <inheritdoc/>
        public RainTomorrowTestResult RainTomorrowTest(AussieWeatherInputCSV testParameters)
        {
            // Load model and predict output.
            AussieRainModel.ModelOutput result = AussieRainModel.Predict(AussieRainModel.CreateInput(testParameters));

            /*
             * ModelOutput.Probability is the probability for a FALSE prediction value.
             * ModelOutput.Score is greater than (>) 0: FALSE
             * ModelOutput.Score is less than (<) 0: TRUE
             * We must therefore negate the ModelOutput.Probability when ModelOutput.Score < 0; i.e. 18% to 82%.
             */

            return new RainTomorrowTestResult()
            {
                Input = new AussieWeatherInputTestDTO(testParameters),
                Prediction = result.PredictedLabel,
                Probability = (result.Score < 0 ? 1 - result.Probability : result.Probability)
            };
        }
    }
}
