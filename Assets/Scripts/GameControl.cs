using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MyBehaviour {
    public enum GameStage { MENU, PAUSE, PLAY, WIN, LOOSE }
    public enum LooseReason { BOAT_SANK, PLAYER_DROWN }
    public enum WinReason { NORMAL, LAST_MOMENT }

    [Range(0f, 10f)] public float sankLooseDelay = 5f; // Delay after boat sank before game over.

    public AudioClip gameOver;
    public AudioClip gameWin;

    private FloatBoat boat;

    [ReadOnly] public GameStage gameStage = GameStage.MENU;
    [ReadOnly] public LooseReason looseReason;
    [ReadOnly] public WinReason winReason;

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
        soundGlobal(gameOver);
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
        winReason = clean ? WinReason.NORMAL : WinReason.LAST_MOMENT;
        UIManager.instance.showEndGame();
        soundGlobal(gameWin);
    }


    public void startGame() {
        gameStage = GameStage.PLAY;
        showMouse(false);
    }


    public void prepareToStartGame() {
        if (gameStage != GameStage.PAUSE) {
            globalBroadcast("onRestart");
        }
        boat.switchWaterSound(true);
    }


    public void openMenu() {
        UIManager.instance.showMainMenu();
        gameStage = (gameStage == GameStage.PLAY) ? GameStage.PAUSE : GameStage.MENU;
        boat.switchWaterSound(false);
    }
}
