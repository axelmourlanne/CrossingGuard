using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{

    public float normalSpeed = 5;
    private float speed;
    private bool decelerate = false;

    // This script will simply instantiate the Prefab when the game starts.
    void Start()
    {
        speed = normalSpeed;
    }

    void DetectionLaser()
    {
        Vector3 current_pos = transform.position;
        //current_pos.z += 0.5f;

        for (float i = 0 ; i <= 0.3f ; i += 0.015f) {

            Vector3 detectionDirection = Vector3.right;
            
            detectionDirection.y = i;

            var ray = new Ray(current_pos, transform.TransformDirection(detectionDirection));

            Vector3 drawDown = transform.TransformDirection(detectionDirection * 10f);
            Debug.DrawRay(current_pos, drawDown, Color.red);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 10f)) {
                decelerate = true;
                return;
            } else {
                decelerate = false;
            }
        }
        

    }

    void Update() {
        DetectionLaser();
    }

    void FixedUpdate() {
        if (decelerate) {
            this.speed -= Mathf.Max(speed, 8) * Time.deltaTime;
            if (this.speed <= 0) {
                this.speed = 0;
            }
        } else {
            this.speed += Mathf.Max(speed, 8) * Time.deltaTime;
            if (this.speed >= normalSpeed) this.speed = normalSpeed;
        }
        this.transform.position += this.transform.right * this.speed * Time.deltaTime;
    }
}
