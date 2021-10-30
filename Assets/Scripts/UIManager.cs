using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public Text quitButtonText; // Quit button.

    // Timings.
    [Range(0, 5)] public int fadeSeconds = 2;
    [Range(0, 10)] public int secondsToEndGame = 5;
    [Range(0, 10)] public int secondsIntroStay = 3;

    // Miscelaneous.
    public Color gameOverColor = Color.red;
    public Color gameWinColor = Color.green;
    public Color gameChapterColoir = Color.gray;


    private Fade fade = Fade.NONE;
    private float fadeStartTime;


    void Start() {
        instance = this;
        staticTranslations();
        showMainMenuImmediate(false);
    }

    private void staticTranslations() {
        quitButtonText.text = Texts.get(Texts.QUIT);
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
            mainMessage.text = Texts.get(Texts.YOU_WIN);
            switch (GameControl.instance.winReason) {
                case GameControl.WinReason.NORMAL:
                    secondMessage.text = Texts.get(Texts.WIN_NORMAL);
                    break;
                case GameControl.WinReason.LAST_MOMENT:
                    secondMessage.text = Texts.get(Texts.WIN_LAST_MOMENT);
                    break;
                default:
                    secondMessage.text = Texts.get(Texts.UNKNOWN_REASON);
                    break;
            }
        } else if (GameControl.instance.gameStage == GameControl.GameStage.LOOSE) {
            mainMessage.color = secondMessage.color = gameOverColor;
            mainMessage.text = Texts.get(Texts.GAME_OVER);
            switch (GameControl.instance.looseReason) {
                case GameControl.LooseReason.BOAT_SANK:
                    secondMessage.text = Texts.get(Texts.LOOSE_BOAT_SANK);
                    break;
                case GameControl.LooseReason.PLAYER_DROWN:
                    secondMessage.text = Texts.get(Texts.LOOSE_OVERBOARD);
                    break;
                default:
                    secondMessage.text = Texts.get(Texts.UNKNOWN_REASON);
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
        mainMessage.text = Texts.get(Texts.CHAPTER) + " III";
        secondMessage.gameObject.SetActive(true);
        secondMessage.color = gameChapterColoir;
        secondMessage.text = Texts.get(Texts.PART3_TITLE);

        yield return new WaitForSeconds(secondsIntroStay);
        //Debug.Log("INTRO: Delayed Fade");
        startFade(Fade.OUT);
        yield return new WaitForSeconds(fadeSeconds);
        //Debug.Log("INTRO: Starting Game");
        GameControl.instance.startGame();
    }


    public void showMainMenu() {
        showMainMenuImmediate(GameControl.instance.gameStage == GameControl.GameStage.PAUSE);
        if (GameControl.instance.gameStage == GameControl.GameStage.PLAY) {
            startFade(Fade.IN);
        }
    }

    private void showMainMenuImmediate(bool isPaused) {
        //Debug.Log("MAIN MENU");
        hideEverything();
        darkPanel.gameObject.SetActive(true);
        playButtonText.text = isPaused ? Texts.get(Texts.CONTINUE) : Texts.get(Texts.PLAY);
        mainMenu.SetActive(true);
        showMouse(true);
    }


    public void play() {
        Debug.Log("MENU: Play");
        showIntroScreen();
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
