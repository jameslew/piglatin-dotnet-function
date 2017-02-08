using System;

public static string GetEnv(string name)
{
  return System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process); 
}

public static string translateToPigLatin(string message)
{
    string english = TrimPunctuation(message);
    string pigLatin = "";
    string firstLetter;
    string restOfWord;
    string vowels = "AEIOUaeiou";
    int letterPos;

    string outBuffer = "";
    foreach (string word in english.Split())
    {
        if (word == "") continue;
        firstLetter = word.Substring(0, 1);
        restOfWord = word.Substring(1, word.Length - 1);
        letterPos = vowels.IndexOf(firstLetter);
        if (letterPos == -1)
        {
            //it's a consonant
            pigLatin = restOfWord + firstLetter + "ay";
        }
        else
        {
            //it's a vowel
            pigLatin = word + "way";
        }
        outBuffer += pigLatin + " ";
    }
    return outBuffer.Trim();
}

/// &llt;summary>
/// TrimPunctuation from start and end of string.
/// </summary>
static string TrimPunctuation(string value)
{
    // Count start punctuation.
    int removeFromStart = 0;
    for (int i = 0; i < value.Length; i++)
    {
        if (char.IsPunctuation(value[i]) || value[i] == '@')
        {
            removeFromStart++;
        }
        else
        {
            break;
        }
    }

    // Count end punctuation.
    int removeFromEnd = 0;
    for (int i = value.Length - 1; i >= 0; i--)
    {
        if (char.IsPunctuation(value[i]))
        {
            removeFromEnd++;
        }
        else
        {
            break;
        }
    }
    // No characters were punctuation.
    if (removeFromStart == 0 &&
        removeFromEnd == 0)
    {
        return value;
    }
    // All characters were punctuation.
    if (removeFromStart == value.Length &&
        removeFromEnd == value.Length)
    {
        return "";
    }
    // Substring.
    return value.Substring(removeFromStart,
        value.Length - removeFromEnd - removeFromStart);
}

