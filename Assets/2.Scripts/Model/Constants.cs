using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants
{
    // SCENE NAME
    public static readonly string SCENE_NAME_MAIN_GAME = "MainScene";

    // TAG
    public static readonly string TAG_TILE = "Tile";
    public static readonly string TAG_HIT_AREA = "HitArea";

    // SCORE
    public static readonly float GOOD_HIT_WINDOW = 0.7f;
    public static readonly float GREAT_HIT_WINDOW = 0.5f;
    public static readonly float PERFECT_HIT_WINDOW = 0.3f;

    public static readonly int GOOD_SCORE = 100;
    public static readonly int GREAT_SCORE = 200;
    public static readonly int PERFECT_SCORE = 300;
}

public enum ComboType
{
    None,
    Good,
    Great,
    Perfect
}

public enum GameState
{
    Idle,
    Playing,
    Won,
    GameOver
}
