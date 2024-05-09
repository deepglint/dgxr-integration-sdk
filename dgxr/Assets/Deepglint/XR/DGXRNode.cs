using ROS2;
using Runtime.Scripts.Ros;

namespace Runtime.Scripts
{
    public class DGXRNode
    {
        private ROS2Node _ros2Node;
        public void InitNode(ROS2UnityManager ros)
        {
            if (_ros2Node == null && ros.Ok())
            {
                _ros2Node = ros.CreateNode(Global.AppName+Global.UniqueID);
                QualityOfServiceProfile qualityOfServiceProfille = new QualityOfServiceProfile();
                qualityOfServiceProfille.SetReliability(ReliabilityPolicy.QOS_POLICY_RELIABILITY_BEST_EFFORT);
                _ros2Node.CreateSubscription<std_msgs.msg.String>("/metapose/pose3d", new Ros2PoseAdapter().DealMsg,
                    qualityOfServiceProfille);
                _ros2Node.CreateSubscription<std_msgs.msg.String>("/metagear/event/B1", new MetaGearAdapter().DealMsg,
                    qualityOfServiceProfille);
            }
        }
    }
}