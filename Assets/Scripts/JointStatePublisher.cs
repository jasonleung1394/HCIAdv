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

public class JointStatePublisher : MonoBehaviour
{
    ROSConnection ros;

    // topic /joint_states is subscribed to /joint_state)publisher. /joint_state_publisher is subscribed to /franka_state_controller/joint_states
    public string topic_name = "franka_state_controller/joint_states";

    public double[] jointAngles_double = new double[7];

    public Transform fr3;
    private float timeElapsed;

    private ClockMsg clockMsg;
    public ClockMsg clockMsg_prop
    {
        get { return clockMsg; }
        set { clockMsg = value; }
    }
    private uint sequence;
    double PublishPeriodSeconds => 1.0f / 30f;
    double m_LastPublishTimeSeconds;

    bool ShouldPublishMessage =>
        Clock.NowTimeInSeconds > m_LastPublishTimeSeconds + PublishPeriodSeconds;

    // Start is called before the first frame update
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<JointStateMsg>(topic_name);

        m_LastPublishTimeSeconds = Clock.time + PublishPeriodSeconds;
    }

    void syncClock(ClockMsg clock)
    {
        clockMsg_prop = clock;
    }

    void getSeq(TFMessageMsg tF)
    {
        // Debug.Log(tF.transforms[0]);
        // sequence = seq;
    }

    // Update is called once per frame
    void Update()
    {
        ros.Subscribe<ClockMsg>("clock", syncClock);

        string[] jointNames = new string[7];
        for (int i = 0; i < 7; i++)
        {
            jointNames[i] = "fr3_joint" + (i + 1);
        }
        double[] jointPos = new double[7];
        double[] jointVel = new double[7];
        double[] jointEff = new double[7];
        jointPos = jointAngles_double;
        if (clockMsg_prop != null && ShouldPublishMessage)
        {
            JointStateMsg JointStateMsg = new JointStateMsg(
                new HeaderMsg(0, clockMsg.clock, ""),
                jointNames,
                jointPos,
                jointVel,
                jointEff
            );
            ros.Publish(topic_name, JointStateMsg);
            m_LastPublishTimeSeconds = Clock.FrameStartTimeInSeconds;
        }
    }
}
