using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton : MonoBehaviour
{
    public static Singleton instance;
    private Vector3 Position;
    private Transform cam;
    private Transform planetObjective;
    private bool active;
    private Vector3 generalCamPosition = new Vector3(0.0f, 200.0f, 0.0f);
    private float Timer;
    // Start is called before the first frame update

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        active = false;
        cam = GameObject.Find("Player").transform;
        Position = new Vector3(0.0f, 200.0f, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (active) goToSpot();
        else moving();
    }

    internal void moving()
    {
        if (planetObjective.name == "Sun_GEO")
        {
            if (cam.position != generalCamPosition)
            { //Transformamos solo si no estamos ya en la camara general
                //destiny = generalCamPosition;
                //cam.rotation = Quaternion.Euler(-90.0f, 180.0f, 90.0f);
            }
            Vector3 transition = new Vector3(generalCamPosition.x - cam.position.x, generalCamPosition.y - cam.position.y, generalCamPosition.z - cam.position.z);
            cam.Translate(transition * Time.deltaTime);
        }
        else
        {
            Vector3 destinyPosition = new Vector3(planetObjective.position.x - 1.5f * (1 + planetObjective.parent.transform.localScale.x), planetObjective.position.y, planetObjective.position.z);
            if (cam.position != destinyPosition)
            {
                if (cam.position == generalCamPosition)
                {//Rotamos solo si procedemos de el sol
                    //cam.rotation = Quaternion.Euler(0.0f, 90.0f, -180.0f);
                }
                Vector3 transition = new Vector3(destinyPosition.x - cam.position.x, destinyPosition.y - cam.position.y, destinyPosition.z - cam.position.z);
                cam.Translate(transition*Time.deltaTime);
                //cam.position = destinyPosition;
            }
        }
        Timer += Time.deltaTime;
        if (Timer > 1.0f) active = true;
    }

    internal void goToSpot()
    {
        if (planetObjective.name == "Sun_GEO")
        {
            cam.position = generalCamPosition;
        }
        else
        {
            Vector3 destinyPosition = new Vector3(planetObjective.position.x - 1.5f * (1 + planetObjective.parent.transform.localScale.x), planetObjective.position.y, planetObjective.position.z);
            if (cam.position != destinyPosition)
            {
              cam.position = destinyPosition;
            }
        }
    }

    internal void setPlanet(Transform current)
    {
        Timer = 0.0f;
        active = false;
        planetObjective = current;
    }

    internal bool isSun()
    {
        Debug.Log("IsSun function: " + planetObjective.name + "   " + (planetObjective.name == "Sun_GEO"));
        return (planetObjective.name == "Sun_GEO");
    }
}
