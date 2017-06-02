using Assets.Classes;
using UnityEngine;

public abstract class Challenge : MonoBehaviour
{
    /// <summary>
    /// name of the challenge
    /// </summary>
    public string name;
    /// <summary>
    /// description of the challenge 
    /// </summary>
    public string description;
    /// <summary>
    /// standard false
    /// </summary>
    private bool _completed;

    public void CompleteChallenge()
    {
        _completed = true;
    }
}

