using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants
{

    //PLAYER PREF KEYS
    public static string KEY_FIRST_LAUNCH = "key_first_launch"; //(0 = yes, 1 = no)
    public static string KEY_LAST_LEVEL = "key_last_level";
    public static string KEY_SOUND = "key_sound"; //(0 = off, 1 = on)
    public static string KEY_COLOR = "key_color"; //(0 = white, 1 = black)
    public static string KEY_TUTORIAL_STATE = "key_tutorial_state"; //(0 = none, 1 = level, 2 = level 1 pawn, 3 = target ...)
    public static string KEY_TUTORIAL_STATE_CSV = "key_tutorials_state"; //(0 = none, 1 = level, 2 = level 1 pawn, 3 = target ...)
    public static string KEY_LEVEL_STARS_CSV = "key_stars_csv";

    //GAME TARGETS
    public static int GOAL_POINTS_MOVES = 10;
    public static int GOAL_POINTS_CHANGES = 50;

    //FIREBASE ANALYTICS EVENTS 
    public static string EVENT_LEVEL_UP = "EVENT_LEVEL_UP";
    public static string PARAM_START_TYPE = "PARAM_MUSIC"; // "START", "REPLAY", "NEXT"
    public static string PARAM_LEVEL = "PARAM_LEVEL";
    public static string PARAM_PIECE_COLOR = "PARAM_PIECE_COLOR";
    public static string PARAM_SOUND_STATUS = "PARAM_MUSIC";

    public static string TYPE_START = "START";
    public static string TYPE_RESTART = "RESTART";
    public static string TYPE_NEXT = "RESTART";
    public static string TYPE_WHITE = "WHITE";
    public static string TYPE_BLACK = "BLACK";
    public static string TYPE_ON = "ON";
    public static string TYPE_OFF = "OFF";

    //ADS
    public static string ANDROID_AD_APP_ID = "ca-app-pub-6252100624852887~2006472155";
    public static string LIVE_INTERSTITIAL_ID_ANDROID = "ca-app-pub-6252100624852887/8388865912";
    public static string LIVE_INTERSTITIAL_ID_IOS = "ca-app-pub-";
    public static string LIVE_BANNER_ID_ANDROID = "ca-app-pub-6252100624852887/7394530209";
    public static string LIVE_BANNER_ID_IOS = "ca-app-pub-";
    public static string DEVELOPMENT_INTERSTITIAL_ID_ANDROID = "ca-app-pub-3940256099942544/1033173712";
    public static string DEVELOPMENT_INTERSTITIAL_ID_IOS = "ca-app-pub-3940256099942544/4411468910";
    public static string DEVELOPMENT_BANNER_ID_ANDROID = "ca-app-pub-3940256099942544/6300978111";
    public static string DEVELOPMENT_BANNER_ID_IOS = "ca-app-pub-3940256099942544/2934735716";

    //PLATFORMS
    public static int CURRENT_PLATFORM = PLATFORM_ANDROID;
    public static int PLATFORM_ANDROID = 0;
    public static int PLATFORM_IOS = 1;


    //RAM
    public static int RAM_HIGH = 4000; // in megabytes
    public static int RAM_MEDIUM = 3000; // in megabytes
    public static int RAM_LOW_ACCEPTED = 2500; // in megabytes

    //movement accuracy
    public const int ACCURECY_FRONT_CAMERA = 9;
    public const int ACCURECY_TOP_CAMERA = 10;

    //COLORS
    public static Color DEFAULT_ALERT_BG_COLOR = new Color(0.6f, 0.6f, 0.6f, 0.6f);
    public static Color DEFAULT_ALERT_TXT_COLOR = new Color(1f, 1f, 1f, 1f);
    public static Color MATRIX_BOX_NEXT_COLOR = new Color(0f, 0.55f, 0.5f, 1f);
    public static Color MATRIX_BOX_START_COLOR = new Color(0.2f, 0.1f, 0.4f, 1f);
    public static Color MATRIX_BOX_START_NEXT_COLOR = new Color(0.15f, 0.35f, 0.45f, 1f);
    public static Color MATRIX_BOX_FINISH_COLOR = new Color(0.06f, 0.3f, 0.06f, 1f);
    public static Color MATRIX_BOX_FINISH_NEXT_COLOR = new Color(0f, 0.4f, 0.25f, 1f);
    public static Color MATRIX_BOX_NEW_PIECE_COLOR = new Color(0.3f, 0.6f, 0.9f, 1f);
    public static Color MATRIX_BLOCK_COLOR_01 = new Color(0.1f, 0.1f, 0.3f, 1f);
    public static Color MATRIX_BLOCK_COLOR_02 = new Color(0.7f, 0.7f, 1f, 1f);
    public static Color MATRIX_PILLAR_COLOR_01 = new Color(0f, 0f, 0f, 1f);
    public static Color MATRIX_PILLAR_COLOR_02_OPAQUE = new Color(0.5f, 0.5f, 0.5f, 1f);
    public static Color MATRIX_PILLAR_COLOR_02 = new Color(1f, 1f, 1f, 0.25f);
    public static Color MATRIX_BOX_WHITE_COLOR = new Color(1f, 1f, 1f, 1f);
    public static Color MATRIX_BOX_BLACK_COLOR = new Color(0f, 0f, 0f, 1f);
    public static Color ARROW_ENABLED_COLOR = new Color(1f, 1f, 1f, 1f);
    public static Color ARROW_DISABLED_COLOR = new Color(0.9f, 0.9f, 0.9f, 0.9f);
    public static Color POWERUP_DOUBLE_FULL_COLOR = new Color(0.9f, 0.8f, 0.4f, 1f);
    public static Color POWERUP_STRONG_COLOR = new Color(0.8f, 0.6f, 0.6f, 1f);
    public static Color POWERUP_DIZZY_COLOR = new Color(0.1f, 0.3f, 0.0f, 1f);
}
