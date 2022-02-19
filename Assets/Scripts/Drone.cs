using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    public GameObject spot = null;
    public bool isActive;
    public bool step1;
    public bool step2;
    public bool step3;
    public bool step4;
    public bool step5;
    public bool step6;
    public Vector3 targetStep1;
    public Vector3 targetStep2;
    public Vector3 targetStep3;
    public float speed;
    public float safeAltitude;
    public ControlStation headquarters;
    public string direction;
    public List<Drone> dronesInMission = new List<Drone>();
    public Drone chief = null;
    public bool pedestrianHasStartedTraversing;
    public int numberOfDetections;
    public float timerMissionEnd = 0f;
    public bool blink;
    public float timerBlink = 0f;
    public Color originalColor; 
    public Vector3 initialPosition;
    public float timerDistance = 0f;
    public GameObject[] missionSpots;


    void DetectionLaser()
    {
        Vector3 current_pos = transform.position;
        for(float i = -0.5f ; i <= 0.5f ; i=i+0.1f) 
            {
                    Vector3 detectionDirection;
                    if(this.spot.name.Contains("Spot1") || this.spot.name.Contains("Spot2") || this.spot.name.Contains("Spot3")) 
                        detectionDirection = Vector3.back;
                    else //(this.spot.name.Contains("spot4") || this.spot.name.Contains("spot5") || this.spot.name.Contains("spot6"))
                        detectionDirection = Vector3.forward;
                    detectionDirection.x = i;
                    var ray = new Ray(current_pos, transform.TransformDirection(detectionDirection)); //each ray has to begin from the drone's current position and has to be directed towards the ground in a way that the intersection is a square
                    Vector3 drawDown = transform.TransformDirection(detectionDirection * 2.5f);
                    Debug.DrawRay(current_pos, drawDown, Color.blue);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, 2.5f)) 
                    {
                        if(!this.chief.pedestrianHasStartedTraversing)
                            this.chief.pedestrianHasStartedTraversing = true;
                        this.chief.numberOfDetections++;
                    }
            }
        

    }

    void IsPedestrianStillTraversing()
    {
        if(this == this.chief)
        {
            if(this.numberOfDetections == 0 && pedestrianHasStartedTraversing)
            {
                this.timerMissionEnd += Time.deltaTime;
                foreach(Drone drone in this.dronesInMission)
                {
                    drone.step4 = false;
                    drone.step5 = true;
                }
                this.headquarters.currentlyUsedSpots.Remove(this.missionSpots);
            }
            else
                this.numberOfDetections = 0;
        }
    }

    int GetSpotID()
    {
        string[] id = this.spot.name.Split('t'); 
        string[] id2 = id[1].Split('(');
        return int.Parse(id[0]);

    }

    // Start is called before the first frame update
    void Start()
    {
        this.isActive = false;
        this.step1 = false;
        this.step2 = false;
        this.step3 = false;
        this.step4 = false;
        this.step5 = false;
        this.step6 = false;
        this.speed = 10f;
        this.safeAltitude = 90f;
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

    // Update is called once per frame
    void Update()
    {

        if(this.isActive)
        {

            foreach(Drone drone in this.chief.dronesInMission)
            {
                if(drone.name == this.name)
                    continue;
                
                float dX = Mathf.Abs(this.transform.position.x - drone.transform.position.x);
                float dY = Mathf.Abs(this.transform.position.y - drone.transform.position.y);
                float dZ = Mathf.Abs(this.transform.position.z - drone.transform.position.z);
                
                if(dX <= 0.5f && dY < 0.5f && dZ <= 0.5f && this.GetDroneID() > drone.GetDroneID())
                {
                    this.isActive = false;
                    this.timerDistance += Time.deltaTime;
                }
            }

            if(this.step1)
            {

                transform.position = Vector3.MoveTowards(transform.position, targetStep1, this.speed * Time.deltaTime);

                float dY = Mathf.Abs(this.transform.position.y - targetStep1.y);

                if(dY <= 0.1f)
                {
                    this.step1 = false;
                    this.step2 = true;
                    this.targetStep2 = new Vector3(this.spot.transform.position.x, this.transform.position.y, this.spot.transform.position.z);
                }
            }

            else if(this.step2)
            {
                float dX = Mathf.Abs(this.transform.position.x - this.spot.transform.position.x);
                float dZ = Mathf.Abs(this.transform.position.z - this.spot.transform.position.z);

                transform.position = Vector3.MoveTowards(transform.position, targetStep2, this.speed * Time.deltaTime);

                if(dX <= 0.1f && dZ <= 0.1f)
                {
                    this.step2 = false;
                    this.step3 = true;
                    this.targetStep3 = new Vector3(this.transform.position.x, this.spot.transform.position.y, this.transform.position.z);
                }
            }
            else if(this.step3)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetStep3, this.speed * Time.deltaTime);

                float dY = Mathf.Abs(this.transform.position.y - targetStep3.y);


                if(dY <= 0.1f)
                {
                    this.step3 = false;
                    this.step4 = true;
                }

            }
            else if(this.step4)
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

                if(this.timerBlink <= 0.75f)
                    this.timerBlink += Time.deltaTime;
                
                else
                {
                    this.blink = !this.blink;
                    this.timerBlink = 0f;
                }
                DetectionLaser();
                IsPedestrianStillTraversing();
            }

            else if(this.step5)
            {

                if(this.timerMissionEnd <= 3f)
                {
                    this.timerMissionEnd += Time.deltaTime;
                    this.transform.GetChild(2).GetComponent<Renderer>().material.color = Color.green;
                }
                else
                {
                    Vector3 newPos = new Vector3(this.transform.position.x, this.initialPosition.y, this.transform.position.z);
                    this.transform.position = Vector3.MoveTowards(this.transform.position, newPos, this.speed * Time.deltaTime);

                    float dY = Mathf.Abs(this.transform.position.y - newPos.y);

                    if(dY <= 0.1f)
                    {
                        this.step5 = false;
                        this.timerMissionEnd = 0f;
                        this.transform.GetChild(2).GetComponent<Renderer>().material.color = this.originalColor;
                        this.step6 = true;
                    }
                }    
            }
            else if(this.step6)
            {
                Vector3 newPos = new Vector3(this.initialPosition.x, this.transform.position.y, this.initialPosition.z);
                this.transform.position = Vector3.MoveTowards(this.transform.position, newPos, this.speed * Time.deltaTime);

                float dX = Mathf.Abs(this.transform.position.x - this.initialPosition.x);
                float dZ = Mathf.Abs(this.transform.position.z - this.initialPosition.z);

                if(dX <= 0.1f && dZ <= 0.1f)
                {
                    this.step6 = false;
                    this.isActive = false;
                    this.pedestrianHasStartedTraversing = false;
                    this.headquarters.numberOfDronesAvailable++;
                    if(this == this.chief)
                    {
                        this.dronesInMission = new List<Drone>();
                    }
                    this.chief = null;
                    this.spot = null;

                }
            }

        }

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