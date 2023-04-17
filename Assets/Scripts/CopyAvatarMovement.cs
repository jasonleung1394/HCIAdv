using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyAvatarMovement : MonoBehaviour
{
    private GameObject avatar_human;
    private GameObject robotic_arm;
    private GameObject right_arm;
    private GameObject right_forearm;
    private GameObject right_hand;
    private GameObject fr3_J1;
    private GameObject fr3_J2;
    private GameObject fr3_J3;
    private GameObject fr3_J4;
    private GameObject fr3_J5;
    private GameObject fr3_J6;
    private GameObject fr3_J7;

    private List<float> preFrameAngle;

    private double[] avatarJointState { get; set; }
    private double[] robotJointState_CheckPoint = new double[7];
    private int initFlag;

    public bool[] jointInitFlag = new bool[7];

    private float reset_speed = 0.5f;

    private float checkPointTime { get; set; }
    private int waitTime_checkPoint = 3;

    // make these into slider in the inspector, to ensure when player standing facing the robot, the dof of each joint will be 180 degree and front facing
    public Vector3 AvatarArm_Offset { get; set; }
    public Vector3 AvatarForeArm_Offset { get; set; }
    public Vector3 AvatarHand_Offset { get; set; }

    private float[] initialPose = new float[] { 0, 0, 0f, -1.59695f, 0, 2.53075f, 0 };

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
        right_arm = GameObject.Find("Right Arm");
        right_forearm = GameObject.Find("Right Forearm");
        right_hand = GameObject.Find("Right Hand");

        fr3_J1 = GameObject.Find("fr3_link1");
        fr3_J2 = GameObject.Find("fr3_link2");
        fr3_J3 = GameObject.Find("fr3_link3");
        fr3_J4 = GameObject.Find("fr3_link4");
        fr3_J5 = GameObject.Find("fr3_link5");
        fr3_J6 = GameObject.Find("fr3_link6");
        fr3_J7 = GameObject.Find("fr3_link7");

        initialPoser();

        initFlag = 0;
    }

    private Quaternion initial_Arm = Quaternion.Euler((1.59695f / 2) * Mathf.Rad2Deg, 0, 0);

    private Quaternion initial_ForeArm = Quaternion.Euler(0, 0, (-1.59695f / 2) * Mathf.Rad2Deg);

    private Quaternion initial_Hand = Quaternion.Euler(0, 0 * Mathf.Rad2Deg, 0);


    void initialPoser()
    {
        LerpToInitialPose lerpToInitialPose = GetComponent<LerpToInitialPose>();
        lerpToInitialPose.Lerp_Index = 1;
    }

    // Update is called once per frame
    void Update()
    {
        OffsetValue offsetValue = GetComponent<OffsetValue>();

        reset_speed += Time.deltaTime * reset_speed;
        checkPointTime += Time.deltaTime;
        // assuming T pose is the origin pose
        Quaternion arm_rotation = right_arm.transform.localRotation;
        Quaternion forearm_rotation = right_forearm.transform.localRotation;
        Quaternion hand_rotation = right_hand.transform.localRotation;

        J1 =
            Mathf.Atan2(
                2 * (arm_rotation.x * arm_rotation.w),
                1 - 2 * (arm_rotation.x * arm_rotation.x)
            ) * Mathf.Rad2Deg + offsetValue.arm_yaw * Mathf.Rad2Deg;
        J2 =
            -Mathf.Atan2(
                2 * (arm_rotation.z * arm_rotation.w),
                1 - 2 * (arm_rotation.z * arm_rotation.z)
            ) * Mathf.Rad2Deg + offsetValue.arm_pitch * Mathf.Rad2Deg;
        J3 =
            Mathf.Atan2(
                2 * (arm_rotation.y * arm_rotation.w),
                1 - 2 * (arm_rotation.y * arm_rotation.y)
            ) * Mathf.Rad2Deg + offsetValue.arm_roll * Mathf.Rad2Deg;
        Vector3 forearm_axis;
        float forearm_angle;
        forearm_rotation.ToAngleAxis(out forearm_angle, out forearm_axis);
        J4 = forearm_angle * forearm_axis.z + offsetValue.forearm_pitch * Mathf.Rad2Deg;
        J5 = -forearm_angle * forearm_axis.y + offsetValue.forearm_roll * Mathf.Rad2Deg;

        Vector3 hand_axis;
        float hand_angle;
        hand_rotation.ToAngleAxis(out hand_angle, out hand_axis);
        J6 = hand_angle * hand_axis.z + offsetValue.hand_pitch * Mathf.Rad2Deg;
        J7 = hand_angle * hand_axis.y + offsetValue.hand_roll * Mathf.Rad2Deg;
        List<float> jointAngles = new List<float> { J1, J2, J3, J4, J5, J6, J7 };
        avatarJointState = new double[] { J1, J2, J3, J4, J5, J6, J7 };
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

        // fr3_J1.transform.localEulerAngles = new Vector3(0, jointAngles[0], 0);
        fr3_J1.transform.localRotation = Quaternion.AngleAxis(-jointAngles[0], Vector3.up);

        fr3_J2.transform.localRotation =
            Quaternion.AngleAxis(-jointAngles[1], Vector3.left)
            * Quaternion.AngleAxis(-90f, Vector3.back);

        fr3_J3.transform.localRotation =
            Quaternion.AngleAxis(-jointAngles[2], Vector3.left)
            * Quaternion.AngleAxis(90f, Vector3.back);

        fr3_J4.transform.localRotation =
            Quaternion.AngleAxis(jointAngles[3], Vector3.left)
            * Quaternion.AngleAxis(90f, Vector3.back);

        fr3_J5.transform.localRotation =
            Quaternion.AngleAxis(-jointAngles[4], Vector3.left)
            * Quaternion.AngleAxis(-90f, Vector3.back);

        fr3_J6.transform.localRotation =
            Quaternion.AngleAxis(jointAngles[5], Vector3.left)
            * Quaternion.AngleAxis(-90f, Vector3.forward);

        fr3_J7.transform.localRotation =
            Quaternion.AngleAxis(jointAngles[6], Vector3.left)
            * Quaternion.AngleAxis(90f, Vector3.back);

        preFrameAngle = new List<float>();
        for (int i = 0; i < jointAngles.Count; i++)
        {
            preFrameAngle.Add(jointAngles[i]);
        }
        if (checkPointTime > waitTime_checkPoint)
        {
            // init a check point
            detectUndesiredJointAngle(robotJointState_CheckPoint, jointAngles);
            checkPointTime = 0f;
        }
        else if (checkPointTime == Time.deltaTime)
        {
            for (int i = 0; i < jointAngles.Count; i++)
            {
                robotJointState_CheckPoint[i] = jointAngles[i];
            }
        }
        else
        {
            // do nothing
        }

        initFlag = 1;
    }

    void detectUndesiredJointAngle(double[] robotJointState, List<float> cur_jointState)
    {
        InitialProcedure initialProcedure = GetComponent<InitialProcedure>();

        bool detectFlag = false;
        // check the status of robotJointAngles, check if it has not been moving for a while
        for (int i = 0; i < robotJointState.Length; i++)
        {
            if ((int)(robotJointState[i]) != (int)(cur_jointState[i]))
            {
                // meaning it has changed
                detectFlag = true;
            }
        }
        if (detectFlag == false && initialProcedure.autoReset == true)
        {
            initFlag = 0;
            initialPoser();
            preFrameAngle = new List<float>();
        }
        robotJointState_CheckPoint = new double[7];
        // avatar joint sync to robot joint
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
        InitialProcedure initialProcedure = GetComponent<InitialProcedure>();
        initialProcedure.overSpeedFlag = true;


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

            float jointVel = Mathf.Abs(desiredJointAngle - currentJointAngle) / Time.deltaTime;


            if (jointVel >= velConstraintVal[i])
            {
                // too fast
                jointAngles[i] = desiredJointAngle - (jointVel * Time.deltaTime);
                initialProcedure.overSpeedFlag = false;
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
            { 2.3093f, -2.3093f }, // 132 ~ -132
            { 1.5133f, -1.5133f }, // 86.7057031 ~ -86.7057031
            { 2.4937f, -2.4937f }, // 142 ~ -142.878485
            { -0.4461f, -2.7478f }, // -25 ~ -157 
            { 2.4800f, -2.4800f }, // 142.0935 ~ -142.0935
            { 4.2094f, 0.8521f }, // 241 ~ 48 changed to
            { 2.6895f, -2.6895f } // 154 ~ -154
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
