using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{

    public ControlStation headquarters;

    private void OnMouseDown()
    {
        this.headquarters.StartMission(int.Parse(this.tag));
    }

    public void Start()
    {
        this.headquarters = GameObject.Find("Headquarters").GetComponent<ControlStation>(); 
    }

}
