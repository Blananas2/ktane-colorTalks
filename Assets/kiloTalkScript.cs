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
                 100, //Milligram
                1000, //Centigram
               10000, //Decigram
               20000, //Carat
              100000, //Gram
             1000000, //Dekagram
             2834952, //Ounce
            10000000, //Hectogram
            45359240, //Pound
           100000000, //Kilogram
           635029300, //Stone
         90718474000, //Short ton
        100000000000, //Metric tonne
        101604690900, //Long ton
    };
    string[] unitNames = { "Milligrams", "Centigrams", "Decigrams", "Carats", "Grams", "Dekagrams", "Ounces", "Hectograms", "Pounds", "Kilograms", "Stone", "Short tons", "Metric tonnes", "Long tons" };
    ulong answer;
    string answerString;
    string givenString = "0";
    int numberOfDigits = 0;
    bool moving = false;

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
        ulong number = (ulong)Rnd.Range(1, 100000);
        int fromUnit = Rnd.Range(0, 14);
        int toUnit = Rnd.Range(0, 14);
        while (fromUnit == toUnit)
        {
            toUnit = Rnd.Range(0, 14);
        }
        answer = number * unitValues[fromUnit] / unitValues[toUnit];
        if (answer <= 0 || answer >= 1000000000000000ul)
        {
            goto tryAgain;
        }
        NumberTexts[0].text = number.ToString();
        UnitText.text = unitNames[fromUnit] + "<size=72> to</size>\n" + unitNames[toUnit];
        answerString = answer.ToString();
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
}
