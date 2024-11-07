using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    CalculationFormula formula;

    [SerializeField]
    MNISTEngine engine;

    [SerializeField]
    DrawingSystem drawingSystem;

    [SerializeField]
    TMP_Text probabilityText;

    [SerializeField]
    TMP_Text numberText;

    private bool isCheck = false;
    private bool wasDrawing = false;
    private float lastCheckTime = 0f;

    private void Start()
    {
        formula.Next();
    }

    private void Update()
    {
        if (wasDrawing && !drawingSystem.isDrawing)
        {
            if (Time.time - lastCheckTime >= 1.0f)
            {
                isCheck = true;
                lastCheckTime = Time.time;
            }
        }

        if (isCheck)
        {
            (float probability, int recognizedDigit) = engine.GetMostLikelyDigitProbability(drawingSystem.drawTexture);
            probabilityText.text = probability.ToString();
            numberText.text = recognizedDigit.ToString();

            if (probability > 0.6f)
            {
                var result = formula.IsCollect(recognizedDigit);
                if (result)
                {
                    drawingSystem.ClearTexture();
                    formula.Next();
                }
            }

            isCheck = false;
        }

        wasDrawing = drawingSystem.isDrawing;
    }
}