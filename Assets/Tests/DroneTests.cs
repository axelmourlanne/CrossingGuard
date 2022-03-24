using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class DroneTests
{

    bool sceneLoaded;
    bool referencesSetup;

    private ControlStation headquarters;
    private Drone drone;
    private List<Drone> drones;
    private int nbDrones = 5;


    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        drones = new List<Drone>();
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
        {
            if (t.name == "Headquarters")
            {
                headquarters = t.GetComponent<ControlStation>();
            }
            else if (t.name == "Drone01")
            {
                drone = t.GetComponent<Drone>();
            }
        }

        for (int i = 0; i < nbDrones; i++) {
            GameObject droneGO = GameObject.Instantiate(drone.gameObject, Vector3.zero, Quaternion.identity);
            drones.Add(droneGO.GetComponent<Drone>());
        }

        referencesSetup = true;
    }
    
    [UnityTest]
    public IEnumerator ReferencesNotNullAfterLoad()
    {
        yield return new WaitWhile(() => sceneLoaded == false);
        SetupReferences();
        Assert.IsNotNull(drone);
    }


    [UnityTest]
    public IEnumerator NoCollisionBetweenTwoDrones()
    {
        yield return new WaitWhile(() => sceneLoaded == false);
        SetupReferences();

        Drone d1 = drones[0], d2 = drones[1];
        d1.id = 101;
        d1.transform.position = new Vector3(10, 3, 0);
        d1.targetPosition = new Vector3(0, 3, 0);
        d2.id = 102;
        d2.transform.position = new Vector3(0, 3, 0);
        d2.targetPosition = new Vector3(10, 3, 0);

        var dronesArray = new Drone[2];
        dronesArray[0] = d1;
        dronesArray[1] = d2;
        headquarters.drones = dronesArray;

        yield return new WaitForSeconds(0.1f);

        while (d1.transform.position != d1.targetPosition) {
            d1.BroadcastPosition();
            d1.Move();
            d2.BroadcastPosition();
            d2.Move();
            Assert.GreaterOrEqual(Vector3.Distance(d1.transform.position, d2.transform.position), 0.5f);
            yield return new WaitForSeconds(0.01f);
        }

    }

}
