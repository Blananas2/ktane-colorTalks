using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public class crazyTalkWithAKScript : MonoBehaviour {

    public KMAudio Audio;
    public KMBombModule Module;

    public Projector Projector;

    string[][] table = {
        new string[] { "HATS", "EARN", "DOPE", "YANK", "FORK", "DUTY", "JERK", "AUNT", "IOTA", "UTES" },
        new string[] { "HONK", "REDS", "ORGY", "COBS", "VAIN", "RAYS", "TECH", "LICK", "SWAN", "LASH" },
        new string[] { "AVOW", "PEAR", "BODY", "JAYS", "BEAD", "FAWN", "DIAL", "SYNC", "BARE", "NUKE" },
        new string[] { "FRET", "MENU", "AXES", "TRAP", "ROBS", "ATOP", "REFS", "SELF", "GAVE", "SOUL" },
        new string[] { "ANTS", "NAIL", "TIES", "SLAG", "DOGS", "MICS", "YURT", "UNTO", "ARMS", "PIGS" },
        new string[] { "ROLE", "FAKE", "SWAY", "TALC", "SKID", "LUGE", "CITE", "ACHE", "FILE", "CHAP" },
        new string[] { "COMB", "MOTH", "OMEN", "FIRS", "BOGS", "RUES", "DISH", "ROSE", "LIPS", "NETS" },
        new string[] { "MOVE", "BRAS", "DENS", "GUNK", "SEWN", "ZIPS", "PAVE", "ORCS", "TRAY", "HAZE" },
        new string[] { "HORN", "HURL", "ROCK", "GUSH", "FAST", "HEAD", "SLOT", "PURE", "STUD", "CLAD" },
        new string[] { "SPUR", "CHIP", "JOYS", "ALOE", "BENT", "TACK", "TONS", "FELT", "SICK", "LIMP" }
    };

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake () {
        moduleId = moduleIdCounter++;
        /*
        foreach (KMSelectable object in keypad) {
            object.OnInteract += delegate () { keypadPress(object); return false; };
        }
        */

        //button.OnInteract += delegate () { buttonPress(); return false; };

    }

    // Use this for initialization
    void Start () {
        float scale = transform.lossyScale.x;
        Projector.nearClipPlane *= scale;
        Projector.farClipPlane *= scale;
        Projector.orthographicSize *= scale;
    }

    /*
    void keypadPress(KMSelectable object) {
        
    }
    */

    /*
    void buttonPress() {

    }
    */
}
