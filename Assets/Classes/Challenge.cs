using UnityEngine;

public abstract class Challenge
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
    private Vector3 _startLocation;
    /// <summary>
    /// location where the location takes place
    /// </summary>
    private Vector3 _location;
    /// <summary>
    /// standard false
    /// </summary>
    private bool _completed;


    /// <summary>
    /// Constructor of the challenge class
    /// </summary>
    /// <param name="name"></param>
    /// <param name="description"></param>
    public Challenge(string name, string description, Vector3 startLocation, Vector3 location)
    {
        _name = name;
        _description = description;
        _startLocation = startLocation;
        _location = location;
        _completed = false;
    }

    public void CompleteChallenge()
    {
        _completed = true;
    }
}

