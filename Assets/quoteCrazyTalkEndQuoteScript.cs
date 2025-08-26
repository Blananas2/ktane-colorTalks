using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public class quoteCrazyTalkEndQuoteScript : MonoBehaviour
{

    public KMAudio Audio;
    public KMBombModule Module;

    public KMSelectable MainScreen;
    public KMSelectable[] SmallScreens;
    public GameObject[] SmallObjs;
    public TextMesh[] Words;
    public TextMesh[] Digits;
    public GameObject HighlightObj;
    public GameObject[] WordObjs;
    public GameObject[] DigitObjs;

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    string[] tableWords = {
        "",     "KNEE", "NUMB", "TOFU", "ZERO", "ENBY", "AXIS", "EACH", "VETO", "",
        "ELSE", "TECH", "IDOL", "ECHO", "JOEY", "TREK", "EXAM", "KIWI", "EXPO", "MOJO",
        "TUTU", "DELI", "CRIB", "NOVA", "ODOR", "SYNC", "FERN", "ONCE", "AMMO", "EURO",
        "VOID", "REDO", "ITEM", "AUTO", "EVIL", "ISLE", "OBOE", "INFO", "PESO", "GURU",
        "ZANY", "GNAW", "HAJJ", "DUTY", "AHOY", "TUFT", "EDGE", "ORCA", "ERGO", "YETI",
        "AURA", "INCH", "DATA", "SHWA", "ONLY", "ULNA", "IDEA", "UREA", "ONYX", "ANAL",
        "POEM", "ATOM", "HOBO", "HIYA", "STYE", "ALLY", "JUDO", "BUZZ", "JAVA", "WIKI",
        "ANON", "GRUB", "YOLK", "CRUX", "ORGY", "LIAR", "BOZO", "LUAU", "WREN", "IOTA",
        "WHOA", "UGLY", "RELY", "HOAX", "CLEF", "EPEE", "UNDO", "SEXY", "LYNX", "DEMO",
        "",     "IDLY", "FOCI", "EPIC", "DODO", "CYAN", "ICKY", "OBEY", "MENU", ""
    };
    string[] tableDigits = {
        "9814072356", //top, from left to right
        "2170348569", //right, from top to bottom
        "1029765384", //bottom, from left to right
        "3816547290" //left, from top to bottom
    };
    int[] ctee = { -1, -1, -1, -1 }; //"closest to each edge", specifically the index where each edge will find its corresponding position
    string[] quedWords = { "", "", "", "" };
    string[] allWords = { "ABLE", "ABUT", "ACED", "ACES", "ACHE", "ACHY", "ACID", "ACME", "ACNE", "ACRE", "ACTS", "ADDS", "AFAR", "AGED", "AGES", "AHEM", "AHOY", "AIDS", "AIMS", "AIRS", "AIRY", "AJAR", "AKIN", "ALAS", "ALLY", "ALOE", "ALSO", "ALTO", "AMEN", "AMID", "AMMO", "AMOK", "AMPS", "ANAL", "ANEW", "ANON", "ANTE", "ANTI", "ANTS", "ANUS", "APES", "APEX", "APPS", "ARCH", "ARCS", "AREA", "ARID", "ARMS", "ARMY", "ARSE", "ARTS", "ARTY", "ASKS", "ATOM", "ATOP", "AUNT", "AURA", "AUTO", "AVOW", "AWAY", "AWED", "AWES", "AWRY", "AXED", "AXES", "AXIS", "AXLE", "AYES", "BAAS", "BABE", "BABY", "BACK", "BAGS", "BAIL", "BAIT", "BAKE", "BALD", "BALE", "BALK", "BALL", "BALM", "BAND", "BANE", "BANG", "BANK", "BANS", "BARE", "BARF", "BARK", "BARN", "BARS", "BASE", "BASH", "BASS", "BATH", "BATS", "BAWL", "BAYS", "BEAD", "BEAK", "BEAM", "BEAN", "BEAR", "BEAT", "BEDS", "BEEF", "BEEN", "BEEP", "BEER", "BEES", "BEET", "BEGS", "BELL", "BELT", "BEND", "BENT", "BEST", "BETA", "BETS", "BIAS", "BIBS", "BIDS", "BIGS", "BIKE", "BILL", "BIND", "BINS", "BIRD", "BITE", "BITS", "BLAB", "BLAH", "BLED", "BLEW", "BLIP", "BLOB", "BLOC", "BLOG", "BLOT", "BLOW", "BLUB", "BLUE", "BLUR", "BOAR", "BOAS", "BOAT", "BOBS", "BODS", "BODY", "BOGS", "BOIL", "BOLD", "BOLT", "BOMB", "BOND", "BONE", "BONG", "BONK", "BONY", "BOOB", "BOOK", "BOOM", "BOOS", "BOOT", "BOPS", "BORE", "BORN", "BOSS", "BOTH", "BOTS", "BOUT", "BOWL", "BOWS", "BOXY", "BOYS", "BOZO", "BRAG", "BRAN", "BRAS", "BRAT", "BRED", "BREW", "BRIG", "BRIM", "BROS", "BROW", "BUCK", "BUDS", "BUFF", "BUGS", "BULB", "BULK", "BULL", "BUMP", "BUMS", "BUNK", "BUNS", "BUNT", "BUOY", "BURN", "BURP", "BURR", "BURY", "BUSH", "BUSK", "BUST", "BUSY", "BUTS", "BUTT", "BUYS", "BUZZ", "BYES", "BYTE", "CABS", "CAFE", "CAGE", "CAGY", "CAKE", "CALF", "CALL", "CALM", "CAME", "CAMP", "CAMS", "CANE", "CANS", "CAPE", "CAPS", "CARB", "CARD", "CARE", "CARP", "CARS", "CART", "CASE", "CASH", "CASK", "CAST", "CATS", "CAVE", "CELL", "CENT", "CERT", "CHAP", "CHAR", "CHAT", "CHEF", "CHEW", "CHIC", "CHIN", "CHIP", "CHOC", "CHOP", "CHOW", "CHUG", "CHUM", "CITE", "CITY", "CLAD", "CLAM", "CLAN", "CLAP", "CLAW", "CLAY", "CLEF", "CLIP", "CLIT", "CLOG", "CLOT", "CLUB", "CLUE", "COAL", "COAT", "COAX", "COBS", "COCK", "CODA", "CODE", "CODS", "COGS", "COIL", "COIN", "COKE", "COLA", "COLD", "COMA", "COMB", "COME", "COMP", "CONE", "CONS", "COOK", "COOL", "COOP", "COOS", "COOT", "COPE", "COPS", "COPY", "CORD", "CORE", "CORK", "CORN", "COST", "COTS", "COUP", "COVE", "COWS", "COZY", "CRAB", "CRAM", "CRAP", "CRED", "CREW", "CRIB", "CROC", "CROP", "CROW", "CRUD", "CRUX", "CUBE", "CUBS", "CUED", "CUES", "CUFF", "CULL", "CULT", "CUNT", "CUPS", "CURB", "CURD", "CURE", "CURL", "CUSP", "CUSS", "CUTE", "CUTS", "CYAN", "CYST", "CZAR", "DABS", "DADS", "DAFT", "DAME", "DAMN", "DAMP", "DAMS", "DANG", "DANK", "DARE", "DARK", "DARN", "DART", "DASH", "DATA", "DATE", "DAWN", "DAYS", "DAZE", "DEAD", "DEAF", "DEAL", "DEAR", "DEBT", "DECK", "DEED", "DEEM", "DEEP", "DEER", "DEFY", "DELI", "DEMO", "DENS", "DENT", "DENY", "DESK", "DEWY", "DIAL", "DIBS", "DICE", "DICK", "DIED", "DIES", "DIET", "DIGS", "DILL", "DIME", "DIMS", "DINE", "DING", "DIPS", "DIRE", "DIRT", "DISC", "DISH", "DISK", "DISS", "DIVA", "DIVE", "DOCK", "DOCS", "DODO", "DOER", "DOES", "DOGS", "DOLL", "DOLT", "DOME", "DONE", "DOOM", "DOOR", "DOPE", "DORK", "DORM", "DOSE", "DOTS", "DOVE", "DOWN", "DOZE", "DRAB", "DRAG", "DRAW", "DREW", "DRIP", "DROP", "DRUG", "DRUM", "DUAL", "DUBS", "DUCK", "DUCT", "DUDE", "DUDS", "DUEL", "DUES", "DUET", "DUKE", "DULL", "DUMB", "DUMP", "DUNE", "DUNG", "DUNK", "DUOS", "DUPE", "DUSK", "DUST", "DUTY", "DYED", "DYES", "EACH", "EARL", "EARN", "EARS", "EASE", "EAST", "EASY", "EATS", "EBBS", "ECHO", "EDDY", "EDGE", "EDGY", "EDIT", "EELS", "EGGS", "EGOS", "ELKS", "ELSE", "EMIT", "EMOS", "EMUS", "ENDS", "ENBY", "ENVY", "EONS", "EPEE", "EPIC", "ERAS", "ERGO", "ETCH", "EURO", "EVEN", "EVER", "EVES", "EVIL", "EWES", "EXAM", "EXEC", "EXES", "EXIT", "EXPO", "EYED", "EYES", "FACE", "FACT", "FADE", "FADS", "FAFF", "FAIL", "FAIR", "FAKE", "FALL", "FAME", "FANG", "FANS", "FARM", "FART", "FAST", "FATE", "FATS", "FAUX", "FAVE", "FAWN", "FEAR", "FEAT", "FEED", "FEEL", "FEES", "FEET", "FELL", "FELT", "FERN", "FEUD", "FIAT", "FIBS", "FIGS", "FILE", "FILL", "FILM", "FIND", "FINE", "FINS", "FIRE", "FIRM", "FIRS", "FISH", "FIST", "FITS", "FIVE", "FIZZ", "FLAB", "FLAG", "FLAK", "FLAN", "FLAP", "FLAT", "FLAW", "FLAY", "FLEA", "FLED", "FLEE", "FLEW", "FLEX", "FLIP", "FLOE", "FLOP", "FLOW", "FLUB", "FLUX", "FOAM", "FOBS", "FOCI", "FOES", "FOIL", "FOLD", "FOLK", "FOND", "FONT", "FOOD", "FOOL", "FOOT", "FORE", "FORK", "FORM", "FORT", "FOUL", "FOUR", "FOWL", "FOXY", "FRAT", "FRAY", "FREE", "FRET", "FROG", "FROM", "FUCK", "FUEL", "FULL", "FUME", "FUND", "FUNK", "FURS", "FURY", "FUSE", "FUSS", "FUTZ", "FUZZ", "GAGS", "GAIN", "GALA", "GALL", "GALS", "GAME", "GANG", "GAPS", "GASH", "GASP", "GATE", "GAVE", "GAWK", "GAYS", "GAZE", "GEAR", "GEEK", "GELS", "GEMS", "GENE", "GENS", "GENT", "GERM", "GETS", "GIFT", "GIGS", "GILL", "GINS", "GIRL", "GIST", "GIVE", "GLAD", "GLAM", "GLEE", "GLOW", "GLUE", "GLUT", "GNAT", "GNAW", "GNUS", "GOAL", "GOAT", "GODS", "GOER", "GOES", "GOLD", "GOLF", "GONE", "GONG", "GOOD", "GOOF", "GOON", "GOOP", "GORE", "GORY", "GOSH", "GOTH", "GOWN", "GRAB", "GRAD", "GRAM", "GRAY", "GREW", "GREY", "GRID", "GRIM", "GRIN", "GRIP", "GRIT", "GROK", "GROW", "GRUB", "GULF", "GULL", "GULP", "GUMS", "GUNK", "GUNS", "GURU", "GUSH", "GUST", "GUTS", "GUYS", "GYMS", "GYRO", "HACK", "HAGS", "HAIL", "HAIR", "HAJJ", "HALF", "HALL", "HALO", "HALT", "HAMS", "HAND", "HANG", "HANK", "HARD", "HARE", "HARM", "HARP", "HASH", "HATE", "HATH", "HATS", "HAUL", "HAVE", "HAWK", "HAZE", "HAZY", "HEAD", "HEAL", "HEAP", "HEAR", "HEAT", "HECK", "HEEL", "HEFT", "HEIR", "HELD", "HELL", "HELM", "HELP", "HENS", "HERB", "HERD", "HERE", "HERO", "HERS", "HIDE", "HIGH", "HIKE", "HILL", "HILT", "HIND", "HINT", "HIPS", "HIRE", "HISS", "HITS", "HIVE", "HIYA", "HOAX", "HOBO", "HOES", "HOGS", "HOLD", "HOLE", "HOLY", "HOME", "HONE", "HONK", "HOOD", "HOOF", "HOOK", "HOOP", "HOOT", "HOPE", "HOPS", "HORN", "HOSE", "HOST", "HOTS", "HOUR", "HOWL", "HUBS", "HUES", "HUFF", "HUGE", "HUGS", "HULA", "HULK", "HUMP", "HUMS", "HUNG", "HUNK", "HUNT", "HURL", "HURT", "HUSH", "HUSK", "HUTS", "HYMN", "HYPE", "ICED", "ICES", "ICKY", "ICON", "IDEA", "IDLE", "IDLY", "IDOL", "IFFY", "ILLS", "IMPS", "INCH", "INFO", "INKS", "INKY", "INNS", "INTO", "IONS", "IOTA", "IRIS", "IRKS", "IRON", "ISLE", "ISMS", "ITCH", "ITEM", "JABS", "JACK", "JADE", "JAIL", "JAMS", "JARS", "JAVA", "JAWS", "JAYS", "JAZZ", "JEEP", "JEEZ", "JELL", "JERK", "JEST", "JETS", "JINX", "JIVE", "JOBS", "JOCK", "JOEY", "JOGS", "JOIN", "JOKE", "JOLT", "JOTS", "JOWL", "JOYS", "JUDO", "JUGS", "JUMP", "JUNK", "JURY", "JUST", "JUTS", "KALE", "KART", "KEEL", "KEEN", "KEEP", "KEGS", "KELP", "KEPT", "KEYS", "KICK", "KIDS", "KILL", "KILN", "KILO", "KILT", "KIND", "KING", "KINK", "KISS", "KITE", "KIWI", "KNEE", "KNEW", "KNIT", "KNOB", "KNOT", "KNOW", "LABS", "LACE", "LACK", "LADS", "LADY", "LAGS", "LAID", "LAIR", "LAKE", "LAKH", "LAMB", "LAME", "LAMP", "LAND", "LANE", "LAPS", "LARD", "LASH", "LASS", "LAST", "LATE", "LAVA", "LAVS", "LAWN", "LAWS", "LAYS", "LAZY", "LEAD", "LEAF", "LEAK", "LEAN", "LEAP", "LEEK", "LEFT", "LEGS", "LEND", "LENS", "LENT", "LESS", "LEST", "LETS", "LEWD", "LIAR", "LICE", "LICK", "LIDS", "LIED", "LIES", "LIFE", "LIFT", "LIKE", "LILY", "LIMB", "LIME", "LIMO", "LIMP", "LINE", "LINK", "LINT", "LION", "LIPS", "LISP", "LIST", "LITE", "LIVE", "LOAD", "LOAF", "LOAM", "LOAN", "LOBE", "LOBS", "LOCK", "LOFT", "LOGO", "LOGS", "LONE", "LONG", "LOOK", "LOOM", "LOOP", "LOOS", "LOOT", "LORD", "LORE", "LOSE", "LOSS", "LOST", "LOTS", "LOUD", "LOVE", "LOWS", "LUAU", "LUBE", "LUCK", "LUGE", "LUMP", "LUNG", "LURE", "LURK", "LUSH", "LUST", "LYNX", "LYRE", "MACE", "MACS", "MADE", "MAGS", "MAID", "MAIL", "MAIN", "MAKE", "MALE", "MALL", "MAMA", "MANE", "MANS", "MANY", "MAPS", "MARK", "MARS", "MART", "MASH", "MASK", "MASS", "MAST", "MATE", "MATH", "MATS", "MAUL", "MAWS", "MAYO", "MAZE", "MEAL", "MEAN", "MEAT", "MEET", "MEGA", "MEGS", "MELD", "MELT", "MEME", "MEMO", "MENU", "MEOW", "MERE", "MESA", "MESH", "MESS", "METH", "MEWS", "MICE", "MICS", "MILD", "MILE", "MILK", "MILL", "MIME", "MIND", "MINE", "MINI", "MINT", "MISS", "MIST", "MITE", "MITT", "MOAN", "MOAT", "MOBS", "MOCK", "MODE", "MODS", "MOJO", "MOLD", "MOLE", "MOMS", "MONK", "MONO", "MOOD", "MOON", "MOOS", "MOOT", "MOPS", "MORE", "MOSS", "MOST", "MOTH", "MOVE", "MOWN", "MOWS", "MUCH", "MUGS", "MULE", "MUMS", "MUSE", "MUSH", "MUSK", "MUST", "MUTE", "MUTT", "MYTH", "NAGS", "NAIL", "NAME", "NANA", "NAPS", "NARC", "NAVY", "NAYS", "NEAR", "NEAT", "NECK", "NEED", "NEON", "NERD", "NEST", "NETS", "NEWS", "NEWT", "NEXT", "NIBS", "NICE", "NICK", "NIGH", "NINE", "NITS", "NOBS", "NODE", "NODS", "NOES", "NONE", "NOOK", "NOON", "NOPE", "NORM", "NOSE", "NOSY", "NOTE", "NOUN", "NOVA", "NUBS", "NUDE", "NUKE", "NULL", "NUMB", "NUNS", "NUTS", "OAFS", "OAKS", "OARS", "OATH", "OATS", "OBEY", "OBOE", "ODDS", "ODES", "ODOR", "OFFS", "OGRE", "OHMS", "OILS", "OILY", "OINK", "OKAY", "OKRA", "OLDE", "OMEN", "OMIT", "ONCE", "ONES", "ONLY", "ONTO", "ONYX", "OOHS", "OOPS", "OOZE", "OPAL", "OPEN", "OPTS", "ORAL", "ORBS", "ORCA", "ORCS", "ORES", "ORGY", "OUCH", "OURS", "OUTS", "OVAL", "OVEN", "OVER", "OWED", "OWES", "OWLS", "OWNS", "OXEN", "PACE", "PACK", "PACT", "PADS", "PAGE", "PAID", "PAIL", "PAIN", "PAIR", "PALE", "PALM", "PALS", "PANE", "PANS", "PANT", "PAPA", "PARK", "PART", "PASS", "PAST", "PATH", "PATS", "PAVE", "PAWN", "PAWS", "PAYS", "PEAK", "PEAR", "PEAS", "PECK", "PECS", "PEED", "PEEK", "PEEL", "PEEP", "PEER", "PEES", "PEGS", "PELT", "PENS", "PERK", "PERM", "PERV", "PESO", "PEST", "PETS", "PEWS", "PHEW", "PICK", "PICS", "PIER", "PIES", "PIGS", "PILE", "PILL", "PIMP", "PINE", "PING", "PINK", "PINS", "PINT", "PIPE", "PIPS", "PISS", "PITH", "PITS", "PITY", "PLAN", "PLAY", "PLEA", "PLEB", "PLOP", "PLOT", "PLOW", "PLOY", "PLUG", "PLUM", "PLUS", "PODS", "POEM", "POET", "POKE", "POLE", "POLL", "POLO", "POLY", "POMS", "POND", "PONG", "PONY", "POOF", "POOL", "POOP", "POOR", "POOS", "POPE", "POPS", "PORE", "PORK", "PORN", "PORT", "POSE", "POSH", "POST", "POTS", "POUR", "POUT", "PRAY", "PREP", "PREY", "PROD", "PROF", "PROM", "PROP", "PROS", "PUBS", "PUCE", "PUCK", "PUFF", "PUGS", "PUKE", "PULL", "PULP", "PUMA", "PUMP", "PUNK", "PUNS", "PUNT", "PUNY", "PUPS", "PURE", "PURR", "PUSH", "PUSS", "PUTS", "PUTT", "RACE", "RACK", "RAFT", "RAGE", "RAGS", "RAID", "RAIL", "RAIN", "RAKE", "RAMP", "RAMS", "RANG", "RANK", "RANT", "RAPE", "RAPS", "RARE", "RASH", "RATE", "RATS", "RAVE", "RAYS", "READ", "REAL", "REAM", "REAP", "REAR", "REDO", "REDS", "REEF", "REEK", "REEL", "REFS", "REIN", "RELY", "RENT", "REPS", "REST", "REVS", "RIBS", "RICE", "RICH", "RIDE", "RIDS", "RIFF", "RIFT", "RIGS", "RIMS", "RING", "RIOT", "RIPE", "RIPS", "RISE", "RISK", "ROAD", "ROAM", "ROAR", "ROBE", "ROBS", "ROCK", "RODS", "ROLE", "ROLL", "ROOF", "ROOK", "ROOM", "ROOT", "ROPE", "ROSE", "ROSY", "ROTE", "ROTS", "ROWS", "RUBE", "RUBS", "RUBY", "RUDE", "RUED", "RUES", "RUFF", "RUGS", "RUIN", "RULE", "RUMS", "RUNE", "RUNG", "RUNS", "RUNT", "RUSE", "RUSH", "RUST", "RUTS", "SACK", "SACS", "SAFE", "SAGA", "SAGE", "SAGS", "SAID", "SAIL", "SAKE", "SALT", "SAME", "SAND", "SANE", "SANG", "SANK", "SANS", "SAPS", "SASS", "SAVE", "SAWS", "SAYS", "SCAB", "SCAM", "SCAN", "SCAR", "SCAT", "SCUM", "SEAL", "SEAM", "SEAS", "SEAT", "SECS", "SEED", "SEEK", "SEEM", "SEEN", "SEEP", "SEER", "SEES", "SELF", "SELL", "SEMI", "SEND", "SENT", "SETS", "SEWN", "SEWS", "SEXY", "SHAM", "SHAT", "SHED", "SHIN", "SHIP", "SHIT", "SHOE", "SHOO", "SHOP", "SHOT", "SHOW", "SHUN", "SHUT", "SHWA", "SICK", "SIDE", "SIFT", "SIGH", "SIGN", "SILK", "SILO", "SILT", "SINE", "SING", "SINK", "SINS", "SIPS", "SIRE", "SIRS", "SITE", "SITS", "SIZE", "SKEW", "SKID", "SKIM", "SKIN", "SKIP", "SKIS", "SKIT", "SLAB", "SLAG", "SLAM", "SLAP", "SLAT", "SLAY", "SLED", "SLEW", "SLID", "SLIM", "SLIP", "SLIT", "SLOB", "SLOP", "SLOT", "SLOW", "SLUG", "SLUR", "SLUT", "SMOG", "SMUG", "SMUT", "SNAG", "SNAP", "SNIP", "SNOB", "SNOT", "SNOW", "SNUB", "SNUG", "SOAK", "SOAP", "SOAR", "SOBS", "SOCK", "SODA", "SODS", "SOFA", "SOFT", "SOIL", "SOLD", "SOLE", "SOLO", "SOME", "SONG", "SONS", "SOON", "SOOT", "SORE", "SORT", "SOUL", "SOUP", "SOUR", "SOWN", "SOWS", "SPAM", "SPAN", "SPAS", "SPAT", "SPAY", "SPEC", "SPED", "SPEW", "SPIN", "SPIT", "SPOT", "SPUD", "SPUN", "SPUR", "STAB", "STAR", "STAT", "STAY", "STEM", "STEP", "STEW", "STIR", "STOP", "STOW", "STUB", "STUD", "STUN", "STYE", "SUBS", "SUCH", "SUCK", "SUDS", "SUED", "SUES", "SUIT", "SUMO", "SUMS", "SUNG", "SUNK", "SUNS", "SUPS", "SURE", "SURF", "SWAB", "SWAG", "SWAM", "SWAN", "SWAP", "SWAT", "SWAY", "SWIM", "SWUM", "SYNC", "TABS", "TACK", "TACO", "TACT", "TAGS", "TAIL", "TAKE", "TALC", "TALE", "TALK", "TALL", "TAME", "TANG", "TANK", "TANS", "TAPE", "TAPS", "TARP", "TARS", "TART", "TASK", "TAUT", "TAXI", "TEAL", "TEAM", "TEAR", "TEAS", "TEAT", "TECH", "TEED", "TEEN", "TEES", "TELL", "TEMP", "TEND", "TENS", "TENT", "TERM", "TEST", "TEXT", "THAN", "THAT", "THAW", "THEE", "THEM", "THEN", "THEY", "THIN", "THIS", "THOU", "THRU", "THUD", "THUG", "THUS", "TICK", "TICS", "TIDE", "TIDY", "TIED", "TIER", "TIES", "TILE", "TILL", "TILT", "TIME", "TINS", "TINT", "TINY", "TIPS", "TIRE", "TITS", "TOAD", "TOED", "TOES", "TOFU", "TOGA", "TOGS", "TOLD", "TOLL", "TOMB", "TONE", "TONS", "TOOK", "TOOL", "TOOT", "TOPS", "TORE", "TORN", "TORT", "TOSS", "TOTE", "TOTS", "TOUR", "TOWN", "TOWS", "TOYS", "TRAP", "TRAY", "TREE", "TREK", "TRIG", "TRIM", "TRIO", "TRIP", "TROT", "TRUE", "TUBA", "TUBE", "TUBS", "TUCK", "TUFT", "TUGS", "TUMS", "TUNA", "TUNE", "TURD", "TURF", "TURN", "TUSH", "TUSK", "TUTS", "TUTU", "TWAT", "TWIG", "TWIN", "TWOS", "TYPE", "TYPO", "TYRE", "UGLY", "ULNA", "UNDO", "UNIS", "UNIT", "UNTO", "UPON", "UREA", "URGE", "URNS", "USED", "USER", "USES", "UTES", "VACS", "VAIN", "VALE", "VANE", "VANS", "VAPE", "VARY", "VASE", "VAST", "VATS", "VEER", "VEIL", "VEIN", "VENT", "VERB", "VERY", "VEST", "VETO", "VETS", "VIAL", "VIBE", "VICE", "VIEW", "VILE", "VINE", "VISA", "VISE", "VOID", "VOLT", "VORE", "VOTE", "VOWS", "WACK", "WADS", "WAFT", "WAGE", "WAGS", "WAIL", "WAIT", "WAKE", "WALK", "WALL", "WAND", "WANK", "WANT", "WARD", "WARM", "WARN", "WARP", "WARS", "WART", "WARY", "WASH", "WASP", "WATT", "WAVE", "WAVY", "WAXY", "WAYS", "WEAK", "WEAR", "WEBS", "WEDS", "WEED", "WEEK", "WEEP", "WEES", "WELD", "WELL", "WENT", "WEPT", "WERE", "WEST", "WETS", "WHAM", "WHAT", "WHEN", "WHET", "WHEY", "WHIM", "WHIP", "WHIR", "WHIZ", "WHOA", "WHOM", "WICK", "WIDE", "WIFE", "WIGS", "WIKI", "WILD", "WILL", "WILT", "WILY", "WIMP", "WIND", "WINE", "WING", "WINK", "WINS", "WIPE", "WIRE", "WISE", "WISH", "WISP", "WITH", "WITS", "WOES", "WOKE", "WOLF", "WOMB", "WOOD", "WOOF", "WOOL", "WOOS", "WORD", "WORE", "WORK", "WORM", "WORN", "WOVE", "WOWS", "WRAP", "WREN", "WUSS", "YAKS", "YAMS", "YANG", "YANK", "YAPS", "YARD", "YARN", "YAWN", "YAWS", "YEAH", "YEAR", "YEAS", "YELL", "YELP", "YETI", "YIPS", "YOGA", "YOLK", "YOUR", "YUCK", "YUPS", "YURT", "ZANY", "ZAPS", "ZERO", "ZEST", "ZINC", "ZINE", "ZING", "ZIPS", "ZITS", "ZONE", "ZOOM", "ZOOS" };
    int[] chosenPermutation = { -1, -1, -1, -1 };
    int quoted;
    string[] uncycledDigits = { "", "", "", "" };
    string[] correctDigits = { "", "", "", "" };
    string[] randDigits;
    List<string> submitted = new List<string> { };
    bool struck = false;
    bool good = false;
    bool started = false;
    bool[] pressed = { false, false, false, false };
    int attps = 0;
    int attws = 0;
    string[] finalWords = { "", "", "", "" };

    void Awake()
    {
        moduleId = moduleIdCounter++;

        MainScreen.OnInteract += delegate () { MainPress(); return false; };

        for (int s = 0; s < 4; s++)
        {
            int sx = s; //this is so incredibly dumb
            SmallScreens[s].OnInteract += delegate { SmallPress(sx); return false; };
        }
    }

    // Use this for initialization
    void Start()
    {
        int[][] allPermutations = GetPermutations(Enumerable.Range(0, 4), 4).Select(i => i.ToArray()).ToArray().Shuffle();
        for (int s = 0; s < 4; s++)
        {
            SmallObjs[s].SetActive(false);
        }

    retry:
        int[] positions = Enumerable.Range(0, 100).Except(new[] { 0, 9, 90, 99 }).ToArray().Shuffle().Take(4).ToArray();
        if (!positionsValid(positions))
        {
            attps++;
            goto retry;
        }
        bool foundWord = false;
        for (int p = 0; p < allPermutations.Count(); p++)
        {
            int[] c = allPermutations[p];
            string w = tableWords[positions[c[0]]][0].ToString() + tableWords[positions[c[1]]][1] + tableWords[positions[c[2]]][2] + tableWords[positions[c[3]]][3];
            if (allWords.Contains(w))
            {
                for (int q = 0; q < 4; q++)
                {
                    int wah = c[q];
                    switch (wah)
                    {
                        case 0: finalWords[q] = w[0] + "   "; break;
                        case 1: finalWords[q] = " " + w[1] + "  "; break;
                        case 2: finalWords[q] = "  " + w[2] + " "; break;
                        case 3: finalWords[q] = "   " + w[3]; break;
                    }
                }
                foundWord = true;
                chosenPermutation = c;
                break;
            }
        }
        if (!foundWord)
        {
            attps++;
            attws++;
            goto retry;
        }
        quoted = Rnd.Range(0, 4);
        finalWords[quoted] = "“" + finalWords[quoted] + "”";
        string[][] wordsForLogging = {
            new string[] { "", "", "", "" },
            new string[] { "", "", "", "" }
        };
        for (int p = 0; p < 4; p++)
        {
            quedWords[p] = insertQ(p == quoted, tableWords[positions[p]], Array.IndexOf(chosenPermutation, p));
            wordsForLogging[0][p] = quedWords[p];
            wordsForLogging[1][p] = tableWords[positions[p]];
        }
        Debug.LogFormat("<Quote Crazy Talk End Quote #{0}> Failed attempts: {1} | {2}", moduleId, attps, attws);
        Debug.LogFormat("[Quote Crazy Talk End Quote #{0}] Displayed words: {1}", moduleId, wordsForLogging[0].Join(" "));
        Debug.LogFormat("[Quote Crazy Talk End Quote #{0}] Displayed with Q removed: {1}", moduleId, wordsForLogging[1].Join(" "));
        for (int e = 0; e < 4; e++)
        {
            int ep = positions[ctee[e]];
            string d = tableDigits[e][e % 2 == 0 ? ep % 10 : ep / 10].ToString();
            uncycledDigits[e] = d;
            correctDigits[(e + 4 - Array.IndexOf(ctee, quoted)) % 4] = d;
        }
        Debug.LogFormat("[Quote Crazy Talk End Quote #{0}] The corresponding digits, from north going clockwise: {1}", moduleId, uncycledDigits.Join(", "));
        Debug.LogFormat("[Quote Crazy Talk End Quote #{0}] The correct order of the digits, quotes going clockwise: {1}", moduleId, correctDigits.Join(", "));
        randDigits = uncycledDigits.Shuffle();
        UpdateScreens();
    }

    void UpdateScreens()
    {
        for (int f = 0; f < 4; f++)
        {
            Words[f].text = quedWords[f];
            Digits[f].text = randDigits[f];
        }
    }

    bool positionsValid(int[] a) //this is deceptively tricky, took a while to get to this point!
    {
        int[] xt = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; //these are TOTALS, if one of our 4 positions is at that x/y...
        int[] yt = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        for (int v = 0; v < 10; v++)
        {
            for (int e = 0; e < 4; e++)
            {
                if (a[e] % 10 == v) { xt[v] += 10 + e; } //...we increase the corresponding value,
                if (a[e] / 10 == v) { yt[v] += 10 + e; } //the "+e" is so we can retrieve the relevant edge later, the 10s digit is the count here
            }
        }

        int p = 1;
        int[] pf = { 2, 3, 5, 7 };
        int n;

        //i swear all of this stuff below isn't as scary as it looks
        //this bit is split into four sections, one for each edge-- starting from top going clockwise, i'll only comment on the first

        int g = 0; //this variable g is what i'm calling a glider, since we're dealing with the top, we start at y = 0...
        while (yt[g] == 0) { g++; } //...and decrease it until we find something in that row, here doing down the rows
        if (yt[g] > 19) //if the sum is higher than 19, that means two or more values were added together...
        {
            return false; //...which we cannot have, return false
        }
        else
        {
            n = yt[g] - 10; //otherwise we retrieve the relevant edge
            ctee[0] = n; //plop it into ctee
            p *= pf[n]; //and multiply a factor, to summarize, if this product ends up as NOT 210, that means an edge is nearest to two of our words
        }

        g = 9; //right
        while (xt[g] == 0) { g--; }
        if (xt[g] > 19)
        {
            return false;
        }
        else
        {
            n = xt[g] - 10;
            ctee[1] = n;
            p *= pf[n];
        }

        g = 9; //bottom
        while (yt[g] == 0) { g--; }
        if (yt[g] > 19)
        {
            return false;
        }
        else
        {
            n = yt[g] - 10;
            ctee[2] = n;
            p *= pf[n];
        }

        g = 0; //left
        while (xt[g] == 0) { g++; }
        if (xt[g] > 19)
        {
            return false;
        }
        else
        {
            n = xt[g] - 10;
            ctee[3] = n;
            p *= pf[n];
        }

        return p == 210;
    }

    void MainPress()
    {
        if (!moduleSolved)
        {
            MainScreen.AddInteractionPunch(0.5f);
            HighlightObj.SetActive(false);
            StartCoroutine(AnimScreens());
            started = true;
        }
    }

    void SmallPress(int s)
    {
        if (!pressed[s])
        {
            SmallScreens[s].AddInteractionPunch(0.5f);
            Audio.PlaySoundAtTransform("QCTEQ_shrink", SmallScreens[s].transform);
            pressed[s] = true;
            StartCoroutine(AnimText(s));
            submitted.Add(randDigits[s]);
            if (correctDigits[submitted.Count() - 1] != randDigits[s])
            {
                struck = true;
            }
            if (submitted.Count() == 4)
            {
                good = true;
            }
        }
    }

    private IEnumerator AnimScreens() //plz pr if you wanna optimize the everloving shit out of this
    {
        for (int s = 0; s < 4; s++)
        {
            SmallObjs[s].transform.localPosition = new Vector3(0f, 0f, 0f);
            SmallObjs[s].SetActive(true);
        }

        Audio.PlaySoundAtTransform("QCTEQ_woosh_small", MainScreen.transform);
        float elapsed = 0f;
        float duration = 0.25f;
        float distanceScale = 5.332f;
        while (elapsed < duration)
        {
            SmallObjs[0].transform.localPosition = new Vector3(0f, 0f, elapsed * distanceScale);
            SmallObjs[1].transform.localPosition = new Vector3(elapsed * distanceScale, 0f, 0f);
            SmallObjs[2].transform.localPosition = new Vector3(0f, 0f, elapsed * -distanceScale);
            SmallObjs[3].transform.localPosition = new Vector3(elapsed * -distanceScale, 0f, 0f);
            yield return null;
            elapsed += Time.deltaTime;
        }
        SmallObjs[0].transform.localPosition = new Vector3(0f, 0f, 1.333f);
        SmallObjs[1].transform.localPosition = new Vector3(1.333f, 0f, 0f);
        SmallObjs[2].transform.localPosition = new Vector3(0f, 0f, -1.333f);
        SmallObjs[3].transform.localPosition = new Vector3(-1.333f, 0f, 0f);

        elapsed = 0f;
        duration = TwitchPlaysActive ? 30f : 4f;
        while (elapsed < duration && !struck && !good)
        {
            yield return null;
            elapsed += Time.deltaTime;
        }
        
        if (good)
        {
            yield return new WaitForSeconds(0.2f);
        }

        Audio.PlaySoundAtTransform("QCTEQ_woosh_small", MainScreen.transform);
        elapsed = 0f;
        duration = 0.25f;
        while (elapsed < duration)
        {
            SmallObjs[0].transform.localPosition = new Vector3(0f, 0f, (duration - elapsed) * distanceScale);
            SmallObjs[1].transform.localPosition = new Vector3((duration - elapsed) * distanceScale, 0f, 0f);
            SmallObjs[2].transform.localPosition = new Vector3(0f, 0f, (duration - elapsed) * -distanceScale);
            SmallObjs[3].transform.localPosition = new Vector3((duration - elapsed) * -distanceScale, 0f, 0f);
            yield return null;
            elapsed += Time.deltaTime;
        }
        if (good)
        {
            Solve();
        }
        else
        {
            Strike();
            started = false;
        }
    }

    private IEnumerator AnimText(int h)
    {
        float elapsed = 0f;
        float duration = 0.2f;
        float scaleScale = 0.625f;
        while (elapsed < duration)
        {
            DigitObjs[h].transform.localScale = new Vector3((duration - elapsed) * scaleScale, (duration - elapsed) * scaleScale, 4f);
            yield return null;
            elapsed += Time.deltaTime;
        }
        Digits[h].text = "";
    }

    void Solve()
    {
        Module.HandlePass();
        Debug.LogFormat("[Quote Crazy Talk End Quote #{0}] Submitted {1}. That is correct, module solved.", moduleId, submitted.Join(", "));
        moduleSolved = true;
        StartCoroutine(SolveAnim());
    }

    private IEnumerator SolveAnim()
    {
        for (int t = 0; t < 4; t++)
        {
            yield return new WaitForSeconds(0.1f);
            for (int w = 0; w < 4; w++)
            {
                Audio.PlaySoundAtTransform("QCTEQ_key", MainScreen.transform);
                Words[w].text = finalWords[w].Substring(0, t + (w == quoted ? 2 : 1)) + quedWords[w].Substring(t + (w == quoted ? 2 : 1), (w == quoted ? 4 : 3) - t);
            }
        }
        yield return new WaitForSeconds(0.1f);
        Audio.PlaySoundAtTransform("QCTEQ_woosh_big", MainScreen.transform);
        float elapsed = 0f;
        float duration = 0.2f;
        while (elapsed < duration)
        {
            float along = (duration - elapsed) / duration * 0.4f;
            WordObjs[0].transform.localPosition = new Vector3(-along, 0.08f, along);
            WordObjs[1].transform.localPosition = new Vector3(along, 0.08f, along);
            WordObjs[2].transform.localPosition = new Vector3(-along, 0.08f, -along);
            WordObjs[3].transform.localPosition = new Vector3(along, 0.08f, -along);
            yield return null;
            elapsed += Time.deltaTime;
        }
        for (int w = 0; w < 4; w++)
        {
            WordObjs[w].transform.localPosition = new Vector3(0f, 0.08f, 0f);
        }
        elapsed = 0f;
        while (elapsed < duration)
        {
            for (int w = 0; w < 4; w++)
            {
                WordObjs[w].transform.localScale = new Vector3(Lerp(0.0225f, 0.04f, elapsed * 5), Lerp(0.0225f, 0.04f, elapsed * 5), 1f);
            }
            yield return null;
            elapsed += Time.deltaTime;
        }
    }

    void Strike()
    {
        Module.HandleStrike();
        Debug.LogFormat("[Quote Crazy Talk End Quote #{0}] Submitted {1}. That is incorrect, strike!", moduleId, submitted.Join(", "));
        HighlightObj.SetActive(true);
        struck = false;
        for (int b = 0; b < 4; b++)
        {
            pressed[b] = false;
            DigitObjs[b].transform.localScale = new Vector3(0.125f, 0.125f, 4f);
        }
        submitted.Clear();
        UpdateScreens();
    }

    IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length) //thanks to Quinn for this handy function
    {
        if (length == 1)
            return list.Select(t => new T[] { t }.Select(i => i));
        return GetPermutations(list, length - 1)
            .SelectMany(t => list.Where(e => !t.Contains(e)),
                (t1, t2) => t1.Concat(new T[] { t2 }));
    }

    string insertQ(bool qts, string str, int pos)
    {
        string qed = "";
        for (int x = 0; x < 4; x++)
        {
            qed += x == pos ? "Q" : str[x].ToString();
        }
        return qts ? "“" + qed + "”" : qed;
    }
    
    float Lerp(float a, float b, float t)
    { //this assumes t is in the range 0-1
        return a * (1f - t) + b * t;
    }

    // Twitch Plays Support by Kilo Bites

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} start [will initialize the module] || !{0} submit 1234 [presses the positions in that order (starting from the top going clockwise)] In Twitch Plays, the limited time is extended to 30 seconds.";
    private bool TwitchPlaysActive;
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        string[] split = command.ToUpperInvariant().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

        yield return null;

        if ("START".ContainsIgnoreCase(split[0]))
        {
            if (started)
            {
                yield return "sendtochaterror The module has already started!";
                yield break;
            }

            if (split.Length > 1)
                yield break;

            MainScreen.OnInteract();
            yield return new WaitForSeconds(0.1f);
            yield break;
        }

        if ("SUBMIT".ContainsIgnoreCase(split[0]))
        {
            if (!started)
            {
                yield return "sendtochaterror The module hasn't been started yet!";
                yield break;
            }

            if (split.Length == 1)
            {
                yield return "sendtochaterror Submit what?";
                yield break;
            }

            if (split.Length > 2)
            {
                yield return "sendtochaterror Too many parameters inputted. Please try again!";
                yield break;
            }

            if (split[1].Length != 4 || split[1].Distinct().Count() != 4)
            {
                yield return "sendtochaterror You must input the exact length of 4 and they must be unique!";
                yield break;
            }

            if (split[1].Select(x => int.Parse(x.ToString())).Any(x => x == -1 || !Enumerable.Range(0, 4).Contains(x - 1)))
            {
                yield return "sendtochaterror The numbers inputted are either invalid or not within the range of 1-4!";
                yield break;
            }

            foreach (var num in split[1].Select(x => int.Parse(x.ToString()) - 1))
            {
                SmallScreens[num].OnInteract();
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
    IEnumerator TwitchHandleForcedSolve()
    {
        if (!started)
        {
            MainScreen.OnInteract();
            yield return new WaitForSeconds(0.1f);
        }

        for (int i = 0; i < 4; i++)
        {
            SmallScreens[randDigits.Count(x => correctDigits[i] == x) > 1 ? Enumerable.Range(0, 4).Where(x => !pressed[x] && randDigits[x] == correctDigits[i]).PickRandom() : Array.IndexOf(randDigits, correctDigits[i])].OnInteract();
            yield return new WaitForSeconds(0.1f);
        }

    }
}
