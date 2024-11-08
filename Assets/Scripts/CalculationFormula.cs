using UnityEngine;
using TMPro;

public class CalculationFormula : MonoBehaviour
{
    [SerializeField]
    TMP_Text formulaText;

    private int answer;
    private string[] operators = { "+", "-", "Å~", "ÅÄ" };
    private string currentFormula;

    private void GenerateFormula()
    {
        int num1, num2;
        string selectedOperator;
        bool validFormula = false;

        do
        {
            num1 = Random.Range(0, 10);
            num2 = Random.Range(1, 10);
            selectedOperator = operators[Random.Range(0, operators.Length)];

            switch (selectedOperator)
            {
                case "+":
                    answer = num1 + num2;
                    validFormula = answer < 10;
                    break;
                case "-":
                    answer = num1 - num2;
                    validFormula = answer >= 0 && answer < 10;
                    break;
                case "Å~":
                    answer = num1 * num2;
                    validFormula = answer < 10;
                    break;
                case "ÅÄ":
                    if (num1 >= num2 && num1 % num2 == 0)
                    {
                        answer = num1 / num2;
                        validFormula = answer < 10;
                    }
                    break;
            }
        }
        while (!validFormula);

        currentFormula = $"{num1} {selectedOperator} {num2} = ?";
        formulaText.text = currentFormula;
    }

    public string GetFormula()
    {
        return currentFormula;
    }

    public void Next()
    {
        GenerateFormula();
    }

    public bool IsCollect(int num)
    {
        return num == answer;
    }
}