using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
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

    private uint sequence;
    double PublishPeriodSeconds => 1.0f / 30f;
    double m_LastPublishTimeSeconds;

    public Vector3 prev_handLocation;
    public List<double[]> jointStateBuffer = new List<double[]>();
    private OffsetValue offsetValue;
    private LerpToInitialPose lerpToInitialPose;

    // Start is called before the first frame update
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<JointCommandMsg>(topic_name);
        ros.RegisterPublisher<PositionsMsg>("joint_trajectory");

        offsetValue = GetComponent<OffsetValue>();
        lerpToInitialPose = GetComponent<LerpToInitialPose>();
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

        string[] jointNames = new string[7];
        for (int i = 0; i < 7; i++)
        {
            jointNames[i] = "panda_joint" + (i + 1);
        }
        // i guess vel and eff needs to be set
        double[] jointPos = new double[7];
        jointPos = jointAngles_double;
        jointPos[2] = -jointPos[2];
        jointPos[4] = -jointPos[4];
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
        else if (publishType_Index == 1 && lerpToInitialPose.Lerp_Index == 0 && !publish_once)
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
}
