using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BodyView : MonoBehaviour
{
    private Dictionary<Astra.JointType, GameObject> jointGOs;
    private LineRenderer[] jointLines;

    public GameObject doll;

    public bool isDisplay = true;

    [Range(10.0f, 200.0f)]
    public float posScale = 10.0f;
    public Vector3 posOffset;
    [Range(0, 1)]
    public float lerpRate = 0.5f;

    public GameObject boneSpine1;
    public GameObject boneSpine2;
    public GameObject boneNeck;
    public GameObject boneHead;

    public GameObject boneRightShoulder;
    public GameObject boneRightArm;
    public GameObject boneRightForeArm;
    public GameObject boneRightHand;

    public GameObject boneLeftShoulder;
    public GameObject boneLeftArm;
    public GameObject boneLeftForeArm;
    public GameObject boneLeftHand;

    public GameObject boneLeftThigh;
    public GameObject boneLeftCalf;
    public GameObject boneLeftFoot;

    public GameObject boneRightThigh;
    public GameObject boneRightCalf;
    public GameObject boneRightFoot;

    // Use this for initialization
    void Start()
    {
        StreamViewModel viewModel = StreamViewModel.Instance;
        viewModel.bodyStream.onValueChanged += OnBodyStreamChanged;

        jointGOs = new Dictionary<Astra.JointType, GameObject>();

        //初始化產生19個關節，使用字典儲存
        for (int i = 0; i < 19; ++i)
        {
            var jointGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            jointGO.name = ((Astra.JointType)i).ToString() + i;
            jointGO.transform.localScale = new Vector3(100f, 100f, 100f);
            jointGO.transform.SetParent(transform, false);
            jointGO.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Diffuse"));
            jointGO.SetActive(false);
            jointGOs.Add((Astra.JointType)i, jointGO);

            jointGO.GetComponent<MeshRenderer>().enabled = isDisplay;
        }

        //初始化產生15條骨頭線，使用陣列儲存
        jointLines = new LineRenderer[15];
        for (int i = 0; i < jointLines.Length; ++i)
        {
            var jointLineGO = new GameObject("Line");
            jointLineGO.transform.SetParent(transform, false);
            var jointLine = jointLineGO.AddComponent<LineRenderer>();
            jointLine.material = new Material(Shader.Find("Diffuse"));
            jointLine.SetWidth(0.1f, 0.1f);
            jointLineGO.SetActive(false);
            jointLines[i] = jointLine;

            jointLine.enabled = isDisplay;
        }
    }

    //當偵測到身體資訊，打開本物件(本物件為骨架之根)
    private void OnBodyStreamChanged(bool value)
    {
        gameObject.SetActive(value);
    }

    // Update is called once per frame
    void Update()
    {
        if (!AstraManager.Instance.Initialized || !AstraManager.Instance.IsBodyOn)
        {
            return;                                 //若Astra還沒初始化，或是沒有軀體，則跳過此次update
        }
        var bodies = AstraManager.Instance.Bodies;  //取得軀體們
        if (bodies == null)
        {
            return;                                 //若沒有軀體，則跳過此次update
        }
        foreach (var body in bodies)                //對每組軀體進行一次遞迴
        {
            var joints = body.Joints;               //取得關節陣列
            if (joints != null)
            {
                foreach (var joint in joints)       //遞迴每個關節
                {
                    if (joint.Status == Astra.JointStatus.Tracked)      //若該關節為追蹤中狀態
                    {
                        var jointPos = joint.WorldPosition;             //暫存起這個關節的位置
                        jointGOs[joint.Type].transform.localPosition = new Vector3(jointPos.X, jointPos.Y, jointPos.Z); //將位置寫入本地關節陣列
                        jointGOs[joint.Type].SetActive(true);           //開啟Activate該關節

                        if (joint.Type == Astra.JointType.LeftHand)     //若為左手
                        {
                            if (body.HandPoseInfo.LeftHand == Astra.HandPose.Grip)  //若左手為緊握，則進行以下調整
                            {
                                jointGOs[joint.Type].transform.localScale = new Vector3(100f, 100f, 100f) * 0.7f;
                                jointGOs[joint.Type].GetComponent<MeshRenderer>().material.color = Color.blue;
                            }
                            else
                            {
                                jointGOs[joint.Type].transform.localScale = new Vector3(100f, 100f, 100f);
                                jointGOs[joint.Type].GetComponent<MeshRenderer>().material.color = Color.white;
                            }
                        }
                        if (joint.Type == Astra.JointType.RightHand)    //若為右手
                        {
                            if (body.HandPoseInfo.RightHand == Astra.HandPose.Grip) //若右手為緊握，則進行以下調整
                            {
                                jointGOs[joint.Type].transform.localScale = new Vector3(100f, 100f, 100f) * 0.7f;
                                jointGOs[joint.Type].GetComponent<MeshRenderer>().material.color = Color.blue;
                            }
                            else
                            {
                                jointGOs[joint.Type].transform.localScale = new Vector3(100f, 100f, 100f);
                                jointGOs[joint.Type].GetComponent<MeshRenderer>().material.color = Color.white;
                            }
                        }
                    }
                    else            //若關節不是追蹤中狀態，失能deactivate該關節
                    {
                        jointGOs[joint.Type].SetActive(false);
                    }
                }

                DrawLine(joints[0], joints[1], 0);
                DrawLine(joints[1], joints[2], 1);
                DrawLine(joints[1], joints[5], 2);
                DrawLine(joints[2], joints[3], 3);
                DrawLine(joints[3], joints[4], 4);
                DrawLine(joints[5], joints[6], 5);
                DrawLine(joints[6], joints[7], 6);
                DrawLine(joints[1], joints[8], 7);
                DrawLine(joints[8], joints[9], 8);
                DrawLine(joints[9], joints[10], 9);
                DrawLine(joints[9], joints[13], 10);
                DrawLine(joints[10], joints[11], 11);
                DrawLine(joints[11], joints[12], 12);
                DrawLine(joints[13], joints[14], 13);
                DrawLine(joints[14], joints[15], 14);

                boneHead.transform.rotation = Quaternion.Slerp(boneHead.transform.rotation, GetRotation(joints[0].Orientation) * Quaternion.Euler(0, 180, 0), lerpRate);
                boneNeck.transform.rotation = Quaternion.Slerp(boneNeck.transform.rotation, GetRotation(joints[18].Orientation) * Quaternion.Euler(0, 180, 0), lerpRate);
                boneSpine2.transform.rotation = Quaternion.Slerp(boneSpine2.transform.rotation, GetRotation(joints[1].Orientation) * Quaternion.Euler(0, 180, 0), lerpRate);
                boneSpine1.transform.rotation = Quaternion.Slerp(boneSpine1.transform.rotation, GetRotation(joints[8].Orientation) * Quaternion.Euler(0, 180, 0), lerpRate);

                boneRightShoulder.transform.rotation = Quaternion.Slerp(boneRightShoulder.transform.rotation, GetRotation(joints[1].Orientation) * Quaternion.Euler(0, 180, 0), lerpRate);
                boneRightArm.transform.rotation = Quaternion.Slerp(boneRightArm.transform.rotation, GetRotation(joints[2].Orientation) * Quaternion.Euler(0, 180, 0), lerpRate);
                boneRightForeArm.transform.rotation = Quaternion.Slerp(boneRightForeArm.transform.rotation, GetRotation(joints[3].Orientation) * Quaternion.Euler(0, 180, 0), lerpRate);
                boneRightHand.transform.rotation = Quaternion.Slerp(boneRightHand.transform.rotation, GetRotation(joints[16].Orientation) * Quaternion.Euler(0, 180, 0), lerpRate);

                //boneRightShoulder.transform.right = V3(joints[1], joints[2]);
                //boneRightArm.transform.right = V3(joints[2], joints[3]);
                //boneRightForeArm.transform.right = V3(joints[3], joints[16]);
                //boneRightHand.transform.right = V3(joints[16], joints[4]);

                boneLeftShoulder.transform.rotation = Quaternion.Slerp(boneLeftShoulder.transform.rotation, GetRotation(joints[1].Orientation) * Quaternion.Euler(0, 180, 0), lerpRate);
                boneLeftArm.transform.rotation = Quaternion.Slerp(boneLeftArm.transform.rotation, GetRotation(joints[5].Orientation) * Quaternion.Euler(0, 180, 0), lerpRate);
                boneLeftForeArm.transform.rotation = Quaternion.Slerp(boneLeftForeArm.transform.rotation, GetRotation(joints[6].Orientation) * Quaternion.Euler(0, 180, 0), lerpRate);
                boneLeftHand.transform.rotation = Quaternion.Slerp(boneLeftHand.transform.rotation, GetRotation(joints[17].Orientation) * Quaternion.Euler(0, 180, 0), lerpRate);

                //boneLeftShoulder.transform.right = -V3(joints[1], joints[5]);
                //boneLeftArm.transform.right = -V3(joints[5], joints[6]);
                //boneLeftForeArm.transform.right = -V3(joints[6], joints[17]);
                //boneLeftHand.transform.right = -V3(joints[17], joints[7]);

                boneRightThigh.transform.rotation = Quaternion.Slerp(boneRightThigh.transform.rotation, GetRotation(joints[10].Orientation) * Quaternion.Euler(0, 180, 0), lerpRate);
                boneRightCalf.transform.rotation = Quaternion.Slerp(boneRightCalf.transform.rotation, GetRotation(joints[11].Orientation) * Quaternion.Euler(0, 180, 0), lerpRate);
                boneRightFoot.transform.rotation = Quaternion.Slerp(boneRightFoot.transform.rotation, GetRotation(joints[12].Orientation) * Quaternion.Euler(0, 180, 0), lerpRate);

                //boneRightThigh.transform.right = -V3(joints[10], joints[11]);
                //boneRightCalf.transform.right = -V3(joints[11], joints[12]);

                boneLeftThigh.transform.rotation = Quaternion.Slerp(boneLeftThigh.transform.rotation, GetRotation(joints[13].Orientation) * Quaternion.Euler(0, 180, 0), lerpRate);
                boneLeftCalf.transform.rotation = Quaternion.Slerp(boneLeftCalf.transform.rotation, GetRotation(joints[14].Orientation) * Quaternion.Euler(0, 180, 0), lerpRate);
                boneLeftFoot.transform.rotation = Quaternion.Slerp(boneLeftFoot.transform.rotation, GetRotation(joints[15].Orientation) * Quaternion.Euler(0, 180, 0), lerpRate);

                //boneLeftThigh.transform.right = -V3(joints[13], joints[14]);
                //boneLeftCalf.transform.right = -V3(joints[14], joints[15]);

                doll.transform.position = V3(joints[8].WorldPosition) / posScale + posOffset;
                //initPos = V3(joints[8].WorldPosition);

                break;
            }
        }
    }

    private Vector3 V3(Astra.Vector3D value)
    {
        return new Vector3(value.X, value.Y, value.Z);
    }

    private Vector3 V3(Astra.Joint joint1, Astra.Joint joint2)
    {
        Astra.Vector3D value1 = joint1.WorldPosition;
        Astra.Vector3D value2 = joint2.WorldPosition;
        return new Vector3(value2.X - value1.X, value2.Y - value1.Y, value2.Z - value1.Z);
    }

    private Quaternion GetRotation(Astra.Matrix3x3 value)
    {
        var rotationMatrix = new Matrix4x4();
        //for (int i = 0; i < 3; i++)
        //{
        //    for (int j = 0; j < 3; j++)
        //    {
        //        rotationMatrix[i, j] = value. [i, j];
        //    }
        //}
        rotationMatrix[0, 0] = value.M00;
        rotationMatrix[0, 1] = value.M01;
        rotationMatrix[0, 2] = value.M02;
        rotationMatrix[1, 0] = value.M10;
        rotationMatrix[1, 1] = value.M11;
        rotationMatrix[1, 2] = value.M12;
        rotationMatrix[2, 0] = value.M20;
        rotationMatrix[2, 1] = value.M21;
        rotationMatrix[2, 2] = value.M22;
        rotationMatrix[3, 3] = 1f;

        var localToWorldMatrix = rotationMatrix;

        //Vector3 scale;
        //scale.x = new Vector4(localToWorldMatrix.m00, localToWorldMatrix.m10, localToWorldMatrix.m20, localToWorldMatrix.m30).magnitude;
        //scale.y = new Vector4(localToWorldMatrix.m01, localToWorldMatrix.m11, localToWorldMatrix.m21, localToWorldMatrix.m31).magnitude;
        //scale.z = new Vector4(localToWorldMatrix.m02, localToWorldMatrix.m12, localToWorldMatrix.m22, localToWorldMatrix.m32).magnitude;
        //transform.localScale = scale;

        //Vector3 position;
        //position.x = localToWorldMatrix.m03;
        //position.y = localToWorldMatrix.m13;
        //position.z = localToWorldMatrix.m23;
        //transform.position = position;

        Vector3 forward;
        forward.x = localToWorldMatrix.m02;
        forward.y = localToWorldMatrix.m12;
        forward.z = localToWorldMatrix.m22;

        Vector3 upwards;
        upwards.x = localToWorldMatrix.m01;
        upwards.y = localToWorldMatrix.m11;
        upwards.z = localToWorldMatrix.m21;

        //transform.rotation = Quaternion.LookRotation(forward, upwards);
        return Quaternion.LookRotation(forward, upwards);
    }

    private void DrawLine(Astra.Joint startJoint, Astra.Joint endJoint, int index)
    {
        if (startJoint.Status == Astra.JointStatus.Tracked && endJoint.Status == Astra.JointStatus.Tracked)
        {
            var jointPos = startJoint.WorldPosition;
            var startPos = transform.TransformVector(jointPos.X, jointPos.Y, jointPos.Z);
            jointPos = endJoint.WorldPosition;
            var endPos = transform.TransformVector(jointPos.X, jointPos.Y, jointPos.Z);
            jointLines[index].SetPositions(new Vector3[] { startPos, endPos });
            jointLines[index].gameObject.SetActive(true);
            jointLines[index].gameObject.name = index.ToString();
        }
        else
        {
            jointLines[index].gameObject.SetActive(false);
        }
    }
}
