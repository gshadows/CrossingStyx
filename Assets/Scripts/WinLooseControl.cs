using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinLooseControl : MonoBehaviour
{
    public enum GameStage { PLAY, WIN, LOOSE }
    public enum LooseReason { NONE, BOAT_SANK, PLAYER_DROWN }

    [Range(0f, 10f)] public float sankLooseDelay = 5f; // Delay after boat sank before game over.

    private FloatBoat boat;

    public GameStage gameStage { get; private set; } = GameStage.PLAY;
    public LooseReason looseReason { get; private set; } = LooseReason.NONE;


    void Start()
    {
        boat = GameObject.FindObjectOfType<FloatBoat>();
        boat.onSink += Boat_onSink;
    }


    private void Boat_onSink() {
        StartCoroutine("GameOverSank");
    }

    IEnumerable GameOverSank() {
        yield return new WaitForSeconds(sankLooseDelay);
        gameStage = GameStage.LOOSE;
        looseReason = LooseReason.BOAT_SANK;
        UIManager.showGameOver();
        Debug.Log("LOOSE: BOAT SANK!");
    }


    public bool isPlaying() {
        return gameStage == GameStage.PLAY;
    }


    public void onBoatArrives(bool clean) {
        gameStage = GameStage.WIN;
        Debug.Log("WIN!!!");
    }
}
