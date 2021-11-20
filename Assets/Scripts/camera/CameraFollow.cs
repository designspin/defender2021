using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //offset from the viewport center to fix damping
    public float m_DampTime = 10f;
    public Transform m_Target;
    public float m_XOffset = 0;
    public float m_YOffset = 0;

    public bool isSwitching = false;


	void Start () {
		if (m_Target==null){
			m_Target = GameObject.FindGameObjectWithTag("Player").transform;
		}
	}

    void LateUpdate() {
        if(m_Target) {
            float desiredX = m_Target.position.x + m_XOffset;
            float smoothedPosition = Mathf.Lerp(transform.position.x, desiredX, 1/m_DampTime * Time.deltaTime);
            transform.position = new Vector3(smoothedPosition, transform.position.y, transform.position.z);
        }
    }

    public void instantUpdate(Vector3 distance)
    {
        float targetX = m_Target.position.x + m_XOffset;
		//float targetY = m_Target.position.y + m_YOffset;
        
        transform.position = distance;//new Vector3(targetX, transform.position.y, transform.position.z) + distance;
    }
}
