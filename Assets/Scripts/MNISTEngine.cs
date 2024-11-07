using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Sentis;
using Unity.Collections;
using System;

public class MNISTEngine : MonoBehaviour
{
    [SerializeField]
    ModelAsset modelAsset;

    Model runtimeModel;
    Worker worker;

    private void Start()
    {
        Model sourceModel = ModelLoader.Load(modelAsset);

        FunctionalGraph graph = new FunctionalGraph();
        FunctionalTensor[] inputs = graph.AddInputs(sourceModel);
        FunctionalTensor[] outputs = Functional.Forward(sourceModel, inputs);
        FunctionalTensor softmax = Functional.Softmax(outputs[0]);

        runtimeModel = graph.Compile(softmax);

        worker = new Worker(runtimeModel, BackendType.GPUCompute);
    }

    public (float, int) GetMostLikelyDigitProbability(Texture2D drawableTexture)
    {
        using Tensor inputTensor = TextureConverter.ToTensor(drawableTexture, width: 28, height: 28, channels: 1);

        worker.Schedule(inputTensor);

        Tensor<float> outputTensor = worker.PeekOutput() as Tensor<float>;
        var outputArray = outputTensor.DownloadToNativeArray();

        float maxProbability = float.MinValue;
        int mostLikelyDigit = -1;

        for (int i = 0; i < outputArray.Length; i++)
        {
            float probability = outputArray[i];
            if (probability > maxProbability)
            {
                maxProbability = probability;
                mostLikelyDigit = i;
            }
        }

        inputTensor.Dispose();
        outputTensor.Dispose();

        return (maxProbability, mostLikelyDigit);
    }

    private void OnDisable()
    {
        worker.Dispose();
    }
}