# HCI Advance Project Git Repo	

This is the git repository for the HCI Advance Project. Which includes both the project file for the Unity Project (including required 3rd Party package) as well as the modified Franka_Ros package.



## System Configuration

The project file were tested and run under the following system environment without error or warning. It is not recommended to go for Windows 11 and Ubuntu 22.04, both system are not fully supported base on the official documentation by both Franka Emika and OptiTrack (Work arounds are available). 

### Windows 10

For the Windows 10, it is required to have at least 16G ram, an up to date Windows 10 system **! Important: Crucial for running Hololens 2 Emulator**, up to date drivers **Especially Network Adapter: For Motive**. Dedicated GPU is recommended, though integrated GPU is a okay. 

### Ubuntu 20.04

For Ubuntu 20.04, Linux Real-time kernel 5.9.1 - rt20 were used.



## Branch Description and Corresponding Applications

The active branches during the development are the following two being described.

### prod-dev (Unity)

this branch contains all project files with corresponding packages. All package can be found its up to date version online, unless it is not required, use the listed version below as it was tested and verified.

| Product              | Version                                                      |
| -------------------- | ------------------------------------------------------------ |
| Unity Editor         | 2021.3.26f1                                                  |
| Mix Reality Tool Kit | 1.0.2209.0 [MRTK](https://www.microsoft.com/en-us/download/details.aspx?id=102778) |
| Hololens 2 Emulator  | [**HoloLens 2 Emulator (Windows Holographic, version 22H2 January 2023 Update)** (Install link: 10.0.20348.1528)](https://go.microsoft.com/fwlink/?linkid=2220897) |
| Motive (Optitrack)   | [Motive 2.3.4](https://optitrack.com/support/downloads/motive.html) |
| Unity ROS TCP        | [GitRepo](https://github.com/Unity-Technologies/ROS-TCP-Connector.git) |

### franka_ros_prod (ROS)

| Product                  | Version                                                      |
| ------------------------ | ------------------------------------------------------------ |
| Gazebo Simulator         | [Gazebo 11](https://classic.gazebosim.org/tutorials?tut=install_ubuntu) |
| Robotic Operating System | ROS Noetic                                                   |
| Franka_ros               | [Up to date](https://github.com/frankaemika/franka_ros src/franka_ros) |
| Libfranka                | 0.10.0                                                       |



## Instruction

Start up both PC and connect to stable same wireless network or connect to same ethernet. Check the IP for both PC. Windows Terminal `ipconfig`, Ubuntu `ifconfig`. Look for the connected adaptor and noted down the IPv4 address, typically it will be the same except for the last digit.

In Ubuntu system, after ensuring all the installation and getting started section in [Franka Ros](https://frankaemika.github.io/docs/franka_ros.html) are done. Switch the robot into FCI mode (if the robot led is white, switch from programming mode to execution) start a terminal and type `roslaunch franka_visualization franka_visualization.launch robot:= panda robot_ip:=172.16.0.2`, the popped up Rviz window should shown the current robot state. This indicates the connection between Ubuntu PC and robot is established and good to go. The terminal can be closed now.

In Ubuntu system, launch the ros_tcp_connector via `roslaunch ros_tcp_connector endpoint.launch tcp_ip:=0.0.0.0`, the terminal will now automatically start up a ros_server (roscore), it will be now active and waiting for connection from Unity. If the robot posture right now is undesired, it is recommended to use Franka_Ros move to start via `roslaunch franka_example_controller move_to_start.launch robot_ip:=172.16.0.2` robot will reset to configured pose when it finished, the node will die right after. Then launch the controller via `roslaunch franka_example_controller trajectory.launch robot_ip:=172.16.0.2`. This will launch the controller as well as loading the joint force torque configuration to the controller (5 Nm for all joint). **The controller wont listen to Unity publish if  the controller were launch prior to the ros_tcp_connector**.

In Windows system, load the "sample_scene" inside the scene folder. Open the "Robotics" tab located on the top of the screen and click on "ROS Settings"

![image-20230603211952774](C:\Users\jason\AppData\Roaming\Typora\typora-user-images\image-20230603211952774.png)

Type in the previously noted IPv4 address for the Ubuntu machine. leave the port at 10000 then close the window. Open the "Mixed Reality" tab -> Remoting -> Hologra...mode. Boot up your Hololens 2 or Hololens 2 Emulator and launch the Remote Play Application. Type in the IP address shown on Hololens 2 to the "Remote Host Name", leave other setting as it is and close the window. 

Put on the HMD, Look towards the Robot, the operator should keep the arm in a bended pose in front of chest, pay attention to the yellow looking arm in the augmented reality scene. click "Play" in Unity. After a short reset, robot should goes to a reset pose. Player can now start to control the robot.

If needed, find the "MainScript" in the Unity Editor's "Hierarchy". In the inspector there will be a list of script loaded, the offset value of both DPI and positional offset can be changed here. Sample Distance Value can also be changed, in Unity, 1 unit = 1 meter in real world. 



