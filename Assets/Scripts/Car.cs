using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{

    private Vector3 initialPosition; //the initial position at the start of the simulation
    public float normalSpeed; //the speed at which a car starts the simulation with
    private float speed; //the current speed of the vehicle
    private bool decelerate = false; //if this boolean is true, it means that the car should decrease its speed
    public float timerBackUp; //this float represents the time during which the car has to back up.


    void Start()
    {
        this.speed = Parameters.carSpeed;
        this.normalSpeed = Parameters.carNormalSpeed;
        this.initialPosition = transform.position;
        this.timerBackUp = 0f;
    }

    /*
    Method called at each iteration.
    When an object, be it a pedestrian, a drone or anything else, is detected, the car's attribute this.decelerate becomes true.
    Otherwise it stays false.
    */
    void DetectionLaser()
    {
        Vector3 current_pos = transform.position;

        for (float i = 0f ; i <= 0.3f ; i += 0.015f)
            for(float j = -0.1f ; j <= 0.1f ; j += 0.015f)
        {
            {

            Vector3 detectionDirection = Vector3.right;
            detectionDirection.z = j;
            detectionDirection.y = i;

            var ray = new Ray(current_pos, transform.TransformDirection(detectionDirection));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Parameters.carLaserRange))
            {
                decelerate = true;
                return;
            }
            else
            {
                decelerate = false;
            }
        }
        }
        

    }


    /*
    Method called at each iteration.
    When an object from the 9th layer is detected, it means this is the end of the road and the car's position goes back to the initial position.
    */
    void EndOfRoad() 
    {
        var ray = new Ray(transform.position, transform.TransformDirection(Vector3.down));
        RaycastHit hit;
        int layerMask = 1 << 9;
        if (Physics.Raycast(ray, out hit, 10f, layerMask))
        {
            transform.position = initialPosition;
            speed = 0;
        }
    }

    void Update()
    {
        DetectionLaser();
        EndOfRoad();
    }

    void FixedUpdate()
    {
        if(decelerate)
        {
            this.speed -= Mathf.Max(speed, 8) * Time.deltaTime; //decrease of the speed because an object was detected
            if(this.speed <= 0) 
                this.speed = 0;
            if(this.timerBackUp > 0f) //timerBackUp is strictly superior to 0 only when the car is blocking a drone
            {
                if(this.speed == 0f) //if possible the car should back up in order to stop blocking the drone
                {
                    this.transform.position += -1f * this.transform.right * this.normalSpeed * Time.deltaTime;
                    this.timerBackUp += Time.deltaTime;
                }
                if(this.timerBackUp > 2f)
                    this.timerBackUp = 0f; //after these 2s the timer goes back to 0, meaning it's not blocking a drone anymore
            }
        }
        else
        {
            this.speed += Mathf.Max(speed, 8) * Time.deltaTime;
            if(this.speed >= normalSpeed) 
                this.speed = normalSpeed;
            this.transform.position += this.transform.right * this.speed * Time.deltaTime;
        }
    }
}
