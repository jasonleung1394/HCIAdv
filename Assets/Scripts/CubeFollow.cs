using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CubeFollow : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject follow_target;
    public GameObject mainObj;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        mainObj.transform.position = follow_target.transform.position;

    }
}
