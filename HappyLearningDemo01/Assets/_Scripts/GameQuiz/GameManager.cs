using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
//using Holoville.HOTween.Plugins;

public class GameManager : MonoBehaviour
{
    // Ч��Ԥ����
    public GameObject goodEffect, badEffect, soulEffect, happyEffect;
    // ��ɫ�������
    public Animator friendAnimator, enemyAnimator;
    // ��ɫѪ���������͵ı���
    public HpManager friendHpMan, enemyHpMan;

    // ���������ʼλ�� ��ʿѪ���� �Ǽ�Ѫ���� ������
    Transform friendHpGroup, enemyHpGroup, shieldGroup;
    //���������ʼλ����ά���� ��ʿλ�� �Ǽ�λ�� ��ʿѪ��λ�� �Ǽ�Ѫ��λ�� ����λ��
    Vector3 friendPos, enemyPos, friendHpPos, enemyHpPos, shieldPos;
    

    // �����������ʺ�ѡ���չʾλ��
    Transform questionTf;
    Transform[] answerTfs;
    //��Ӧ�����ѡ���UI Text
    Text questionLabel;
    Text[] answerLabels;

    // ���ʷ�Χ���б����飩
    List<QuizData> quizList;
    //�б����������
    int quizTotal;
    //����ϵ��
    int quizIndex = 0;

    //��Inspector�������������ĳ��ȵĵ�����
    [HideInInspector]
    public int quizLength = 0;

    //������ж�����
    bool quizOn = true;

    //void Awake()
    //{
        // ������Ļ�ֱ���
        //Screen.SetResolution(1920, 1080, false); 
    //}
    void Start()
    {
        //��ʼ����Ϸ
        InitGame();
        //���ػ�ӭ��Ϸ
        HideGame();
        //StartGame();
    }

    // Ϊ������������Ϸ��ӭ���
    void HideGame()
    {
        ClearQuiz();
        Vector3 pos = friendPos;
        friendAnimator.transform.localPosition = new Vector3(pos.x * 3f, pos.y, pos.z);
        pos = enemyPos;

        enemyAnimator.transform.localPosition = new Vector3(pos.x * 3f, pos.y, pos.z);
        pos = friendHpPos;

        friendHpGroup.localPosition = new Vector3(pos.x * 5f, pos.y, pos.z);
        pos = enemyHpPos;

        enemyHpGroup.localPosition = new Vector3(pos.x * 5f, pos.y, pos.z);
        shieldGroup.localScale = new Vector3(2f, 2f, 1f);

        pos = shieldPos;
        shieldGroup.localPosition = new Vector3(pos.x, 0f, pos.z);
    }

    // ��ʼ��Ϸ�������µ�����
    public void StartGame()
    {
        IntroGame();
        DrawQuiz();
    }

    // ����������
    void DrawQuiz()
    {
        HideQuiz();
        StartCoroutine(DelayActoin(1f, () =>
        {
            SetQuiz();
            ShowQuiz();
        }));
    }

    // ��ʼ�����ʺ�ѡ����б�
    void QuizInit()
    {
        //ʵ������Ϸ�б��Լ�д���ࣩ ���������ѡ�������id�ͶԴ� �б����Ա���������ݵĲ���
        quizList = new List<QuizData>();
        //ʵ�����ַ������͵����ʺ����������б��⣩ 
        List<string> champs = new List<string>();
        //�����ʺ�ѡ��Ķ�ά����dic��ֵ
        string[,] dic = LolSkillData.dic;

        //������ά�����е��������� ��ȡ��������� ���ŵ�
        for (int i = 0; i < dic.GetLength(0); i++)
        {
            //ÿ�α�����ѡ������
            string champ = dic[i,2];
            //��ǰ������ʺ�ѡ���б�������ѡ������� ������Ϊ�˲��ظ���� ���ص��ǲ�ͬ��ַ Ҳ������ͬ���ַ���ֻҪ��ַ��ͬҲ�ᱻ��� 
            if (!champs.Contains(champ)) champs.Add(champ);
        }
        //������ά�����е�    ��ȡ������� 
        for (int i = 0; i < dic.GetLength(0); i++)
        {
            //ÿ�α�����Id����
            string idx = dic[i, 0];
            //ÿ�α��������������
            string skill = dic[i, 1];
            //ÿ�α�����ѡ�������
            string champ = dic[i, 2];
            //��ʼ����Ϸ�д������ݵ��� �����б�
            QuizData quiz = new QuizData();

            //���ַ������͵����ʺ�ѡ����б���Ѱ�Ҳ����ַ��������һ�γ��ֵ�λ�ò����ظ�λ�����
            int t = champs.IndexOf(champ);
            //��ʼ�����͵�Id���б�
            List<int> ansIdList = new List<int>();
            //��ʼ�� ��ϣ�� �������Ϊ һά һһ��Ӧ
            Hashtable ansValList = new Hashtable();
            //��
            ansValList[0] = champs[t];
            //��
            ansValList[1] = champs[(t+1)%champs.Count];
            //��
            ansValList[2] = champs[(t+2)%champs.Count];
            //��
            ansValList[3] = champs[(t+3)%champs.Count];
            //��
            for (int j = 0; j < 4; j++) ansIdList.Add(j);
            //��
            ansIdList.Shuffle();
            //��
            for (int j = 0; j < 4; j++)
                if (ansIdList[j] == 0) quiz.correct = j;
            quiz.answer1 = "1. " + ansValList[ansIdList[0]] as string;
            quiz.answer2 = "2. " + ansValList[ansIdList[1]] as string;
            quiz.answer3 = "3. " + ansValList[ansIdList[2]] as string;
            quiz.answer4 = "4. " + ansValList[ansIdList[3]] as string;
            quiz.question = skill + "?";
            quiz.id = int.Parse(idx);
            quizList.Add(quiz);
        }

        quizTotal = quizList.Count;
    }

    // ��ʼ����Ϸ
    void InitGame()
    {
        //��ʼ��Ѫ��
        friendHpMan.InitHp();
        enemyHpMan.InitHp();
        //ͨ�����ַ������ʵ�λ��
        questionTf = GameObject.Find("Question").transform;
        //��ȡ�����������µ�text���
        questionLabel = questionTf.GetComponentInChildren<Text>();
        //��ѡ��ʵ������Text�������Transform����
        answerLabels = new Text[4];
        answerTfs = new Transform[4];
        //���ͱ���i Ϊ�˴�0��ʼ���������������в����ĸ�ѡ���λ�� �൱����������
        int i = 0;
        //��������ѡ���λ��
        foreach (Transform tf in GameObject.Find("Answers").transform)
        {
            //ÿ�β鵽��ѡ���λ�þͰ�λ�÷Ž�������
            answerTfs[i] = tf;
            //ÿ�β��һ�ȡѡ�����ѡ���Label��text���
            answerLabels[i] = tf.GetComponentInChildren<Text>();
            //i��0��ʼ����ֱ��������������Answers������λ��
            i++;
        }
        //��ʼ�������б�
        QuizInit();

        //���ܵĳ�ʼλ�ø�ֵ
        shieldGroup = GameObject.Find("ShieldGroup").transform;
        //���ܵĳ�ʼλ��������ֵ
        shieldPos = shieldGroup.localPosition;
        //����ʿ�͹Ǽܵ�λ��������ֵ
        friendPos = friendAnimator.transform.localPosition;
        enemyPos = enemyAnimator.transform.localPosition;
        //����ʿ�͹Ǽܵ�Ѫ����������λ�ø�ֵ Ϊ�˻�ȡ��������
        friendHpGroup = friendHpMan.hpBar.transform.parent;
        enemyHpGroup = enemyHpMan.hpBar.transform.parent;
        //����ʿ�͹Ǽܵ�Ѫ��λ��������ֵ
        friendHpPos = friendHpGroup.localPosition;
        enemyHpPos = enemyHpGroup.localPosition;
    }

    // ������Ϸ��ʼ����
    void IntroGame()
    {
        friendAnimator.CrossFade("Walk", 0.2f);
        enemyAnimator.CrossFade("Walk", 0.2f);

        Vector3 pos = friendPos;
        friendAnimator.transform.localPosition = new Vector3(pos.x * 3f, pos.y, pos.z);
        TweenParms parms = new TweenParms().Prop("localPosition", friendPos).Ease(EaseType.Linear).OnComplete(OnFriendStop);
        HOTween.To(friendAnimator.transform, 2f, parms);
        
        pos = enemyPos;
        enemyAnimator.transform.localPosition = new Vector3(pos.x * 3f, pos.y, pos.z);
        parms = new TweenParms().Prop("localPosition", pos).Ease(EaseType.Linear).OnComplete(OnEnemyStop);
        HOTween.To(enemyAnimator.transform, 2f, parms);

        pos = shieldPos;
        shieldGroup.localPosition = new Vector3(pos.x, 0f, pos.z);
        parms = new TweenParms().Prop("localPosition", pos).Delay(1f);
        HOTween.To(shieldGroup, 1f, parms);

        shieldGroup.localScale = new Vector3(2f, 2f, 1f);
        parms = new TweenParms().Prop("localScale", new Vector3(0.8f, 0.8f, 1f));
        HOTween.To(shieldGroup, 1f, parms);

        pos = friendHpPos;
        friendHpGroup.localPosition = new Vector3(pos.x * 5f, pos.y, pos.z);
        parms = new TweenParms().Prop("localPosition", pos).Delay(0.5f);
        HOTween.To(friendHpGroup, 1f, parms);

        pos = enemyHpPos;
        enemyHpGroup.localPosition = new Vector3(pos.x * 5f, pos.y, pos.z);
        parms = new TweenParms().Prop("localPosition", pos).Delay(0.5f);
        HOTween.To(enemyHpGroup, 1f, parms);
    }

    // �ö���ֹͣ���� Ĭ��״̬
    void OnFriendStop()
    {
        friendAnimator.CrossFade("Idle", 0.2f);
    }

    // �ö���ֹͣ���� Ĭ��״̬
    void OnEnemyStop()
    {
        enemyAnimator.CrossFade("Idle", 0.2f);
    }

    // �����Ϸ��ʾ
    void ClearQuiz()
    {
        questionTf.localScale = new Vector3(0f, 1f, 1f);
        int i = -1;
        foreach (Transform tf in answerTfs)
        {
            tf.localPosition = new Vector3(1000f * i, tf.localPosition.y, tf.localPosition.z);
            i *= -1;
        }
    }

    // �������⶯�� 
    void HideQuiz()
    {
        TweenParms parms = new TweenParms().Prop("localScale", new Vector3(0f, 1f, 1f));
        HOTween.To(questionTf, 0.5f, parms);
        int i = -1;
        foreach (Transform tf in answerTfs)
        {
            parms = new TweenParms().Prop("localPosition", new Vector3(1000f * i, tf.localPosition.y, tf.localPosition.z));
            HOTween.To(tf, 0.5f, parms);
            i *= -1;
        }
    }

    // ����ֻ�һ����ʾ�ʴ���
    void TypeQuiz()
    {
        questionLabel.text = quizList[quizIndex].question.Substring(0, quizLength);
    }

    // ��ʾ�ʴ𶯻�
    void ShowQuiz()
    {
        TweenParms parms = new TweenParms().Prop("localScale", new Vector3(1f, 1f, 1f));
        HOTween.To(questionTf, 0.5f, parms);
        int i = 1;
        foreach (Transform tf in answerTfs)
        {
            parms = new TweenParms().Prop("localPosition", new Vector3(0f, tf.localPosition.y, tf.localPosition.z)).Delay(0.3f * i++);
            HOTween.To(tf, 0.5f, parms);
        }
        quizOn = true;

        quizLength = 0;
        parms = new TweenParms().Prop("quizLength", quizList[quizIndex].question.Length).Ease(EaseType.Linear).OnUpdate(TypeQuiz);
        HOTween.To(this, 1f, parms);
    }

    // �����ַ�������󳤶�
    string QuizMakeString(string str) 
    {
        return (str.Length > 41) ? str.Substring(0, 40) : str;
    }

    // ��������ͻش�ı���
    void SetQuiz()
    {
        quizIndex = Random.Range(0, quizTotal) % quizTotal;
        QuizData item = quizList[quizIndex];
        answerLabels[0].text = QuizMakeString(item.answer1);
        answerLabels[1].text = QuizMakeString(item.answer2);
        answerLabels[2].text = QuizMakeString(item.answer3);
        answerLabels[3].text = QuizMakeString(item.answer4);
        questionLabel.text = item.question;
    }
    
	void Update () {
        // �˳���ʽ
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
        
	}

    void ClickAnswer(int no)
    {
        if (!quizOn) return;
        quizOn = false;
        QuizData item = quizList[quizIndex];

        // Is answer correct?
        if (item.correct == no) 
        {
            // Display good Effect
            Instantiate(goodEffect);
            // Display soul trail effect
            GameObject go = Instantiate(soulEffect) as GameObject;
            go.GetComponent<SoulEffect>().posX = -1f;
            // Display Happy Effect
            StartCoroutine(DelayActoin(0.6f, () =>
            {
                go = Instantiate(happyEffect, new Vector3(-0.7f, 1f, 0f), Quaternion.identity) as GameObject;
                enemyHpMan.DoDamageHp(10);
            }));
            // Display Actor's motion
            friendAnimator.CrossFade("Good", 0.2f);
            enemyAnimator.CrossFade("Bad", 0.2f);
        }
        else
        {
            // Display Bad Effect
            Instantiate(badEffect);
            // Display soul trail effect
            GameObject go = Instantiate(soulEffect) as GameObject;
            go.GetComponent<SoulEffect>().posX = 1f;
            // Display Happy Effect
            StartCoroutine(DelayActoin(0.6f, () =>
            {
                go = Instantiate(happyEffect, new Vector3(0.7f, 1f, 0f), Quaternion.identity) as GameObject;
                friendHpMan.DoDamageHp(10);
            }));
            // Display Actor's motion
            friendAnimator.CrossFade("Bad", 0.2f);
            enemyAnimator.CrossFade("Good", 0.2f);
        }

        StartCoroutine(DelayActoin(3f, () =>
        {
            DrawQuiz();
        }));
    }

    //����ش�ѡ���Ķ�Ӧ����
    public void OnClickAnswer1()
    {
        ClickAnswer(0);
    }
    public void OnClickAnswer2()
    {
        ClickAnswer(1);
    }
    public void OnClickAnswer3()
    {
        ClickAnswer(2);
    }
    public void OnClickAnswer4()
    {
        ClickAnswer(3);
    }

    // ʱ���ӳٵĶ���
    public IEnumerator DelayActoin(float dtime, System.Action callback)
    {
        yield return new WaitForSeconds(dtime);
        callback();
    }
}
