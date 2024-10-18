using System;

public class NeuralNetwork
{
    private int inputSize;
    private int hiddenSize;
    private int outputSize;

    private float[] hiddenLayer;
    private float[] outputLayer;

    private float[,] weightsInputHidden;
    private float[,] weightsHiddenOutput;

    private float[] biasHidden;
    private float[] biasOutput;

    public NeuralNetwork(int inputSize, int hiddenSize, int outputSize)
    {
        this.inputSize = inputSize;
        this.hiddenSize = hiddenSize;
        this.outputSize = outputSize;

        hiddenLayer = new float[hiddenSize];
        outputLayer = new float[outputSize];

        weightsInputHidden = new float[inputSize, hiddenSize];
        weightsHiddenOutput = new float[hiddenSize, outputSize];

        biasHidden = new float[hiddenSize];
        biasOutput = new float[outputSize];

        InitializeWeights();
    }

    private void InitializeWeights()
    {
        Random rand = new Random();

        for (int i = 0; i < inputSize; i++)
            for (int j = 0; j < hiddenSize; j++)
                weightsInputHidden[i, j] = (float)(rand.NextDouble() * 2 - 1);

        for (int i = 0; i < hiddenSize; i++)
        {
            biasHidden[i] = (float)(rand.NextDouble() * 2 - 1);

            for (int j = 0; j < outputSize; j++)
                weightsHiddenOutput[i, j] = (float)(rand.NextDouble() * 2 - 1);
        }

        for (int i = 0; i < outputSize; i++)
            biasOutput[i] = (float)(rand.NextDouble() * 2 - 1);
    }

    public float[] Forward(float[] inputs)
    {
        // Вычисление скрытого слоя
        for (int i = 0; i < hiddenSize; i++)
        {
            hiddenLayer[i] = biasHidden[i];
            for (int j = 0; j < inputSize; j++)
                hiddenLayer[i] += inputs[j] * weightsInputHidden[j, i];

            hiddenLayer[i] = (float)Math.Tanh(hiddenLayer[i]);
        }

        // Вычисление выходного слоя
        for (int i = 0; i < outputSize; i++)
        {
            outputLayer[i] = biasOutput[i];
            for (int j = 0; j < hiddenSize; j++)
                outputLayer[i] += hiddenLayer[j] * weightsHiddenOutput[j, i];

            outputLayer[i] = (float)Math.Tanh(outputLayer[i]);
        }

        return outputLayer;
    }
}
