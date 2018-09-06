using UnityEngine;
using UnityEngine.UI;

public class HpManager : MonoBehaviour {

    public Slider hpBar;

    public int hpMax = 100;

    int hp = 100;

    public void InitHp()
    {
        SetHp(hpMax);
    }

    public void DoDamageHp(int point)
    {
        SetHp(hp - point);
    }

    public void DoSaveHp(int point)
    {
        SetHp(hp + point);
    }

    public void SetHp(int point)
    {
        hp = Mathf.Clamp(point, 0, hpMax);
        if (hpBar)
        hpBar.value = hp / (float)hpMax;
    }

}
