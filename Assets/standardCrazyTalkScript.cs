using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public class standardCrazyTalkScript : MonoBehaviour
{

    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMBombModule Module;
    public KMColorblindMode CBmode;

    public KMSelectable[] Buttons;
    public MeshRenderer[] ButtonMeshes;
    public Material[] ButtonColors;
    public TextMesh Instruction;
    public TextMesh[] CBtexts;
    public GameObject LIVES;
    public KMSelectable Victor;

    int[] colors;
    string[] colorNames = { "blue", "red", "green", "yellow" };
    string[] colorHexs = { "303AFC", "FB1E24", "90E508", "F9CA09" };
    string[] positionNames = { "top-left", "top-right", "bottom-left", "bottom-right" };
    int correctButton;
    bool CBactive = false;
    string[] splitIns = { "", "", "", "" };
    int buttonHistory = 0;
    bool easter = false;

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake()
    {
        moduleId = moduleIdCounter++;

        for (int b = 0; b < 4; b++)
        {
            int bx = b; //this is so incredibly dumb
            Buttons[b].OnHighlight += delegate { ButtonHighlight(bx); };
            Buttons[b].OnInteract += delegate { ButtonPress(bx); return false; };
        }

        Victor.OnInteract += delegate () { VictorPress(); return false; };

    }

    public class SCTRule
    {
        public string Phrase;
        public bool Condition;
        public int? ItButton;

        public SCTRule(string phrase, bool condition, int? itButton)
        {
            Phrase = phrase;
            Condition = condition;
            ItButton = itButton;
        }
    }

    public class SCTButton
    {
        public string Phrase;
        public int Button;

        public SCTButton(string phrase, int button)
        {
            Phrase = phrase;
            Button = button;
        }
    }

    public class SCTModification
    {
        public string Phrase;
        public int[] Mapping;

        public SCTModification(string phrase, int[] mapping)
        {
            Phrase = phrase;
            Mapping = mapping;
        }
    }

    // Use this for initialization
    void Start()
    {
        CBactive = CBmode.ColorblindModeActive;

        colors = Enumerable.Range(0, 4).ToArray().Shuffle();
        for (int b = 0; b < 4; b++)
        {
            ButtonMeshes[b].material = ButtonColors[colors[b]];
        }
        Debug.LogFormat("[Standard Crazy Talk #{0}] Button colors: {1}", moduleId, colors.Select(c => colorNames[c]).Join(", "));

        SCTRule[] rules = new SCTRule[] {
            new SCTRule("a D battery is present", Bomb.GetBatteryCount(Battery.D) != 0, null), //i guess this can not be as repetitive in places but i'm unsure how adding to a class works
            new SCTRule("a DVI-D port is present", Bomb.IsPortPresent(Port.DVI), null),
            new SCTRule("a PS/2 port is present", Bomb.IsPortPresent(Port.PS2), null),
            new SCTRule("a Parallel port is present", Bomb.IsPortPresent(Port.Parallel), null),
            new SCTRule("an RJ-45 port is present", Bomb.IsPortPresent(Port.RJ45), null),
            new SCTRule("a Serial port is present", Bomb.IsPortPresent(Port.Serial), null),
            new SCTRule("a Stereo RCA port is present", Bomb.IsPortPresent(Port.StereoRCA), null),
            new SCTRule("a lit indicator is present", Bomb.GetOnIndicators().Count() != 0, null),
            new SCTRule("an empty port plate is present", Bomb.GetPortPlates().Any(p => p.Length == 0), null),
            new SCTRule("an unlit indicator is present", Bomb.GetOffIndicators().Count() != 0, null),
            new SCTRule("any AA batteries are present", Bomb.GetBatteryCount(Battery.AA) != 0, null),
            new SCTRule("the first character of the serial number is a digit", "0123456789".Contains(Bomb.GetSerialNumber()[0]), null),
            new SCTRule("the first character of the serial number is a letter", "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Contains(Bomb.GetSerialNumber()[0]), null),
            new SCTRule("the first digit of the serial number is even", "02468".Contains(Bomb.GetSerialNumberNumbers().First().ToString()), null),
            new SCTRule("the first digit of the serial number is odd", "13579".Contains(Bomb.GetSerialNumberNumbers().First().ToString()), null),
            new SCTRule("the last digit of the serial number is even", "02468".Contains(Bomb.GetSerialNumberNumbers().Last().ToString()), null),
            new SCTRule("the last digit of the serial number is odd", "13579".Contains(Bomb.GetSerialNumberNumbers().Last().ToString()), null),
            new SCTRule("the second character of the serial number is a digit", "0123456789".Contains(Bomb.GetSerialNumber()[1]), null),
            new SCTRule("the second character of the serial number is a letter", "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Contains(Bomb.GetSerialNumber()[1]), null),
            new SCTRule("the serial number contains a vowel", Bomb.GetSerialNumberLetters().Any(ch => "AEIOU".Contains(ch)), null),
            new SCTRule("the top-left button is blue", colors[0] == 0, 0),
            new SCTRule("the top-left button is red", colors[0] == 1, 0),
            new SCTRule("the top-left button is green", colors[0] == 2, 0),
            new SCTRule("the top-left button is yellow", colors[0] == 3, 0),
            new SCTRule("the top-right button is blue", colors[1] == 0, 1),
            new SCTRule("the top-right button is red", colors[1] == 1, 1),
            new SCTRule("the top-right button is green", colors[1] == 2, 1),
            new SCTRule("the top-right button is yellow", colors[1] == 3, 1),
            new SCTRule("the bottom-left button is blue", colors[2] == 0, 2),
            new SCTRule("the bottom-left button is red", colors[2] == 1, 2),
            new SCTRule("the bottom-left button is green", colors[2] == 2, 2),
            new SCTRule("the bottom-left button is yellow", colors[2] == 3, 2),
            new SCTRule("the bottom-right button is blue", colors[3] == 0, 3),
            new SCTRule("the bottom-right button is red", colors[3] == 1, 3),
            new SCTRule("the bottom-right button is green", colors[3] == 2, 3),
            new SCTRule("the bottom-right button is yellow", colors[3] == 3, 3),
        };

        SCTButton[] buttons = new SCTButton[] {
            new SCTButton("the top-left button", 0),
            new SCTButton("the top-right button", 1),
            new SCTButton("the bottom-left button", 2),
            new SCTButton("the bottom-right button", 3),
            new SCTButton("the blue button", Array.IndexOf(colors, 0)),
            new SCTButton("the red button", Array.IndexOf(colors, 1)),
            new SCTButton("the green button", Array.IndexOf(colors, 2)),
            new SCTButton("the yellow button", Array.IndexOf(colors, 3)),
        };

        SCTModification[] modifications = new SCTModification[] {
            new SCTModification("above or below", new int[] { 2, 3, 0, 1 }),
            new SCTModification("left or right of", new int[] { 1, 0, 3, 2 }),
            new SCTModification("opposite", new int[] { 3, 2, 1, 0 }),
        };

        string insText = "If {r}, press {a}. Otherwise, press the button {m} {b}.";
        SCTRule chosenRule;
        SCTButton buttonA;
        SCTModification chosenModification;
        SCTButton buttonB;

        do
        {
            chosenRule = rules.PickRandom();
            buttonA = buttons.PickRandom();
            chosenModification = modifications.PickRandom();
            buttonB = buttons.PickRandom();
        } while (buttonA.Button == chosenModification.Mapping[buttonB.Button]);
        bool useIt = chosenRule.ItButton == buttonA.Button;
        insText = insText.Replace("{r}", chosenRule.Phrase).Replace("{a}", useIt ? "it" : buttonA.Phrase).Replace("{m}", chosenModification.Phrase).Replace("{b}", buttonB.Phrase);
        Debug.LogFormat("[Standard Crazy Talk #{0}] \"{1}\"", moduleId, insText);
        correctButton = chosenRule.Condition ? buttonA.Button : chosenModification.Mapping[buttonB.Button];
        Debug.LogFormat("[Standard Crazy Talk #{0}] The correct button is {1}.", moduleId, positionNames[correctButton]);
        insText = wordWrap(insText, 35);
        GenerateSplitIns(insText);
        Instruction.text = splitIns.PickRandom();

        LIVES.SetActive(false);
        SetColorblindMode(CBactive);
    }

    void SetColorblindMode(bool mode)
    {
        for (int i = 0; i < 4; i++)
            CBtexts[i].text = mode ? "BRGY"[colors[i]].ToString() : string.Empty;
    }

    string wordWrap(string text, int limit)
    {
        if (text.Length > limit)
        {
            int edge = text.Substring(0, limit).LastIndexOf(' ');
            if (edge > 0)
            {
                string line = text.Substring(0, edge);
                string remainder = text.Substring(edge + 1);
                return line + '\n' + wordWrap(remainder, limit);
            }
        }
        return text;
    }

    void GenerateSplitIns(string ins)
    {
        string splitForLog = "";
        splitIns = new string[] { "", "", "", "" };

        for (int q = 0; q < ins.Length; q++)
        {
            char ch = ins[q];
            if (ch == ' ' || ch == '\n')
            {
                if (ch == ' ') { splitForLog += " "; }
                for (int s = 0; s < 4; s++)
                {
                    splitIns[s] += ch;
                }
            }
            else
            {
                int wh = Rnd.Range(0, 4);
                splitForLog += "brgy"[wh];
                for (int s = 0; s < 4; s++)
                {
                    splitIns[s] += s == wh ? ch : ' ';
                }
            }
        }
        if (!moduleSolved)
        {
            Debug.LogFormat("[Standard Crazy Talk #{0}] The split: {1}", moduleId, splitForLog);
        }

        for (int s = 0; s < 4; s++)
        {
            splitIns[s] = "<color=#" + colorHexs[colors[s]] + ">" + splitIns[s] + "</color>";
        }
    }

    void ButtonHighlight(int b)
    {
        if (easter) { return; }
        Instruction.text = splitIns[b];
    }

    void ButtonPress(int b)
    {
        Audio.PlaySoundAtTransform(easter ? "FTANG" : "ButtonPress", Buttons[b].transform);
        Buttons[b].AddInteractionPunch(1f);
        if (!moduleSolved)
        {
            if (b == correctButton)
            {
                Module.HandlePass();
                moduleSolved = true;
                Debug.LogFormat("[Standard Crazy Talk #{0}] Pressed {1}, that is correct. Module solved.", moduleId, positionNames[b]);
                GenerateSplitIns("CLICK THE\n'V' IN LIVES");
            }
            else
            {
                Module.HandleStrike();
                Debug.LogFormat("[Standard Crazy Talk #{0}] Pressed {1}, that is incorrect, strike!", moduleId, positionNames[b]);
            }
        }
        else
        {
            buttonHistory = (buttonHistory * 10 + 1 + colors[b]) % 10000;
            LIVES.SetActive(buttonHistory == 1214 && !easter);
            if (easter)
            {
                for (int m = 0; m < 4; m++)
                {
                    ButtonMeshes[m].material = ButtonColors[b == correctButton ? 2 : 1];
                    if (CBactive)
                    {
                        CBtexts[m].text = b == correctButton ? "✓" : "✗";
                    }
                }
                GenerateQuestion();
            }
        }
    }

    void VictorPress()
    {
        Audio.PlaySoundAtTransform("FTANG", Victor.transform);
        easter = true;
        LIVES.SetActive(false);
        GenerateQuestion();
    }

    void GenerateQuestion()
    {
        string[] questions = {
            "HOW MANY HOLES\nIN A POLO?\n<color=#{0}>ONE</color> <color=#{1}>TWO</color>\n<color=#{2}>THREE</color> <color=#{3}>FOUR</color>",
            ".SDRAWKCAB\nNOITSEUQ SIHT\nREWSNA\n<color=#{0}>K.O</color> <color=#{1}>WHAT?</color>\n<color=#{2}>I DON'T UNDERSTAND</color> <color=#{3}>TENNIS ELBOW</color>",
            "√ONION\n<color=#{0}>28</color> <color=#{1}>CARROT</color>\n<color=#{2}>SHALLOTS</color> <color=#{3}>π</color>",
            "WHAT WAS THE ANSWER\nTO QUESTION 2?\n<color=#{0}>THAT ONE→</color> <color=#{1}>↙THAT ONE</color>\n<color=#{2}>↑THAT ONE</color> <color=#{3}>THIS ONE</color>",
            "WHAT FOLLOWS\nDECEMBER 2nd?\n<color=#{0}>DECEMBER 3rd</color> <color=#{1}>n</color>\n<color=#{2}>A QUESTION MARK</color> <color=#{3}>142 DWARVES</color>",
            "WHAT CAN YOU PUT\nIN A BUCKET TO\nMAKE IT LIGHTER?\n<color=#{0}>GYPSIES</color> <color=#{1}>TORCH</color>\n<color=#{2}>A HOLE</color> <color=#{3}>CANNED LAUGHTER</color>",
            "WHAT IS THE\n7th LETTER OF\nTHE ALPHABET?\n<color=#{0}>G</color> <color=#{1}>H</color>\n<color=#{2}>I</color> <color=#{3}>J</color>",
            "DEAL OR\nNO DEAL?\n<color=#{0}>DEAL!</color> <color=#{1}>NO DEAL!</color>\n<color=#{2}>SEAL!</color> <color=#{3}>NO SEAL!</color>",
            "THE CHOICE\nIS YOURS\n<color=#{0}>+1 LIFE</color> <color=#{1}>-1 LIFE</color>\n<color=#{2}>ESCAPE!!</color> <color=#{3}>+1 SKIP</color>",
            "SAVE CHANGES TO\n'UNTITLED'?\n<color=#{0}>YES</color> <color=#{1}>NO</color>\n<color=#{2}>CANCEL</color> <color=#{3}>BRAN</color>",
            "HOW DO YOU KILL\nA WEREWOLF?\n<color=#{0}>SHOE POLISH</color> <color=#{1}>GRAVY GRANULES</color>\n<color=#{2}>BLACK PUDDING</color> <color=#{3}>CILLIT BANG</color>",
            "I HOPE YOU'VE BEEN\nPAYING ATTENTION\nTO THE QUESTION NUMBERS!\n<color=#{0}>GO TO 21</color> <color=#{1}>GO TO 26</color>\n<color=#{2}>GO TO 24</color> <color=#{3}>GO TO 28</color>",
            "WHAT FLAVOUR IS\nCARDBOARD?\n<color=#{0}>HONEY</color> <color=#{1}>PORK SCRATCHINGS</color>\n<color=#{2}>EGG MAYONNAISE</color> <color=#{3}>TALC</color>",
            "WHAT DO YOU CALL\nA WINGLESS FLY?\n<color=#{0}>A FLAP</color> <color=#{1}>A WALK</color>\n<color=#{2}>A PLUM</color> <color=#{3}>JASON</color>",
            "MARY ROSE SAT\nON A PIN.\n<color=#{0}>O RLY?</color> <color=#{1}>MARY ROSE</color>\n<color=#{2}>BURST HER PILES</color> <color=#{3}>AHAHAHAAHA!</color>",
            "BRIDGET MAKES\nEVERYONE...\n<color=#{0}>GAY</color> <color=#{1}>STRAIGHT</color>\n<color=#{2}>BI</color> <color=#{3}>TOM CRUISE</color>",
            "SNAKE?\nSNAKE!?\n<color=#{0}>SNAKE!</color> <color=#{1}>SNAAAAKE!</color>\n<color=#{2}>SNAKE?</color> <color=#{3}>SNAIL!</color>",
            "WHICH IS THE\nCORRECT SPELLING?\n<color=#{0}>SLAP-ME-DO</color> <color=#{1}>SLAPP-ME-DO</color>\n<color=#{2}>SPAPP-ME-DO</color> <color=#{3}>SPLAPP-ME-DO</color>",
            "ON THE SUBJECT OF\nDRACULA, WHAT IS HIS\nFAVOURITE FOOD?\n<color=#{0}>BLOOD</color> <color=#{1}>URINE</color>\n<color=#{2}>CHICKEN CHOW MEIN</color> <color=#{3}>SHEPHERD'S PIE</color>",
            "WHICH IS TRUE?\n<color=#{0}>PSP>DS</color> <color=#{1}>DS>PSP</color>\n<color=#{2}>PSP=DS</color> <color=#{3}>EGG>28</color>",
            "ARE YOU ENJOYING\nTHE QUIZ?\n<color=#{0}>NO</color> <color=#{1}>YES</color>\n<color=#{2}>I'M LOVIN' IT!</color> <color=#{3}>LOL, 69</color>",
            "DÉJÀ VU?\n<color=#{0}>FOUR</color> <color=#{1}>100</color>\n<color=#{2}>CILLIT BANG</color> <color=#{3}>+1 LIFE</color>",
            "SELL YOUR\nLIVER TO...\n<color=#{0}>FILTHY ROMANIANS</color> <color=#{1}>SATAN</color>\n<color=#{2}>ANNE FRANK</color> <color=#{3}>4chan FOR THE LULZ</color>",
            "I'M GREEN AND HAVE\nSTICKY BALLS. WHO AM I?\n<color=#{0}>THE INCREDIBLE HULK</color> <color=#{1}>HE-MAN</color>\n<color=#{2}>THE PRINCE</color> <color=#{3}>SLIPPY TOAD</color>",
            "WHICH MAGICAL PROPERTY\nDO DOG EGGS CONTAIN?\n<color=#{0}>ETERNAL YOUTH</color> <color=#{1}>BLINDNESS</color>\n<color=#{2}>OODLES OF CASH</color> <color=#{3}>INVINCIBILITY</color>",
            "HOW MANY TIMES HAS\nMICHAEL JACKSON HAD\nA NOSE JOB?\n<color=#{0}>THRICE</color> <color=#{1}>TWICE</color>\n<color=#{2}>ONCE</color> <color=#{3}>NONCE</color>",
            "WHAT DO YOU GET IF YOU\nPUT A NUMBER 1 INTO\nYOUR CALCULATOR AND THEN\nADD A NUMBER 2?\n<color=#{0}>3</color> <color=#{1}>12</color>\n<color=#{2}>A RIGHT MESS</color> <color=#{3}>ERROR!</color>",
            "HOW MANY TIMES\nHAVE YOU HAD TO\nRESTART?\n<color=#{0}>NONE!</color> <color=#{1}>1-5 TIMES</color>\n<color=#{2}>6-9 TIMES</color> <color=#{3}>10+ TIMES!</color>",
            "HOW MANY HOLES\nIN TWO POLOS?\n<color=#{0}>ONE</color> <color=#{1}>TWO</color>\n<color=#{2}>THREE</color> <color=#{3}>FOUR</color>"
        };
        int[] answers = { 3, 0, 2, 1, 1, 1, 1, 2, 3, 3, 0, 3, 2, 1, 1, 3, 1, 3, 3, 3, 3, 0, 0, 2, 1, 3, 2, 3, 1 };
        int chosenQuestion = Rnd.Range(0, questions.Length);
        while (correctButton == answers[chosenQuestion]) //this is so it will never show the same question twice in a row
        {
            chosenQuestion = Rnd.Range(0, questions.Length);
        }
        string questionText = questions[chosenQuestion];
        for (int r = 0; r < 4; r++)
        {
            questionText = questionText.Replace("{" + r + "}", colorHexs[colors[r]]);
        }
        Instruction.text = questionText;
        correctButton = answers[chosenQuestion];
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = "!{0} hover brgybrgy [Hover over the specified buttons.] || !{0} press blue [Press the button with the specified color.] | !{0} colorblind [Toggle colorblind mode.]";
#pragma warning restore 414
    private bool TwitchShouldCancelCommand;

    IEnumerator ProcessTwitchCommand(string command)
    {
        command = command.Trim().ToLowerInvariant();
        var m = Regex.Match(command, @"^\s*colou?rblind|cb\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        if (m.Success)
        {
            yield return null;
            CBactive = !CBactive;
            yield break;
        }

        m = Regex.Match(command, @"^\s*(?:hover\s+)(?<colors>[brgy,; ]+)\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        if (m.Success)
        {
            var cs = m.Groups["colors"].Value;
            var list = new List<int>();
            foreach (var color in cs)
            {
                int ix = "brgy,; ".IndexOf(color);
                if (ix > 3)
                    continue;
                if (ix == -1)
                    yield break;
                list.Add(ix);
            }

            yield return null;
            foreach (var color in list)
            {
                yield return "trycancel";
                int colorIx = Array.IndexOf(colors, color);
                Buttons[colorIx].OnHighlight();
                yield return new WaitForSeconds(0.25f);
            }
            yield break;
        }

        m = Regex.Match(command, @"^\s*(?:press\s+)(?<input>b|blue|g|green|r|red|y|yellow)\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        if (m.Success)
        {
            yield return null;
            var color = m.Groups["input"].Value[0];
            int arrIx = "brgy".IndexOf(color);
            int colorIx = Array.IndexOf(colors, arrIx);
            Buttons[colorIx].OnInteract();
            yield break;
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        Buttons[correctButton].OnInteract();
        yield return new WaitForSeconds(0.1f);
    }
}
