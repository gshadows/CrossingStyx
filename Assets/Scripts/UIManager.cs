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

    // Timings.
    [Range(0, 5)] public int fadeSeconds = 2;
    [Range(0, 10)] public int secondsToEndGame = 5;
    [Range(0, 10)] public int secondsIntroStay = 5;

    // Miscelaneous.
    public Color gameOverColor = Color.red;
    public Color gameWinColor = Color.green;
    public Color gameChapterColoir = Color.gray;


    private Fade fade = Fade.NONE;
    private float fadeStartTime;


    void Start() {
        instance = this;
        showMainMenuImmediate(false); // Show start menu.
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
        Debug.Log("START FADE: " + fade);
        this.fade = fade;
        fadeStartTime = Time.time;
    }


    public void hideEverything() {
        Debug.Log("HIDE EVERYTHING");
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
        Debug.Log("END GAME: Show Message");
        hideEverything();
        darkPanel.gameObject.SetActive(true);
        mainMessage.gameObject.SetActive(true);
        startFade(Fade.IN);

        // Setup text and color.
        if (GameControl.instance.gameStage == GameControl.GameStage.WIN) {
            mainMessage.color = gameWinColor;
            mainMessage.text = "You Win!";
        } else if (GameControl.instance.gameStage == GameControl.GameStage.WIN) {
            mainMessage.color = gameOverColor;
            mainMessage.text = "Game Over";
        } else {
            mainMessage.color = Color.red;
            mainMessage.text = "WTF: " + GameControl.instance.gameStage;
        }

        yield return new WaitForSeconds(secondsToEndGame);
        Debug.Log("END GAME: Open Menu Delayed");
        GameControl.instance.openMenu();
    }


    public void showIntroScreen() {
        StartCoroutine("introSequence");
    }
    private IEnumerator introSequence() {
        hideEverything();
        Debug.Log("INTRO: Show");
        darkPanel.gameObject.SetActive(true);
        mainMessage.gameObject.SetActive(true);
        mainMessage.color = gameChapterColoir;
        mainMessage.text = "Chapter III";
        secondMessage.gameObject.SetActive(true);
        secondMessage.color = gameChapterColoir;
        mainMessage.text = "Crossing the Styx";

        yield return new WaitForSeconds(secondsIntroStay);
        Debug.Log("INTRO: Delayed Fade");
        startFade(Fade.OUT);
        yield return new WaitForSeconds(fadeSeconds);
        Debug.Log("INTRO: Starting Game");
        GameControl.instance.startGame();
    }


    public void showMainMenu(bool inGame) {
        playButtonText.text = inGame ? "Continue" : "Play";
        StartCoroutine("showMenuSequence");
    }
    private IEnumerator showMenuSequence() {
        hideEverything();
        Debug.Log("MENU: Start Delayed");
        startFade(Fade.IN);
        darkPanel.gameObject.SetActive(true);

        yield return new WaitForSeconds(fadeSeconds);
        Debug.Log("MENU: Show Delayed");
        mainMenu.SetActive(true);
    }


    private void showMainMenuImmediate(bool inGame) {
        hideEverything();
        Debug.Log("MENU: Show Immediate");
        showMouse(true);
        playButtonText.text = inGame ? "Continue" : "Play";
        darkPanel.gameObject.SetActive(true);
        mainMenu.SetActive(true);
    }


    public void play() {
        Debug.Log("MENU: Play");
        GameControl.instance.startGame();
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
