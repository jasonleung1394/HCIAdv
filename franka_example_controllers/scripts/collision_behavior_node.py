#!/usr/bin/env python
import sys
import rospy as ros
from franka_msgs.srv import SetForceTorqueCollisionBehaviorRequest, SetForceTorqueCollisionBehavior
from franka_msgs.msg import FrankaState

def collision_behavior_node():
    ros.wait_for_service('/franka_control/set_force_torque_collision_behavior')
    set_collision_behavior_client = ros.ServiceProxy('/franka_control/set_force_torque_collision_behavior', SetForceTorqueCollisionBehavior)
    request = SetForceTorqueCollisionBehaviorRequest()
    request.lower_torque_thresholds_nominal = [0.05] * 7     #Nm
    request.upper_torque_thresholds_nominal = [20,20,20,40,20,20,20]     
    request.lower_force_thresholds_nominal = [2.0] * 6   
    request.upper_force_thresholds_nominal = [20] * 6 


    try:
        response = set_collision_behavior_client(request)
        if response.success:
            ros.loginfo('Collision behavior set successfully.')
        else:
            ros.logerr('Failed to set collision behavior.')
    except ros.ServiceException as e:
        ros.logerr('Service call failed: ' + str(e))
    
def checkCollision(data):
    joint_contact = data.joint_contact
    joint_collision = data.joint_collision
    if 1 in joint_collision: ros.signal_shutdown("collision")

if __name__ == '__main__':
    ros.init_node('collision_behavior_node',disable_signals=True)
    collision_behavior_node()
    ros.Subscriber("franka_state_controller/franka_states",FrankaState,checkCollision)
    ros.spin()
