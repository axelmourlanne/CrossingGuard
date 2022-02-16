using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlStation : MonoBehaviour
{

    public List<Drone> drones = new List<Drone>();
    //public List<Drone> dronesInMission = new List<Drone>();
    public int numberOfDronesAvailable;
    public List<GameObject[]> allSpots = new List<GameObject[]>();
    public int range = 100;
    public int layer_mask;
    
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


    public int IndexOf(GameObject[] spots, GameObject spot)
    {
        int i = 0;
        foreach(GameObject current_spot in spots)
        {
            if(current_spot == spot)
                break;
            else
                i++;
        }
        return i;
    }


    public void StartMission(int buttonTag)
    {
        GameObject[] missionSpots = this.allSpots[buttonTag - 1];
        Drone chief = null;
        if(this.numberOfDronesAvailable >= missionSpots.Length)
        {
            foreach(Drone drone in FindObjectsOfType(typeof(Drone)) as Drone[])
            {
                if(!drone.isActive)
                {
                    if(chief == null)
                    {
                        chief = drone;
                    }
                    chief.dronesInMission.Add(drone);
                    this.numberOfDronesAvailable--;
                    drone.targetStep1 = new Vector3(drone.transform.position.x, drone.safeAltitude, drone.transform.position.z);
                    drone.isActive = true;
                    drone.step1 = true;
                    drone.chief = chief;
                }
                if(chief.dronesInMission.Count == 6)
                    break;
            }
            int i = 0;
            foreach(Drone drone in chief.dronesInMission)
            {
                drone.spot = missionSpots[i];
                // if(this.IndexOf(missionSpots, drone.spot) < 3) 
                //     drone.direction = "back";
                // else if(this.IndexOf(missionSpots, drone.spot) < 6)
                //     drone.direction = "forward";
                // else // >= 6, weird
                //     this.allSpots[buttonTag - 1].Remove(drone);
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
