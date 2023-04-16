//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.FrankaGripper
{
    [Serializable]
    public class GraspEpsilonMsg : Message
    {
        public const string k_RosMessageName = "franka_gripper/GraspEpsilon";
        public override string RosMessageName => k_RosMessageName;

        public double inner;
        //  [m]
        public double outer;
        //  [m]

        public GraspEpsilonMsg()
        {
            this.inner = 0.0;
            this.outer = 0.0;
        }

        public GraspEpsilonMsg(double inner, double outer)
        {
            this.inner = inner;
            this.outer = outer;
        }

        public static GraspEpsilonMsg Deserialize(MessageDeserializer deserializer) => new GraspEpsilonMsg(deserializer);

        private GraspEpsilonMsg(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.inner);
            deserializer.Read(out this.outer);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.inner);
            serializer.Write(this.outer);
        }

        public override string ToString()
        {
            return "GraspEpsilonMsg: " +
            "\ninner: " + inner.ToString() +
            "\nouter: " + outer.ToString();
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
