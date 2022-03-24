using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlStation : MonoBehaviour
{

    public Drone[] drones; //the list than contains every crossing guards
    public int numberOfDronesNecessary; //the number of drones required to succeed a mission
    public List<GameObject[]> allSpots = new List<GameObject[]>(); //the list that contains every array of spots from every crosswalk
    public List<GameObject[]> currentlyUsedSpots = new List<GameObject[]>(); //the list that contains array of spots from the current missions
    

    void Start()
    {

        this.numberOfDronesNecessary = Parameters.controlStationNumberOfDronesNecessary;
        GameObject[] passageSpots;
        for(int i = 1 ; i <= GameObject.FindGameObjectsWithTag("crosswalk").Length ; i++) //for every crosswalk in the map, we search the drone spots associated with it and add the arrays of spots in the allSpots attribute.
        {
            passageSpots = GameObject.FindGameObjectsWithTag("crossingSystem" + i.ToString());
            this.allSpots.Add(passageSpots);
        }
        this.drones = FindObjectsOfType(typeof(Drone)) as Drone[]; //every drone in the map is a crossing guard.
    }


    /*
    Method called each time a mission starts and when an active drone requests a substitution.
    Returns a list of drones sorted by their distance to the human who pressed the button on the terminal.
    Parameters: dronesList if the list of drones to be sorted.
    */
    public Drone[] sortDronesAccordingToDistance(Drone[] dronesArray)
    {
        GameObject thierry = GameObject.Find("Thierry");
        Drone[] copyDronesArray = new Drone[dronesArray.Length];
        dronesArray.CopyTo(copyDronesArray, 0);
        for(int i = 0 ; i < copyDronesArray.Length ; i++)
        {
            for(int j = i+1 ; j < copyDronesArray.Length ; j++)
            {
                float dXi = Mathf.Abs(thierry.transform.position.x - copyDronesArray[i].transform.position.x);
                float dYi = Mathf.Abs(thierry.transform.position.y - copyDronesArray[i].transform.position.y);
                float dZi = Mathf.Abs(thierry.transform.position.z - copyDronesArray[i].transform.position.z);
                float dXj = Mathf.Abs(thierry.transform.position.x - copyDronesArray[j].transform.position.x);
                float dYj = Mathf.Abs(thierry.transform.position.y - copyDronesArray[j].transform.position.y);
                float dZj = Mathf.Abs(thierry.transform.position.z - copyDronesArray[j].transform.position.z);
                if(Mathf.Sqrt(Mathf.Pow(2, dXi) + Mathf.Pow(2, dYi) + Mathf.Pow(2, dZi)) > Mathf.Sqrt(Mathf.Pow(2, dXj) + Mathf.Pow(2, dYj) + Mathf.Pow(2, dZj)))
                {
                    Drone tmp = copyDronesArray[i];
                    copyDronesArray[i] = copyDronesArray[j];
                    copyDronesArray[j] = tmp;
                }
            }
        }

        return copyDronesArray;
    }


    /*
    Method called each time a mission starts.
    Returns true if among the available crossing guards the number of drones with enough autonomy is superior or equal to the requested number of drones for a mission. 
    */
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


    /*
    Method called each time a drone substitution is requested.
    Returns the closest drone to the pedestrian who pressed the button on the terminal which has enough autonomy is complete the mission. 
    */
    public Drone SelectAvailableDrone()
    {
        foreach(Drone chargedDrone in sortDronesAccordingToDistance(this.drones))
        {
            if(!chargedDrone.isActive && chargedDrone.autonomy >= Parameters.droneRequiredAutonomy)
            {
                return chargedDrone;
            }
        }
        return null;
    }


    /*
    Method called when a chief from a current mission is about to request a change of chief.
    The headquarters choose among the drones in the mission the one that has the biggest autonomy and then make this new chief take responsability. 
    */
    public void NewChiefIsChosen(Drone currentChief)
    {
        Drone worthiest = null;
        for(int i = 0 ; i < currentChief.dronesInMission.Count ; i++)
        {
            worthiest = currentChief.dronesInMission[i];
            if(worthiest != currentChief && !worthiest.waitingForCar) //the new chief has to be different from the current one
                break;
        }

        foreach(Drone drone in currentChief.dronesInMission)
        {
            if(drone == currentChief)
                continue;
            if(drone.autonomy > worthiest.autonomy && !drone.waitingForCar)
                worthiest = drone;
        }

        worthiest.numberOfDronesReady = currentChief.numberOfDronesReady;

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
        currentChief.numberOfDronesReady = 0;
    }


    /*
    Method called each time a drone substitution is requested.
    Prepares a inactive drone to replace a drone that has not enough autonomy to continue the mission. 
    */
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


    /*
    Method called when one of the buttons from one of the terminals is pressed.
    Chooses from the available crossing guards the ones closest to the pedestrian with enough autonomy to complete the mission and prepare each drone to start.
    */
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
                drone.MissionPlanning();
                i++;
            }
        }
        else //either there aren't enough drones with the required autonomy or there is already a guards team assigned to this crosswalk.
            print("It is currently impossible to call drones here. Please wait.");
    }

    /*
    Method called when a drone wants to send a message to all other drones.
    */
    public void BroadcastMessage(Drone src, Message message)
    {
        foreach (var drone in drones)
        {
            if (drone != src && Vector3.Distance(src.transform.position, drone.transform.position) < Parameters.maximumCommunicationDistance)
            {
                drone.IncomingMessage(message);
            }
        }
    }

    /*
    Method called when a drone wants to send a message to another drone.
    */
    public void SendMessage(Message message, int dstId)
    {
        foreach (var drone in drones)
            if (drone.id == dstId)
                drone.IncomingMessage(message);
    }

}
