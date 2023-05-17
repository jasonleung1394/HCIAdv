#!/usr/bin/env python

import sys
import rospy as ros
from franka_example_controllers.msg import positions
from actionlib import SimpleActionClient
from sensor_msgs.msg import JointState
from trajectory_msgs.msg import JointTrajectory, JointTrajectoryPoint
from control_msgs.msg import FollowJointTrajectoryAction, \
                             FollowJointTrajectoryGoal, FollowJointTrajectoryResult
from franka_msgs.srv import SetFullCollisionBehavior, SetFullCollisionBehaviorRequest 
def moveAction(data):

    ros.loginfo("trajectory: Waiting for '" + action + "' action to come up")
    client.wait_for_server()
    param = ros.resolve_name('~joint_pose')
    pose = data
    if pose is None:
        ros.logerr('trajectory: Could not find required parameter "' + param + '"')
        sys.exit(1)
    pose = dict(zip(pose.joint_names,pose.positions))
    topic = ros.resolve_name('~joint_states')
    ros.loginfo("trajectory: Waiting for message on topic '" + topic + "'")
    joint_state = ros.wait_for_message(topic, JointState)
    initial_pose = dict(zip(joint_state.name, joint_state.position))
    max_movement = max(abs(pose[joint] - initial_pose[joint]) for joint in pose)

    point = JointTrajectoryPoint()
    point.time_from_start = ros.Duration.from_sec(
        # Use either the time to move the furthest joint with 'max_dq' or 500ms,
        # whatever is greater
        max(max_movement / ros.get_param('~max_dq', 0.5), 0.5)
    )
    goal = FollowJointTrajectoryGoal()

    goal.trajectory.joint_names, point.positions = [list(x) for x in zip(*pose.items())]
    ros.loginfo(joint_state.velocity)
    point.velocities = [0] * len(pose)

    goal.trajectory.points.append(point)
    goal.goal_time_tolerance = ros.Duration.from_sec(0.5)

    ros.loginfo('Sending trajectory Goal to move into initial config')
    client.send_goal_and_wait(goal)


    result = client.get_result()
    if result.error_code != FollowJointTrajectoryResult.SUCCESSFUL:
        ros.logerr('trajectory: Movement was not successful: ' + {
            FollowJointTrajectoryResult.INVALID_GOAL:
            """
            The joint pose you want to move to is invalid (e.g. unreachable, singularity...).
            Is the 'joint_pose' reachable?
            """,

            FollowJointTrajectoryResult.INVALID_JOINTS:
            """
            The joint pose you specified is for different joints than the joint trajectory controller
            is claiming. Does you 'joint_pose' include all 7 joints of the robot?
            """,

            FollowJointTrajectoryResult.PATH_TOLERANCE_VIOLATED:
            """
            During the motion the robot deviated from the planned path too much. Is something blocking
            the robot?
            """,

            FollowJointTrajectoryResult.GOAL_TOLERANCE_VIOLATED:
            """
            After the motion the robot deviated from the desired goal pose too much. Probably the robot
            didn't reach the joint_pose properly
            """,
        }[result.error_code])

    else:
        ros.loginfo('trajectory: Successfully moved into next pose')


ros.init_node('trajectory')
# queueSize = ros.get_param('queueSize')
ros.logdebug("node initialized as trajectory")
action = ros.resolve_name('~follow_joint_trajectory')
client = SimpleActionClient(action, FollowJointTrajectoryAction)
# 
ros.Subscriber("joint_trajectory", positions,moveAction,queue_size=3)
# ros.loginfo(queueSize)
ros.spin()


# collision behavior
# ros.init_node('collision_behavior_node')
# ros.wait_for_service('/franka_control/set_full_collision_behavior')
# set_collision_behavior_client = ros.ServiceProxy('/franka_control/set_full_collision_behavior', SetFullCollisionBehavior)
# request = SetFullCollisionBehaviorRequest()
# request.lower_torque_thresholds_acceleration = [0.1] * 7  # Example torque threshold values
# request.upper_torque_thresholds_acceleration = [1.0] * 7  # Example torque threshold values
# request.lower_torque_thresholds_nominal = [0.05] * 7     # Example torque threshold values
# request.upper_torque_thresholds_nominal = [0.5] * 7      # Example torque threshold values
# request.lower_force_thresholds_acceleration = [5.0] * 3   # Example force threshold values
# request.upper_force_thresholds_acceleration = [20.0] * 3  # Example force threshold values
# request.lower_force_thresholds_nominal = [2.0] * 3       # Example force threshold values
# request.upper_force_thresholds_nominal = [10.0] * 3      # Example force threshold values
# response = set_collision_behavior_client(request)
# if response.success:
#     rospy.loginfo('Collision behavior set successfully.')
# else:
#     rospy.logerr('Failed to set collision behavior.')
