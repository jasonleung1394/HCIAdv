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
    List<double[]> jointStateBuffer = new List<double[]>();
    public OffsetValue offsetValue;

    bool ShouldPublishMessage =>
        Clock.NowTimeInSeconds > m_LastPublishTimeSeconds + PublishPeriodSeconds;

    // Start is called before the first frame update
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<JointCommandMsg>(topic_name);

        m_LastPublishTimeSeconds = Clock.time + PublishPeriodSeconds;
        prev_handLocation = GameObject.Find("Right Hand").transform.position;
        offsetValue = GetComponent<OffsetValue>();
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
    public void PublishJointState()
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
        double[] jointVel = new double[7];
        double[] jointEff = new double[7];
        jointPos = jointAngles_double;
        if (Vector3.Distance(prev_handLocation, GameObject.Find("Right Hand").transform.position) > offsetValue.sampleDistanceVal)
        {
            addNewToBuffer(jointPos);
        }

        jointPos[1] = -jointPos[1];
        jointPos[2] = -jointPos[2];
        // publisher_buffer.Add(jointPos);

        if (ShouldPublishMessage && lerpToInitialPose.Lerp_Index != 2 && lerpToInitialPose.Lerp_Index != 1)
        {
            JointCommandMsg jointCommandMsg = new JointCommandMsg(
                new HeaderMsg(0, clockMsg, ""),
                1,
                jointNames,
                jointPos,
                jointVel,
                jointEff,
                jointEff
            );
            ros.Publish(topic_name, jointCommandMsg);
            m_LastPublishTimeSeconds = Clock.FrameStartTimeInSeconds;
        }
    }
    void addNewToBuffer(double[] jointPos)
    {
        if (!jointPos.Equals(jointStateBuffer.Last()))
        {
            SmoothSeq(jointPos);
            jointStateBuffer.Add(jointPos);
        }
    }
    public void PublishJointStateSeq()
    {
        //smooth the joint state seq
    }
    void SmoothSeq(double[] new_Pos)
    {
        float[] velConstraintVal = new float[] { 2f, 1f, 1.5f, 1.25f, 3f, 1.5f, 3f };
        double[] next_Pos = new double[7];
        //the time gap is 0.02 constance
        double[] pre_Pos = jointStateBuffer.Last();
        for (int i = 0; i < new_Pos.Length; i++)
        {
            // speed
            var disTravel = PublishPeriodSeconds * velConstraintVal[i];
            // too fast
            if (pre_Pos[i] > new_Pos[i])
            {
                // decreasing, minus
                while (pre_Pos[i] - disTravel > new_Pos[i])
                {
                    
                }
            }
        }
    }
}
