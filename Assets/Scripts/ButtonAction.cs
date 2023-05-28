using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.Core;
using RosMessageTypes.FrankaGripper;
using RosMessageTypes.Actionlib;
using RosMessageTypes.Std;
using RosMessageTypes.BuiltinInterfaces;
using RosMessageTypes.Sensor;
 using UnityEngine.SceneManagement;

public class ButtonAction : MonoBehaviour
{
    // Start is called before the first frame update
    private ROSConnection ros;
    public Button syncUnityToRosBtn;
    public Button syncRosToUnityBtn;
    public Button GripperActionBtn;

    private int _gripperState_Index = 0; //Suppose close is 0, open is 1
    public int GripperState_Index
    {
        get { return _gripperState_Index; }
        set { _gripperState_Index = value; }
    }
    private GameObject fr3_J1, fr3_J2, fr3_J3, fr3_J4, fr3_J5, fr3_J6, fr3_J7;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        syncUnityToRosBtn.onClick.AddListener(SyncUnityToRos);
        GripperActionBtn.onClick.AddListener(GripperAction);

        ros.RegisterPublisher<MoveActionGoalMsg>("/franka_gripper/move/goal");
        ros.RegisterPublisher<GraspActionGoalMsg>("/franka_gripper/grasp/goal");


        fr3_J1 = GameObject.Find("fr3_link1");
        fr3_J2 = GameObject.Find("fr3_link2");
        fr3_J3 = GameObject.Find("fr3_link3");
        fr3_J4 = GameObject.Find("fr3_link4");
        fr3_J5 = GameObject.Find("fr3_link5");
        fr3_J6 = GameObject.Find("fr3_link6");
        fr3_J7 = GameObject.Find("fr3_link7");
    }


    void SyncUnityToRos()
    {
        // sync action needs to be a lerp
        JointStatePublisher jointStatePublisher = GetComponent<JointStatePublisher>();
        LerpToInitialPose lerpToInitialPose = GetComponent<LerpToInitialPose>();
        // lerpToInitialPose.Lerp_Index = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

    void GripperAction()
    {
        if (GripperState_Index == 0)
        {
            GripperActionOpen();
            GripperState_Index = 1;
        }
        else
        {
            GripperActionClose();
            GripperState_Index = 0;
        }
    }

    private void GripperActionOpen()
    {
        TimeMsg timeMsg = new TimeMsg((uint)0, (uint)0);
        HeaderMsg header = new HeaderMsg(1, timeMsg, "");
        GoalIDMsg goalIDMsg = new GoalIDMsg(timeMsg, "");
        MoveActionGoalMsg msg_to_publish = new MoveActionGoalMsg(header, goalIDMsg, new MoveGoalMsg(0.08, 0.1));
        ros.Publish("/franka_gripper/move/goal", msg_to_publish);
    }

    private void GripperActionClose()
    {
        TimeMsg timeMsg = new TimeMsg((uint)0, (uint)0);
        HeaderMsg header = new HeaderMsg(1, timeMsg, "");
        GoalIDMsg goalIDMsg = new GoalIDMsg(timeMsg, "");
        GraspActionGoalMsg msg_to_publish = new GraspActionGoalMsg(
           header, goalIDMsg, new GraspGoalMsg(0.03, new GraspEpsilonMsg(0.005, 0.005), 0.1, 5.0)
        );
        ros.Publish("/franka_gripper/grasp/goal", msg_to_publish);
    }
}
