using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public class arrowTalkScript : MonoBehaviour
{

    public KMAudio Audio;
    public KMBombModule Module;
    public KMColorblindMode CBmode;

    public TextMesh Phrase;
    public KMSelectable[] Arrows; //ordered clockwise starting from the top so 1 arrow clockwise then 2 then 3 and so on
    public MeshRenderer[] ArrowObjs;
    public Material[] Colors;
    public Material Black;
    public TextMesh CBtext;
    public GameObject CBtextObj;

    int[] currentLoop = { 0, 1, 2, 3, 4, 5, 6, 7 };
    int completeLoops = 0;
    int positionInLoop = 0;
    int correctArrow;
    int redArrow;
    bool[] berserkArrows = { false, false, false, false, false, false, false, false };
    bool CBactive;
    float[] CBfloats = { 67.5f, 22.5f, 337.5f, 292.5f, 247.5f, 202.5f, 157.5f, 112.5f };

    int relativePosition;
    

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake()
    {
        moduleId = moduleIdCounter++;

        for (int a = 0; a < 8; a++)
        {
            int ax = a; //this is so incredibly dumb
            Arrows[a].OnInteract += delegate { ArrowPress(ax); return false; };
        }
    }

    // Use this for initialization
    void Start()
    {
        CBactive = CBmode.ColorblindModeActive;

        redArrow = Rnd.Range(0, 8);
        currentLoop.Shuffle();
        for (int w = 0; w < 8; w++)
        {
            ArrowObjs[(redArrow + w) % 8].material = Colors[w];
        }
        Debug.LogFormat("[Arrow Talk #{0}] The red arrow is {1} clockwise from the top.", moduleId, redArrow + 1);
        correctArrow = currentLoop[0];
        Phrase.text = GeneratePhrase(correctArrow);
        Debug.LogFormat("[Arrow Talk #{0}] \"{1}\" is {2} clockwise.", moduleId, Phrase.text.Replace("\n", " "), correctArrow + 1);
    }

    string GeneratePhrase(int x)
    {
        string[] colorNames = {
            "Red",
            "Orange",
            "Yellow",
            Rnd.Range(0, 2) == 0 ? "Green" : "Lime", //Lime is from Hue
            Rnd.Range(0, 2) == 0 ? "Cyan" : "Aqua", //Aqua is from Hue
            "Blue",
            Rnd.Range(0, 2) == 0 ? "Purple" : "Violet", //Hue uses both
            Rnd.Range(0, 2) == 0 ? "Pink" : "Fuschia", //Hue uses both
        };
        string hollowArrow = "        ⇨";

        switch (Rnd.Range(0, 7))
        {
            case 0: //color
                SetCBForTP(x);
                if (CBactive)
                {
                    CBtext.text = hollowArrow;
                    CBtextObj.transform.localRotation = Quaternion.Euler(90f, 0f, CBfloats[x]);
                }
                return colorNames[(x - redArrow + 8) % 8];
            case 1: //clock face
                string[] clockTimes = { "12:45", "2:15", "3:45", "5:15", "6:45", "8:15", "9:45", "11:15" };
                string[] clockWords = { "Twelve Forty-five", "Two Fifteen", "Three Forty-five", "Five Fifteen", "Six Forty-five", "Eight Fifteen", "Nine Forty-five", "Eleven Fifteen" };

                return Rnd.Range(0, 2) == 0 ? clockTimes[x] : clockWords[x].Replace(" ", "\n") + "\n" + (Rnd.Range(0, 2) == 0 ? "AM" : "PM");
            case 2: //compass
                string[] compassLetters = { "NNE", "ENE", "ESE", "SSE", "SSW", "WSW", "WNW", "NNW" };
                string[] compassWords = { "North North East", "East North East", "East South East", "South South East", "South South West", "West South West", "West North West", "North North West" };

                return Rnd.Range(0, 2) == 0 ? compassLetters[x] : Rnd.Range(0, 10) == 0 ? compassWords[x].Replace(" ", "\n") : compassWords[x].Split(' ').Shuffle().Join("\n");
            case 3: //commands
                string[][] commands = {
                    new string[] { "North then East", "Up then Right",  "Top then Right" , "Clockwise from Top", "CW from Top" },
                    new string[] { "East then North", "Right then Up", "Counter from Right", "CCW from Right" },
                    new string[] { "East then South", "Right then Down", "Clockwise from Right", "CW from Right" },
                    new string[] { "South then East", "Down then Right",  "Bottom then Right" , "Counter from Bottom", "CCW from Bottom" },
                    new string[] { "South then West", "Down then Left",  "Bottom then Left" , "Clockwise from Bottom", "CW from Bottom" },
                    new string[] { "West then South", "Left then Down", "Counter from Left", "CCW from Left" },
                    new string[] { "West then North", "Left then Up", "Clockwise from Left", "CW from Left" },
                    new string[] { "North then West", "Up then Left",  "Top then Left", "Counter from Top", "CCW from Top" },
                };

                return commands[x].PickRandom().Replace(" ", "\n");
            case 4: //conditions (aka degrees)
                string[] degrees = { "22.5°", "67.5°", "112.5°", "157.5°", "202.5°", "247.5°", "292.5°", "337.5°" };

                return degrees[x];
            case 5: //coordinates
                string[] coords = { "(1, 2)", "(2, 1)", "(2, -1)", "(1, -2)", "(-1, -2)", "(-2, -1)", "(-2, 1)", "(-1, 2)" };

                return coords[x];
            default: //clockwise from color
                int relativeTo;
                do
                {
                    relativeTo = Rnd.Range(0, 8);
                } while (relativeTo == x);
                string relativeColor = colorNames[(relativeTo - redArrow + 8) % 8];

                SetCBForTP(relativeTo);
                if (CBactive)
                {
                    CBtext.text = hollowArrow;
                    CBtextObj.transform.localRotation = Quaternion.Euler(90f, 0f, CBfloats[relativeTo]);
                }

                return Rnd.Range(0, 2) == 0 ? ((x + 8 - relativeTo) % 8 + "\nClockwise\nfrom\n" + relativeColor) : (8 - ((x + 8 - relativeTo) % 8) + "\nCounter\nfrom\n" + relativeColor);
        }
    }

    void SetCBForTP(int relativeTo)
    {
        relativePosition = relativeTo;
    }

    void ToggleCBforTP()
    {
        CBactive = !CBactive;

        var colorNames = new[]
        {
            "Red",
            "Orange",
            "Yellow",
            "Green", "Lime",
            "Cyan", "Aqua",
            "Blue",
            "Purple", "Violet",
            "Pink", "Fuschia"
        };

        if (!colorNames.Contains(Phrase.text) || !CBactive)
        {
            CBtext.text = string.Empty;
            return;
        }

        CBtext.text = "        ⇨";
        CBtextObj.transform.localRotation = Quaternion.Euler(90f, 0f, CBfloats[relativePosition]);
    }

    void ArrowPress(int p)
    {
        Arrows[p].AddInteractionPunch(0.5f);
        if (p == correctArrow)
        {
            Audio.PlaySoundAtTransform("AT_click", Arrows[p].transform);
            berserkArrows[p] = !berserkArrows[p];
            CBtext.text = null;
            Debug.LogFormat("[Arrow Talk #{0}] Pressed {1} clockwise, good.", moduleId, correctArrow + 1);
            positionInLoop++;
            if (positionInLoop == 8)
            {
                completeLoops++;
                positionInLoop = 0;
                currentLoop.Shuffle();
                if (completeLoops == 2)
                {
                    Module.HandlePass();
                    moduleSolved = true;
                    Debug.LogFormat("[Arrow Talk #{0}] Phrases exhausted. Module solved.", moduleId);
                    Phrase.text = null;
                    return;
                }
            }
            else if (completeLoops == 0 && positionInLoop == 1)
            {
                StartCoroutine(Spinning());
            }
            correctArrow = currentLoop[positionInLoop];
            Phrase.text = GeneratePhrase(correctArrow);
            Debug.LogFormat("[Arrow Talk #{0}] \"{1}\" is {2} clockwise.", moduleId, Phrase.text.Replace("\n", " "), correctArrow + 1);
        }
        else
        {
            Module.HandleStrike();
            Debug.LogFormat("[Arrow Talk #{0}] Pressed {1} clockwise, incorrect. Strike!", moduleId, p + 1);
        }
    }

    private IEnumerator Spinning()
    {
        int f = 0;
        int pos = 0;
        while (f < 8)
        {
            pos = (pos + 1) % 8;
            if (!moduleSolved)
            {
                if (berserkArrows[pos])
                {
                    ArrowObjs[pos].material = Black;
                }
                ArrowObjs[(pos + 7) % 8].material = Colors[(pos - redArrow + 7) % 8];
            }
            else
            {
                ArrowObjs[pos].material = Black;
                f++;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = "!{0} arrow NNE [Press the North-North-East arrow.] | Commands also accept numbers, with 1 starting at NNE, going clockwise. | \"arrow\" is optional. | !{0} colorblind [Toggle colorblind mode.]";
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        command = command.Trim().ToLowerInvariant();
        var m = Regex.Match(command, @"^\s*(colou?rblind|cb)\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        if (m.Success)
        {
            yield return null;
            ToggleCBforTP();
            yield break;
        }

        var validCmds = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "nne", "ene", "ese", "sse", "ssw", "wsw", "wnw", "nnw" };
        m = Regex.Match(command, @"^\s*(?:(arrow|press|submit)\s+)?(?<arr>(1|2|3|4|5|6|7|8|nne|ene|ese|sse|ssw|wsw|wnw|nnw))\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        if (m.Success)
        {
            var inputCmd = m.Groups["arr"].Value;
            var ix = Array.IndexOf(validCmds, inputCmd) % 8;
            if (ix == -1)
                yield break;
            yield return null;
            Arrows[ix].OnInteract();
            yield break;
        }
        yield break;
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        while (!moduleSolved)
        {
            Arrows[correctArrow].OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
    }
}
