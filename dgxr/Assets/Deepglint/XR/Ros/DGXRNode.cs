using ROS2;

namespace Deepglint.XR.Ros
{
    /// <summary>
    /// Node节点管理类
    /// </summary> 
    public class DGXRNode
    {
        private ROS2Node _ros2Node;
        
        /// <summary>
        /// 订阅node topic 
        /// </summary>
        /// <param name="ros">ros实体对象</param> 
        public void InitNode(ROS2UnityManager ros)
        {
            if (_ros2Node == null && ros.Ok())
            {
                _ros2Node = ros.CreateNode("unity_"+Global.UniqueID);
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