using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{

    public ControlStation headquarters;


    public void Start()
    {
        this.headquarters = GameObject.Find("Headquarters").GetComponent<ControlStation>(); 
    }


    /*
    Method called when a button on a terminal is pressed.
    Calls the method StartMission from the headquarters with the tag of the button in order to request crossing guards.
    */
    public void OnMouseDown()
    {
        this.headquarters.StartMission(int.Parse(this.tag));
    }

}
