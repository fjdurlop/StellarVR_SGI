using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{

    private Transform Sun;
    private Transform planet;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        planet = this.transform;
        Sun = GameObject.Find("Sun").transform;
    }

    // Update is called once per frame
    void Update()
    {
        planet.RotateAround(Sun.position, Vector3.up, speed * Time.deltaTime);
    }
}
