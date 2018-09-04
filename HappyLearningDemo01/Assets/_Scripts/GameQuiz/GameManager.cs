using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
//using Holoville.HOTween.Plugins;

public class GameManager : MonoBehaviour
{
    // 效果预制体
    public GameObject goodEffect, badEffect, soulEffect, happyEffect;
    // 角色动画组件
    public Animator friendAnimator, enemyAnimator;
    // 角色血量管理类型的变量
    public HpManager friendHpMan, enemyHpMan;

    // 用来保存初始位置 骑士血量组 骨架血量组 盾牌组
    Transform friendHpGroup, enemyHpGroup, shieldGroup;
    //用来保存初始位置三维向量 骑士位置 骨架位置 骑士血量位置 骨架血量位置 盾牌位置
    Vector3 friendPos, enemyPos, friendHpPos, enemyHpPos, shieldPos;
    

    // 用来保存提问和选项的展示位置
    Transform questionTf;
    Transform[] answerTfs;
    //对应问题和选项的UI Text
    Text questionLabel;
    Text[] answerLabels;

    // 提问范围的列表（数组）
    List<QuizData> quizList;
    //列表问题的总数
    int quizTotal;
    //问题系数
    int quizIndex = 0;

    //在Inspector面板里隐藏问题的长度的调整框
    [HideInInspector]
    public int quizLength = 0;

    //问题的判断条件
    bool quizOn = true;

    //void Awake()
    //{
        // 设置屏幕分辨率
        //Screen.SetResolution(1920, 1080, false); 
    //}
    void Start()
    {
        //初始化游戏
        InitGame();
        //隐藏欢迎游戏
        HideGame();
        //StartGame();
    }

    // 为了提问隐藏游戏欢迎面板
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

    // 开始游戏并设置新的提问
    public void StartGame()
    {
        IntroGame();
        DrawQuiz();
    }

    // 设置新提问
    void DrawQuiz()
    {
        HideQuiz();
        StartCoroutine(DelayActoin(1f, () =>
        {
            SetQuiz();
            ShowQuiz();
        }));
    }

    // 初始化提问和选项的列表
    void QuizInit()
    {
        //实例化游戏列表（自己写的类） 除了问题和选项还包括了id和对错 列表是以便后续对数据的操作
        quizList = new List<QuizData>();
        //实例化字符串类型的提问和问题数组列表（库） 
        List<string> champs = new List<string>();
        //给提问和选项的二维数组dic赋值
        string[,] dic = LolSkillData.dic;

        //遍历二维数组中的提问数据 获取数组的行数 横着的
        for (int i = 0; i < dic.GetLength(0); i++)
        {
            //每次遍历的选项数据
            string champ = dic[i,2];
            //给前面的提问和选项列表进行添加选项的数据 条件是为了不重复添加 返回的是不同地址 也就是相同的字符串只要地址不同也会被添加 
            if (!champs.Contains(champ)) champs.Add(champ);
        }
        //遍历二维数组中的    获取数组的行 
        for (int i = 0; i < dic.GetLength(0); i++)
        {
            //每次遍历的Id数据
            string idx = dic[i, 0];
            //每次遍历的问题的数据
            string skill = dic[i, 1];
            //每次遍历的选项的数据
            string champ = dic[i, 2];
            //初始化游戏中处理数据的类 不是列表
            QuizData quiz = new QuizData();

            //在字符串类型的提问和选项的列表中寻找参数字符串数组第一次出现的位置并返回该位置序号
            int t = champs.IndexOf(champ);
            //初始化整型的Id的列表
            List<int> ansIdList = new List<int>();
            //初始化 哈希表 可以理解为 一维 一一对应
            Hashtable ansValList = new Hashtable();
            //？
            ansValList[0] = champs[t];
            //？
            ansValList[1] = champs[(t+1)%champs.Count];
            //？
            ansValList[2] = champs[(t+2)%champs.Count];
            //？
            ansValList[3] = champs[(t+3)%champs.Count];
            //？
            for (int j = 0; j < 4; j++) ansIdList.Add(j);
            //？
            ansIdList.Shuffle();
            //？
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

    // 初始化游戏
    void InitGame()
    {
        //初始化血量
        friendHpMan.InitHp();
        enemyHpMan.InitHp();
        //通过名字返回提问的位置
        questionTf = GameObject.Find("Question").transform;
        //获取提问子物体下的text组件
        questionLabel = questionTf.GetComponentInChildren<Text>();
        //给选项实例化成Text的数组和Transform数组
        answerLabels = new Text[4];
        answerTfs = new Transform[4];
        //整型变量i 为了从0开始递增依次在数组中查找四个选项的位置 相当于数组的序号
        int i = 0;
        //遍历查找选项的位置
        foreach (Transform tf in GameObject.Find("Answers").transform)
        {
            //每次查到到选项的位置就把位置放进数组中
            answerTfs[i] = tf;
            //每次查找获取选项的子选项的Label的text组件
            answerLabels[i] = tf.GetComponentInChildren<Text>();
            //i从0开始递增直至遍历到场景中Answers的所有位置
            i++;
        }
        //初始化提问列表
        QuizInit();

        //给盾的初始位置赋值
        shieldGroup = GameObject.Find("ShieldGroup").transform;
        //给盾的初始位置向量赋值
        shieldPos = shieldGroup.localPosition;
        //给骑士和骨架的位置向量赋值
        friendPos = friendAnimator.transform.localPosition;
        enemyPos = enemyAnimator.transform.localPosition;
        //给骑士和骨架的血量管理器的位置赋值 为了获取上面的组件
        friendHpGroup = friendHpMan.hpBar.transform.parent;
        enemyHpGroup = enemyHpMan.hpBar.transform.parent;
        //给骑士和骨架的血量位置向量赋值
        friendHpPos = friendHpGroup.localPosition;
        enemyHpPos = enemyHpGroup.localPosition;
    }

    // 设置游戏初始动画
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

    // 让队友停止动画 默认状态
    void OnFriendStop()
    {
        friendAnimator.CrossFade("Idle", 0.2f);
    }

    // 让对手停止动画 默认状态
    void OnEnemyStop()
    {
        enemyAnimator.CrossFade("Idle", 0.2f);
    }

    // 清除游戏显示
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

    // 隐藏问题动画 
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

    // 像打字机一样显示问答题
    void TypeQuiz()
    {
        questionLabel.text = quizList[quizIndex].question.Substring(0, quizLength);
    }

    // 显示问答动画
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

    // 设置字符串的最大长度
    string QuizMakeString(string str) 
    {
        return (str.Length > 41) ? str.Substring(0, 40) : str;
    }

    // 设置问题和回答的变量
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
        // 退出方式
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

    //点击回答选项后的对应方法
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

    // 时间延迟的动作
    public IEnumerator DelayActoin(float dtime, System.Action callback)
    {
        yield return new WaitForSeconds(dtime);
        callback();
    }
}
