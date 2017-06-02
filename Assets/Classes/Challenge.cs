using Assets.Classes;
using UnityEngine;

public abstract class Challenge : MonoBehaviour
{
    /// <summary>
    /// name of the challenge
    /// </summary>
    private string _name;
    /// <summary>
    /// description of the challenge 
    /// </summary>
    private string _description;
    /// <summary>
    /// start location of challenge
    /// </summary>
    private Location _startLocation;
    /// <summary>
    /// location where the location takes place
    /// </summary>
    private Location _location;
    /// <summary>
    /// standard false
    /// </summary>
    private bool _completed;

    public void CompleteChallenge()
    {
        _completed = true;
    }
}

