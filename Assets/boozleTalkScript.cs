using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public class boozleTalkScript : MonoBehaviour
{

    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMBombModule Module;

    public KMSelectable Selectable;
    public GameObject SpritesObj;
    public SpriteRenderer[] SpriteSlots;
    public Sprite[] Sprites;
    public TextMesh[] Letters;

    string[] wordlists = {
        "EIGHT,ELEVEN,FIVE,FOUR,NINE,ONE,SEVEN,SIX,TEN,THIRTEEN,THREE,TWELVE,TWENTY,TWO,ZERO",
        "AZURE,BLACK,BLUE,BROWN,CYAN,FUCHSIA,GRAY,GREEN,MAGENTA,PINK,PURPLE,RED,VIOLET,WHITE,YELLOW",
        "CIRCLE,CUBE,DIAMOND,ELLIPSE,HEART,HEXAGON,KITE,OCTAGON,PENTAGON,PRISM,PYRAMID,RHOMBUS,SPHERE,SQUARE,STAR",
        "BRAZIL,CANADA,CHINA,EGYPT,FRANCE,GERMANY,INDIA,ITALY,JAPAN,MEXICO,POLAND,RUSSIA,SPAIN,THAILAND,VIETNAM",
        "ARSENIC,BORON,CARBON,COBALT,HELIUM,IRON,LEAD,NEON,NICKEL,NITROGEN,OXYGEN,SILICON,SODIUM,SULFUR,ZINC",
        "BEAVER,BIRD,CAT,DOG,FERRET,FOX,GIRAFFE,HORSE,KOALA,LION,LLAMA,MONKEY,NARWHAL,PANDA,RACCOON",
        "BACON,BREAD,BURRITO,CAKE,CHEESE,COOKIE,CORN,KEBAB,MANGO,PASTA,PIE,PIZZA,RICE,SAUSAGE,TACO",
        "ANGER,AWE,DISGUST,FEAR,GRIEF,GUILTY,JOY,LOATHING,LOVE,PENSIVE,RAGE,REMORSE,SADNESS,SURPRISE,TERROR",
        "ARCHERY,BASEBALL,BOWLING,CURLING,CYCLING,DARTS,FOOTBALL,FRISBEE,GOLF,HOCKEY,LACROSSE,RUGBY,SNOOKER,SOCCER,TENNIS",
        "BANJO,CELLO,CLARINET,COWBELL,CYMBAL,DRUM,FLUTE,GUITAR,OBOE,OCARINA,PIANO,RECORDER,TRUMPET,UKULELE,VIOLIN"
    };
    string[] categories = { "a number", "a color", "a shape", "a country", "an element", "an animal", "a food", "an emotion", "a sport", "an instrument" };
    string word;
    int digit;
    string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    int letterCount;

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    private BoozleTalkSettings Settings = new BoozleTalkSettings();
    bool setOne = true;
    bool setTwo = false;
    bool setThree = false;
    public bool[] testBools;

    void Awake()
    {
        moduleId = moduleIdCounter++;

        if (!Application.isEditor)
        {
            ModConfig<BoozleTalkSettings> modConfig = new ModConfig<BoozleTalkSettings>("BoozleTalkSettings");
            Settings = modConfig.Settings;
            modConfig.Settings = Settings;

            setOne = Settings.SetOne || (!Settings.SetTwo && !Settings.SetThree);
            setTwo = Settings.SetTwo;
            setThree = Settings.SetThree;
        }
        else
        {
            setOne = testBools[0] || (!testBools[1] && !testBools[2]);
            setTwo = testBools[1];
            setThree = testBools[2];
        }

        Debug.LogFormat("<BoozleTalk #{0}> Set 1: {1}, Set 2: {2}, Set 3: {3}", moduleId, setOne ? "Enabled" : "Disabled", setTwo ? "Enabled" : "Disabled", setThree ? "Enabled" : "Disabled");

        Selectable.OnInteract += delegate () { Press(); return false; };
    }

    // Use this for initialization
    void Start()
    {
        digit = Rnd.Range(0, 10);
        word = wordlists[digit].Split(',').PickRandom();
        Debug.LogFormat("[BoozleTalk #{0}] Your word is {1}, which is {2}. It's corresponding digit is {3}.", moduleId, word, categories[digit], digit);

        letterCount = word.Length;
        string wPadding = "*";
        for (int h = 0; h < (8 - word.Length) / 2; h++)
        {
            wPadding = " " + wPadding + " ";
        }
        if (word.Length % 2 == 0)
        {
            SpritesObj.transform.localPosition = new Vector3(-0.0095f, 0f, 0f);
        }
        else
        {
            wPadding += " ";
        }
        word = wPadding.Replace("*", word);

        List<int> validSets = new List<int> { };
        if (setOne) { validSets.Add(0); }
        if (setTwo) { validSets.Add(1); }
        if (setThree) { validSets.Add(2); }
        string loggingSets = "";

        for (int q = 0; q < 8; q++)
        {
            int chosenSet = validSets.PickRandom();
            SpriteSlots[q].sprite = word[q] == ' ' ? null : Sprites[26 * chosenSet + alphabet.IndexOf(word[q])];
            loggingSets += word[q] == ' ' ? "" : (chosenSet + 1).ToString();
        }
        Debug.LogFormat("<BoozleTalk #{0}> Sets used: {1}", moduleId, loggingSets);
    }

    void Press()
    {
        Selectable.AddInteractionPunch(1f);
        if (moduleSolved) { return; }
        if (Bomb.GetFormattedTime().Contains(digit.ToString()))
        {
            Audio.PlaySoundAtTransform("BT_solve", Selectable.transform);
            Module.HandlePass();
            moduleSolved = true;
            Debug.LogFormat("[BoozleTalk #{0}] Pressed at {1}, which is correct. Module solved.", moduleId, Bomb.GetFormattedTime());
            StartCoroutine(SolveAnim());
        }
        else
        {
            Module.HandleStrike();
            Debug.LogFormat("[BoozleTalk #{0}] Pressed at {1}, which is incorrect, strike!", moduleId, Bomb.GetFormattedTime());
        }
    }

    private IEnumerator SolveAnim()
    {
        for (int q = 0; q < 8; q++)
        {
            if (word[q] == ' ')
            {
                continue;
            }
            SpriteSlots[q].sprite = null;
            Letters[q].text = word[q].ToString();
            yield return new WaitForSeconds(0.809f / letterCount);
        }
    }

    class BoozleTalkSettings
    {
        public bool SetOne = true;
        public bool SetTwo = false;
        public bool SetThree = false;
    }

    static Dictionary<string, object>[] TweaksEditorSettings = new Dictionary<string, object>[]
    {
        new Dictionary<string, object>
        {
            { "Filename", "BoozleTalkSettings.json" },
            { "Name", "BoozleTalk Settings" },
            { "Listing", new List<Dictionary<string, object>>{
                new Dictionary<string, object>
                {
                    { "Key", "SetOne" },
                    { "Text", "Whether Boozleglyph Set 1 is used." }
                },
                new Dictionary<string, object>
                {
                    { "Key", "SetTwo" },
                    { "Text", "Whether Boozleglyph Set 2 is used." }
                },
                new Dictionary<string, object>
                {
                    { "Key", "SetThree" },
                    { "Text", "Whether Boozleglyph Set 3 is used." }
                },
            } }
        }
    };

    // Twitch Plays Support by Kilo Bites

#pragma warning disable 414
    private readonly string TwitchHelpCommand = @"!{0} submit 2 [submits when the timer contains that digit]";
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        string[] split = command.ToUpperInvariant().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

        yield return null;

        if ("SUBMIT".ContainsIgnoreCase(split[0]))
        {
            if (split.Length == 1)
            {
                yield return "Submit on what digit?";
                yield break;
            }

            if (split.Length > 2)
                yield break;

            int digitToPress;

            if (!int.TryParse(split[1], out digitToPress))
            {
                yield return string.Format("The digit you tried to input ({0}) is not valid. Make sure the digit you want to submit is 0-9.", split[1]);
                yield break;
            }

            while (!Bomb.GetFormattedTime().Contains(digitToPress.ToString()))
                yield return "trycancel";

            Selectable.OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        while (!Bomb.GetFormattedTime().Contains(digit.ToString()))
            yield return true;

        Selectable.OnInteract();
        yield return new WaitForSeconds(0.1f);
    }
}
