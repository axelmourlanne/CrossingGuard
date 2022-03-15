using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    public GameObject spot = null; //assigned spot
    public bool isActive; //this bool is true if the drone has been assigned to a mission, false otherwise
    public bool startFromStation; //during this step the drone goes over its assigned spot
    public bool descendToSpot; //during this step the drone descends towards its spot
    public bool detection; //during this step the drone throws many raycasts to detect pedestrians
    public bool endOfMission; //during this step the drone takes altitude again in order to be high enough
    public bool backToStation; //during this step the drone goes back to its original location
    public Vector3 targetPosition; //location where the drone has to go. Its value changes many times as the steps change
    public float speed;
    public ControlStation headquarters; //control station
    public List<Drone> dronesInMission = new List<Drone>();
    public Drone chief = null;
    public bool pedestrianHasStartedTraversing; //true from the moment a pedestrian is detected, false otherwise
    public int numberOfDetections; //the number of rays which detected a pedestrian at each iteration
    public float timerMissionEnd = 0f;
    public bool blink;
    public float timerBlink = 0f;
    public Color originalColor; 
    public Vector3 initialPosition;
    public float timerDistance = 0f;
    public GameObject[] missionSpots;
    public int cpt = 0; //this counter is incremented for the chief as each drone arrives to its assigned spot


    void DetectionLaser()
    {
        Vector3 detectionDirection;
        if(this.spot.name.Contains("Spot1") || this.spot.name.Contains("Spot2") || this.spot.name.Contains("Spot3")) 
            detectionDirection = Vector3.back;
        else
            detectionDirection = Vector3.forward;

        for(float i = -0.5f ; i <= 0.5f ; i=i+0.1f) 
        {
            detectionDirection.x = i;
            var ray = new Ray(this.transform.position, transform.TransformDirection(detectionDirection));
            Vector3 drawDown = transform.TransformDirection(detectionDirection * 2.5f);
            Debug.DrawRay(this.transform.position, drawDown, Color.blue);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 2.5f)) 
            {
                this.chief.pedestrianHasStartedTraversing = true;
                this.chief.numberOfDetections++;
            }
        }
        

    }

    bool ShouldMissionEnd()
    {
        return (this.chief == this) ? (this.numberOfDetections == 0 && this.pedestrianHasStartedTraversing) ? true : (this.chief.timerMissionEnd > 5f) ? true : false : false; //hell
    }



    int GetSpotID()
    {
        string[] id = this.spot.name.Split('t'); 
        string[] id2 = id[1].Split('(');
        return int.Parse(id[0]);

    }


    void MissionPlanning()
    {
        this.targetPosition = new Vector3(this.spot.transform.position.x, this.transform.position.y, this.spot.transform.position.z);
    }


    // Start is called before the first frame update
    void Start()
    {
        this.isActive = false;
        this.startFromStation = false;
        this.descendToSpot = false;
        this.detection = false;
        this.endOfMission = false;
        this.backToStation = false;
        this.speed = Parameters.droneSpeed;
        this.headquarters = GameObject.Find("Headquarters").GetComponent<ControlStation>();
        this.pedestrianHasStartedTraversing = false;
        this.numberOfDetections = 0;
        this.blink = false;
        this.originalColor = this.transform.GetChild(2).GetComponent<Renderer>().material.color;
        this.initialPosition = this.transform.position;
    }

    int GetDroneID()
    {

        string[] id = this.name.Split('e'); 
        return int.Parse(id[1]);
    }


    float[] GetDistanceFromTarget(int x, int y, int z)
    {
        float[] returnTab = {0f,0f,0f};
        if(x == 1)
            returnTab[0] = Mathf.Abs(this.transform.position.x - this.targetPosition.x);
        if(y == 1)
            returnTab[1] = Mathf.Abs(this.transform.position.y - this.targetPosition.y);
        if(z == 1)
            returnTab[2] = Mathf.Abs(this.transform.position.z - this.targetPosition.z);
    
        return returnTab;
    }


    void Move()
    {
        this.transform.position = Vector3.MoveTowards(this.transform.position, this.targetPosition, this.speed * Time.deltaTime);
    }

    // Update is called once per frame
    void Update()
    {

        if(this.isActive)
        {
            //this foreach loop is supposed to prevent collisions but it's meh
            /*foreach(Drone drone in this.chief.dronesInMission)
            {
                if(drone.name == this.name)
                    continue;
                
                float[] dist = GetDistanceFromTarget(1,1,1);
                
                if(dist[0] <= 0.5f && dist[1] < 0.5f && dist[2] <= 0.5f && this.GetDroneID() > drone.GetDroneID())
                {
                    this.isActive = false;
                    this.timerDistance += Time.deltaTime;
                }
            }
            */

            if(this.startFromStation) //moves from station to over the spot       ===> startFromStation
            {
                float[] dist = GetDistanceFromTarget(1,0,1);
                
                Move();

                if(dist[0] <= 0.1f && dist[2] <= 0.1f)
                {
                    this.startFromStation = false;
                    this.descendToSpot = true;
                    this.targetPosition = new Vector3(this.transform.position.x, this.spot.transform.position.y, this.transform.position.z);
                }
            }
            else if(this.descendToSpot) //decreases altitude to the spot             ===> descendToSpotStep
            {
                Move();

                if(GetDistanceFromTarget(0,1,0)[1] <= 0.1f)
                {
                    this.descendToSpot = false;
                    this.chief.cpt++;
                    this.detection = true;
                    this.targetPosition = new Vector3(this.transform.position.x, this.initialPosition.y, this.transform.position.z);
                }

            }

            else if(this.detection) //starts blinking and detecting
            {
                if(blink)
                {
                    if(this.spot.name.Contains("Spot2") || this.spot.name.Contains("Spot5"))
                        this.transform.GetChild(2).GetComponent<Renderer>().material.color = Color.blue;
                    else
                        this.transform.GetChild(2).GetComponent<Renderer>().material.color = Color.red;
                }

                else
                {
                    if(this.spot.name.Contains("Spot2") || this.spot.name.Contains("Spot5"))
                        this.transform.GetChild(2).GetComponent<Renderer>().material.color = Color.red;
                    else
                        this.transform.GetChild(2).GetComponent<Renderer>().material.color = Color.blue;
                }

                if(this.chief.cpt == 6) //every drone has arrived and starts blinking + detecting pedestrians
                {
                    if(this.timerBlink <= 0.75f)
                        this.timerBlink += Time.deltaTime;
                        
                    else
                    {
                        this.blink = !this.blink;
                        this.timerBlink = 0f;
                    }
                    DetectionLaser();

                    if(!this.chief.pedestrianHasStartedTraversing)
                        this.timerMissionEnd += Time.deltaTime;

                    if(ShouldMissionEnd())
                    {
                        foreach(Drone drone in this.dronesInMission)
                        {
                            drone.detection = false;
                            drone.endOfMission = true;
                        }
                        this.headquarters.currentlyUsedSpots.Remove(this.missionSpots);
                    }
                    this.numberOfDetections = 0;
                    
                }
            }

            else if(this.endOfMission) //pedestrian has finished crossing, drone elevates again
            {

                if(this.timerMissionEnd < 2f)
                {
                    this.timerMissionEnd += Time.deltaTime;
                    this.transform.GetChild(2).GetComponent<Renderer>().material.color = Color.green;
                }
                else
                {
                    Move();

                    if(GetDistanceFromTarget(0,1,0)[1] <= 0.1f)
                    {
                        this.endOfMission = false;
                        this.timerMissionEnd = 0f;
                        this.transform.GetChild(2).GetComponent<Renderer>().material.color = this.originalColor;
                        this.backToStation = true;
                        this.targetPosition = this.initialPosition;
                    }
                }    
            }
            else if(this.backToStation) // goes to its original station
            {
                Move();

                float[] dist = GetDistanceFromTarget(1,0,1);

                if(dist[0] <= 0.1f && dist[2] <= 0.1f)
                {
                    this.backToStation = false;
                    this.isActive = false;
                    this.headquarters.numberOfDronesAvailable++;
                    if(this == this.chief)
                    {
                        this.pedestrianHasStartedTraversing = false;
                        this.dronesInMission = new List<Drone>();
                        this.cpt = 0;
                    }
                    this.chief = null;
                    this.spot = null;

                }
            }

        }

        //this else works with the collision foreach loop
        else
        {
            if(this.timerDistance > 0f)
            {
                if(this.timerDistance < 1f)
                    this.timerDistance += Time.deltaTime;
                else
                {
                    this.isActive = true;
                    this.timerDistance = 0f;
                }
            }
        }
    }

}