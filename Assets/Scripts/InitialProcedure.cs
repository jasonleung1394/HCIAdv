using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Tf2;
using RosMessageTypes.Sensor;
using RosMessageTypes.Franka;

public class InitialProcedure : MonoBehaviour
{
    ROSConnection ros;

    // // Start is called before the first frame update
    // private string JointState_TopicName = "joint_states";

    public bool overSpeedFlag { get; set; }

    void Start()
    {
        // establish Ros Connection
        // ROSConnection.GetOrCreateInstance().Subscribe<TFMessageMsg>("tf",ShowTF);
        ros = ROSConnection.GetOrCreateInstance();

        indicator_man = GameObject.Find("indicator_man");
        banana_man = GameObject.Find("Banana Man");

    }

    void Update()
    {
        LerpToInitialPose lerpToInitialPose = GetComponent<LerpToInitialPose>();
        if (lerpToInitialPose.Lerp_Index != 1)
        {
            GameObject.Find("Right Arm").transform.localRotation = GameObject.Find("Right Arm OT").transform.localRotation;
            GameObject.Find("Right Forearm").transform.localRotation = GameObject.Find("Right Forearm OT").transform.localRotation;
            GameObject.Find("Right Hand").transform.localRotation = GameObject.Find("Right Hand OT").transform.localRotation;
            // GameObject.Find("Right Forearm OT").GetComponent<OptitrackRigidBody>().enabled = true;
            // GameObject.Find("Right Hand OT").GetComponent<OptitrackRigidBody>().enabled = true;
            // GameObject.Find("Right Arm OT").GetComponent<OptitrackRigidBody>().enabled = true;
            GameObject.Find("Right Arm OT").GetComponent<testScript>().enabled = true;

        }
        else
        {
            // GameObject.Find("Right Forearm OT").GetComponent<OptitrackRigidBody>().enabled = false;
            // GameObject.Find("Right Hand OT").GetComponent<OptitrackRigidBody>().enabled = false;
            // GameObject.Find("Right Arm OT").GetComponent<OptitrackRigidBody>().enabled = false;
            GameObject.Find("Right Arm OT").GetComponent<testScript>().enabled = false;

            GameObject.Find("Right Arm OT").transform.localRotation = GameObject.Find("Right Arm").transform.localRotation;
            GameObject.Find("Right Forearm OT").transform.localRotation = GameObject.Find("Right Forearm").transform.localRotation;
            GameObject.Find("Right Hand OT").transform.localRotation = GameObject.Find("Right Hand").transform.localRotation;
        }
        JointStatePublisher jointStatePublisher = GetComponent<JointStatePublisher>();
        jointStatePublisher.PublishJointState(publishType_Index);
    }

    public int publishType_Index = 0;

    private GameObject indicator_man;
    private GameObject banana_man;

    /// <summary>
    /// scene debug information
    /// </summary>
    private void OnGUI()
    {
        CopyAvatarMovement copyAvatarMovement = GetComponent<CopyAvatarMovement>();
        bool[] jointInitFlag = copyAvatarMovement.jointInitFlag;

        ButtonAction buttonAction = GetComponent<ButtonAction>();
        int GripperState_Index = buttonAction.GripperState_Index;
        GUI.Label(new Rect(700, 150, 500, 1000), "Is the movement overspeed? -- " + !overSpeedFlag);
        GUI.Label(new Rect(700, 600, 500, 1000), GripperState_Index == 0 ? "Gripper is Closed" : "Gripper is Open");
    }
}
