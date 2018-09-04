using UnityEngine;
using UnityEngine.UI;
//using System.Collections;

/// <summary>
/// 血量管理器
/// </summary>
public class HpManager : MonoBehaviour {
    // 血量滑条
    public Slider hpBar;

    // 最大血量值 
    public int hpMax = 100;
    //int mpMax = 100;

    //当前血量值
    int hp = 100;
    //int mp = 100;

    // 初始化血量值
    public void InitHp()
    {
        //传入参数hpMax=100相当于最后是100/100=1 血量为满
        SetHp(hpMax);
    }

    // Init MP State
    //public void InitMp()
    //{
      //  SetHp(mpMax);
    //}

    // 给血量设置伤害
    public void DoDamageHp(int point)
    {
        //传入参数 （初始血量减去伤害值）
        SetHp(hp - point);
    }

    // 回血设置
    public void DoSaveHp(int point)
    {
        //传入参数（初始值加上回血量）
        SetHp(hp + point);
    }

    // Set Recover on MP State
   // public void DoSaveMp(int point)
    //{
      //  SetMp(mp + point);
    //}

    // Set HP State
    public void SetHp(int point)
    {
        //限制血量在0到100之间
        hp = Mathf.Clamp(point, 0, hpMax);
        //给滑条赋值
        if (hpBar)
            hpBar.value = (float)hp / (float)hpMax;
    }

    // Set MP State
   // public void SetMp(int point)
    //{
      //  mp = Mathf.Clamp(point, 0, mpMax);
        //if (mpBar)
          //  mpBar.value = (float)mp / (float)mpMax;
    //}

}
