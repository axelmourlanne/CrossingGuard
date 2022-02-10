using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{

    public ControlStation headquarters;

    private void OnMouseDown()
    {
        this.headquarters.StartMission();
    }

    public void Start()
    {
        this.headquarters = GameObject.Find("Headquarters").GetComponent<ControlStation>();    
    }

}
