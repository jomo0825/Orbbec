using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyStage1 : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown("j"))
		{
			StreamViewModel.Instance.bodyStream.Value = true;
		}
		if (Input.GetKeyDown("k"))
		{
			StreamViewModel.Instance.bodyStream.Value = false;
		}


	}
}
