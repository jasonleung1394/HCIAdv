//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.Assets
{
    [Serializable]
    public class PositionMsg : Message
    {
        public const string k_RosMessageName = "Assets/position";
        public override string RosMessageName => k_RosMessageName;

        public string[] joint_names;
        public double[] positions;

        public PositionMsg()
        {
            this.joint_names = new string[0];
            this.positions = new double[0];
        }

        public PositionMsg(string[] joint_names, double[] positions)
        {
            this.joint_names = joint_names;
            this.positions = positions;
        }

        public static PositionMsg Deserialize(MessageDeserializer deserializer) => new PositionMsg(deserializer);

        private PositionMsg(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.joint_names, deserializer.ReadLength());
            deserializer.Read(out this.positions, sizeof(double), deserializer.ReadLength());
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.WriteLength(this.joint_names);
            serializer.Write(this.joint_names);
            serializer.WriteLength(this.positions);
            serializer.Write(this.positions);
        }

        public override string ToString()
        {
            return "PositionMsg: " +
            "\njoint_names: " + System.String.Join(", ", joint_names.ToList()) +
            "\npositions: " + System.String.Join(", ", positions.ToList());
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
