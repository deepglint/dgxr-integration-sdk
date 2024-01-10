using System;
using System.IO;
using System.Xml;
using Moat;
using UnityEngine;

namespace DGXR
{
    /// <summary>
    /// This class is responsible for loading the calibration settings from the xml
    /// </summary>
    public class XRLoadCalibration
    {
        private XmlDocument _xmlDoc;

        public enum SensorType
        {
            DG,
            ArtTrack
        }
        public struct Sensor
        {
            public SensorType Type;
            public bool HeadTracker;
            public int BodyID;
            public bool Stereo;
            public Vector3 Position;
            public Vector3 Rotation;
        }
        public Sensor Sensors;

        public struct Projector
        {
            public Vector3 Position;
            public Vector3 Rotation;
            public float Fy;
            public float FOV;
            public int Display;
        }
        public Projector[] Projectors;

        public struct Surface
        {
            public Vector3 Position;
            public Vector3 Rotation;
            public Vector3 Size;
            public int Display;
            public Vector2[] Vertices;
        }
        public Surface[] Surfaces;

        public struct Screen
        {
            public Vector3 Position;
            public Vector3 Rotation;
            public Vector3 Size;
            public int Display;
        }
        public Screen[] Screens;

        public void LoadConfiguration(string filepath)
        {
            _xmlDoc = new XmlDocument();

            if (File.Exists(filepath))
            {
                _xmlDoc.Load(filepath);

                LoadSensorConfiguration(out Sensors);
                LoadProjectorsConfiguration(out Projectors);
                LoadSurfacesConfiguration(out Surfaces);
                LoadScreensConfiguration(out Screens);
            }
            else
            {
                MDebug.LogError("Error loading the calibration file, calibration.xml file not found. Using default.");
                LoadDefaultSensorConfiguration(out Sensors);
                Projectors = new Projector[0];
                Surfaces = new Surface[0];
                LoadDefaultScreensConfiguration(out Screens);
            }
        }

        private void LoadDefaultSensorConfiguration(out Sensor sensor)
        {
            sensor = new Sensor
            {
                Position =
                {
                    x = 0,
                    y = 1.5f,
                    z = .5f
                },
                Rotation =
                {
                    x = 0,
                    y = 180,
                    z = 0
                }
            };
        }

        private void LoadDefaultScreensConfiguration(out Screen[] screens)
        {
            screens = new Screen[1];
            screens[0] = new Screen
            {
                Position =
                {
                    x = 0,
                    y = 1.3f,
                    z = .5f
                },
                Rotation =
                {
                    x = 0,
                    y = 0,
                    z = 0
                },
                Size =
                {
                    x = .48f,
                    y = .27f,
                    z = .01f
                },
                Display = 1
            };
        }

        private void LoadSensorConfiguration(out Sensor sensor)
        {
            sensor = new Sensor();
            var sensorsConfig = _xmlDoc.GetElementsByTagName("Sensor");
            var sensorConfig = sensorsConfig[0].ChildNodes;
            foreach (XmlNode parameter in sensorConfig)
            {
                switch (parameter.Name)
                {
                    case "Type":
                        switch (parameter.InnerText)
                        {
                            case "DG":
                                sensor.Type = SensorType.DG;
                                break;
                            case "ARTTrack":
                                sensor.Type = SensorType.ArtTrack;
                                break;
                            default:
                                MDebug.Log("Loading the sensor type value: " + parameter.Name + " is an unknown type");
                                break;
                        }
                        break;
                    case "HeadTracker":
                        sensor.HeadTracker = string.Equals(parameter.InnerText, "true", StringComparison.InvariantCultureIgnoreCase);
                        break;
                    case "BodyID":
                        sensor.BodyID = int.Parse(parameter.InnerText);
                        break;
                    case "Stereo":
                        sensor.Stereo = string.Equals(parameter.InnerText, "true", StringComparison.InvariantCultureIgnoreCase);
                        break;
                    case "Pos.X":
                        sensor.Position.x = float.Parse(parameter.InnerText);
                        break;
                    case "Pos.Y":
                        sensor.Position.y = float.Parse(parameter.InnerText);
                        break;
                    case "Pos.Z":
                        sensor.Position.z = float.Parse(parameter.InnerText);
                        break;
                    case "Rot.X":
                        sensor.Rotation.x = float.Parse(parameter.InnerText);
                        break;
                    case "Rot.Y":
                        sensor.Rotation.y = float.Parse(parameter.InnerText);
                        break;
                    case "Rot.Z":
                        sensor.Rotation.z = float.Parse(parameter.InnerText);
                        break;
                    case "#comment":
                        break;
                    default:
                        MDebug.Log("Loading the sensor calibration values: " + parameter.Name + " is an unknown parameter");
                        break;
                }
            }
        }

        private void LoadProjectorsConfiguration(out Projector[] projectors)
        {
            var projectorsConfig = _xmlDoc.GetElementsByTagName("Projector");

            projectors = new Projector[projectorsConfig.Count];

            for (int i = 0; i < projectorsConfig.Count; i++)
            {
                projectors[i] = new Projector();
                var projectorConfig = projectorsConfig[i].ChildNodes;
                foreach (XmlNode parameter in projectorConfig)
                {
                    switch (parameter.Name)
                    {
                        case "Pos.X":
                            projectors[i].Position.x = float.Parse(parameter.InnerText);
                            break;
                        case "Pos.Y":
                            projectors[i].Position.y = float.Parse(parameter.InnerText);
                            break;
                        case "Pos.Z":
                            projectors[i].Position.z = float.Parse(parameter.InnerText);
                            break;
                        case "Rot.X":
                            projectors[i].Rotation.x = float.Parse(parameter.InnerText);
                            break;
                        case "Rot.Y":
                            projectors[i].Rotation.y = float.Parse(parameter.InnerText);
                            break;
                        case "Rot.Z":
                            projectors[i].Rotation.z = float.Parse(parameter.InnerText);
                            break;
                        case "Fy":
                            projectors[i].Fy = float.Parse(parameter.InnerText);
                            break;
                        case "FOV":
                            projectors[i].FOV = float.Parse(parameter.InnerText);
                            break;
                        case "Display":
                            projectors[i].Display = int.Parse(parameter.InnerText);
                            break;
                        case "#comment":
                            break;
                        default:
                            MDebug.Log("Loading the projector calibration values: " + parameter.Name + " is an unknown parameter");
                            break;
                    }
                }
            }
        }

        private void LoadSurfacesConfiguration(out Surface[] surfaces)
        {
            var surfacesConfig = _xmlDoc.GetElementsByTagName("Surface");

            surfaces = new Surface[surfacesConfig.Count];

            for (int i = 0; i < surfacesConfig.Count; i++)
            {
                surfaces[i] = new Surface {Vertices = new Vector2[4]};
                var surfaceConfig = surfacesConfig[i].ChildNodes;
                foreach (XmlNode parameter in surfaceConfig)
                {
                    switch (parameter.Name)
                    {
                        case "Pos.X":
                            surfaces[i].Position.x = float.Parse(parameter.InnerText);
                            break;
                        case "Pos.Y":
                            surfaces[i].Position.y = float.Parse(parameter.InnerText);
                            break;
                        case "Pos.Z":
                            surfaces[i].Position.z = float.Parse(parameter.InnerText);
                            break;
                        case "Rot.X":
                            surfaces[i].Rotation.x = float.Parse(parameter.InnerText);
                            break;
                        case "Rot.Y":
                            surfaces[i].Rotation.y = float.Parse(parameter.InnerText);
                            break;
                        case "Rot.Z":
                            surfaces[i].Rotation.z = float.Parse(parameter.InnerText);
                            break;
                        case "Size.X":
                            surfaces[i].Size.x = float.Parse(parameter.InnerText);
                            break;
                        case "Size.Y":
                            surfaces[i].Size.y = float.Parse(parameter.InnerText);
                            break;
                        case "Size.Z":
                            surfaces[i].Size.z = float.Parse(parameter.InnerText);
                            break;
                        case "Vertices":
                            var vertices = parameter.ChildNodes;
                            foreach (XmlNode vertice in vertices)
                            {
                                switch (vertice.Name)
                                {
                                    case "BottomLeft.X":
                                        surfaces[i].Vertices[0].x = float.Parse(vertice.InnerText);
                                        break;
                                    case "BottomLeft.Y":
                                        surfaces[i].Vertices[0].y = float.Parse(vertice.InnerText);
                                        break;
                                    case "TopLeft.X":
                                        surfaces[i].Vertices[1].x = float.Parse(vertice.InnerText);
                                        break;
                                    case "TopLeft.Y":
                                        surfaces[i].Vertices[1].y = float.Parse(vertice.InnerText);
                                        break;
                                    case "TopRight.X":
                                        surfaces[i].Vertices[2].x = float.Parse(vertice.InnerText);
                                        break;
                                    case "TopRight.Y":
                                        surfaces[i].Vertices[2].y = float.Parse(vertice.InnerText);
                                        break;
                                    case "BottomRight.X":
                                        surfaces[i].Vertices[3].x = float.Parse(vertice.InnerText);
                                        break;
                                    case "BottomRight.Y":
                                        surfaces[i].Vertices[3].y = float.Parse(vertice.InnerText);
                                        break;
                                    default:
                                        MDebug.Log("Loading the surface vertices values: " + vertice.Name + " is an unknown parameter");
                                        break;
                                }
                            }
                            break;
                        case "Display":
                            surfaces[i].Display = int.Parse(parameter.InnerText);
                            break;
                        case "#comment":
                            break;
                        default:
                            MDebug.Log("Loading the surface calibration values: " + parameter.Name + " is an unknown parameter");
                            break;
                    }
                }
            }
        }

        private void LoadScreensConfiguration(out Screen[] screens)
        {
            var surfacesConfig = _xmlDoc.GetElementsByTagName("Screen");

            screens = new Screen[surfacesConfig.Count];

            for (int i = 0; i < surfacesConfig.Count; i++)
            {
                screens[i] = new Screen();
                var surfaceConfig = surfacesConfig[i].ChildNodes;
                foreach (XmlNode parameter in surfaceConfig)
                {
                    switch (parameter.Name)
                    {
                        case "Pos.X":
                            screens[i].Position.x = float.Parse(parameter.InnerText);
                            break;
                        case "Pos.Y":
                            screens[i].Position.y = float.Parse(parameter.InnerText);
                            break;
                        case "Pos.Z":
                            screens[i].Position.z = float.Parse(parameter.InnerText);
                            break;
                        case "Rot.X":
                            screens[i].Rotation.x = float.Parse(parameter.InnerText);
                            break;
                        case "Rot.Y":
                            screens[i].Rotation.y = float.Parse(parameter.InnerText);
                            break;
                        case "Rot.Z":
                            screens[i].Rotation.z = float.Parse(parameter.InnerText);
                            break;
                        case "Size.X":
                            screens[i].Size.x = float.Parse(parameter.InnerText);
                            break;
                        case "Size.Y":
                            screens[i].Size.y = float.Parse(parameter.InnerText);
                            break;
                        case "Size.Z":
                            screens[i].Size.z = float.Parse(parameter.InnerText);
                            break;
                        case "Display":
                            screens[i].Display = int.Parse(parameter.InnerText);
                            break;
                        case "#comment":
                            break;
                        default:
                            MDebug.Log("Loading the surface calibration values: " + parameter.Name + " is an unknown parameter");
                            break;
                    }
                }
            }
        }
    }
}
