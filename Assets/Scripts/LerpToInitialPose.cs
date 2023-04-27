using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LerpToInitialPose : MonoBehaviour
{
    private int lerp_Index = 0;
    public int Lerp_Index { get { return lerp_Index; } set { lerp_Index = value; } }

    private Transform arm_transform { get; set; }
    private Transform foreArm_transform { get; set; }
    private Transform hand_transform { get; set; }

    public GameObject reset_warning;

    public float lerpSpeed = 1f;

    void Start()
    {
        arm_transform = GameObject.Find("Right Arm").transform;
        foreArm_transform = GameObject.Find("Right Forearm").transform;
        hand_transform = GameObject.Find("Right Hand").transform;

    }
    private Quaternion initial_Arm = Quaternion.Euler((1.59695f / 2) * Mathf.Rad2Deg, 0, 0);

    private Quaternion initial_ForeArm = Quaternion.Euler(0, 0, (-1.59695f / 2) * Mathf.Rad2Deg);

    private Quaternion initial_Hand = Quaternion.Euler(0, 0 * Mathf.Rad2Deg, 0);

    public Quaternion RosArm;
    public Quaternion RosForeArm;
    public Quaternion RosHand;


    void Update()
    {
        Text reset_text = reset_warning.GetComponentInChildren<Text>();


        if (Lerp_Index == 1)
        {
            arm_transform.localRotation = Quaternion.Lerp(arm_transform.localRotation, initial_Arm, lerpSpeed * Time.deltaTime);
            foreArm_transform.localRotation = Quaternion.Lerp(foreArm_transform.localRotation, initial_ForeArm, lerpSpeed * Time.deltaTime);
            hand_transform.localRotation = Quaternion.Lerp(hand_transform.localRotation, initial_Hand, lerpSpeed * Time.deltaTime);
            reset_text.text = "Robot Pose Resetting !! ";
        }
        else if (lerp_Index == 2)
        {
            // syncing human to robot
            arm_transform.localRotation = Quaternion.Lerp(arm_transform.localRotation, RosArm, lerpSpeed * Time.deltaTime);
            foreArm_transform.localRotation = Quaternion.Lerp(foreArm_transform.localRotation, RosForeArm, lerpSpeed * Time.deltaTime);
            hand_transform.localRotation = Quaternion.Lerp(hand_transform.localRotation, RosHand, lerpSpeed * Time.deltaTime);
            reset_text.text = "Robot Pose Syncing !! ";
        }

        if (arm_transform.localRotation == initial_Arm && foreArm_transform.localRotation == initial_ForeArm && hand_transform.localRotation == initial_Hand && lerp_Index == 1)
        {
            lerp_Index = 0;
            reset_text.text = "";

        }
        else if (arm_transform.localRotation == RosArm && foreArm_transform.localRotation == RosForeArm && hand_transform.localRotation == RosHand && lerp_Index == 2)
        {
            lerp_Index = 0;
            reset_text.text = "";
        }
        else
        {
            arm_transform = GameObject.Find("Right Arm").transform;
            foreArm_transform = GameObject.Find("Right Forearm").transform;
            hand_transform = GameObject.Find("Right Hand").transform;
        }

        Debug.Log(lerp_Index);
    }
}
