using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public class kayMazeyTalkScript : MonoBehaviour
{
    public KMAudio Audio;
    public KMBombModule Module;

    public KMSelectable Screen;
    public KMSelectable[] Arrows;
    public TextMesh Word;
    public GameObject[] ArrowObjs;

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    string[] mazeWords = {
        "Knit",   "Knows",   "Knock",   "",       "Knew",   "Knoll",
        "Kneed",  "Knuff",   "Knork",   "Knout",  "Knits",  "",
        "Knife",  "Knights", "Knap",    "Knee",   "Knocks", "",
        "Knacks", "Knab",    "Knocked", "Knight", "Knitch", "",
        "Knots",  "Knish",   "Knob",    "Knox",   "Knur",   "",
        "Knook",  "Know",    "",        "Knack",  "Knurl",  "Knot"
    };
    int[] mazeDirs = {
        2, 14, 12, -1,  6,  8,
        2, 13,  1,  4,  5, -1,
        6, 11, 14, 11, 13, -1,
        7, 12,  7, 12,  1, -1,
        1,  5,  1,  7, 12, -1,
        2,  9, -1,  1,  3,  8
    };
    bool traversable = false;
    bool invert = false;
    bool tpInvertKnown = false;
    int currentPosition = -1;
    int goalPosition = -1;
    int[] vectors = { -6, 1, 6, -1 };
    string[] dirNames = { "up", "right", "down", "left" };

    void Awake()
    {
        moduleId = moduleIdCounter++;

        Screen.OnInteract += delegate () { ScreenPress(); return false; };

        for (int a = 0; a < 4; a++) {
            int ax = a; //this is so incredibly dumb
            Arrows[a].OnInteract += delegate { ArrowPress(ax); return false; };
        }
    }

    // Use this for initialization
    void Start()
    {
        do
        {
            currentPosition = Rnd.Range(0, 36);
            goalPosition = Rnd.Range(0, 36);
        } while (mazeWords[currentPosition] == "" || mazeWords[goalPosition] == "" || currentPosition == goalPosition);

        Word.text = mazeWords[goalPosition];

        for (int o = 0; o < 4; o++)
        {
            ArrowObjs[o].SetActive(false);
        }

        invert = Rnd.Range(0, 10) < 4;
        tpInvertKnown = invert;

        Debug.LogFormat("[KayMazey Talk #{0}] Your goal is {1}.", moduleId, mazeWords[goalPosition]);
        Debug.LogFormat("[KayMazey Talk #{0}] You start at {1}.", moduleId, mazeWords[currentPosition]);
    }

    void ScreenPress()
    {
        Screen.AddInteractionPunch(0.5f);
        if (!traversable)
        {
            Audio.PlaySoundAtTransform("KMT_bwomp", Screen.transform);
            traversable = true;
            Word.text = InvertIfNeeded();
            StartCoroutine(ActivateArrows());
            Debug.LogFormat("<KayMazey Talk #{0}> screen", moduleId);
        }
    }

    void ArrowPress(int a)
    {
        Arrows[a].AddInteractionPunch(0.5f);
        int r = invert ? a ^ 2 : a;
        int p = (int)Math.Pow(2, r);
        if ((mazeDirs[currentPosition] & p) == p)
        {
            currentPosition += vectors[r];
            StartCoroutine(ArrowAnim(1.03f, r));
            if (currentPosition == goalPosition)
            {
                Word.text = "";
                Module.HandlePass();
                Debug.LogFormat("[KayMazey Talk #{0}] You made it to the goal. Module solved.", moduleId);
                Audio.PlaySoundAtTransform("KMT_solve", Screen.transform);
                StartCoroutine(SolveAnim());
                return;
            }
            Audio.PlaySoundAtTransform("KMT_step", Screen.transform);
            invert = Rnd.Range(0, 10) < 4;
            Word.text = InvertIfNeeded();
            Debug.LogFormat("<KayMazey Talk #{0}> {1} ({2}), {3}", moduleId, dirNames[a], dirNames[r], InvertIfNeeded());
        }
        else
        {
            Module.HandleStrike();
            Debug.LogFormat("[KayMazey Talk #{0}] Moving {1} from {2} go into a wall, strike!", moduleId, dirNames[a], InvertIfNeeded());
            traversable = false;
            Word.text = mazeWords[goalPosition];
            StartCoroutine(DeactivateArrows());
        }
    }

    string InvertIfNeeded()
    {
        return mazeWords[currentPosition].Replace("K", invert ? "" : "K");
    }

    private IEnumerator ActivateArrows()
    {
        for (int o = 0; o < 4; o++)
        {
            ArrowObjs[o].SetActive(true);
        }
        float elapsed = 0f;
        float duration = 1f;
        while (elapsed < duration)
        {
            for (int o = 0; o < 4; o++)
            {
                //use a bezier to make smooth
                float scale = Lerp(Lerp(Lerp(0.95f, 1.122f, elapsed), Lerp(1.122f, 1.07f, elapsed), elapsed), Lerp(Lerp(1.122f, 1.07f, elapsed), Lerp(1.07f, 1f, elapsed), elapsed), elapsed);
                ArrowObjs[o].transform.localScale = new Vector3(scale, 1f, scale);
            }
            yield return null;
            elapsed += Time.deltaTime;
        }
        for (int o = 0; o < 4; o++)
        {
            ArrowObjs[o].transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    private IEnumerator ArrowAnim(float z, int w)
    {
        float elapsed = 0f;
        float duration = 0.2f;
        while (elapsed < duration)
        {
            float scale = Lerp(z, 1f, elapsed * 5);
            ArrowObjs[w].transform.localScale = new Vector3(scale, 1f, scale);
            yield return null;
            elapsed += Time.deltaTime;
        }
        ArrowObjs[w].transform.localScale = new Vector3(1f, 1f, 1f);
    }

    private IEnumerator DeactivateArrows()
    {
        float elapsed = 0f;
        float duration = 0.1f;
        while (elapsed < duration)
        {
            float scale = Lerp(1f, 0f, elapsed * 10);
            for (int o = 0; o < 4; o++)
            {
                ArrowObjs[o].transform.localScale = new Vector3(o % 2 == 0 ? scale : 1f, 1f, o % 2 == 0 ? 1f : scale);
            }
            yield return null;
            elapsed += Time.deltaTime;
        }
        for (int o = 0; o < 4; o++)
        {
            ArrowObjs[o].SetActive(false);
            ArrowObjs[o].transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    private IEnumerator SolveAnim()
    {
        float elapsed = 0f;
        float duration = 0.4f;
        float om = 0.04f;
        while (elapsed < duration)
        {
            for (int o = 0; o < 4; o++)
            {
                ArrowObjs[o].SetActive(Rnd.Range(0, 2) == 0);
            }
            yield return new WaitForSeconds(om);
            yield return null;
            elapsed += Time.deltaTime + om;
            for (int o = 0; o < 4; o++)
            {
                ArrowObjs[o].SetActive(false);
            }
        }
    }
    
    float Lerp(float a, float b, float t)
    { //this assumes t is in the range 0-1
        return a * (1f - t) + b * t;
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} start [will start the module] || !{0} move urdl [to move in those directions]";
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        command = command.ToLowerInvariant();
        var m = Regex.Match(command, @"^\s*start\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        if (m.Success)
        {
            if (traversable)
            {
                yield return "sendtochaterror The module has already started. Command ignored.";
                yield break;
            }
            yield return null;
            Screen.OnInteract();
            yield break;
        }

        m = Regex.Match(command, @"^\s*(?:move\s+)(?<dirs>[urdl,; ]+)\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        if (!m.Success)
            yield break;

        if (!traversable)
        {
            yield return "sendtochaterror The module is still in its initial state. Command ignored.";
            yield break;
        }

        var input = m.Groups["dirs"].Value;
        var list = new List<int>();
        foreach (var inp in input)
        {
            int ix = "urdl,; ".IndexOf(inp);
            if (ix == -1)
                yield break;
            if (ix > 3)
                continue;
            list.Add(ix);
        }
        yield return null;

        for (int i = 0; i < list.Count; i++)
        {
            Arrows[list[i]].OnInteract();
            yield return new WaitForSeconds(0.2f);
            if (invert != tpInvertKnown)
            {
                yield return string.Format("sendtochat Module {0} (KayMazey Talk) halted its command because the current display {1} a capital K. The command was halted after {2} movement{3}.",
                    GetModuleCode(),
                    invert ? "no longer contains" : "now contains",
                    i + 1,
                    i == 0 ? "" : "s");
                yield break;
            }
        }
    }

    private string GetModuleCode()
    {
        Transform closest = null;
        float closestDistance = float.MaxValue;
        foreach (Transform children in transform.parent)
        {
            var distance = (transform.position - children.position).magnitude;
            if (children.gameObject.name == "TwitchModule(Clone)" && (closest == null || distance < closestDistance))
            {
                closest = children;
                closestDistance = distance;
            }
        }
        return closest != null ? closest.Find("MultiDeckerUI").Find("IDText").GetComponent<UnityEngine.UI.Text>().text : null;
    }

    // Autosolver by Kilo Bites

    private struct QueueInfo
    {
        public int CurrentPos;
        public int? ParentPos;
        public int? Direction;

        public QueueInfo(int currentPos, int? parentPos, int? direction)
        {
            CurrentPos = currentPos;
            ParentPos = parentPos;
            Direction = direction;
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        if (!traversable)
        {
            Screen.OnInteract();
            yield return new WaitForSeconds(0.1f);
        }

        var q = new Queue<QueueInfo>();
        var visited = new Dictionary<int, QueueInfo>();

        q.Enqueue(new QueueInfo(currentPosition, null, null));

        while (q.Count > 0)
        {
            var qi = q.Dequeue();
            if (visited.ContainsKey(qi.CurrentPos))
                continue;
            visited[qi.CurrentPos] = qi;
            if (qi.CurrentPos == goalPosition)
                goto goalfound;

            for (int i = 0; i < 4; i++)
            {
                var p = (int)Math.Pow(2, i);
                if ((mazeDirs[qi.CurrentPos] & p) == p)
                    q.Enqueue(new QueueInfo(qi.CurrentPos + vectors[i], qi.CurrentPos, i));
            }
        }

        throw new InvalidOperationException(string.Format("KayMazey Talk #{0} is not solvable.", moduleId));

    goalfound:

        var r = goalPosition;
        var path = new List<int>();

        while (true)
        {
            var nr = visited[r];

            if (nr.ParentPos == null)
                break;

            path.Add(nr.Direction.Value);

            r = nr.ParentPos.Value;
        }

        var opposites = new Dictionary<int, int>
        {
            { 0, 2 },
            { 1, 3 },
            { 2, 0 },
            { 3, 1 }
        };

        for (int i = path.Count - 1; i >= 0; i--)
        {
            Arrows[invert ? opposites[path[i]] : path[i]].OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
    }
}
