#!/usr/bin/env python

import rospy
from franka_msgs.srv import SetForceTorqueCollisionBehaviorRequest, SetForceTorqueCollisionBehavior

def collision_behavior_node():
    rospy.wait_for_service('/franka_control/set_force_torque_collision_behavior')
    set_collision_behavior_client = rospy.ServiceProxy('/franka_control/set_force_torque_collision_behavior', SetForceTorqueCollisionBehavior)
    rospy.loginfo("here")
    request = SetForceTorqueCollisionBehaviorRequest()
    request.lower_torque_thresholds_nominal = [0.05] * 7    
    request.upper_torque_thresholds_nominal = [0.5] * 7     
    request.lower_force_thresholds_nominal = [2.0] * 6   
    request.upper_force_thresholds_nominal = [10.0] * 6  

    try:
        response = set_collision_behavior_client(request)
        if response.success:
            rospy.loginfo('Collision behavior set successfully.')
        else:
            rospy.logerr('Failed to set collision behavior.')
    except rospy.ServiceException as e:
        rospy.logerr('Service call failed: ' + str(e))

if __name__ == '__main__':
    rospy.init_node('collision_behavior_node')
    collision_behavior_node()
    rospy.spin()
