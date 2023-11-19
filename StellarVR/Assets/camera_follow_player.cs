using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class follow_player : MonoBehaviour
{
    public Transform player;

    // Update is called once per frame
    void Start () {
        transform.position = player.transform.position + new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update () {
        transform.position = player.transform.position + new Vector3(0, 0, 0);
    }
}
