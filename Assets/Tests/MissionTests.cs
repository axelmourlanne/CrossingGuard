using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement; 

public class MissionTests
{
    private ControlStation headquarters;

    bool sceneLoaded;
    bool referencesSetup;

    
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
            if (t.name == "Headquarters")
                headquarters = t.GetComponent<ControlStation>();
        
        referencesSetup = true;
    }
    
    [UnityTest]
    public IEnumerator TestReferencesNotNullAfterLoad()
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
                nbDronesInMission++;

        Assert.AreEqual(nbDronesWanted, nbDronesInMission);
    }
}
