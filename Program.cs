using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.Transforms.Onnx;

namespace SpeakerModel
{
    public class ModelInputDirect
    {
        [VectorType(1, 1, 40, 40)]
        public float[] Input { get; set; }
    }

    public class ModelOutputDirect
    {
        public float[] Output { get; set; }
    }

    public class ModelInput
    {
        [ColumnName("input")]
        [VectorType(1, 1, 40, 40)]  // Adjust dimensions to match your model input
        public float[] Features { get; set; } = new float[1 * 1 * 40 * 40];  // Flattened 4D array for ONNX input
    }

    public class ModelOutput
    {
        [ColumnName("output")]
        public float Prediction { get; set; }
    }

    public class ModelOutputArray
    {
        [ColumnName("output")]
        public float[] Prediction { get; set; }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            // https://netron.app/ can see model's input and output

            var model = "sample.onnx";
            PredictDirect(model);

            try
            {
                PredictWithMLContextSingle(model);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            try
            {
                PredictWithMLContextArray(model);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void PredictDirect(string modelPath)
        {
            // Create MLContext
            MLContext mlContext = new MLContext();
            using var session = new InferenceSession(modelPath);

            Console.WriteLine("\r\nRun PredictDirect:");

            // Prepare input data (replace this with your actual input data)
            float[] inputData = new float[1 * 1 * 40 * 40];
            for (int i = 0; i < inputData.Length; i++)
            {
                inputData[i] = 0.1f; // Example value, replace with your actual data
            }

            var input = new ModelInputDirect { Input = inputData };

            // Create input tensor
            var inputTensor = new DenseTensor<float>(input.Input, new[] { 1, 1, 40, 40 });
            var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("input", inputTensor) };

            // Run inference
            using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = session.Run(inputs);

            // Get output
            var outputTensor = results.First().AsTensor<float>();
            var output = new ModelOutputDirect { Output = outputTensor.ToArray() };

            // Print results
            Console.WriteLine("Direct Prediction result:");
            for (int i = 0; i < output.Output.Length; i++)
            {
                Console.WriteLine($"Output[{i}]: {output.Output[i]}");
            }
        }

        public static void PredictWithMLContextSingle(string modelPath)
        {
            Console.WriteLine("\r\nRun PredictWithMLContext:");

            var mlContext = new MLContext();

            // Define the ONNX pipeline
            var pipeline = mlContext.Transforms.ApplyOnnxModel(
                modelFile: modelPath,
                outputColumnNames: new[] { "output" },
                inputColumnNames: new[] { "input" });
            // Prepare input data (replace this with your actual input data)
            var input = new ModelInput();
            for (int i = 0; i < input.Features.Length; i++)
            {
                input.Features[i] = 0.1f; // Example value, replace with your actual data
            }

            // Create the data view with a single item wrapped in a list
            var emptyDataView = mlContext.Data.LoadFromEnumerable(new[] { input });

            // Fit the pipeline
            var mlModel = pipeline.Fit(emptyDataView);

            // Load the model
            var predictionEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);

            // Perform prediction
            var output = predictionEngine.Predict(input);

            Console.WriteLine("Using MLContext Prediction result:");
            Console.WriteLine($"Output: {output}");
        }

        public static void PredictWithMLContextArray(string modelPath)
        {
            Console.WriteLine("\r\nRun PredictWithMLContextArray:");

            var mlContext = new MLContext();

            // Define the ONNX pipeline
            var pipeline = mlContext.Transforms.ApplyOnnxModel(
                modelFile: modelPath,
                outputColumnNames: new[] { "output" },
                inputColumnNames: new[] { "input" });
            // Prepare input data (replace this with your actual input data)
            var input = new ModelInput();
            for (int i = 0; i < input.Features.Length; i++)
            {
                input.Features[i] = 0.1f; // Example value, replace with your actual data
            }

            // Create the data view with a single item wrapped in a list
            var emptyDataView = mlContext.Data.LoadFromEnumerable(new[] { input });

            // Fit the pipeline
            var mlModel = pipeline.Fit(emptyDataView);

            // Load the model
            var predictionEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutputArray>(mlModel);

            // Perform prediction
            var output = predictionEngine.Predict(input);

            Console.WriteLine("Using MLContext Prediction result:");
            Console.WriteLine($"Output: {output}");
        }

    }



}