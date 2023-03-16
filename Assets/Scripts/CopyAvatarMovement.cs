using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyAvatarMovement : MonoBehaviour
{
    private GameObject avatar_human;
    private GameObject robotic_arm;
    private GameObject right_shoulder;
    private GameObject right_arm;
    private GameObject right_forearm;
    private GameObject right_hand;
    private GameObject Panda_J1;
    private GameObject Panda_J2;
    private GameObject Panda_J3;
    private GameObject Panda_J4;
    private GameObject Panda_J5;
    private GameObject Panda_J6;
    private GameObject Panda_J7;
    private float J1, J2, J3, J4, J5, J6, J7;
    // Start is called before the first frame update
    void Start()
    {
        avatar_human = GameObject.Find("Banana Man");
        robotic_arm = GameObject.Find("pandaAnimator");
        right_shoulder = GameObject.Find("Right Shoulder");
        right_arm = GameObject.Find("Right Arm");
        right_forearm = GameObject.Find("Right Forearm");
        right_hand = GameObject.Find("Right Hand");

        Panda_J1=GameObject.Find("panda_link1");
        Panda_J2=GameObject.Find("panda_link2");
        Panda_J3=GameObject.Find("panda_link3");
        Panda_J4=GameObject.Find("panda_link4");
        Panda_J5=GameObject.Find("panda_link5");
        Panda_J6=GameObject.Find("panda_link6");
        Panda_J7=GameObject.Find("panda_link7");
    }

    // Update is called once per frame
    void Update()
    {
        J1 = right_shoulder.transform.localEulerAngles.x;
        J2 = right_shoulder.transform.localEulerAngles.z;
        J3 = right_arm.transform.localEulerAngles.y;
        J4 = right_forearm.transform.localEulerAngles.z;
        J5 = right_forearm.transform.localEulerAngles.y;
        J6 = right_hand.transform.localEulerAngles.z;
        J7 = right_hand.transform.localEulerAngles.y;
        //right_shoulder.transform.localEulerAngles = new Vector3(0, 0, 50);

        Panda_J1.transform.localEulerAngles = new Vector3(0,J1,0);
        Panda_J2.transform.localEulerAngles = new Vector3(J2+90, 0, 90);
        Panda_J3.transform.localEulerAngles = new Vector3(J3, 0, -90);
        Panda_J4.transform.localEulerAngles = new Vector3(J4, 0, -90);
        Panda_J5.transform.localEulerAngles = new Vector3(J5, 0, 90);
        Panda_J6.transform.localEulerAngles = new Vector3(J6-180, 0, -90);
        Panda_J7.transform.localEulerAngles = new Vector3(J7, 0, -90);
        
    }
}
