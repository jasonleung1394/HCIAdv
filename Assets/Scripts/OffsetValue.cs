using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetValue : MonoBehaviour
{
    public float sampleDistanceVal;
    // When DPI is set to 1, the movement is one to one
    // Human DOF is ROBOT_DOF / DPI
    [Header("Human Arm DPI Offset Value")]
    [Range(0,2)]
    public float human_arm_yaw = 1f;
    // 4.6186f
    [Range(0,2)]
    public float human_arm_roll = 1f;
    // 3.0266
    [Range(0,2)]
    public float human_arm_pitch = 1f;
    // 4.9874
    [Range(0,2)]
    public float human_forearm_roll = 1f;
    // 2.3017
    [Range(0,2)]
    public float human_forearm_pitch = 1f;
    // 4.96
    [Range(0,2)]
    public float human_hand_roll = 1f;
    // 3.3573
    [Range(0,2)]
    public float human_hand_pitch = 1f;
    // 5.379


    [Header("Robot Posing Offset Value")]
    [Range(-2.3093f, 2.3093f)]
    public float arm_yaw = 0;

    [Range(-1.5133f, 1.5133f)]
    public float arm_pitch = 0;

    [Range(-2.4937f, 2.4937f)]
    public float arm_roll = 0;

    [Range(-2.7478f, 0)]
    public float forearm_pitch = 0;

    [Range(-2.4800f, 2.4800f)]
    public float forearm_roll = 0;

    [Range(0.8521f, 4.2094f)]
    // public float hand_pitch = 2.53075f;
        public float hand_pitch = 0f;


    [Range(-2.6895f, 2.6895f)]
    public float hand_roll = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
