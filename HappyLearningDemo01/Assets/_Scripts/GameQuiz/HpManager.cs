using UnityEngine;
using UnityEngine.UI;
//using System.Collections;

/// <summary>
/// Ѫ��������
/// </summary>
public class HpManager : MonoBehaviour {
    // Ѫ������
    public Slider hpBar;

    // ���Ѫ��ֵ 
    public int hpMax = 100;
    //int mpMax = 100;

    //��ǰѪ��ֵ
    int hp = 100;
    //int mp = 100;

    // ��ʼ��Ѫ��ֵ
    public void InitHp()
    {
        //�������hpMax=100�൱�������100/100=1 Ѫ��Ϊ��
        SetHp(hpMax);
    }

    // Init MP State
    //public void InitMp()
    //{
      //  SetHp(mpMax);
    //}

    // ��Ѫ�������˺�
    public void DoDamageHp(int point)
    {
        //������� ����ʼѪ����ȥ�˺�ֵ��
        SetHp(hp - point);
    }

    // ��Ѫ����
    public void DoSaveHp(int point)
    {
        //�����������ʼֵ���ϻ�Ѫ����
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
        //����Ѫ����0��100֮��
        hp = Mathf.Clamp(point, 0, hpMax);
        //��������ֵ
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
