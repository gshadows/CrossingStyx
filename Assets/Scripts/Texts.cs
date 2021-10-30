using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Texts {

    public const int LANG_EN = 0;
    public const int LANG_RU = 1;

    private static int lang;


    public const int YOU_WIN = 0;
    public const int GAME_OVER = 1;
    public const int LOOSE_BOAT_SANK = 2;
    public const int LOOSE_OVERBOARD = 3;
    public const int UNKNOWN_REASON = 4;
    public const int CHAPTER = 5;
    public const int GAME_TITLE = 6;
    public const int CONTINUE = 7;
    public const int PLAY = 8;
    public const int QUIT = 9;
    public const int WIN_NORMAL = 10;
    public const int WIN_LAST_MOMENT = 11;
    public const int PART1_TITLE = 12;
    public const int PART2_TITLE = 13;
    public const int PART3_TITLE = 14;

    private static readonly Dictionary<int, string[]> translations = new Dictionary<int, string[]> {
        { YOU_WIN, new string[]{ "You Win!", "Победа!" } },
        { GAME_OVER, new string[]{ "Game Over", "Game Over" } },
        { LOOSE_BOAT_SANK, new string[]{ "You sank Charon's boat! He extremly disappointed!", "Вы утопили лодку Харона! Он крайне разочарован!" } },
        { LOOSE_OVERBOARD, new string[]{ "You fell overboard! What a shame!", "Вы выпали за борт. Это фейл!" } },
        { UNKNOWN_REASON, new string[]{ "But no idea why...", "Знать бы ещё почему..." } },
        { CHAPTER, new string[]{ "Chapter", "Часть" } },
        { GAME_TITLE, new string[]{ "Crossing the Styx", "Crossing the Styx" } },
        { CONTINUE, new string[]{ "Continue", "Продолжить" } },
        { PLAY, new string[]{ "Play", "Играть" } },
        { QUIT, new string[]{ "Quit", "Выход" } },
        { WIN_NORMAL, new string[]{ "Good Job!", "Молодец!" } },
        { WIN_LAST_MOMENT, new string[]{ "At last moment!", "В последний момент, однако!" } },
        { PART1_TITLE, new string[]{ "To the last journey...", "В последний путь..." } },
        { PART2_TITLE, new string[]{ "Bying the ticket", "Оплата за проезд" } },
        { PART3_TITLE, new string[]{ "Crossing the Styx", "Переплывая Стикс" } },
    };



    public static string get(int textId) {
        return translations[textId][lang];
    }


    public static setLanguage (int newLanguage) {
        lang = newLanguage;
    }


    static Texts() {
        SystemLanguage sysLang = Application.systemLanguage;
        Debug.LogFormat("System Language: {0}", sysLang);
        switch (sysLang) {
            case SystemLanguage.Russian: lang = LANG_RU; break;
            default: lang = LANG_EN; break;
        }
    }

}

