using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    public int id;
    public GameObject spot = null; //assigned spot
    public bool isActive; //this bool is true if the drone has been assigned to a mission, false otherwise
    public bool startFromStation; //during this step the drone goes over its assigned spot
    public bool descendToSpot; //during this step the drone descends towards its spot
    public bool detection; //during this step the drone throws many raycasts to detect pedestrians
    public bool endOfMission; //during this step the drone takes altitude again in order to be high enough
    public bool backToStation; //during this step the drone goes back to its original location
    public Vector3 targetPosition; //location where the drone has to go. Its value changes many times as the steps change
    public Vector3 antiCollisionPosition; //location where the drone has to go to avoid a collision with another drone
    public Dictionary<int, bool> tooCloseDrones; //list of the drones for which an anti-collision move must be made,with a bool which indicated if this drone must stop
    public float speed;
    public ControlStation headquarters; //control station
    public List<Drone> dronesInMission = new List<Drone>(); //the list of every drone for the mission
    public Drone chief = null; //the drone which will take responsability for the mission
    public bool pedestrianHasStartedTraversing; //true from the moment a pedestrian is detected, false otherwise
    public int numberOfDetections; //the number of rays which detected a pedestrian at each iteration
    public float timerMissionEnd = 0f;
    public bool blink; //this boolean is there to specify the color to emit by the drone
    public float timerBlink = 0f; //this timer is used to change the attribute this.blink periodically
    public Color originalColor; //the color of the drone's position before the mission started
    public Vector3 initialPosition; //the spatial position of the drone's position before the mission started
    public float timerAutonomy = 0f; //every second, the drone's autonomy is updated
    public GameObject[] missionSpots; //the list of all the spots from the drone's mission
    public int numberOfDronesReady = 0; //this counter is incremented for the chief as each drone arrives to its assigned spot
    public int autonomy; //the number of seconds a drone can be active. It's increased when the drone is charging at the station
    public bool batteryIsTooLow; //when this boolean is true, a drone cannot continue its mission anymore and has to request a substition from the headquarters
    public bool substitutionInProgress; //when this boolean is true, no other drone of the same team should request a substitution
    public bool waitingForCar; //when this boolean is true, the drone can't descend further because of a car

    void Start()
    {
        this.isActive = false;
        this.startFromStation = false;
        this.descendToSpot = false;
        this.detection = false;
        this.endOfMission = false;
        this.backToStation = false;
        this.tooCloseDrones = new Dictionary<int, bool>();
        this.speed = Parameters.droneSpeed;
        this.autonomy = Random.Range(Parameters.droneRequiredAutonomy, Parameters.droneMaximumAutonomy);
        this.headquarters = GameObject.Find("Headquarters").GetComponent<ControlStation>();
        this.pedestrianHasStartedTraversing = false;
        this.numberOfDetections = 0;
        this.blink = false;
        this.originalColor = this.transform.GetChild(2).GetComponent<Renderer>().material.color;
        this.initialPosition = this.transform.position;
        this.batteryIsTooLow = false;
        this.substitutionInProgress = false;
        this.waitingForCar = false;
    }

    /*
    Method called from the headquarters right before the mission starts. 
    It lets the drone target the position over its assigned spot at the current altitude.
    */
    public void MissionPlanning()
    {
        this.targetPosition = new Vector3(this.spot.transform.position.x, this.transform.position.y, this.spot.transform.position.z);
    }


    /*
    Method called each time the drone has to get to its current targetted position.
    */
    public void Move()
    {
        if (tooCloseDrones.Count > 0)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, this.antiCollisionPosition, this.speed * Time.deltaTime);
        }
        else 
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, this.targetPosition, this.speed * Time.deltaTime);
        }
    }


    /*
    Mission called by every drone during the detection step.
    Returns true is the drone is the chief no longer detects a pedestrian of if it still hasn't detected anyone for too long. 
    */
    bool ShouldMissionEnd()
    {
        if(this == this.chief)
        {
            if((this.numberOfDetections == 0 && this.pedestrianHasStartedTraversing) || this.timerMissionEnd > Parameters.droneMissionTimeout)
                return true;
        }
        return false;
    }


    /*
    Method called at each iteration when a drone has to go to a targetted position.
    Returns an array of float giving information about the distance from the current targetted position in the three axis.
    Parameters: if x = 1, the distance from the targetted position on the x axis will be calculated. 
                if y = 1, the distance from the targetted position on the y axis will be calculated.
                if z = 1, the distance from the targetted position on the z axis will be calculated.
    */
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


    /*
    Method called by the drone during the detection step.
    When the drone detects a pedestrian in front of itself, it informs that a human has started traversing and
    increments the chief's detection counter.
    */
    void PedestrianDetectionLaser()
    {
        Vector3 detectionDirection;
        if(this.spot.name.Contains("Spot1") || this.spot.name.Contains("Spot2") || this.spot.name.Contains("Spot3")) 
            detectionDirection = Vector3.back;
        else
            detectionDirection = Vector3.forward;

        for(float i = -0.25f ; i <= 0.25f ; i=i+0.1f) 
        {
            for(float j = 0f ; j >= -0.25f ; j-=0.1f)
            {
                detectionDirection.x = i;
                detectionDirection.y = j;
                var ray = new Ray(this.transform.position, transform.TransformDirection(detectionDirection));
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Parameters.droneLaserRange, 1 << 8)) 
                {
                    this.chief.pedestrianHasStartedTraversing = true;
                    this.chief.numberOfDetections++;
                }
            }
        }
        

    }


    /*
    Method called by the drone during the descendToSpot step.
    When the drone detects a car under itself, the UAV tells the chief to start the mission without it and informs the car of
    the inconveniency.
    If the car is not detected, the drone descend to its assigned spot.
    */
    void CarDetectionLaser() 
    {
        var ray = new Ray(transform.position, transform.TransformDirection(Vector3.down));
        RaycastHit hit;
        int layerMask = 1 << 10; //10 is the layer of cars
        if (Physics.Raycast(ray, out hit, Parameters.droneLaserRange, layerMask))
        {
            if(!this.waitingForCar)
            {
                this.chief.numberOfDronesReady++;
                this.waitingForCar = true;
                if(this == this.chief)
                    this.headquarters.NewChiefIsChosen(this);    
            }
            
            hit.transform.gameObject.GetComponent<Car>().timerBackUp += Time.deltaTime; //the drones asks the car to back up a bit if possible
        }    
        else
        {
            this.waitingForCar = false;
            Move();
        }
    }

     /*
    Method called when a message is sent by a nearby drone
    */
    public void IncomingMessage(Message incomingMsg)
    {
        if (incomingMsg.type == MessageType.Position)
        {
            PositionMessage message = (PositionMessage)incomingMsg;
            if (Vector3.Distance(transform.position, message.position) < Parameters.minimumDistanceBetweenDrones)
            {
                if (!tooCloseDrones.ContainsKey(message.droneId))
                    tooCloseDrones.Add(message.droneId, false);

                antiCollisionPosition = transform.position;
                if (id > message.droneId)
                {
                    AntiCollisionMessage antiColMessage;
                    if (transform.position.y < message.position.y)
                    {
                        antiColMessage = new AntiCollisionMessage(id, message.droneId, false);
                        tooCloseDrones[message.droneId] = true;
                    }
                    else
                    {
                        antiColMessage = new AntiCollisionMessage(id, message.droneId, true);
                        if (!tooCloseDrones.ContainsValue(true))
                            antiCollisionPosition.y = transform.position.y + 10;
                    }
                    headquarters.BroadcastMessage(this, antiColMessage);
                }
            }
            else
            {
                tooCloseDrones.Remove(message.droneId);
            }
        }
        else if (incomingMsg.type == MessageType.AntiCollision)
        {
            AntiCollisionMessage message = (AntiCollisionMessage)incomingMsg;

            if (message.receiverDroneId != id) return;

            if (message.droneId > id)
            {
                if (!tooCloseDrones.ContainsKey(message.droneId))
                    tooCloseDrones.Add(message.droneId, true);

                antiCollisionPosition = transform.position;
                if (!message.stopInstruction)
                {
                    tooCloseDrones[message.droneId] = false;
                    if (!tooCloseDrones.ContainsValue(true))
                        antiCollisionPosition.y = transform.position.y + 10;
                }
            }
        }
    }

    /*
    Method called when the drone wants to broadcast its position
    */
    public void BroadcastPosition()
    {
        headquarters.BroadcastMessage(this, new PositionMessage(id, transform.position));
    }



    void Update()
    {
        this.timerAutonomy += Time.deltaTime;

        if(this.isActive)
        {
            BroadcastPosition();
            
            if(this.timerAutonomy > 1f) //update of the autonomy attribute with the drone active
            {
                this.timerAutonomy = 0f;
                if(this.autonomy > 0)
                    this.autonomy--; //as long as the drone's autonomy has a positive value, it is decreased by 1 every second.
            }

            if(this.chief == this && this.autonomy <= Parameters.droneRequiredAutonomy / 2) //the chief should longer take responsability for the mission and thus the headquarters will chose another drone with more autonomy.
            {
                this.headquarters.NewChiefIsChosen(this);
            }

            if(this.autonomy <= Parameters.droneRequiredAutonomy / 3 && !this.batteryIsTooLow && !this.chief.substitutionInProgress) //the drone doesn't have much battery left and thus requests a substitution from the headquarters
            {
                this.batteryIsTooLow = true;
                this.chief.substitutionInProgress = true; //other drones from the same mission should not request a substitution at the same time.
                this.headquarters.RequestForChargedDrone(this);
                this.transform.GetChild(2).GetComponent<Renderer>().material.color = Color.black;
            }

            if(this.autonomy == 0) //if a drone's autonomy hits 0 seconds before having returned to its station, it ceases to function. 
                this.isActive = false;

            if(this.startFromStation) //moves from the station to over the spot
            {
                float[] dist = GetDistanceFromTarget(1,0,1); //dist array has information on the x and z axis
                
                Move();

                if(dist[0] <= 0.1f && dist[2] <= 0.1f) 
                {
                    this.startFromStation = false;
                    this.descendToSpot = true;
                    this.targetPosition = new Vector3(this.transform.position.x, this.spot.transform.position.y, this.transform.position.z);
                }
            }
            else if(this.descendToSpot) //decreases altitude to the spot
            {
                CarDetectionLaser();
                if(GetDistanceFromTarget(0,1,0)[1] <= 0.1f)
                {
                    this.descendToSpot = false;
                    this.chief.numberOfDronesReady++; //each drone which arrives at their spot increments this chief's attribute in order to start the detection at the same time.
                    if(!this.chief.endOfMission && !this.chief.backToStation && this.chief.isActive)
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

                if(this.chief.numberOfDronesReady >= 6) //every drone has suposedly arrived and starts blinking + detecting pedestrians
                {
                    if(this.timerBlink <= Parameters.droneBlinkFrequency)
                        this.timerBlink += Time.deltaTime;
                        
                    else
                    {
                        this.blink = !this.blink;
                        this.timerBlink = 0f;
                    }
                    PedestrianDetectionLaser();

                    if(!this.chief.pedestrianHasStartedTraversing) //if all the drones are there and a pedestrian still hasn't yet been detected, a timer is incremented. 
                        this.timerMissionEnd += Time.deltaTime;
                    else
                        this.timerMissionEnd = 0f;

                    if(ShouldMissionEnd())
                    {
                        foreach(Drone drone in this.dronesInMission) //'this' is nessessarily the chief here since ShouldMissionEnd() returned true
                        {
                            drone.detection = false;
                            if(drone.startFromStation || drone.descendToSpot) //case of a car blocking the drone for the entirety of the mission or even more rare case of a substitution starting right before the end of the mission
                            {
                                drone.targetPosition = drone.initialPosition;
                                drone.startFromStation = false;
                                drone.descendToSpot = false;
                            }

                            drone.endOfMission = true;
                            drone.transform.GetChild(2).GetComponent<Renderer>().material.color = Color.green;
                        }
                        this.headquarters.currentlyUsedSpots.Remove(this.missionSpots); //this way another pedestrian can call upon another team of drones at the same crosswalk right away
                    }
                    this.numberOfDetections = 0; //at each iteration the detection counter is returned to 0. If at the next iteration it's still zero when ShouldMissionEnd() is called even though a pedestrian has already been detected before, the mission should end.
                    
                }
            }

            else if(this.endOfMission) //the pedestrian has finished crossing or the mission has timed out, so the drone elevate again
            {

                if(this.timerMissionEnd < (this.batteryIsTooLow ? Parameters.droneMissionTimeout / 2 : Parameters.droneMissionTimeout)) //we don't want a drone with low battery to wait too much
                    this.timerMissionEnd += Time.deltaTime;
                
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
                    this.batteryIsTooLow = false;
                    this.chief.substitutionInProgress = false;
                    this.isActive = false;
                    if(this == this.chief)
                    {
                        this.pedestrianHasStartedTraversing = false;
                        this.dronesInMission = new List<Drone>();
                        this.numberOfDronesReady = 0;
                    }
                    this.chief = null;
                    this.spot = null;

                }
            }

        }

        else
        {

            if(this.timerAutonomy > 1f) //update of the autonomy attribute with the drone not active
            {
                this.timerAutonomy = 0f;
                if(this.autonomy < Parameters.droneMaximumAutonomy && !this.batteryIsTooLow)
                    this.autonomy += 2; //as long as the drone's autonomy has a value bellow the maximal capacity, it is increased by 2 every second.
            }

        }
    }

}