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

    public GameConfig(float bpm, float heightScalingFactor, float tileHeight, int laneCount, float missY, int initialPoolSize)
    {
        this.bpm = bpm;
        this.tileHeight = tileHeight;
        this.laneCount = laneCount;
        this.missY = missY;
        this.initialPoolSize = initialPoolSize;
        this.beatInterval = 60f / bpm;
        this.tileSpeed = tileHeight * heightScalingFactor / beatInterval; // tile speed is calculated based on the tile height with canvas scaling and the height scaling factor
    }
}