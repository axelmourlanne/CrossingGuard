using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlStation : MonoBehaviour
{

    public Drone[] drones;
    public int numberOfDronesNecessary;
    public List<GameObject[]> allSpots = new List<GameObject[]>();
    public int range = 100;
    public List<GameObject[]> currentlyUsedSpots = new List<GameObject[]>();
    

    void Start()
    {

        this.numberOfDronesNecessary = Parameters.controlStationNumberOfDronesNecessary;
        GameObject[] passageSpots;
        for(int i = 1 ; i <= GameObject.FindGameObjectsWithTag("crossing").Length ; i++)
        {
            passageSpots = GameObject.FindGameObjectsWithTag("spot" + i.ToString());
            this.allSpots.Add(passageSpots);
        }
        this.drones = FindObjectsOfType(typeof(Drone)) as Drone[];
    }


    public Drone[] sortDronesAccordingToDistance(Drone[] dronesList)
    {
        GameObject thierry = GameObject.Find("Thierry");
        for(int i = 0 ; i < dronesList.Length ; i++)
        {
            for(int j = i+1 ; j < dronesList.Length ; j++)
            {
                float dXi = Mathf.Abs(thierry.transform.position.x - dronesList[i].transform.position.x);
                float dYi = Mathf.Abs(thierry.transform.position.y - dronesList[i].transform.position.y);
                float dZi = Mathf.Abs(thierry.transform.position.z - dronesList[i].transform.position.z);
                float dXj = Mathf.Abs(thierry.transform.position.x - dronesList[j].transform.position.x);
                float dYj = Mathf.Abs(thierry.transform.position.y - dronesList[j].transform.position.y);
                float dZj = Mathf.Abs(thierry.transform.position.z - dronesList[j].transform.position.z);
                if(Mathf.Sqrt(Mathf.Pow(2, dXi) + Mathf.Pow(2, dYi) + Mathf.Pow(2, dZi)) > Mathf.Sqrt(Mathf.Pow(2, dXj) + Mathf.Pow(2, dYj) + Mathf.Pow(2, dZj)))
                {
                    Drone tmp = dronesList[i];
                    dronesList[i] = dronesList[j];
                    dronesList[j] = tmp;
                }
            }
        }

        return dronesList;
    }


    public bool AutonomyCheck()
    {
        int counterDronesAvailable = 0;
        foreach(Drone drone in this.drones)
        {
            if(drone.autonomy >= Parameters.droneRequiredAutonomy && !drone.isActive)
                counterDronesAvailable++;
        }
        return counterDronesAvailable >= this.numberOfDronesNecessary;
    }


    public Drone SelectAvailableDrone()
    {
        List<Drone> dronesList = new List<Drone>();
        foreach(Drone chargedDrone in sortDronesAccordingToDistance(this.drones))
        {
            if(!chargedDrone.isActive && chargedDrone.autonomy >= Parameters.droneRequiredAutonomy)
            {
                dronesList.Add(chargedDrone);
            }
        }
        return dronesList.Count == 0 ? null : dronesList[0];
    }



    public void NewChiefIsChosen(Drone currentChief)
    {
        Drone worthiest = currentChief.dronesInMission[0];
        foreach(Drone drone in currentChief.dronesInMission)
        {
            if(drone == currentChief)
                continue;
            if(drone.autonomy > worthiest.autonomy)
                worthiest = drone;
        }

        worthiest.cpt = currentChief.cpt;

        foreach(Drone drone in currentChief.dronesInMission)
        {
            if(drone == currentChief)
                continue;
            drone.chief = worthiest;
        } 
        currentChief.chief = worthiest;
        worthiest.dronesInMission = currentChief.dronesInMission;
        worthiest.missionSpots = currentChief.missionSpots;
        worthiest.numberOfDetections = currentChief.numberOfDetections;
        worthiest.pedestrianHasStartedTraversing = currentChief.pedestrianHasStartedTraversing;
        currentChief.dronesInMission = new List<Drone>();
    }


    public void RequestForChargedDrone(Drone droneToBeCharged)
    {
        droneToBeCharged.detection = false;
        droneToBeCharged.endOfMission = true;
        Drone chargedDrone = this.SelectAvailableDrone();
        
        if(chargedDrone != null)
        {
            chargedDrone.isActive = true;
            chargedDrone.startFromStation = true;
            chargedDrone.chief = droneToBeCharged.chief;
            chargedDrone.spot = droneToBeCharged.spot;
            chargedDrone.chief.dronesInMission.Add(chargedDrone);
            chargedDrone.chief.dronesInMission.Remove(droneToBeCharged);
            chargedDrone.targetPosition = new Vector3(chargedDrone.spot.transform.position.x, chargedDrone.transform.position.y, chargedDrone.spot.transform.position.z);
            droneToBeCharged.pedestrianHasStartedTraversing = false;
            droneToBeCharged.missionSpots = null;
        }
        else
            print("None of the remaining drones has enough charge!");
    }


    public void StartMission(int buttonTag)
    {
        GameObject[] missionSpots = this.allSpots[buttonTag - 1];
        Drone chief = null;
        
        if(this.AutonomyCheck() && !this.currentlyUsedSpots.Contains(missionSpots))
        {
            this.currentlyUsedSpots.Add(missionSpots);
            foreach(Drone drone in sortDronesAccordingToDistance(this.drones))
            {
                if(drone.spot == null && drone.autonomy >= Parameters.droneRequiredAutonomy)
                {
                    if(chief == null)
                    {
                        chief = drone;
                        chief.missionSpots = missionSpots;
                    }
                    chief.dronesInMission.Add(drone);
                    drone.isActive = true;
                    drone.startFromStation = true;
                    drone.chief = chief;

                    if(chief.dronesInMission.Count == this.numberOfDronesNecessary)
                        break;
                }
            }
            int i = 0;
            foreach(Drone drone in chief.dronesInMission)
            {
                drone.spot = missionSpots[i];
                drone.targetPosition = new Vector3(drone.spot.transform.position.x, drone.transform.position.y, drone.spot.transform.position.z);
                i++;
                if(drone.gameObject.name == "Drone08")
                    drone.autonomy = 35;
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
