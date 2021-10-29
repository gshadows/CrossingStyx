using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MyBehaviour {
    public enum GameStage { MENU, PLAY, WIN, LOOSE }
    public enum LooseReason { NONE, BOAT_SANK, PLAYER_DROWN }

    [Range(0f, 10f)] public float sankLooseDelay = 5f; // Delay after boat sank before game over.

    public AudioClip gameOver;
    public AudioClip gameWin;

    private FloatBoat boat;

    public GameStage gameStage { get; private set; } = GameStage.MENU;
    public LooseReason looseReason { get; private set; } = LooseReason.NONE;

    public static GameControl instance;


    void Start()
    {
        instance = this;
        boat = GameObject.FindObjectOfType<FloatBoat>();
        boat.onSink += Boat_onSink;
    }


    public void Update() {
        if ((gameStage != GameStage.MENU) && Input.GetButtonUp("Cancel")) {
            openMenu();
            return;
        }
    }


    private void Boat_onSink() {
        StartCoroutine("gameOverSank");
    }

    IEnumerable gameOverSank() {
        yield return new WaitForSeconds(sankLooseDelay);
        gameStage = GameStage.LOOSE;
        looseReason = LooseReason.BOAT_SANK;
        UIManager.instance.showEndGame();
        sound(gameOver);
    }


    public bool isPlaying() {
        return gameStage == GameStage.PLAY;
    }


    public void onBoatArrives(bool clean) {
        gameStage = GameStage.WIN;
        UIManager.instance.showEndGame();
        sound(gameWin);
    }


    public void startGame() {
        gameStage = GameStage.PLAY;
        UIManager.instance.hideEverything();
        showMouse(false);
    }


    public void openMenu() {
        showMouse(true);
        UIManager.instance.showMainMenu(gameStage == GameStage.PLAY);
        gameStage = GameStage.MENU;
    }
}
