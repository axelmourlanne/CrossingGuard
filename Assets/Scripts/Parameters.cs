public static class Parameters
{
    //For script Human.cs
    public static float humanSpeed = 5f;
    public static float humanLookSpeed = 135f;
    public static float humanRotationY = 0f;

    //For script Car.cs
    public static float carSpeed = 0f;
    public static float carNormalSpeed = 5f;
    public static float carLaserRange = 10f; //in meters
    
    //For script Drone.cs
    public static float droneSpeed = 10f;
    public static int droneRequiredAutonomy = 120; //in seconds
    public static int droneMaximumAutonomy = 300; //in seconds
    public static float droneBlinkFrequency = 0.75f; //in seconds
    public static float droneMissionTimeout = 4f; //in seconds
    public static float droneLaserRange = 5f; //in meters
    public static float minimumDistanceBetweenDrones = 1; //in meters

    
    //For script ControlStation.cs
    public static int controlStationNumberOfDronesNecessary = 6;
    public static int maximumCommunicationDistance = 50; //in meters
}
