using UnityEngine;

public class DigitCheck: MonoBehaviour
{
    [SerializeField]
    MNISTEngine engine;

    [SerializeField]
    DrawingSystem drawing;

    public void CheckDigit()
    {
        var result = engine.GetMostLikelyDigitProbability(drawing.drawTexture);
        Debug.Log(result.Item1 + " " + result.Item2);
    }
}
