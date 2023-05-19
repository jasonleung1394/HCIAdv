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
    private GameObject fr3_J1, fr3_J2, fr3_J3, fr3_J4, fr3_J5, fr3_J6, fr3_J7;
    private GameObject fr3_J1_mimic, fr3_J2_mimic, fr3_J3_mimic, fr3_J4_mimic, fr3_J5_mimic, fr3_J6_mimic, fr3_J7_mimic;

    private List<float> preFrameAngle;

    private float[] avatarJointState { get; set; }
    private double[] robotJointState_CheckPoint = new double[7];

    public bool[] jointInitFlag = new bool[7];

    private float reset_speed = 0.5f;

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
    private LerpToInitialPose lerpToInitialPose;

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

        fr3_J1_mimic = GameObject.Find("fr3_link1_mimic");
        fr3_J2_mimic = GameObject.Find("fr3_link2_mimic");
        fr3_J3_mimic = GameObject.Find("fr3_link3_mimic");
        fr3_J4_mimic = GameObject.Find("fr3_link4_mimic");
        fr3_J5_mimic = GameObject.Find("fr3_link5_mimic");
        fr3_J6_mimic = GameObject.Find("fr3_link6_mimic");
        fr3_J7_mimic = GameObject.Find("fr3_link7_mimic");

        initialPoser();

        lerpToInitialPose = GetComponent<LerpToInitialPose>();
    }

    private Quaternion initial_Arm = Quaternion.Euler((1.59695f / 2) * Mathf.Rad2Deg, -0.785398163397f * Mathf.Rad2Deg, 0);

    private Quaternion initial_ForeArm = Quaternion.Euler(0, 0, -2.35619449019f * Mathf.Rad2Deg);

    private Quaternion initial_Hand = Quaternion.Euler(0, 0 * Mathf.Rad2Deg, 0);


    void initialPoser()
    {
        LerpToInitialPose lerpToInitialPose = GetComponent<LerpToInitialPose>();
        lerpToInitialPose.Lerp_Index = 1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        // No Collision

        OffsetValue offsetValue = GetComponent<OffsetValue>();

        reset_speed += Time.deltaTime * reset_speed;
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
            Mathf.Atan2(
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
        J4 = right_forearm.transform.localEulerAngles.z + offsetValue.forearm_pitch * Mathf.Rad2Deg;
        J5 = forearm_angle * forearm_axis.y + offsetValue.forearm_roll * Mathf.Rad2Deg;
        Vector3 hand_axis;
        float hand_angle;
        hand_rotation.ToAngleAxis(out hand_angle, out hand_axis);
        J6 = hand_angle * hand_axis.z + offsetValue.hand_pitch * Mathf.Rad2Deg;
        J7 = hand_angle * hand_axis.y + offsetValue.hand_roll * Mathf.Rad2Deg;

        List<float> jointAngles = new List<float> { J1, -J2, J3, J4, -J5, J6, J7 };
        jointAngleConstraint(jointAngles);
        moveMimic(jointAngles);
        if (lerpToInitialPose.Lerp_Index == 2)
        {
            jointAngles = preFrameAngle;
        }
        preFrameAngle = new List<float>();
        for (int i = 0; i < jointAngles.Count; i++)
        {
            preFrameAngle.Add(jointAngles[i]);
        }
        // let publisher know what to publish
        JointStatePublisher jointStatePublisher = GetComponent<JointStatePublisher>();
        double[] tmp = new double[7];
        for (int i = 0; i < jointAngles.Count; i++)
        {
            jointStatePublisher.jointAngles_double[i] = jointAngles[i] * Mathf.Deg2Rad;
            tmp[i] = (double)(tmp[i] * Mathf.Deg2Rad);
        }
        if (lerpToInitialPose.Lerp_Index == 0 && jointStatePublisher.jointStateBuffer.Count == 0)
        {
            jointStatePublisher.jointStateBuffer.Add(tmp);
            var record = jointStatePublisher.jointStateBuffer[0];
            jointStatePublisher.prev_handLocation = right_hand.transform.position;
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





        


    }
    private void rad2Deg(float[,] radArray)
    {
        for (int i = 0; i < radArray.GetLength(0); i++)
        {
            radArray[i, 0] = radArray[i, 0] * Mathf.Rad2Deg;
            radArray[i, 1] = radArray[i, 1] * Mathf.Rad2Deg;
        }
    }

    void moveMimic(List<float> jointAngles){
        // fr3_J1.transform.localEulerAngles = new Vector3(0, jointAngles[0], 0);
        fr3_J1_mimic.transform.localRotation = Quaternion.AngleAxis(-jointAngles[0], Vector3.up);

        fr3_J2_mimic.transform.localRotation =
            Quaternion.AngleAxis(-jointAngles[1], Vector3.left)
            * Quaternion.AngleAxis(-90f, Vector3.back);

        fr3_J3_mimic.transform.localRotation =
            Quaternion.AngleAxis(-jointAngles[2], Vector3.left)
            * Quaternion.AngleAxis(90f, Vector3.back);

        fr3_J4_mimic.transform.localRotation =
            Quaternion.AngleAxis(jointAngles[3], Vector3.left)
            * Quaternion.AngleAxis(90f, Vector3.back);

        fr3_J5_mimic.transform.localRotation =
            Quaternion.AngleAxis(-jointAngles[4], Vector3.left)
            * Quaternion.AngleAxis(-90f, Vector3.back);

        fr3_J6_mimic.transform.localRotation =
            Quaternion.AngleAxis(jointAngles[5], Vector3.left)
            * Quaternion.AngleAxis(-90f, Vector3.forward);

        fr3_J7_mimic.transform.localRotation =
            Quaternion.AngleAxis(jointAngles[6], Vector3.left)
            * Quaternion.AngleAxis(90f, Vector3.back);

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
        LerpToInitialPose lerpToInitialPose = GetComponent<LerpToInitialPose>();
        OffsetValue offsetValue = GetComponent<OffsetValue>();
        float[] Human_dpi_offset = { offsetValue.human_arm_yaw, offsetValue.human_arm_pitch, offsetValue.human_arm_roll, offsetValue.human_forearm_pitch, offsetValue.human_forearm_roll, offsetValue.human_hand_pitch, offsetValue.human_hand_roll };
        rad2Deg(constraintVal);
        for (int i = 0; i < jointAngles.Count; i++)
        {
            if (jointAngles[i] > 180f && i != 5)
            {
                jointAngles[i] = jointAngles[i] - 360f;
            }

            // total DOF of Robot
            var robotDOF = Mathf.Abs(constraintVal[i, 0] - constraintVal[i, 1]);
            if (lerpToInitialPose.Lerp_Index == 0)
            {
                jointAngles[i] = jointAngles[i] * Human_dpi_offset[i];
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
            }
            else
            {
                // how?
            }
        }
    }
}
