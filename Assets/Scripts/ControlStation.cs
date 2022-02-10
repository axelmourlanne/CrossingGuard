using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlStation : MonoBehaviour
{

    public List<Drone> drones = new List<Drone>();
    public List<Drone> dronesInMission = new List<Drone>();
    public int numbersOfDronesAvailable;
    public List<GameObject> spots = new List<GameObject>();
    public int range = 100;
    
    // Start is called before the first frame update
    void Start()
    {
        foreach(Drone drone in FindObjectsOfType(typeof(Drone)) as Drone[])
        {
            this.drones.Add(drone);
        }
        this.numbersOfDronesAvailable = this.drones.Count;

        foreach(GameObject spot in GameObject.FindGameObjectsWithTag("spot") as GameObject[])
        {
            this.spots.Add(spot);
        }
    }

    public void StartMission()
    {
        if(this.numbersOfDronesAvailable >= this.spots.Count)
        {
            foreach(Drone drone in FindObjectsOfType(typeof(Drone)) as Drone[])
            {
                if(!drone.isActive)
                {
                    this.dronesInMission.Add(drone);
                    this.numbersOfDronesAvailable--;
                    drone.isActive = true;
                    drone.step1 = true;
                }
                if(this.dronesInMission.Count == 6)
                    break;
            }
            int i = 0;
            foreach(Drone drone in this.dronesInMission)
            {
                drone.spot = this.spots[i];
                i++;
            }
        }
        else    
            print("Not enough drones to start the mission!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
