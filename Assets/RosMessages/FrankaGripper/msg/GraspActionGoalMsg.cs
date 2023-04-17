//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.FrankaGripper
{
    [Serializable]
    public class GraspActionGoalMsg : Message
    {
        public const string k_RosMessageName = "franka_gripper/GraspActionGoal";
        public override string RosMessageName => k_RosMessageName;

        public Std.HeaderMsg header;
        public Actionlib.GoalIDMsg goal_id;
        public GraspGoalMsg goal;

        public GraspActionGoalMsg()
        {
            this.header = new Std.HeaderMsg();
            this.goal_id = new Actionlib.GoalIDMsg();
            this.goal = new GraspGoalMsg();
        }

        public GraspActionGoalMsg(Std.HeaderMsg header, Actionlib.GoalIDMsg goal_id, GraspGoalMsg goal)
        {
            this.header = header;
            this.goal_id = goal_id;
            this.goal = goal;
        }

        public static GraspActionGoalMsg Deserialize(MessageDeserializer deserializer) => new GraspActionGoalMsg(deserializer);

        private GraspActionGoalMsg(MessageDeserializer deserializer)
        {
            this.header = Std.HeaderMsg.Deserialize(deserializer);
            this.goal_id = Actionlib.GoalIDMsg.Deserialize(deserializer);
            this.goal = GraspGoalMsg.Deserialize(deserializer);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.header);
            serializer.Write(this.goal_id);
            serializer.Write(this.goal);
        }

        public override string ToString()
        {
            return "GraspActionGoalMsg: " +
            "\nheader: " + header.ToString() +
            "\ngoal_id: " + goal_id.ToString() +
            "\ngoal: " + goal.ToString();
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        public static void Register()
        {
            MessageRegistry.Register(k_RosMessageName, Deserialize);
        }
    }
}
