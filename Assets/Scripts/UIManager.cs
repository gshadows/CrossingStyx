using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class UIManager : MyBehaviour {
    private enum Fade { NONE, IN, OUT }

    public static UIManager instance;

    // UI backgrounds.
    public Image darkPanel;

    // Full screen notifications.
    public Text mainMessage;
    public Text secondMessage;
    public Image introImage;

    // Menus.
    public GameObject mainMenu;
    public Text playButtonText; // Play button, switching between "Play" and "Continue".
    public Text versionText;

    // Timings.
    [Range(0, 5)] public int fadeSeconds = 2;
    [Range(0, 10)] public int secondsToEndGame = 5;
    [Range(0, 10)] public int secondsIntroStay = 3;

    // Miscelaneous.
    public Color gameOverColor = Color.red;
    public Color gameWinColor = Color.green;
    public Color gameChapterColoir = Color.gray;

    [Header("Localization")]
    public LocalizedString playButtonString;
    public LocalizedString continueButtonString;
    public LocalizedString youWinString;
    public LocalizedString youLooseString;
    public LocalizedString winCleanString;
    public LocalizedString winLastMomentString;
    public LocalizedString looseReasonSankString;
    public LocalizedString looseReaseonOverboardString;
    public LocalizedString chapterString;
    public LocalizedString chapterTitleString;
    public LocalizedString unknownReasonString;


    private Fade fade = Fade.NONE;
    private float fadeStartTime;


    void Start() {
        instance = this;
        versionText.text = "v" + Application.version;
        showMainMenuImmediate(false);
    }

    private void Update() {
        switch (fade) {
            case Fade.IN:
                darkPanel.color = Color.Lerp(Color.clear, Color.black, (Time.time - fadeStartTime) / fadeSeconds);
                break;
            case Fade.OUT:
                darkPanel.color = Color.Lerp(Color.black, Color.clear, (Time.time - fadeStartTime) / fadeSeconds);
                break;
        }
    }


    private void startFade(Fade fade) {
        //Debug.Log("START FADE: " + fade);
        this.fade = fade;
        fadeStartTime = Time.time;
        switch (fade) {
            case Fade.IN:
                darkPanel.color = Color.clear;
                break;
            case Fade.OUT:
                darkPanel.color = Color.black;
                break;
        }
        darkPanel.gameObject.SetActive(true);
    }


    public void hideEverything() {
        darkPanel.gameObject.SetActive(false);
        mainMessage.gameObject.SetActive(false);
        secondMessage.gameObject.SetActive(false);
        introImage.gameObject.SetActive(false);
        mainMenu.SetActive(false);
    }


    public void showEndGame() {
        StartCoroutine("endGameSequence");
    }
    private IEnumerator endGameSequence() {
        //Debug.Log("END GAME: Show Message");
        hideEverything();
        mainMessage.gameObject.SetActive(true);
        secondMessage.gameObject.SetActive(true);
        startFade(Fade.IN);

        // Setup text and color.
        if (GameControl.instance.gameStage == GameControl.GameStage.WIN) {
            mainMessage.color = gameWinColor;
            mainMessage.text = youWinString.GetLocalizedString();
            switch (GameControl.instance.winReason) {
                case GameControl.WinReason.NORMAL:
                    secondMessage.text = winCleanString.GetLocalizedString();
                    break;
                case GameControl.WinReason.LAST_MOMENT:
                    secondMessage.text = winLastMomentString.GetLocalizedString();
                    break;
                default:
                    secondMessage.text = unknownReasonString.GetLocalizedString();
                    break;
            }
        } else if (GameControl.instance.gameStage == GameControl.GameStage.LOOSE) {
            mainMessage.color = secondMessage.color = gameOverColor;
            mainMessage.text = youLooseString.GetLocalizedString();
            switch (GameControl.instance.looseReason) {
                case GameControl.LooseReason.BOAT_SANK:
                    secondMessage.text = looseReasonSankString.GetLocalizedString();
                    break;
                case GameControl.LooseReason.PLAYER_DROWN:
                    secondMessage.text = looseReaseonOverboardString.GetLocalizedString();
                    break;
                default:
                    secondMessage.text = unknownReasonString.GetLocalizedString();
                    break;
            }
        } else {
            mainMessage.color = Color.red;
            mainMessage.text = "WTF: " + GameControl.instance.gameStage;
        }

        yield return new WaitForSeconds(secondsToEndGame);
        //Debug.Log("END GAME: Open Menu Delayed");
        GameControl.instance.openMenu();
    }


    public void showIntroScreen() {
        StartCoroutine("introSequence");
    }
    private IEnumerator introSequence() {
        hideEverything();
        showMouse(false);
        //Debug.Log("INTRO: Show");
        darkPanel.gameObject.SetActive(true);
        mainMessage.gameObject.SetActive(true);
        mainMessage.color = gameChapterColoir;
        string str = "???";
        try {
            Debug.Log("Chapter num looking...");
            str = chapterString.GetLocalizedString(new string[]{"3"});
            Debug.Log("Chapter num look END");
        }
        catch (Exception ex) {
            Debug.LogError("Fail: " + ex);
        }
        Debug.LogFormat("Chapter num localized STR: [{0}]", str);
        mainMessage.text = SafeLocalizedStr(chapterString, "", "3");
        Debug.LogFormat("Chapter num localized: [{0}]", mainMessage.text);
        secondMessage.gameObject.SetActive(true);
        secondMessage.color = gameChapterColoir;
        secondMessage.text = chapterTitleString.GetLocalizedString();
        introImage.gameObject.SetActive(true);

        GameControl.instance.prepareToStartGame();

        yield return new WaitForSeconds(secondsIntroStay);
        //Debug.Log("INTRO: Delayed Fade");
        startFade(Fade.OUT);
        yield return new WaitForSeconds(fadeSeconds + 1f);
        //Debug.Log("INTRO: Starting Game");
        hideEverything();
        GameControl.instance.startGame();
    }


    public void showMainMenu() {
        showMainMenuImmediate(GameControl.instance.isPlaying());
        if (GameControl.instance.isPlaying()) {
            startFade(Fade.IN);
        }
    }
    
    private void showMainMenuImmediate(bool isPaused) {
        //Debug.Log("MAIN MENU");
        hideEverything();
        darkPanel.gameObject.SetActive(true);
        LocalizedString buttonText = isPaused ? continueButtonString : playButtonString;
        playButtonText.text = buttonText.GetLocalizedString();
        mainMenu.SetActive(true);
        showMouse(true);
    }


    public void play() {
        Debug.Log("MENU: Play");
        if (GameControl.instance.isGameStarted()) {
            hideEverything();
            darkPanel.gameObject.SetActive(true);
            startFade(Fade.OUT);
            GameControl.instance.startGame();
        } else {
            showIntroScreen();
        }
    }

    public void quit() {
        Debug.Log("MENU: Quit");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
