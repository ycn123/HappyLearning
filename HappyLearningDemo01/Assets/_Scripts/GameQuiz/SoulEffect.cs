using UnityEngine;
//using System.Collections;
using Holoville.HOTween;
using Holoville.HOTween.Plugins;

/// <summary>
/// 拖尾效果
/// </summary>
public class SoulEffect : MonoBehaviour {
    //位置变量
    Transform tr;
    //开始位置的向量
    Vector3 startPos;
    //X方向的数值
    public float posX;

	void Start () {
        //将位置进行初始赋值 用于获取位置上的组件
        tr = transform;
        //自身坐标向量赋值给开始位置
        startPos = tr.localPosition;
        //startPos.z = -1f;
        //posX = ((UnityEngine.Random.Range(0, 2) % 2) * 2 - 1)*1f;
        DoSkillEffect();
    }

    // 动画完成后销毁的方法
    void OnDoneEffect()
    {
        Destroy(gameObject, 1f);
    }

    // 动画效果
    public void DoSkillEffect()
    {
        //设置路径的向量组
        Vector3[] path = new Vector3[] { startPos, new Vector3(posX, 2f, startPos.z), new Vector3(posX * -2f, 6f, startPos.z) };
        //将开始位置赋值给效果的自身位置
        tr.localPosition = startPos;
        //用动画的插件实现动画效果 To(动画目标，持续的时间，怎样的动画（名字，动画的状态以及结束后的方法）)
        HOTween.To(tr, 1f, new TweenParms().Prop("localPosition", new PlugVector3Path(path, EaseType.Linear, true)).OnComplete(OnDoneEffect));
    }
}
