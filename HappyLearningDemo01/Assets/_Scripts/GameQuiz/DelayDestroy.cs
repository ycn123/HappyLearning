using UnityEngine;
using System.Collections;

/// <summary>
/// 为了延迟销毁游戏对象
/// </summary>
public class DelayDestroy : MonoBehaviour {
    //延迟时间
	public float delayTime = 2f;
    //一开始只执行一次 延迟销毁游戏对象
    void Start () {
		Destroy(gameObject, delayTime);
	}
}
