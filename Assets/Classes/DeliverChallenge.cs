using UnityEngine;

public class DeliverChallenge : Challenge{
    private string _item;

    public DeliverChallenge(string name, string description, Vector3 startLocation, Vector3 location, string item) : base(name, description, startLocation, location)
    {
        _item = item;
    }

    public string GetItem()
    {
        return _item;
    }
}
