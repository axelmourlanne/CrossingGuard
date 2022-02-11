using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlStation : MonoBehaviour
{

    public List<Drone> drones = new List<Drone>();
    public List<Drone> dronesInMission = new List<Drone>();
    public int numberOfDronesAvailable;
    public List<GameObject[]> allSpots = new List<GameObject[]>();
    public int range = 100;
    
    // Start is called before the first frame update
    void Start()
    {
        foreach(Drone drone in FindObjectsOfType(typeof(Drone)) as Drone[])
        {
            this.drones.Add(drone);
        }
        this.numberOfDronesAvailable = this.drones.Count;

        GameObject[] passageSpots;
        for(int i = 1 ; i <= GameObject.FindGameObjectsWithTag("crossing").Length ; i++)
        {
            passageSpots = GameObject.FindGameObjectsWithTag("spot" + i.ToString());
            this.allSpots.Add(passageSpots);
        }

    }

    public void StartMission(int buttonTag)
    {
        GameObject[] missionSpots = this.allSpots[buttonTag - 1];

        if(this.numberOfDronesAvailable >= missionSpots.Length)
        {
            foreach(Drone drone in FindObjectsOfType(typeof(Drone)) as Drone[])
            {
                if(!drone.isActive)
                {
                    this.dronesInMission.Add(drone);
                    this.numberOfDronesAvailable--;
                    drone.isActive = true;
                    drone.step1 = true;
                }
                if(this.dronesInMission.Count == 6)
                    break;
            }
            int i = 0;
            foreach(Drone drone in this.dronesInMission)
            {
                drone.spot = missionSpots[i];
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
