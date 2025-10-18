using UnityEngine;

public class Note : MonoBehaviour
{
    //Use to leave comments in the inspector for when leaving a project for a long time

    public string ToDo;
    public string For;

    public enum urgency {NotVery, aLittle, aLot, URGENT};
    public urgency HowImportant;
    public enum dropdown {PastJoe};
    public dropdown SincerlyFrom;
}
