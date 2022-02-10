using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    public GameObject spot;
    public bool isActive;
    public bool step1;
    public bool step2;
    public bool step3;
    public bool step4;
    public Vector3 targetStep1;
    public Vector3 targetStep2;
    public Vector3 targetStep3;


    // Start is called before the first frame update
    void Start()
    {
        this.isActive = false;
        this.step1 = false;
        this.step2 = false;
        this.step3 = false;
        this.step4 = false;
        this.targetStep1 = new Vector3(this.transform.position.x, this.transform.position.y + 20f, this.transform.position.z);
        

    }

    // Update is called once per frame
    void Update()
    {
        
        if(this.isActive)
        {
            if(this.step1)
            {

                transform.position = Vector3.MoveTowards(transform.position, targetStep1, 5f * Time.deltaTime);

                float dY = Mathf.Abs(this.transform.position.y - targetStep1.y);

                if(dY <= 0.1f)
                {
                    this.step1 = false;
                    this.step2 = true;
                    this.targetStep2 = new Vector3(this.spot.transform.position.x, this.transform.position.y, this.spot.transform.position.z);
                }
            }

            if(this.step2)
            {
                float dX = Mathf.Abs(this.transform.position.x - this.spot.transform.position.x);
                float dZ = Mathf.Abs(this.transform.position.z - this.spot.transform.position.z);

                transform.position = Vector3.MoveTowards(transform.position, targetStep2, 5f * Time.deltaTime);

                if(dX <= 0.1f && dZ <= 0.1f)
                {
                    this.step2 = false;
                    this.step3 = true;
                    this.targetStep3 = new Vector3(this.transform.position.x, this.transform.position.y - 20f, this.transform.position.z);
                }
            }
            if(this.step3)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetStep3, 5f * Time.deltaTime);

                float dY = Mathf.Abs(this.transform.position.y - targetStep3.y);


                if(dY <= 0.1f)
                {
                    this.step3 = false;
                    this.step4 = true;
                }

            }
            
        }
    }
}
