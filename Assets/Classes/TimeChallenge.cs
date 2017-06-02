using Assets.Classes;
using UnityEngine;

public class TimeChallenge : Challenge {
    /// <summary>
    /// time of the challenge in seconds
    /// </summary>
    private int _time;

    public int GetTime()
    {
        return _time;
    }

}
