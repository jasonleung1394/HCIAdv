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

    public bool jointStateSynced { get; set; }

    void Start()
    {
        // establish Ros Connection
        // ROSConnection.GetOrCreateInstance().Subscribe<TFMessageMsg>("tf",ShowTF);
        ros = ROSConnection.GetOrCreateInstance();

        indicator_man = GameObject.Find("indicator_man");
        banana_man = GameObject.Find("Banana Man");
    }

    void syncJointState()
    {
        ros.Subscribe<JointStateMsg>(JointState_TopicName, getJointState);
    }


    void getJointState(JointStateMsg jointStateMsg)
    {
        double[] JointPosition = jointStateMsg.position;
        CopyAvatarMovement copyAvatarMovement = GetComponent<CopyAvatarMovement>();

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
    void Update()
    {
        GameObject.Find("Right Arm OT").transform.localRotation = GameObject.Find("Right Arm").transform.localRotation;
        GameObject.Find("Right Forearm OT").transform.localRotation = GameObject.Find("Forearm Arm").transform.localRotation;
        GameObject.Find("Right Hand OT").transform.localRotation = GameObject.Find("Right Hand").transform.localRotation;
    }

    public int publishType_Index = 0;

    void FixedUpdate()
    {

        JointStatePublisher jointStatePublisher = GetComponent<JointStatePublisher>();
        jointStatePublisher.PublishJointState(publishType_Index);
    }
    private GameObject indicator_man;
    private GameObject banana_man;
    /// <summary>
    /// No longer required
    /// </summary>
    /// <returns></returns>
    private bool syncTrackingToUnity()
    {
        bool sync_result = true;

        Transform[] Banana_man_Transforms = { GameObject.Find("Right Arm").transform, GameObject.Find("Right Forearm").transform, GameObject.Find("Right Hand").transform };
        Transform[] indicator_man_Transforms = { GameObject.Find("Right Arm OT").transform, GameObject.Find("Right Forearm OT").transform, GameObject.Find("Right Hand OT").transform };

        for (int i = 0; i < Banana_man_Transforms.Length; i++)
        {
            var Banana_localR = Banana_man_Transforms[i].localRotation;
            var Indicator_localR = indicator_man_Transforms[i].localRotation;
            var diffX = Mathf.Abs(Banana_localR.eulerAngles.x - Indicator_localR.eulerAngles.x);
            var diffY = Mathf.Abs(Banana_localR.eulerAngles.y - Indicator_localR.eulerAngles.y);
            var diffZ = Mathf.Abs(Banana_localR.eulerAngles.z - Indicator_localR.eulerAngles.z);
            if (diffX >= 10f || diffY >= 10f || diffZ >= 10f)
            {
                Debug.DrawRay(indicator_man_Transforms[i].position, Banana_man_Transforms[i].up, Color.red);
                sync_result = false;
            }
            else
            {
                Debug.DrawRay(indicator_man_Transforms[i].position, Banana_man_Transforms[i].up, Color.green);
            }
        }

        return sync_result;
    }

    private void OnGUI()
    {
        CopyAvatarMovement copyAvatarMovement = GetComponent<CopyAvatarMovement>();
        bool[] jointInitFlag = copyAvatarMovement.jointInitFlag;

        ButtonAction buttonAction = GetComponent<ButtonAction>();
        int GripperState_Index = buttonAction.GripperState_Index;
        // string jointInitText = "";
        // for (int i = 0; i < jointInitFlag.Length; i++)
        // {
        //     if (jointInitFlag[i] != true)
        //     {
        //         jointInitText += "Joint " + (i + 1) + " Is Not In Starting Position \n";
        //     }
        //     else
        //     {
        //         jointInitText += "Joint " + (i + 1) + " Is Good To Go\n";
        //     }
        // }
        // GUI.Label(new Rect(700, 5, 500, 1000), "Checking sync between Unity To ROS");
        // GUI.Label(new Rect(700, 10, 500, 1000), jointInitText);
        GUI.Label(new Rect(700, 150, 500, 1000), "Is the movement overspeed? -- " + !overSpeedFlag);
        GUI.Label(new Rect(700, 600, 500, 1000), GripperState_Index == 0 ? "Gripper is Closed" : "Gripper is Open");
    }
}
