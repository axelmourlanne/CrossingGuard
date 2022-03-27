using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class CarTests
{
    bool sceneLoaded;
    bool referencesSetup;
    private Drone drone;
    private Car car;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
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
            if (t.name == "Car")
            {
                car = t.GetComponent<Car>();
            }
            else if (t.name == "Drone01")
            {
                drone = t.GetComponent<Drone>();
            }
        }

        GameObject droneGO = GameObject.Instantiate(drone.gameObject, Vector3.zero, Quaternion.identity);
        drone = droneGO.GetComponent<Drone>();

        GameObject carGO = GameObject.Instantiate(car.gameObject, Vector3.zero, Quaternion.identity);
        car = carGO.GetComponent<Car>();

        referencesSetup = true;
    }

    [UnityTest]
    public IEnumerator CarStopsWhenDroneInFront()
    {
        yield return new WaitWhile(() => sceneLoaded == false);
        SetupReferences();
        
        Vector3 carPosition = new Vector3(-10, 0, 0);
        car.transform.position = carPosition;

        yield return new WaitForSeconds(5);

        Vector3 beforePosition = car.transform.position;

        yield return new WaitForSeconds(1);

        Assert.AreEqual(Vector3.Distance(beforePosition, car.transform.position), 0);
    }

    [UnityTest]
    public IEnumerator CarDoesNotStopsWhenNothingInFront()
    {
        yield return new WaitWhile(() => sceneLoaded == false);
        SetupReferences();
        
        Vector3 carPosition = new Vector3(10, 0, 0);
        car.transform.position = carPosition;

        yield return new WaitForSeconds(5);

        Vector3 beforePosition = car.transform.position;

        yield return new WaitForSeconds(1);

        Assert.AreNotEqual(Vector3.Distance(beforePosition, car.transform.position), 0);
    }

}
