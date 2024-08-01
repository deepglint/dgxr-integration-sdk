using System;
using System.Text.RegularExpressions;
using Deepglint.XR;
using Deepglint.XR.Ros;
using ROS2;

// Copyright 2019-2021 Robotec.ai.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UnityEngine;


namespace Deepglint.XR.Ros
{

    /// <summary>
    /// An example class provided for testing of basic ROS2 communication
    /// </summary>
    public class DGXRNode : MonoBehaviour
    {
        // Start is called before the first frame update
        private ROS2UnityManager ros2Unity;
        private ROS2Node ros2Node;

        void Start()
        {
            ros2Unity = GetComponent<ROS2UnityManager>();
        }

        void Update()
        {
            if (ros2Unity.Ok())
            {
                if (ros2Node == null)
                {
                    string sanitizedName = DGXR.ApplicationSettings.id;
                    ros2Node = ros2Unity.CreateNode("unity_"+sanitizedName);
                    Debug.Log("ros2 node name: "+sanitizedName);
                    QualityOfServiceProfile qualityOfServiceProfille = new QualityOfServiceProfile();
                    qualityOfServiceProfille.SetReliability(ReliabilityPolicy.QOS_POLICY_RELIABILITY_BEST_EFFORT);
                    ros2Node.CreateSubscription<std_msgs.msg.String>("/metapose/pose3d", new Ros2PoseAdapter().DealMsg,
                        qualityOfServiceProfille);
                    ros2Node.CreateSubscription<std_msgs.msg.String>("/metagear/event/B1", new MetaGearAdapter().DealMsg,
                        qualityOfServiceProfille);
                }
            }
        }
    }

}  // namespace ROS2
