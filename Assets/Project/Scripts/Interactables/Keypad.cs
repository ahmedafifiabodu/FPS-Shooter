using UnityEngine;

public class Keypad : Interactable
{
    protected override void Interact()
    {
        Debug.Log("(Override) Interacting with " + gameObject.name);
    }
}