using UnityEngine;

public static class QuestionFactory
{
    public static int[] GenerateDigitsForQuestion(int questionIndex)
    {
        int digitCount = questionIndex switch
        {
            1 => 2,
            2 => 2,
            3 => 3,
            4 => 4,
            5 => 5,
            _ => 5
        };

        int min = (int)Mathf.Pow(10, digitCount - 1);
        int max = (int)Mathf.Pow(10, digitCount) - 1;

        int number = Random.Range(min, max + 1);

        string s = number.ToString();
        int[] digits = new int[s.Length];
        for (int i = 0; i < s.Length; i++)
            digits[i] = s[i] - '0';

        return digits;
    }
}
