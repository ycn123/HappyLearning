using UnityEngine;
using Holoville.HOTween;
using Holoville.HOTween.Plugins;

public class SoulEffect : MonoBehaviour {
    Transform tr;
    Vector3 startPos;
    public float posX;

	void Start ()
    {
        tr = transform;
        startPos = tr.localPosition;
        
        DoSkillEffect();
    }

    void OnDoneEffect()
    {
        Destroy(gameObject, 1f);
    }

    public void DoSkillEffect()
    {
        Vector3[] path = new Vector3[] { startPos, new Vector3(posX, 2f, startPos.z), new Vector3(posX * -2f, 6f, startPos.z) };
        tr.localPosition = startPos;
        HOTween.To(tr, 1f, new TweenParms().Prop("localPosition", new PlugVector3Path(path, EaseType.Linear, true)).OnComplete(OnDoneEffect));
    }
}
