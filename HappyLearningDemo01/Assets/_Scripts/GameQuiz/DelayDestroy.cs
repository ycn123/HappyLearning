using UnityEngine;
using System.Collections;

/// <summary>
/// Ϊ���ӳ�������Ϸ����
/// </summary>
public class DelayDestroy : MonoBehaviour {
    //�ӳ�ʱ��
	public float delayTime = 2f;
    //һ��ʼִֻ��һ�� �ӳ�������Ϸ����
    void Start () {
		Destroy(gameObject, delayTime);
	}
}
