using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public class crazyTalkWithAKScript : MonoBehaviour
{

    public KMAudio Audio;
    public KMBombModule Module;

    public Projector Projector;
    public Transform Wrapper;
    public KMSelectable[] Arrows;
    public KMSelectable Screen;
    public TextMesh Letter;
    public MeshFilter MeshFilter;
    public Mesh[] Meshes;
    public Texture[] Textures;

    string possibleLetters = "ABCDEFGHIKLMNOPRSTUY";
    string[] table = {
        "HATS", "EARN", "DOPE", "YANK", "FORK", "DUTY", "JERK", "AUNT", "IOTA", "UTES",
        "HONK", "REDS", "ORGY", "COBS", "VAIN", "RAYS", "TECH", "LICK", "SWAN", "LASH",
        "AVOW", "PEAR", "BODY", "JAYS", "BEAD", "FAWN", "DIAL", "SYNC", "BARE", "NUKE",
        "FRET", "MENU", "AXES", "TRAP", "ROBS", "ATOP", "REFS", "SELF", "GAVE", "SOUL",
        "ANTS", "NAIL", "TIES", "SLAG", "DOGS", "MICS", "YURT", "UNTO", "ARMS", "PIGS",
        "ROLE", "FAKE", "SWAY", "TALC", "SKID", "LUGE", "CITE", "ACHE", "FILE", "CHAP",
        "COMB", "MOTH", "OMEN", "FIRS", "BOGS", "RUES", "DISH", "ROSE", "LIPS", "NETS",
        "MOVE", "BRAS", "DENS", "GUNK", "SEWN", "ZIPS", "PAVE", "ORCS", "TRAY", "HAZE",
        "HORN", "HURL", "ROCK", "GUSH", "FAST", "HEAD", "SLOT", "PURE", "STUD", "CLAD",
        "SPUR", "CHIP", "JOYS", "ALOE", "BENT", "TACK", "TONS", "FELT", "SICK", "LIMP"
    };
    int[][] pairings = { new int[] { 0, 1 }, new int[] { 0, 2 }, new int[] { 0, 3 }, new int[] { 0, 4 }, new int[] { 0, 5 }, new int[] { 0, 6 }, new int[] { 0, 7 }, new int[] { 0, 8 }, new int[] { 1, 2 }, new int[] { 1, 3 }, new int[] { 1, 4 }, new int[] { 1, 5 }, new int[] { 1, 6 }, new int[] { 1, 7 }, new int[] { 1, 8 }, new int[] { 2, 3 }, new int[] { 2, 4 }, new int[] { 2, 5 }, new int[] { 2, 6 }, new int[] { 2, 7 }, new int[] { 2, 8 }, new int[] { 3, 4 }, new int[] { 3, 5 }, new int[] { 3, 6 }, new int[] { 3, 7 }, new int[] { 3, 8 }, new int[] { 4, 5 }, new int[] { 4, 6 }, new int[] { 4, 7 }, new int[] { 4, 8 }, new int[] { 5, 6 }, new int[] { 5, 7 }, new int[] { 5, 8 }, new int[] { 6, 7 }, new int[] { 6, 8 }, new int[] { 7, 8 } };
    char solutionLetter;

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake()
    {
        moduleId = moduleIdCounter++;

        for (int a = 0; a < 4; a++)
        {
            int ax = a; //this is so incredibly dumb
            Arrows[a].OnInteract += delegate { ArrowPress(ax); return false; };
        }

        Screen.OnInteract += delegate () { ScreenPress(); return false; };
    }

    private float projectorNearClipPlane;
    private float projectorFarClipPlane;
    private float projectorOrthographicSize;

    private void Update()
    {
        float scale = transform.lossyScale.x;
        Projector.nearClipPlane = projectorNearClipPlane * scale;
        Projector.farClipPlane = projectorFarClipPlane * scale;
        Projector.orthographicSize = projectorOrthographicSize * scale;
    }

    // Use this for initialization
    void Start()
    {
        projectorNearClipPlane = Projector.nearClipPlane;
        projectorFarClipPlane = Projector.farClipPlane;
        projectorOrthographicSize = Projector.orthographicSize;

        solutionLetter = possibleLetters.PickRandom();
        bool foundSolution = false;
        int centerIx = -1;
        int chosenPairing = -1;
        int[] centerOrd = Enumerable.Range(0, 100).ToArray().Shuffle();
        int[] pairingOrd = Enumerable.Range(0, 36).ToArray().Shuffle();
        int co = -1;
        int[] pair = { -1, -1 };
        while (!foundSolution)
        {
            co++;
            centerIx = centerOrd[co];
            for (int po = 0; po < 36; po++)
            {
                chosenPairing = pairingOrd[po];
                pair = pairings[chosenPairing];
                if (InCommon(table[Offset(centerIx, pair[0])], table[Offset(centerIx, pair[1])]) == solutionLetter)
                {
                    foundSolution = true;
                    break;
                }
            }
        }

        MeshFilter.mesh = Meshes[chosenPairing];
        // clone the mat
        Projector.material = new Material(Projector.material);
        Projector.material.mainTexture = Textures[centerIx];
        Wrapper.transform.localEulerAngles = new Vector3(Rnd.Range(-5500, 5100) * 0.01f, 0f, Rnd.Range(-2700, 4000) * 0.01f);

        Debug.LogFormat("<Crazy Talk With A K #{0}> Applied distortion: {1}", moduleId, Wrapper.localRotation);
        Debug.LogFormat("[Crazy Talk With A K #{0}] Distorted word is {1}.", moduleId, table[centerIx]);
        Debug.LogFormat("[Crazy Talk With A K #{0}] The words corresponding to the raised regions are {1} and {2}.", moduleId, table[Offset(centerIx, pair[0])], table[Offset(centerIx, pair[1])]);
        Debug.LogFormat("[Crazy Talk With A K #{0}] The letter they have in common is {1}.", moduleId, solutionLetter.ToString());
    }

    int Offset(int c, int v)
    {
        int x = c % 10;
        int y = c / 10;
        switch (v)
        {
            case 0: return (y + 9) % 10 * 10 + ((x + 9) % 10);
            case 1: return (y + 9) % 10 * 10 + (x % 10);
            case 2: return (y + 9) % 10 * 10 + ((x + 1) % 10);
            case 3: return y * 10 + ((x + 9) % 10);
            case 4: return y * 10 + (x % 10);
            case 5: return y * 10 + ((x + 1) % 10);
            case 6: return (y + 1) % 10 * 10 + ((x + 9) % 10);
            case 7: return (y + 1) % 10 * 10 + (x % 10);
            default: /*case 8:*/ return (y + 1) % 10 * 10 + ((x + 1) % 10);
        }
    }

    char? InCommon(string a, string b)
    {
        char? w = null;
        for (int ar = 0; ar < 4; ar++)
        {
            for (int br = 0; br < 4; br++)
            {
                if (a[ar] == b[br])
                {
                    if (w == null)
                    {
                        w = a[ar];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
        return w;
    }

    void ArrowPress(int w)
    {

    }

    void ScreenPress()
    {

    }
}
