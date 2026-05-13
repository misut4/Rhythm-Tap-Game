using System;
using UnityEngine;

public class GameConfig
{
    public float bpm = 148f;
    public float tileHeight = 300;
    public float tileSpeed;
    public int laneCount = 4;
    public float missY = -5.5f;
    public int initialPoolSize = 12;
    public float beatInterval;
    public float audioStartDelay = 0.5f;

    public GameConfig(float bpm, float tileSpeed, float tileHeight, int laneCount, float missY, int initialPoolSize, float beatInterval, float audioStartDelay)
    {
        this.bpm = bpm;
        this.tileHeight = tileHeight;
        this.laneCount = laneCount;
        this.missY = missY;
        this.initialPoolSize = initialPoolSize;
        this.beatInterval = beatInterval;
        this.tileSpeed = tileSpeed;
        this.audioStartDelay = audioStartDelay;
    }
}