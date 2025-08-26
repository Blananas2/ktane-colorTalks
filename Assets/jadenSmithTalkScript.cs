using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public class jadenSmithTalkScript : MonoBehaviour
{

    public KMAudio Audio;
    public KMBombModule Module;
    public KMRuleSeedable RuleSeed;

    public TextMesh Quote;
    public KMSelectable Check;
    public KMSelectable Cross;

    string[] quotes = {
        "Shout Out To My Guy Zac Efron",
        "Green White And Blue Make Dreams Come True.",
        "When I Die. Then You Will Realize",
        "If A Cup Cake Falls From A Tree How Far Away Will It Be From Down. #Jupiter",
        "When The First Animal Went Extinct That Should've Bin A Sign.",
        "A lot Of My Tweets Will Come True In 2018",
        "Education Is Rebellion.",
        "I Honestly Love When People Hate Even When There Close To You.",
        "You Think You Get It. YOU DONT YOU DONT YOU DONT!!!!!!!",
        "The Head Of The Sphinx Will Fall Off In The Near Future.",
        "I Got 3 Instagram Followers",
        "The Biggest Flex Anyone Will Ever Have Is Dying.",
        "I Am Not A Human And I Don't Speak English",
        "\"the displacement from zero at zero time of a body undergoing simple harmonic motion.\"",
        "I Only Apply To The Sixth Amendment",
        "Reading Is My Passion And My Escape Since I Was 5 Years Old. Overall, Children Don't Realize The Magic That Can Live Inside Their Own Heads. Better Even Then Any Movie.",
        "Once All 100% Is Neglected You Have A Citizen. A Walking Zombie Who Criticizes Every Thing They See. Have Fun Its A Really Awesome Place.",
        "The Moment That Truth Is Organized It Becomes A Lie.",
        "If Newborn Babies Could Speak They Would Be The Most Intelligent Beings On Planet Earth",
        "Unawareness Is The Only Sin, And If You Were Aware You Would Know.",
        "Me: Hey Wanna Talk About The Economic And Political State Of World Together. Girl: Nah Me: Ok Cool",
        "We Need To Stop Teaching The Youth About The Past And Encourage Them To Change The Future",
        "I'm Glad That Our Distance Makes Us Witness Ourselves From A Different Entrance.",
        "Don't Argue With Anybody About What Color The Sky Is.",
        "People Tell Me To Smile I Tell Them The Lack Of Emotion In My Face Doesn't Mean I'm Unhappy",
        "\"Hey Are You Jaden Can I Have A Picture With You\" No Cause I'm Super Sad But We Can Sit And Talk.",
        "Pay Attention To The Numbers In Your Life They Are Vastly Important.",
        "Why Is It Always 3 WHY IS IT ALWAYS 3!!!!!",
        "I Encourage You All To Unfollow Me So I Can Be Left With The People Who Actually Appreciate Philosophy And Poetry. #CoolTapeVol2",
        "I'm Here If You Need A Fellow Insane Person To Talk To. But I'm Seriously Here Not Like One Of Those I'm Here For You's That Everybody Says.",
        "Don't Worry Bae I'll Talk To You About SpaceTime Over FaceTime.",
        "REAL TALK: Sometimes I Am Scared By All My Wisdom. Jaden Out.",
        "Water In The Eyes And Alcohol In The Eyes Are Pretty Much The Same I Know This From First Hand Experience.",
        "como os espelhos podem ser reais se nossos olhos não são reais",
        "How Can Mirrors Be Real If Our Eyes Aren't Real",
        "Everybody Get Off Your Phones And Go Do What You Actually Wanna Do",
        "You Do Not Know Who You Are Or Why Your Here So When You See Someone Who Dose The Society Comes Together As A Whole And Destroys Them.",
        "Pop Changes Week To Week, Month To Month. But Great Music Is Like Literature.",
        "If It's Lit, It's Truly Lit",
        "People Use To Ask Me What Do You Wanna Be When You Get Older And I Would Say What A Stupid Question The Real Question Is What Am I Right Now",
        "I Will Always Give You The Truth I Will Never Lie To You In My music If You Cant Handle My Feelings And Emotions Please Unfollow Me",
        "I Just Like Showing Pretty Girls A Good Time Weather I'm Physically There Or Not Doesn't Matter.",
        "All The Rules In This World Were Made By Someone No Smarter Than You. So Make Your Own",
        "I Should Just Stop Tweeting, The Human Consciousness Must Raise Before I Speak My Juvenile Philosophy. Shouts Out To @TIME",
        "School Is The Tool To Brainwash The Youth.",
        "Females Are Amazing.",
        "A Little Girl Just Asked Me If I Was Willow Smith I Humbly Said Yes And Took A Selfie.",
        "When You Party I'm On The Treadmill,When You Sleep I'm On The Treadmill, When I Tweet I'm On TheTreadmill. Remember This",
        "What Is The Definition Of \"Light\"?",
        "Trees Are Never Sad Look At Them Every Once In Awhile They're Quite Beautiful",
        "If A Book Store Never Runs Out Of A Certain Book, Dose That Mean That Nobody Reads It, Or Everybody Reads It",
        "Dying Is MainStream #MONEY",
        "Every 7 Years Your Body Is Completely Replaced With Entirely New Cells So Just Because You Look The Same Doesn't Mean You Are.",
        "The More Time You Spend Awake The More Time You Spend Asleep.",
        "Here's The Deal We Can All Follow Christ,Buddha,Krishna You Choose. Or. We Can Become Them.",
        "If Everybody In The World Dropped Out Of School We Would Have A Much More Intelligent Society",
        "I Don't Want You Guys To Think Because I Was Born In America That I Speak And Abide By English Grammar. I Speak Jaden, Indefinitely.",
        "Most Trees Are Blue",
        "You Can Discover Everything You Need To Know About Everything By Looking At Your Hands",
        "I Hope It Doesn't Take For Me To Die For You To See What I Do For You",
        "If I Were White And Not Will Smith's \"\"\"\"\"SILLY\"\"\"\"\" Son I Would Be The Most Respected Philosopher Of Our And All Time.",
        "To The Artist Of This Coming Generation And Of The Renaissance. The People That Truly Understand Your Art are The People Who Don't Comment",
        "People Think A RelationShip Makes You Whole, That It's Two 50%'s Coming Together To Make 100% When It Should Be Two 100%'s Making 200%•••",
        "Stop Gossiping Reflect Internally You Will Find Yourself A Neglected Part Of Your Self.",
        "There Is No World Outside Us. The World Is, In Fact, Our Collective Projections Of Love And Fear, Hopes And Conflicts. In Taking Responsibility For Our Own Thoughts And Feelings, We play Our Part In The Healing Of The World.",
        "If I Die In My Flannel Will You Write My Poems On Tyler's 5 Panels And Jesusus Sandals This Plane Is Just To Much To Handle.",
        "Girls Don't Like Gentlemen",
        "Ill Never Forget The Blogs That Believed In Me Since The Begging.",
        "Either I Lie To You Or We Cry Together",
        "Umm Who Has The Floss",
        "You Taught Me How To Play The Piano But Have Never Heard Me.",
        "Im Not A Person",
        "If I Wanna Wear A Dress, Then I Will, And That Will Set The New Wave... -JADEN SMITH #ICON",
        "Once You Go In You Always Come Out Alive",
        "Female Energy",
        "When You Live Your Whole Life In A Prison Freedom Can Be So Dull.",
        "The Great Gatsby Is One Of The Greatest Movies Of All Time, Coachella.",
        "If There Is Bread Winners, There Is Bread Losers. But You Can't Toast What Isn't Real. #BreadPeopleLives",
        "Jealousy Just Reassures Your Love.",
        "The More Intelligent Somebody Becomes The More I Fall In Love With Them.",
        "I Just Scrolled Through My Tweets And \"I\" Started Laughing.",
        "Luke. Who Has The Trident",
        "You Would Have To Eat 5 Apples Today To Get The Same Nutritional Value As An Apple From 1950. #Fallow",
        "\"It's Your Birthday\" Mateo Said. I Didn't Respond. \"Are You Not Excited To Be 15\" He Asked. Reading My Book I Uttered \"I Turned 15 Long Ago\"",
        "I Build Pyramids Constantly",
        "I Scare People Away",
        "Anyone Born On This Planet Should Have A Planetary Citizenship Enabling Them To Freely Explore There Home",
        "If You Focus On Success, You'll Have Stress. But If You Pursue Excellence, Success Will Be Guaranteed.",
        "Lately People Call Me Scoop Life",
        "I Only Skate When I Have Emotional Trauma",
        "Commit To Love Not To People, Commit To Happiness Because When You Commit To People The Love Leaves And You Are Forced To Stay.",
        "Either I Only Have One Pair Of Shoes Or Every Pair You Choose.",
        "I've Bin Drinking Distilled Water For So Long That When I Drink Normal Water It Feels Like I'm Swallowing Huge Chunks Of Aluminum.",
        "I'm Here For You. We're About To Flip Things Around",
        "If I Had A Nickel For Every Time I've Cried In The Back Of An Uber, I Would Have Another Pair Of Yeezy's.",
        "I Watch Twilight Every Night",
        "There Is No Nutrients In Our Food Anymore Or In Our Soil OR IN OUR WATER",
        "Don't Tell Me You Cried Cause I Know That You Didn't",
        "You Must Not Know Fashion"
    };
    bool ans;
    const int MANUAL_HEIGHT = 32;
    const int MANUAL_WIDTH = 79;

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake()
    {
        moduleId = moduleIdCounter++;

        Check.OnInteract += delegate () { ButtonPress(true); return false; };
        Cross.OnInteract += delegate () { ButtonPress(false); return false; };
    }

    // Use this for initialization
    void Start()
    {
        var rs = RuleSeed.GetRNG();
        string[] og = (string[])quotes.Clone();
        rs.ShuffleFisherYates(quotes);

        List<string> quoteList = new List<string> { };
        string[] quoteArray = { };
        int lineTotal = 0;
        for (int q = 0; q < quotes.Length; q++)
        {
            int lineCount = wordWrap(quotes[q], MANUAL_WIDTH).Split('\n').Length;
            if (lineTotal + lineCount > MANUAL_HEIGHT)
            {
                quoteArray = quoteList.ToArray();
                Array.Sort(quoteArray);
                break;
            }
            quoteList.Add(quotes[q]);
            lineTotal += lineCount;
        }
        Debug.LogFormat("<Jaden Smith Talk #{0}> Quote list:\n{1}", moduleId, quoteArray.Join("\n"));

        ans = Rnd.Range(0, 2) == 0;
        string chosenQuote = ans ? quoteArray.PickRandom() : jadenSmithFakes.fakeQuotes[Array.IndexOf(og, quoteArray.PickRandom())].PickRandom();
        Quote.text = wordWrap(chosenQuote, 35);
        Debug.LogFormat("[Jaden Smith Talk #{0}] Chosen Quote: {1}", moduleId, chosenQuote);
        Debug.LogFormat("[Jaden Smith Talk #{0}] The above quote {1} tweeted by Jaden Smith.", moduleId, ans ? "was" : "was not");
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

    void ButtonPress(bool b)
    {
        if (b) { Check.AddInteractionPunch(1f); } else { Cross.AddInteractionPunch(1f); }
        if (moduleSolved) { return; }
        if (ans == b)
        {
            Audio.PlaySoundAtTransform("JST_solve", b ? Check.transform : Cross.transform);
            Module.HandlePass();
            moduleSolved = true;
            Debug.LogFormat("[Jaden Smith Talk #{0}] Pressed {1}. That is correct. Module solved.", moduleId, b ? "✓" : "✗");
            StartCoroutine(SolveAnim());
        }
        else
        {
            Module.HandleStrike();
            Debug.LogFormat("[Jaden Smith Talk #{0}] Pressed {1}. That is incorrect, strike!", moduleId, b ? "✓" : "✗");
        }
    }

    private IEnumerator SolveAnim()
    {
        float elapsed = 0f;
        float duration = 0.25f;
        while (elapsed < duration)
        {
            Quote.transform.localScale = new Vector3(Lerp(0.00065f, 0f, elapsed * 4), Lerp(0.00065f, 0.00123f, elapsed * 4), 1f);
            yield return null;
            elapsed += Time.deltaTime;
        }
        Quote.text = null;
    }
    
    float Lerp(float a, float b, float t)
    { //this assumes t is in the range 0-1
        return a * (1f - t) + b * t;
    }

    // Twitch Plays by Kilo Bites

#pragma warning disable 414
    private readonly string TwitchHelpCommand = @"!{0} press check || !{0} press x [presses the specified button]";
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        string[] split = command.ToUpperInvariant().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

        yield return null;

        if ("PRESS".ContainsIgnoreCase(split[0]))
        {
            if (split.Length == 1)
            {
                yield return "sendtochaterror Press what?";
                yield break;
            }

            if (split.Length > 2)
            {
                yield return "sendtochaterror You have inputted too many parameters. Please try again!";
                yield break;
            }

            var valids = new[] { "CHECK", "X" };

            if (!valids.Contains(split[1]))
            {
                yield return string.Format("{0} is not a valid button!", split[1]);
                yield break;
            }

            if ("CHECK".ContainsIgnoreCase(split[1]))
            {
                Check.OnInteract();
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                Cross.OnInteract();
                yield return new WaitForSeconds(0.1f);
            } 
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        if (ans)
        {
            Check.OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
        else
        {
            Cross.OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
    }
}
