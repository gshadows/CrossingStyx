using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Texts {

    private const int LANG_EN = 0;
    private const int LANG_RU = 1;

    private static int lang;


    public const int YOU_WIN = 0;
    public const int GAME_OVER = 1;
    public const int LOOSE_BOAT_SANK = 2;
    public const int LOOSE_OVERBOARD = 3;
    public const int LOOSE_UNKNOWN = 4;
    public const int CHAPTER = 5;
    public const int GAME_TITLE = 6;
    public const int CONTINUE = 7;
    public const int PLAY = 8;
    public const int QUIT = 9;

    private static readonly Dictionary<int, string[]> translations = new Dictionary<int, string[]> {
        { YOU_WIN, new string[]{ "You Win!", "������!" } },
        { GAME_OVER, new string[]{ "Game Over", "Game Over" } },
        { LOOSE_BOAT_SANK, new string[]{ "You sank Charon's boat! He extremly disappointed!", "�� ������ ����� ������! �� ������ �����������!" } },
        { LOOSE_OVERBOARD, new string[]{ "You fell overboard! What a shame!", "�� ����� �� ����. ��� ����!" } },
        { LOOSE_UNKNOWN, new string[]{ "No idea why though...", "����� �� ��� ������..." } },
        { CHAPTER, new string[]{ "Chapter", "�����" } },
        { GAME_TITLE, new string[]{ "Crossing the Styx", "Crossing the Styx" } },
        { CONTINUE, new string[]{ "Continue", "����������" } },
        { PLAY, new string[]{ "Play", "������" } },
        { QUIT, new string[]{ "Quit", "�����" } },
    };



    public static string get(int textId) {
        return translations[textId][lang];
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

