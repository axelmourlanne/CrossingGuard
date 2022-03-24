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
    private int nbDrones = 4;


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
            Drone droneComponent = droneGO.GetComponent<Drone>();
            droneComponent.id = i;
            droneComponent.tooCloseDrones = new Dictionary<int, bool>();
            drones.Add(droneComponent);
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

        Drone d1 = drones[0], d2 = drones[1], d3 = drones[2], d4 = drones[3];
        d1.transform.position = new Vector3(10, 2.5f, 0);
        d1.targetPosition = new Vector3(0, 2.5f, 0);
        d2.transform.position = new Vector3(0, 3, 0);
        d2.targetPosition = new Vector3(10, 3, 0);
        d3.transform.position = new Vector3(10, 3f, 3);
        d3.targetPosition = new Vector3(0, 3, 3);
        d4.transform.position = new Vector3(0, 2.5f, 3);
        d4.targetPosition = new Vector3(10, 2.5f, 3);

        headquarters.drones = drones.ToArray();

        while (d1.transform.position != d1.targetPosition && d2.transform.position != d2.targetPosition && d3.transform.position != d3.targetPosition && d4.transform.position != d4.targetPosition) {
            d1.BroadcastPosition();
            d1.Move();
            d2.BroadcastPosition();
            d2.Move();
            Assert.GreaterOrEqual(Vector3.Distance(d1.transform.position, d2.transform.position), 0.5f);
            d3.BroadcastPosition();
            d3.Move();
            d4.BroadcastPosition();
            d4.Move();
            Assert.GreaterOrEqual(Vector3.Distance(d3.transform.position, d4.transform.position), 0.5f);
            yield return new WaitForSeconds(0.05f);
        }

    }

    [UnityTest]
    public IEnumerator NoCollisionBetweenNumerousDrones()
    {
        yield return new WaitWhile(() => sceneLoaded == false);
        SetupReferences();

        Drone d1 = drones[0], d2 = drones[1], d3 = drones[2], d4 = drones[3];
        d4.transform.position = new Vector3(10, 3, 0);
        d4.targetPosition = new Vector3(0, 3, 0);
        d2.transform.position = new Vector3(0, 3, 0);
        d2.targetPosition = new Vector3(10, 3, 0);
        d3.transform.position = new Vector3(0, 4, 0);
        d3.targetPosition = new Vector3(10, 4, 0);
        d1.transform.position = new Vector3(0, 5, 0);
        d1.targetPosition = new Vector3(10, 5, 0);

        headquarters.drones = drones.ToArray();

        while (d1.transform.position != d1.targetPosition && d2.transform.position != d2.targetPosition && d3.transform.position != d3.targetPosition && d4.transform.position != d4.targetPosition) {
            d1.BroadcastPosition();
            d1.Move();
            d2.BroadcastPosition();
            d2.Move();
            d3.BroadcastPosition();
            d3.Move();
            d4.BroadcastPosition();
            d4.Move();
            Assert.GreaterOrEqual(Vector3.Distance(d1.transform.position, d2.transform.position), 0.5f);
            Assert.GreaterOrEqual(Vector3.Distance(d3.transform.position, d4.transform.position), 0.5f);
            Assert.GreaterOrEqual(Vector3.Distance(d1.transform.position, d3.transform.position), 0.5f);
            Assert.GreaterOrEqual(Vector3.Distance(d2.transform.position, d4.transform.position), 0.5f);
            Assert.GreaterOrEqual(Vector3.Distance(d1.transform.position, d4.transform.position), 0.5f);
            Assert.GreaterOrEqual(Vector3.Distance(d2.transform.position, d3.transform.position), 0.5f);
            yield return new WaitForSeconds(0.01f);
        }

    }

}
