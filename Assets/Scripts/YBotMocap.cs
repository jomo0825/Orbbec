using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YBotMocap : MonoBehaviour {

	public Animator anim;
	public Vector3 a;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void OnAnimatorIK (int layer) {
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
            var joints = body.Joints;

            anim.SetBoneLocalRotation(HumanBodyBones.RightLowerArm, GetRotation(joints[3].Orientation) * Quaternion.Euler(a.x, a.y, a.z));
        }
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

}
