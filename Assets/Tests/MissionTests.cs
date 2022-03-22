using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class MissionTests
{
    bool sceneLoaded;
    bool referencesSetup;

    private ControlStation headquarters;

    private List<Drone> dronesInMission;


    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        dronesInMission = new List<Drone>();
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("Demo Scene", LoadSceneMode.Single);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        sceneLoaded = true;
    }

    void SetupReferences()
    {
        if (referencesSetup) return;

        Transform[] objects = Resources.FindObjectsOfTypeAll<Transform>();
        foreach (Transform t in objects)
            if (t.name == "Headquarters")
            {
                headquarters = t.GetComponent<ControlStation>();
                break;
            }
        
        referencesSetup = true;
    }
    
    [UnityTest]
    public IEnumerator ReferencesNotNullAfterLoad()
    {
        yield return new WaitWhile(() => sceneLoaded == false);
        SetupReferences();
        Assert.IsNotNull(headquarters);
    }


    [UnityTest]
    public IEnumerator DronesInMissionAfterButtonPressed()
    {

        yield return new WaitWhile(() => sceneLoaded == false);
        SetupReferences();

        int numSpot = 1;
        var buttons = GameObject.FindGameObjectsWithTag(numSpot.ToString());
        var button = buttons[0].GetComponent<Button>();

        int nbDronesInMission = 0;
        foreach (var drone in headquarters.drones)
            if (drone.isActive)
                nbDronesInMission++;

        button.OnMouseDown();

        int nbDronesWanted = Mathf.Min(headquarters.drones.Length, nbDronesInMission + 6);

        foreach (var drone in headquarters.drones)
            if (drone.isActive)
            {
                nbDronesInMission++;
                dronesInMission.Add(drone);
            }

        Assert.AreEqual(nbDronesWanted, nbDronesInMission);
    }

    [UnityTest]
    public IEnumerator DroneWellPositionedAtEachStep() {

        yield return new WaitWhile(() => dronesInMission.Count == 0);
        SetupReferences();

        int index = Random.Range(0, dronesInMission.Count);
        Drone drone = dronesInMission[index];

        yield return new WaitWhile(() => drone.startFromStation == true);

        Assert.Greater(drone.gameObject.transform.position.y, drone.spot.gameObject.transform.position.y + 5);
        Assert.AreEqual(drone.gameObject.transform.position.x, drone.spot.transform.position.x, 0.1f);
        Assert.AreEqual(drone.gameObject.transform.position.z, drone.spot.transform.position.z, 0.1f);

        yield return new WaitWhile(() => drone.descendToSpot == true);

        Assert.AreEqual(drone.gameObject.transform.position.x, drone.spot.transform.position.x, 0.1f);
        Assert.AreEqual(drone.gameObject.transform.position.y, drone.spot.transform.position.y, 0.1f);
        Assert.AreEqual(drone.gameObject.transform.position.z, drone.spot.transform.position.z, 0.1f);

        yield return new WaitWhile(() => drone.detection == true);
        yield return new WaitForSeconds(0.1f);
        yield return new WaitWhile(() => drone.endOfMission == true);

        Assert.Greater(drone.gameObject.transform.position.y, drone.spot.gameObject.transform.position.y + 5);
        Assert.AreEqual(drone.gameObject.transform.position.x, drone.spot.transform.position.x, 0.1f);
        Assert.AreEqual(drone.gameObject.transform.position.z, drone.spot.transform.position.z, 0.1f);

        yield return new WaitWhile(() => drone.backToStation == true);

        Assert.AreEqual(drone.gameObject.transform.position, drone.initialPosition);
    }
}
