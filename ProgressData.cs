using System;

[Serializable]
public class ProgressData
{
    public float BestTimeInSeconds = float.MaxValue;
    public bool[] UnlockedLevels = new bool[4]; // počet levelů
}
