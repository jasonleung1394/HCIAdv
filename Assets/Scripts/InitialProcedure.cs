using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Tf2;
using RosMessageTypes.Sensor;

public class InitialProcedure : MonoBehaviour
{
    ROSConnection ros;

    // Start is called before the first frame update
    private string JointState_TopicName = "joint_states";

    public bool overSpeedFlag { get; set; }

    public bool autoReset = false;

    public bool jointStateSynced { get; set; }

    void Start()
    {
        // establish Ros Connection
        // ROSConnection.GetOrCreateInstance().Subscribe<TFMessageMsg>("tf",ShowTF);
        ros = ROSConnection.GetOrCreateInstance();
        // establish remote play

        // connect with optitrack

        // start the following sequence
    }

    void syncJointState()
    {
        ros.Subscribe<JointStateMsg>(JointState_TopicName, getJointState);
    }


    void getJointState(JointStateMsg jointStateMsg)
    {
        double[] JointPosition = jointStateMsg.position;
        CopyAvatarMovement copyAvatarMovement = GetComponent<CopyAvatarMovement>();

        // Debug.Log(JointPosition[5]);
        if (copyAvatarMovement.ifPosSynced(JointPosition))
        {
            jointStateSynced = true;
        }
        else
        {
            jointStateSynced = false;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        syncJointState();
        if (jointStateSynced == true)
        {
            JointStatePublisher jointStatePublisher = GetComponent<JointStatePublisher>();
            jointStatePublisher.PublishJointState();
        }
    }

    private void OnGUI()
    {
        CopyAvatarMovement copyAvatarMovement = GetComponent<CopyAvatarMovement>();
        bool[] jointInitFlag = copyAvatarMovement.jointInitFlag;

        ButtonAction buttonAction = GetComponent<ButtonAction>();
        int GripperState_Index = buttonAction.GripperState_Index;
        string jointInitText = "";
        for (int i = 0; i < jointInitFlag.Length; i++)
        {
            if (jointInitFlag[i] != true)
            {
                jointInitText += "Joint " + (i + 1) + " Is Not In Starting Position \n";
            }
            else
            {
                jointInitText += "Joint " + (i + 1) + " Is Good To Go\n";
            }
        }
        GUI.Label(new Rect(700, 10, 500, 1000), jointInitText);
        GUI.Label(new Rect(700, 150, 500, 1000), "Is the movement overspeed? -- " + !overSpeedFlag);
        GUI.Label(new Rect(700, 600, 500, 1000), GripperState_Index == 0 ? "Gripper is Closed" : "Gripper is Open");
    }
}
