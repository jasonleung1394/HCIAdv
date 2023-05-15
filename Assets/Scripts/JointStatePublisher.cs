using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.Core;
using RosMessageTypes.Franka;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using RosMessageTypes.Rosgraph;
using RosMessageTypes.Tf2;
using RosMessageTypes.FrankaPositionServo;
using RosMessageTypes.FrankaExampleControllers;
using System;
using System.Linq;
public class JointStatePublisher : MonoBehaviour
{
    ROSConnection ros;

    // topic /joint_states is subscribed to /joint_state)publisher. /joint_state_publisher is subscribed to /franka_state_controller/joint_states
    private string topic_name = "motion_controller/arm/joint_commands";

    public double[] jointAngles_double = new double[7];

    public Transform fr3;
    private float timeElapsed;

    private TimeStamp clockMsg;
    public TimeStamp clockMsg_prop
    {
        get { return clockMsg; }
        set { clockMsg = value; }
    }
    private uint sequence;
    double PublishPeriodSeconds => 1.0f / 30f;
    double m_LastPublishTimeSeconds;

    public Vector3 prev_handLocation;
    public List<double[]> jointStateBuffer = new List<double[]>();
    private OffsetValue offsetValue;
    private LerpToInitialPose lerpToInitialPose;
    bool ShouldPublishMessage =>
        Clock.NowTimeInSeconds > m_LastPublishTimeSeconds + PublishPeriodSeconds;

    // Start is called before the first frame update
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<JointCommandMsg>(topic_name);
        ros.RegisterPublisher<PositionsMsg>("joint_trajectory");

        m_LastPublishTimeSeconds = Clock.time + PublishPeriodSeconds;
        offsetValue = GetComponent<OffsetValue>();
        lerpToInitialPose = GetComponent<LerpToInitialPose>();
    }
    private void FixedUpdate()
    {

    }
    void syncClock(JointStateMsg JointStateMsg)
    {
        clockMsg_prop = JointStateMsg.header.stamp;
    }

    void getSeq(TFMessageMsg tF)
    {
        // Debug.Log(tF.transforms[0]);
        // sequence = seq;
    }

    // private List<double[]> publisher_buffer;
    public bool publish_once = false;
    public void PublishJointState(int publishType_Index)
    {

        LerpToInitialPose lerpToInitialPose = GetComponent<LerpToInitialPose>();
        ros.Subscribe<JointStateMsg>("joint_states", syncClock);

        string[] jointNames = new string[7];
        for (int i = 0; i < 7; i++)
        {
            jointNames[i] = "panda_joint" + (i + 1);
        }
        // i guess vel and eff needs to be set
        double[] jointPos = new double[7];
        jointPos = jointAngles_double;
        var movedDis = Vector3.Distance(prev_handLocation, GameObject.Find("Right Hand").transform.position);
        if (publishType_Index == 0 && !publish_once)
        {
            if (movedDis > offsetValue.sampleDistanceVal && lerpToInitialPose.Lerp_Index == 0)
            {
                Debug.Log("publish distance");

                PositionsMsg positionsMsg = new PositionsMsg(
                    jointNames,
                    jointPos
                );
                // Debug.Log(positionsMsg);
                ros.Publish("joint_trajectory", positionsMsg);
                prev_handLocation = GameObject.Find("Right Hand").transform.position;
                // addNewToBuffer(jointPos);
            }
        }
        else if (publishType_Index == 1 && lerpToInitialPose.Lerp_Index == 0 && publish_once!)
        {
            Debug.Log("publish freq");

            PositionsMsg positionsMsg = new PositionsMsg(
               jointNames,
               jointPos
           );
            // Debug.Log(positionsMsg);
            ros.Publish("joint_trajectory", positionsMsg);
        }
        else if (publish_once)
        {
            Debug.Log("publish once");
            PositionsMsg positionsMsg = new PositionsMsg(jointNames, jointPos);
            // Debug.Log(positionsMsg);
            ros.Publish("joint_trajectory", positionsMsg);
            publish_once = false;
        }
    }
    /// <summary>
    /// no longer required
    /// </summary>
    /// <param name="jointPos"></param>
    void addNewToBuffer(double[] jointPos)
    {
        if (jointStateBuffer.Count == 1)
        {
            SmoothSeq(jointPos);
        }

    }
    public void PublishJointStateSeq()
    {
        //smooth the joint state seq
    }

    /// <summary>
    /// no longer required
    /// </summary>
    /// <param name="new_Pos"></param>
    void SmoothSeq(double[] new_Pos)
    {
        float[] velConstraintVal = new float[] { 2f, 1f, 1.5f, 1.25f, 3f, 1.5f, 3f };

        for (int i = 0; i < velConstraintVal.Length; i++)
        {
            velConstraintVal[i] = offsetValue.velocityOffsetVal * velConstraintVal[i];
        }
        double[] next_Pos = new double[7];
        //the time gap is 0.02 constance
        double[] pre_Pos = jointStateBuffer[0];
        Debug.Log(new_Pos[1] - pre_Pos[1]);


        for (int i = 0; i < pre_Pos.Length; i++)
        {
            var Distance = Math.Abs(pre_Pos[i] - new_Pos[i]);
            var timeToTravel = Distance / velConstraintVal[i];
            var steps = Mathf.CeilToInt((float)timeToTravel / Time.deltaTime);
            var distPerstep = Distance / steps;

            for (int j = 0; j < steps; j++)
            {
                jointStateBuffer.Add(next_Pos);

                if (pre_Pos[i] - new_Pos[i] > 0)
                {
                    jointStateBuffer[j][i] = -distPerstep + pre_Pos[i];

                    // increase
                }
                else
                {
                    jointStateBuffer[j][i] = distPerstep + pre_Pos[i];
                }
            }
        }
        foreach (var item in jointStateBuffer)
        {
            Debug.Log(item[1]);
        }
        jointStateBuffer.Clear();
    }
}
