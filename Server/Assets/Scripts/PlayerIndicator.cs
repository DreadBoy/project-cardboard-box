using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIndicator : MonoBehaviour {

    Vector3 position;
    float time = 0;

	void Start () {
        position = transform.localPosition;
	}
	
	void Update () {
        time += Time.deltaTime;
        transform.localPosition = position + new Vector3(0, Mathf.Sin(time * 2) / 4, 0);
	}
}
