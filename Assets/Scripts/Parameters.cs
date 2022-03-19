using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Parameters
{
    public static float humanSpeed = 5f;
    public static float humanLookSpeed = 135f;
    public static float humanRotationY = 0f;

    public static float carSpeed = 0f;
    public static float carNormalSpeed = 5f;
    
    public static float droneSpeed = 10f;
    public static int droneRequiredAutonomy = 120; //in seconds
    public static int droneMaximumAutonomy = 300; //in seconds
    public static float droneBlinkFrequency = 0.75f; //in seconds
    
    public static int controlStationNumberOfDronesNecessary = 6;
}
