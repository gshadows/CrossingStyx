using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    
    
    void Start()
    {
        instance = this;
    }


    public static void showGameOver() {
    }


    public static void showWin() {
    }


    public static void showIntroScreen() {
    }
}
