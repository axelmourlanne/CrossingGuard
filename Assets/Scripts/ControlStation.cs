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
    public List<GameObject[]> currentlyUsedSpots = new List<GameObject[]>();
    
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

    public Drone[] sortDroneAccordingToDistance(Drone[] drones)
    {
        GameObject thierry = GameObject.Find("Thierry");
        for(int i = 0 ; i < drones.Length ; i++)
        {
            for(int j = i+1 ; j < drones.Length ; j++)
            {
                float dXi = Mathf.Abs(thierry.transform.position.x - drones[i].transform.position.x);
                float dYi = Mathf.Abs(thierry.transform.position.y - drones[i].transform.position.y);
                float dZi = Mathf.Abs(thierry.transform.position.z - drones[i].transform.position.z);
                float dXj = Mathf.Abs(thierry.transform.position.x - drones[j].transform.position.x);
                float dYj = Mathf.Abs(thierry.transform.position.y - drones[j].transform.position.y);
                float dZj = Mathf.Abs(thierry.transform.position.z - drones[j].transform.position.z);
                if(Mathf.Sqrt(Mathf.Pow(2, dXi) + Mathf.Pow(2, dYi) + Mathf.Pow(2, dZi)) > Mathf.Sqrt(Mathf.Pow(2, dXj) + Mathf.Pow(2, dYj) + Mathf.Pow(2, dZj)))
                {
                    Drone tmp = drones[i];
                    drones[i] = drones[j];
                    drones[j] = tmp;
                }
            }
        }

        return drones;
    }

    public void StartMission(int buttonTag)
    {
        GameObject[] missionSpots = this.allSpots[buttonTag - 1];
        Drone chief = null;
        if(this.numberOfDronesAvailable >= missionSpots.Length && !this.currentlyUsedSpots.Contains(missionSpots))
        {
            this.currentlyUsedSpots.Add(missionSpots);
            foreach(Drone drone in sortDroneAccordingToDistance(FindObjectsOfType(typeof(Drone)) as Drone[]))
            {
                //if(!drone.isActive && drone.timerDistance == 0)
                if(drone.spot == null)
                {
                    if(chief == null)
                    {
                        chief = drone;
                        chief.missionSpots = missionSpots;
                    }
                    chief.dronesInMission.Add(drone);
                    this.numberOfDronesAvailable--;
                    drone.targetStep1 = new Vector3(drone.transform.position.x, drone.safeAltitude, drone.transform.position.z);
                    drone.isActive = true;
                    drone.step1 = true;
                    drone.chief = chief;

                    if(chief.dronesInMission.Count == 6)
                        break;
                }
                // if(chief.dronesInMission.Count == 6)
                //     break;
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
            print("It is currently impossible to call drones here. Please wait.");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
