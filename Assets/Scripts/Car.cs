using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{

    private Vector3 initialPosition;
    public float normalSpeed;
    private float speed;
    private bool decelerate = false;
    public float timerBackUp;

    void Start()
    {
        this.speed = Parameters.carSpeed;
        this.normalSpeed = Parameters.carNormalSpeed;
        this.initialPosition = transform.position;
        this.timerBackUp = 0f;
    }

    void DetectionLaser()
    {
        Vector3 current_pos = transform.position;
        //current_pos.z += 0.5f;

        for (float i = 0 ; i <= 0.3f ; i += 0.015f)
        {

            Vector3 detectionDirection = Vector3.right;
            
            detectionDirection.y = i;

            var ray = new Ray(current_pos, transform.TransformDirection(detectionDirection));

            Vector3 drawDown = transform.TransformDirection(detectionDirection * 10f);
            Debug.DrawRay(current_pos, drawDown, Color.red);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 10f))
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
        if (decelerate)
        {
            this.speed -= Mathf.Max(speed, 8) * Time.deltaTime;
            if(this.speed <= 0) 
                this.speed = 0;
            if(this.timerBackUp > 0f)
            {
                if(this.speed == 0f)
                {
                    this.transform.position += -1f * this.transform.right * this.normalSpeed * Time.deltaTime;
                    this.timerBackUp += Time.deltaTime;
                }
                if(this.timerBackUp > 2f)
                    this.timerBackUp = 0f;
            }
        }
        else
        {
            this.speed += Mathf.Max(speed, 8) * Time.deltaTime;
            if (this.speed >= normalSpeed) this.speed = normalSpeed;
        }
        
        if(this.decelerate && this.timerBackUp > 0f) //it's blocking a drone
        {    
            this.transform.position += -1f * this.transform.right * this.speed * Time.deltaTime;
            this.timerBackUp += Time.deltaTime;
        }
        else
            this.transform.position += this.transform.right * this.speed * Time.deltaTime;
    }
}
