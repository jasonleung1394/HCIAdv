//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.Control
{
    [Serializable]
    public class PointHeadResult : Message
    {
        public const string k_RosMessageName = "control_msgs/PointHead";
        public override string RosMessageName => k_RosMessageName;


        public PointHeadResult()
        {
        }
        public static PointHeadResult Deserialize(MessageDeserializer deserializer) => new PointHeadResult(deserializer);

        private PointHeadResult(MessageDeserializer deserializer)
        {
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
        }

        public override string ToString()
        {
            return "PointHeadResult: ";
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        public static void Register()
        {
            MessageRegistry.Register(k_RosMessageName, Deserialize, MessageSubtopic.Result);
        }
    }
}
