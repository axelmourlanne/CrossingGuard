using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : MonoBehaviour
{

    public float speed;
    public Camera mainCamera;
    public float lookSpeed; //speed at which the camera can rotate on the y axis 
    public float rotationY; //float the record how much the mouse moves (left/right)
    public bool isKeyWPressed; //boolean that is set to true when the 'W' key is pressed (Input.GetKeyDown) and to false when the key isn't pressed anymore (Input.GetKeyUp)
    public bool isKeyRightPressed; //boolean that is set to true when the right arrow key is pressed (Input.GetKeyDown) and to false when the key isn't pressed anymore (Input.GetKeyUp)
    public bool isKeyLeftPressed; //boolean that is set to true when the left arrow key is pressed (Input.GetKeyDown) and to false when the key isn't pressed anymore (Input.GetKeyUp)


    void CameraMove(){
        this.mainCamera.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 1.5f, this.transform.position.z);
        mainCamera.transform.localRotation = Quaternion.Euler(0f, -1f * rotationY, 0f);
        this.transform.localRotation = this.mainCamera.transform.localRotation;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.isKeyWPressed = false;
        this.isKeyLeftPressed = false;
        this.isKeyRightPressed = false;
        this.speed = Parameters.humanSpeed;
        this.lookSpeed = Parameters.humanLookSpeed;
        this.rotationY = Parameters.humanRotationY;
    }

    // Update is called once per frame
    void Update()
    {
        CameraMove();

        if(Input.GetKeyDown(KeyCode.W)) //in first person view the 'W' key is used to make the player move forward
            this.isKeyWPressed = true;
        if(this.isKeyWPressed)
            this.transform.position += this.transform.forward * this.speed * 10 * Time.deltaTime;
        if(Input.GetKeyUp(KeyCode.W))
            this.isKeyWPressed = false;

        if(Input.GetKeyDown(KeyCode.LeftArrow)) //to move left (from the camera's view it would be left)
            this.isKeyLeftPressed = true;
        if(this.isKeyLeftPressed)
            this.rotationY += 90 * lookSpeed * Time.deltaTime;
        if(Input.GetKeyUp(KeyCode.LeftArrow))
            this.isKeyLeftPressed = false;

        if(Input.GetKeyDown(KeyCode.RightArrow)) //to move right (from the camera's view it would be right)
            this.isKeyRightPressed = true;
        if(this.isKeyRightPressed)
            this.rotationY += -90 * lookSpeed * Time.deltaTime;
        if(Input.GetKeyUp(KeyCode.RightArrow))
            this.isKeyRightPressed = false;
    }
}
