//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;
using RosMessageTypes.Std;

namespace RosMessageTypes.FrankaPositionServo
{
    [Serializable]
    public class JointControllerStatesMsg : Message
    {
        public const string k_RosMessageName = "franka_position_servo/JointControllerStates";
        public override string RosMessageName => k_RosMessageName;

        public HeaderMsg header;
        public string controller_name;
        public string[] names;
        //  Joint names order for command
        public Control.JointControllerStateMsg[] joint_controller_states;

        public JointControllerStatesMsg()
        {
            this.header = new HeaderMsg();
            this.controller_name = "";
            this.names = new string[0];
            this.joint_controller_states = new Control.JointControllerStateMsg[0];
        }

        public JointControllerStatesMsg(HeaderMsg header, string controller_name, string[] names, Control.JointControllerStateMsg[] joint_controller_states)
        {
            this.header = header;
            this.controller_name = controller_name;
            this.names = names;
            this.joint_controller_states = joint_controller_states;
        }

        public static JointControllerStatesMsg Deserialize(MessageDeserializer deserializer) => new JointControllerStatesMsg(deserializer);

        private JointControllerStatesMsg(MessageDeserializer deserializer)
        {
            this.header = HeaderMsg.Deserialize(deserializer);
            deserializer.Read(out this.controller_name);
            deserializer.Read(out this.names, deserializer.ReadLength());
            deserializer.Read(out this.joint_controller_states, Control.JointControllerStateMsg.Deserialize, deserializer.ReadLength());
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.header);
            serializer.Write(this.controller_name);
            serializer.WriteLength(this.names);
            serializer.Write(this.names);
            serializer.WriteLength(this.joint_controller_states);
            serializer.Write(this.joint_controller_states);
        }

        public override string ToString()
        {
            return "JointControllerStatesMsg: " +
            "\nheader: " + header.ToString() +
            "\ncontroller_name: " + controller_name.ToString() +
            "\nnames: " + System.String.Join(", ", names.ToList()) +
            "\njoint_controller_states: " + System.String.Join(", ", joint_controller_states.ToList());
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