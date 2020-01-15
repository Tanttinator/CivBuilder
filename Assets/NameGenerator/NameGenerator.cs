using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class NameGenerator {

    public enum Style
    {
        Anglic
    }

    static int chunks = 8;
    static int minLength = 3;

    static Dictionary<Style, Dictionary<string, List<string>>> nextCharsSurname = new Dictionary<Style, Dictionary<string, List<string>>>();
    static Dictionary<Style, Dictionary<string, List<string>>> nextCharsMale = new Dictionary<Style, Dictionary<string, List<string>>>();
    static Dictionary<Style, Dictionary<string, List<string>>> nextCharsFemale = new Dictionary<Style, Dictionary<string, List<string>>>();
    static Dictionary<Style, Dictionary<string, List<string>>> nextCharsCity = new Dictionary<Style, Dictionary<string, List<string>>>();

    public static void Init()
    {
        foreach(Style style in (Style[])System.Enum.GetValues(typeof(Style)))
        {
            TextAsset surnames = Resources.Load<TextAsset>("Surname/" + style.ToString());
            TextAsset males = Resources.Load<TextAsset>("Male/" + style.ToString());
            TextAsset females = Resources.Load<TextAsset>("Female/" + style.ToString());
            TextAsset cities = Resources.Load<TextAsset>("City/" + style.ToString());

            if (surnames == null)
                Debug.Log("Couldn't load surnames for " + style.ToString());
            if (males == null)
                Debug.Log("Couldn't load male names for " + style.ToString());
            if (females == null)
                Debug.Log("Couldn't load female names for " + style.ToString());
            if (cities == null)
                Debug.Log("Couldn't load cities for " + style.ToString());

            string[] surnameList = surnames.ToString().Split('\n');
            string[] maleList = males.ToString().Split('\n');
            string[] femaleList = females.ToString().Split('\n');
            string[] cityList = cities.ToString().Split('\n');

            nextCharsSurname.Add(style, GenerateNextCharDictionary(surnameList));
            nextCharsMale.Add(style, GenerateNextCharDictionary(maleList));
            nextCharsFemale.Add(style, GenerateNextCharDictionary(femaleList));
            nextCharsCity.Add(style, GenerateNextCharDictionary(cityList));
        }
    }

    static Dictionary<string, List<string>> GenerateNextCharDictionary(string[] names)
    {
        Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
        dictionary.Add("start", new List<string>());
        foreach (string nameUpper in names)
        {
            string name = nameUpper.ToLower();
            char[] chars = name.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {

                string s_char = "";
                for (int j = 0; j < chunks; j++)
                {
                    if (i - j == -1)
                    {
                        string[] next = GetNextChars(chars, 0);

                        foreach (string nextString in next)
                        {
                            dictionary["start"].Add(nextString);
                        }

                        continue;
                    }


                    if (i - j >= 0)
                    {
                        s_char = chars[i - j].ToString() + s_char;

                        if (!dictionary.ContainsKey(s_char))
                        {
                            dictionary.Add(s_char, new List<string>());
                        }

                        string[] next = GetNextChars(chars, i);

                        foreach(string nextString in next)
                        {
                            dictionary[s_char].Add(nextString);
                        }
                    }
                }
            }
        }

        return dictionary;
    }

    static string[] GetNextChars(char[] word, int i)
    {
        string s_next = "";
        List<string> chars = new List<string>();
        for (int k = 1; k <= chunks; k++)
        {
            if (i + k < word.Length)
            {
                s_next += word[i + k].ToString();
                chars.Add(s_next);
            }
            if (i + k == word.Length)
            {
                s_next += "_end_";
                chars.Add(s_next);
                return chars.ToArray();
            }
        }

        return chars.ToArray();
    }

    public static string GenerateSurname(Style style, string start = "")
    {
        return GenerateName(nextCharsSurname[style], start);
    }

    public static string GenerateMaleName(Style style, string start = "")
    {
        return GenerateName(nextCharsMale[style], start);
    }

    public static string GenerateFemaleName(Style style, string start = "")
    {
        return GenerateName(nextCharsFemale[style], start);
    }

    public static string GenerateCityName(Style style, string start = "")
    {
        return GenerateName(nextCharsCity[style], start);
    }

    public static string GenerateName(Dictionary<string, List<string>> nextChar, string start)
    {
        string name = start;

        string next = "";

        if(name == "")
        {
            next = Random(nextChar, "start");
        }

        int i = 0;

        string nextTry;

        while(( name.Length < minLength || (next.Length < 5 || next.Substring(next.Length - 5) != "_end_")) && i < 1000)
        {
            if (next.Length < 5 || next.Substring(next.Length - 5) != "_end_")
            {
                name += next;
            }
            nextTry = Random(nextChar, name.Length > chunks? name.Substring(chunks) : name);
            if(nextTry == null)
            {
                break;
            }
            next = nextTry;
            i++;
        }
        
        if (next.Length >= 5 && next.Substring(next.Length - 5) == "_end_")
            name += next.Remove(next.Length - 5);
        


        return name.First().ToString().ToUpper() + name.Substring(1);
    }

    public static string Random(Dictionary<string, List<string>> nextChar, string character)
    {
        for(int i = 0; i < character.Length; i++)
        {
            if (nextChar.ContainsKey(character.Substring(i)))
                return nextChar[character.Substring(i)][UnityEngine.Random.Range(0, nextChar[character.Substring(i)].Count - 1)];
        }

        return null;
    }

}
