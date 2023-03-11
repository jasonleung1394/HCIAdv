using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;


public class RobotJointStatePublisher : MonoBehaviour
{
    private GameObject avatar_human;
    private GameObject robotic_arm;
    private GameObject right_shoulder;
    private GameObject right_arm;
    private GameObject right_forearm;
    private GameObject right_hand;
    private GameObject Panda_J1_an;
    private GameObject Panda_J2_an;
    private GameObject Panda_J3_an;
    private GameObject Panda_J4_an;
    private GameObject Panda_J5_an;
    private GameObject Panda_J6_an;
    private GameObject Panda_J7_an;
    private float[] previousFrameMovement;
    private float J1, J2, J3, J4, J5, J6, J7;


    // Start is called before the first frame update
    void Start()
    {
        //initialize
        avatar_human = GameObject.Find("Banana Man An");
        robotic_arm = GameObject.Find("pandaAnimator");
        right_shoulder = GameObject.Find("Right Shoulder an");
        right_arm = GameObject.Find("Right Arm an");
        right_forearm = GameObject.Find("Right Forearm an");
        right_hand = GameObject.Find("Right Hand an");

    }

    // Update is called once per frame
    void Update()
    {
        previousFrameMovement =new float[] {right_shoulder.transform.localEulerAngles.x, right_shoulder.transform.localEulerAngles.z, right_arm.transform.localEulerAngles.y, right_forearm.transform.localEulerAngles.z,right_forearm.transform.localEulerAngles.y,right_hand.transform.localEulerAngles.z,right_hand.transform.localEulerAngles.y};
        J1 = right_shoulder.transform.localEulerAngles.x;
        J2 = right_shoulder.transform.localEulerAngles.z;
        J3 = right_arm.transform.localEulerAngles.y;
        J4 = right_forearm.transform.localEulerAngles.z;
        J5 = right_forearm.transform.localEulerAngles.y;
        J6 = right_hand.transform.localEulerAngles.z;
        J7 = right_hand.transform.localEulerAngles.y;
        //right_shoulder.transform.localEulerAngles = new Vector3(0, 0, 50);
        Debug.Log(right_shoulder.transform.localEulerAngles);
        Panda_J1_an=GameObject.Find("Panda_J1_an");
        Panda_J2_an=GameObject.Find("Panda_J2_an");
        Panda_J3_an=GameObject.Find("Panda_J3_an");
        Panda_J4_an=GameObject.Find("Panda_J4_an");
        Panda_J5_an=GameObject.Find("Panda_J5_an");
        Panda_J6_an=GameObject.Find("Panda_J6_an");
        Panda_J7_an=GameObject.Find("Panda_J7_an");
        Panda_J1_an.transform.localEulerAngles = new Vector3(0,0,J1);
        Panda_J2_an.transform.localEulerAngles = new Vector3(90, 0, J2);
        Panda_J3_an.transform.localEulerAngles = new Vector3(-90, 0, J3);
        Panda_J4_an.transform.localEulerAngles = new Vector3(-90, 0, J4);
        Panda_J5_an.transform.localEulerAngles = new Vector3(90, 0, J5);
        Panda_J6_an.transform.localEulerAngles = new Vector3(-90, 0, J6);
        Panda_J7_an.transform.localEulerAngles = new Vector3(-90, 0, J7);
        // should pitch and roll: joint 1 x axis & joint 2 z axis; showder yaw: joint 3 arm an y axis; 
        // elbow pitch: joint 4 forarm-an z axis; elbow yaw: joint 5 forearm-an y axis
        // write pitch: joint 6 hand-an z; wrist yaw: joint 7 hand-an y axis
    }
}
