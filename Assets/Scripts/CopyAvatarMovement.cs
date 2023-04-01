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

    private List<float> preFrameAngle;

    private int initFlag;

    public bool[] jointInitFlag = new bool[7];

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

        Panda_J1 = GameObject.Find("fr3_link1");
        Panda_J2 = GameObject.Find("fr3_link2");
        Panda_J3 = GameObject.Find("fr3_link3");
        Panda_J4 = GameObject.Find("fr3_link4");
        Panda_J5 = GameObject.Find("fr3_link5");
        Panda_J6 = GameObject.Find("fr3_link6");
        Panda_J7 = GameObject.Find("fr3_link7");

        initFlag = 0;
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
        List<float> jointAngles = new List<float> { J1, J2, J3, J4, J5, J6, J7 };
        jointAngleConstraint(jointAngles);
        if (initFlag != 0)
        {
            jointVelocityConstraint(jointAngles);
        }
        JointStatePublisher jointStatePublisher = GetComponent<JointStatePublisher>();
        for (int i = 0; i < jointAngles.Count; i++)
        {
            jointStatePublisher.jointAngles_double[i] = jointAngles[i] * Mathf.Deg2Rad;
        }
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
        preFrameAngle = new List<float>();
        for (int i = 0; i < jointAngles.Count; i++)
        {
            preFrameAngle.Add(jointAngles[i]);
        }

        initFlag = 1;
    }

    public double[] cur_jointAngles { get; set; }

    public bool ifPosSynced(double[] jointPos)
    {
        bool flag = true;
        JointStatePublisher jointStatePublisher = GetComponent<JointStatePublisher>();
        cur_jointAngles = jointStatePublisher.jointAngles_double;

        for (int i = 0; i < cur_jointAngles.Length; i++)
        {
            double diff;
            diff = cur_jointAngles[i] - jointPos[i];
            diff = Mathf.Abs((float)diff);

            if (diff < 0.2f)
            {
                // close enough
                jointInitFlag[i] = true;
            }
            else
            {
                jointInitFlag[i] = false;
                flag = false;
            }
        }

        return flag;
    }

    private void jointVelocityConstraint(List<float> jointAngles)
    {
        // list of max || min velocity
        float[] velConstraintVal = new float[] { 2f, 1f, 1.5f, 1.25f, 3f, 1.5f, 3f };
        for (int i = 0; i < velConstraintVal.Length; i++)
        {
            velConstraintVal[i] = velConstraintVal[i] * Mathf.Rad2Deg;
        }

        for (int i = 0; i < preFrameAngle.Count; i++)
        {
            var currentJointAngle = preFrameAngle[i];
            var desiredJointAngle = jointAngles[i];

            float jointVel = (desiredJointAngle - currentJointAngle) / Time.deltaTime;

            if (jointVel >= velConstraintVal[i])
            {
                // too fast
                jointAngles[i] = desiredJointAngle - (jointVel * Time.deltaTime);
            }
            else
            {
                jointAngles[i] = jointAngles[i];
            }
        }
    }

    private void rad2Deg(float[,] radArray)
    {
        for (int i = 0; i < radArray.GetLength(0); i++)
        {
            radArray[i, 0] = radArray[i, 0] * Mathf.Rad2Deg;
            radArray[i, 1] = radArray[i, 1] * Mathf.Rad2Deg;
        }
    }

    private void jointAngleConstraint(List<float> jointAngles)
    {
        float[,] constraintVal = new float[,]
        {
            { 2.3093f, -2.3093f },
            { 1.5133f, -1.5133f },
            { 2.4937f, -2.4937f },
            { -0.4461f, -2.7478f },
            { 2.4800f, -2.4800f },
            { 4.2094f, 0.8521f },
            { 2.6895f, -2.6895f }
        };
        rad2Deg(constraintVal);
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
