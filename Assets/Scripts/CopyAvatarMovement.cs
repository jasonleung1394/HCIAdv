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

    private float J1,
        J2,
        J3,
        J4,
        J5,
        J6,
        J7;

    // Start is called before the first frame update
    void Start()
    {
        avatar_human = GameObject.Find("Banana Man");
        robotic_arm = GameObject.Find("pandaAnimator");
        right_shoulder = GameObject.Find("Right Shoulder");
        right_arm = GameObject.Find("Right Arm");
        right_forearm = GameObject.Find("Right Forearm");
        right_hand = GameObject.Find("Right Hand");

        Panda_J1 = GameObject.Find("panda_link1");
        Panda_J2 = GameObject.Find("panda_link2");
        Panda_J3 = GameObject.Find("panda_link3");
        Panda_J4 = GameObject.Find("panda_link4");
        Panda_J5 = GameObject.Find("panda_link5");
        Panda_J6 = GameObject.Find("panda_link6");
        Panda_J7 = GameObject.Find("panda_link7");
    }

    // Update is called once per frame
    void Update()
    {
        // assuming T pose is the origin pose
        Quaternion arm_rotation = right_arm.transform.localRotation;
        Quaternion forearm_rotation = right_forearm.transform.localRotation;
        Quaternion hand_rotation = right_hand.transform.localRotation;
        // J1 = right_arm.transform.localEulerAngles.y;
        J1 = Quaternion.FromToRotation(Vector3.left, arm_rotation * Vector3.left).eulerAngles.y;
        J2 = Quaternion.FromToRotation(Vector3.right, arm_rotation * Vector3.right).eulerAngles.z;
        J3 = Quaternion.FromToRotation(Vector3.up, arm_rotation * Vector3.up).eulerAngles.x;
        // J2 = right_arm.transform.localEulerAngles.z;
        // J3 = right_arm.transform.localEulerAngles.x;
        J4 = Quaternion
            .FromToRotation(Vector3.right, forearm_rotation * Vector3.right)
            .eulerAngles.z;
        J5 = Quaternion
            .FromToRotation(Vector3.right, forearm_rotation * Vector3.right)
            .eulerAngles.y;
        J6 = Quaternion.FromToRotation(Vector3.right, hand_rotation * Vector3.right).eulerAngles.z;
        J7 = Quaternion.FromToRotation(Vector3.right, hand_rotation * Vector3.right).eulerAngles.y;

        //right_shoulder.transform.localEulerAngles = new Vector3(0, 0, 50);
        // joint angle constraint
        Debug.Log(J6);
        List<float> jointAngles = new List<float> { J1, J2, J3, J4, J5, J6, J7 };
        jointAngleConstraint(jointAngles);

        // Debug.Log(jointAngles[1]);

        // Panda_J1.transform.localEulerAngles = new Vector3(0, jointAngles[0], 0);
        Panda_J1.transform.localRotation = Quaternion.AngleAxis(jointAngles[0], Vector3.up);

        Panda_J2.transform.localRotation =
            Quaternion.AngleAxis(jointAngles[1], Vector3.left)
            * Quaternion.AngleAxis(-90f, Vector3.back);
        Panda_J3.transform.localRotation =
            Quaternion.AngleAxis(jointAngles[2], Vector3.left)
            * Quaternion.AngleAxis(90f, Vector3.back);
        Panda_J4.transform.localRotation =
            Quaternion.AngleAxis(jointAngles[3], Vector3.left)
            * Quaternion.AngleAxis(90f, Vector3.back);
        Panda_J5.transform.localRotation =
            Quaternion.AngleAxis(jointAngles[4], Vector3.left)
            * Quaternion.AngleAxis(-90f, Vector3.back);
        Panda_J6.transform.localRotation =
            Quaternion.AngleAxis(jointAngles[5], Vector3.left)
            * Quaternion.AngleAxis(-90, Vector3.forward);
        Panda_J7.transform.localRotation =
            Quaternion.AngleAxis(jointAngles[6], Vector3.left)
            * Quaternion.AngleAxis(90, Vector3.back);
        // Panda_J2.transform.rotation =
        // Panda_J2.transform.localEulerAngles = new Vector3(jointAngles[1], 0, 90);
        // Panda_J4.transform.localEulerAngles = new Vector3(-jointAngles[3], 0, -90);
    }

    private void jointAngleConstraint(List<float> jointAngles)
    {
        float[,] constraintVal = new float[,]
        {
            { 157.20243f, -157.20243f },
            { 102.198482f, -102.198482f },
            { 166.197868f, -166.197868f },
            { -8.69749933f, -174.299491f },
            { 160.800605f, -160.800605f },
            { 258.799306f, 31.1975519f },
            { 172.798341f, -172.798341f }
        };
        for (int i = 0; i < jointAngles.Count; i++)
        {
            if (jointAngles[i] > 180f && i != 5)
            {
                jointAngles[i] = jointAngles[i] - 360f;
            }
            if (constraintVal[i, 0] > jointAngles[i] && constraintVal[i, 1] < jointAngles[i]) { }
            else if (constraintVal[i, 0] < jointAngles[i])
            {
                // larger than max
                jointAngles[i] = constraintVal[i, 0];
                // Debug.Log("joint " + (i + 1f) + "  out of Range");
            }
            else if (constraintVal[i, 1] > jointAngles[i])
            {
                // lower than min
                jointAngles[i] = constraintVal[i, 1];

                // Debug.Log("joint " + (i + 1f) + "  out of Range");
            }
            else
            {
                // how?
            }
        }
    }
}
