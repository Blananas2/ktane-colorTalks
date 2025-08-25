using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public class kiloTalkScript : MonoBehaviour
{

    public KMAudio Audio;
    public KMBombModule Module;

    public TextMesh[] NumberTexts;
    public TextMesh UnitText; //a klaxon will go off if you misread this
    public Transform ArrowsObj;
    public Transform RightObj;
    public KMSelectable Submit;
    public KMSelectable RightArrow;
    public KMSelectable[] UpDownArrows;

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    ulong[] unitValues = {
      //TGMkhχ-dcmμnpfaz
                 1000000, //Milligrams
                10000000, //Centigrams
               100000000, //Decigrams
               200000000, //Carats (= 200 mg)
              1000000000, //Grams
             10000000000, //Dekagrams
             28349523125, //Ounces (= 28.349523125 g)
            100000000000, //Hectograms
            453592370000, //Pounds (≡ 0.45359237 kg)
           1000000000000, //Kilograms
           6350293180000, //Stone (= 6.35029318 kg)
         907184740000000, //Short Tons (= 907.18474 kg)
        1000000000000000, //Metric Tonnes (= 1000 kg)
        1016046908800000, //Long Tons (= 1016.0469088 kg)
    };
    string[] unitNames = { "Milligrams", "Centigrams", "Decigrams", "Carats", "Grams", "Dekagrams", "Ounces", "Hectograms", "Pounds", "Kilograms", "Stone", "Short Tons", "Metric Tonnes", "Long Tons" };
    ulong answer;
    string answerString;
    string givenString = "0";
    int numberOfDigits = 0;
    bool moving = false;
    int attps = 0;

    void Awake()
    {
        moduleId = moduleIdCounter++;

        Submit.OnInteract += delegate () { SubmitPress(); return false; };
        RightArrow.OnInteract += delegate () { AddDigit(); return false; };

        UpDownArrows[0].OnInteract += delegate { ChangeDigit(1); return false; };
        UpDownArrows[1].OnInteract += delegate { ChangeDigit(9); return false; };
    }

    // Use this for initialization
    void Start()
    {
    tryAgain:
        ulong number = (ulong)Rnd.Range(1, 10000);
        int fromUnit = Rnd.Range(0, 14);
        int toUnit = Rnd.Range(0, 14);
        while (fromUnit == toUnit)
        {
            toUnit = Rnd.Range(0, 14);
        }
        answer = number * unitValues[fromUnit] / unitValues[toUnit];
        if (answer <= 0 || answer >= 1000000000000000ul)
        {
            attps++;
            goto tryAgain;
        }
        NumberTexts[0].text = number.ToString();
        UnitText.text = unitNames[fromUnit] + "<size=72> to</size>\n" + unitNames[toUnit];
        answerString = answer.ToString();
        Debug.LogFormat("<Kilo Talk #{0}> Failed attempts: {1}", moduleId, attps);
        Debug.LogFormat("[Kilo Talk #{0}] {1} {2} to {3} is {4}", moduleId, number, unitNames[fromUnit], unitNames[toUnit], answer);
    }

    void SubmitPress()
    {
        Submit.AddInteractionPunch(0.1f);
        if (moving || moduleSolved) { return; }
        if (answerString == givenString)
        {
            Audio.PlaySoundAtTransform("KT_submit", Submit.transform);
            Module.HandlePass();
            moduleSolved = true;
            Debug.LogFormat("[Kilo Talk #{0}] Correct answer submitted. Module solved.", moduleId);
        }
        else
        {
            Module.HandleStrike();
            Debug.LogFormat("[Kilo Talk #{0}] The answer is not {1}, strike!", moduleId, givenString);
        }
    }

    void AddDigit()
    {
        RightArrow.AddInteractionPunch(0.1f);
        if (moving || moduleSolved) { return; }
        if (answerString.StartsWith(givenString) && answerString != givenString)
        {
            Audio.PlaySoundAtTransform("KT_shove", RightArrow.transform);
            StartCoroutine(MoveArrow());
        }
        else
        {
            Module.HandleStrike();
            Debug.LogFormat("[Kilo Talk #{0}] The answer does not start with {1}, strike!", moduleId, givenString);
        }
    }

    void ChangeDigit(int dif)
    {
        UpDownArrows[dif == 1 ? 0 : 1].AddInteractionPunch(0.1f);
        if (moving || moduleSolved) { return; }
        Audio.PlaySoundAtTransform("KT_" + (dif == 1 ? "up" : "down"), UpDownArrows[dif == 1 ? 0 : 1].transform);
        string before = givenString.Substring(0, numberOfDigits);
        char last = "0123456789"[(Int32.Parse(givenString[numberOfDigits].ToString()) + dif) % 10];
        givenString = before + last;
        NumberTexts[1].text = givenString;
    }

    private IEnumerator MoveArrow()
    {
        moving = true;
        givenString += "0";
        NumberTexts[1].text = givenString;
        float elapsed = 0f;
        float duration = 0.2f;
        while (elapsed < duration)
        {
            ArrowsObj.localPosition = new Vector3(Lerp(0.0102f * numberOfDigits, 0.0102f * (numberOfDigits + 1), elapsed * 5), 0f, 0f);
            if (numberOfDigits == 13) { RightObj.localScale = new Vector3(Lerp(0.02f, 0f, elapsed * 5), Lerp(0.01f, 0f, elapsed * 5), Lerp(0.018f, 0f, elapsed * 5)); }
            yield return null;
            elapsed += Time.deltaTime;
        }
        numberOfDigits++;
        ArrowsObj.localPosition = new Vector3(0.0102f * numberOfDigits, 0f, 0f);
        if (numberOfDigits == 14) { RightObj.localScale = new Vector3(0f, 0f, 0f); }
        moving = false;
    }
    
    float Lerp(float a, float b, float t)
    { //this assumes t is in the range 0-1
        return a * (1f - t) + b * t;
    }

    // Twitch Plays Support by Kilo Bites

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} enter 0123456789 [enters the number you want to input from the current index] || !{0} submit [submits the number you have]";
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        string[] split = command.ToUpperInvariant().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

        yield return null;

        if ("ENTER".ContainsIgnoreCase(split[0]))
        {
            if (split.Length == 1)
            {
                yield return "sendtochaterror Enter what?";
                yield break;
            }

            if (split.Length > 2)
            {
                yield return "sendtochaterror Too many parameters. Please try again!";
                yield break;
            }

            if (!split[1].Any(char.IsDigit))
            {
                yield return string.Format("sendtochaterror {0} is/are invalid!", split[1].Where(x => !char.IsDigit(x)).Join(", "));
                yield break;
            }

            if (split[1].Length > 14)
            {
                yield return "sendtochaterror You have too many numbers!";
                yield break;
            }

            for (int i = 0; i < split[1].Length; i++)
            {
                var targetNumber = int.Parse(split[1][i].ToString());
                var currentNumber = int.Parse(givenString[numberOfDigits].ToString());

                while (currentNumber != targetNumber)
                {
                    UpDownArrows[currentNumber < targetNumber ? 0 : 1].OnInteract();
                    yield return new WaitForSeconds(0.1f);
                    currentNumber = int.Parse(givenString[numberOfDigits].ToString());
                }

                if (i != split[1].Length - 1)
                {
                    RightArrow.OnInteract();
                    yield return new WaitWhile(() => moving);
                }
            }

            yield break;
        }

        if ("SUBMIT".ContainsIgnoreCase(split[0]))
        {
            if (split.Length > 1)
                yield break;

            Submit.OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        if (givenString == answerString)
        {
            Submit.OnInteract();
            yield return new WaitForSeconds(0.1f);
            yield break;
        }

        for (int i = numberOfDigits; i < answerString.Length; i++)
        {
            var targetNumber = int.Parse(answerString[i].ToString());
            var currentNumber = int.Parse(givenString[i].ToString());

            while (currentNumber != targetNumber)
            {
                UpDownArrows[currentNumber < targetNumber ? 0 : 1].OnInteract();
                yield return new WaitForSeconds(0.1f);
                currentNumber = int.Parse(givenString[i].ToString());
            }

            if (i != answerString.Length - 1)
            {
                RightArrow.OnInteract();
                yield return new WaitWhile(() => moving);
            }
        }

        yield return null;

        Submit.OnInteract();
        yield return new WaitForSeconds(0.1f);
    }
}
