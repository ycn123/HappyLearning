using UnityEngine;
//using System.Collections;
using Holoville.HOTween;
using Holoville.HOTween.Plugins;

/// <summary>
/// ��βЧ��
/// </summary>
public class SoulEffect : MonoBehaviour {
    //λ�ñ���
    Transform tr;
    //��ʼλ�õ�����
    Vector3 startPos;
    //X�������ֵ
    public float posX;

	void Start () {
        //��λ�ý��г�ʼ��ֵ ���ڻ�ȡλ���ϵ����
        tr = transform;
        //��������������ֵ����ʼλ��
        startPos = tr.localPosition;
        //startPos.z = -1f;
        //posX = ((UnityEngine.Random.Range(0, 2) % 2) * 2 - 1)*1f;
        DoSkillEffect();
    }

    // ������ɺ����ٵķ���
    void OnDoneEffect()
    {
        Destroy(gameObject, 1f);
    }

    // ����Ч��
    public void DoSkillEffect()
    {
        //����·����������
        Vector3[] path = new Vector3[] { startPos, new Vector3(posX, 2f, startPos.z), new Vector3(posX * -2f, 6f, startPos.z) };
        //����ʼλ�ø�ֵ��Ч��������λ��
        tr.localPosition = startPos;
        //�ö����Ĳ��ʵ�ֶ���Ч�� To(����Ŀ�꣬������ʱ�䣬�����Ķ��������֣�������״̬�Լ�������ķ�����)
        HOTween.To(tr, 1f, new TweenParms().Prop("localPosition", new PlugVector3Path(path, EaseType.Linear, true)).OnComplete(OnDoneEffect));
    }
}
