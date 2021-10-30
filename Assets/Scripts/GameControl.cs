using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MyBehaviour {
    public enum GameStage { MENU, PAUSE, PLAY, WIN, LOOSE }
    public enum LooseReason { NONE, BOAT_SANK, PLAYER_DROWN }

    [Range(0f, 10f)] public float sankLooseDelay = 5f; // Delay after boat sank before game over.

    public AudioClip gameOver;
    public AudioClip gameWin;

    private FloatBoat boat;

    [ReadOnly] public GameStage gameStage = GameStage.MENU;
    [ReadOnly] public LooseReason looseReason = LooseReason.NONE;

    public static GameControl instance;


    void Start()
    {
        instance = this;
        boat = GameObject.FindObjectOfType<FloatBoat>();
        boat.onSink += Boat_onSink;
    }


    public void Update() {
#if UNITY_EDITOR
        if (!isMenu() && Input.GetButtonUp("Jump")) {
#else
        if (!isMenu() && Input.GetButtonUp("Cancel")) {
#endif
            openMenu();
            return;
        }
    }


    private void Boat_onSink() {
        StartCoroutine("gameOverSank");
    }

    IEnumerator gameOverSank() {
        yield return new WaitForSeconds(sankLooseDelay);
        gameStage = GameStage.LOOSE;
        looseReason = LooseReason.BOAT_SANK;
        UIManager.instance.showEndGame();
        sound(gameOver);
    }


    public bool isPlaying() {
        return gameStage == GameStage.PLAY;
    }

    public bool isGameStarted() {
        return (gameStage != GameStage.MENU);
    }

    public bool isMenu() {
        return (gameStage == GameStage.MENU) || (gameStage == GameStage.PAUSE);
    }


    public void onBoatArrives(bool clean) {
        gameStage = GameStage.WIN;
        UIManager.instance.showEndGame();
        sound(gameWin);
    }


    public void startGame() {
        if (gameStage != GameStage.PAUSE) {
            globalBroadcast("onRestart");
        }
        gameStage = GameStage.PLAY;
        UIManager.instance.hideEverything();
        showMouse(false);
    }


    public void openMenu() {
        UIManager.instance.showMainMenu();
        gameStage = (gameStage == GameStage.PLAY) ? GameStage.PAUSE : GameStage.MENU;
    }
}
