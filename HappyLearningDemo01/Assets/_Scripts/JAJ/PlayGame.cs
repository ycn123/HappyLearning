using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[Serializable]class GameScore
{
    public int score;
    public string playerName;
}

class QueryRankResult
{
    public List<GameScore> results = null;
}

public class PlayGame : MonoBehaviour {

    
    public float Factor;
    public float MaxDistance = 5;

    public Transform Spring;
    public Transform WordCanvasPos;
    

    public GameObject Stage;
    public GameObject WordCanvas;
    public GameObject Particle;
    public GameObject[] BoxTemplates;
    public GameObject[] WordTemplates;
    public GameObject SaveScorePanel;
    public GameObject RankName;
    public GameObject RankScore;
    public GameObject RankPanel;


    public Text TotalScoreText;
    public Text SingleScoreText;
    public Text WordText;
    public InputField NameField;
    public Button SaveButton;
    public Button RestartButton;

    public string LeanCloudAppId;
    public string LeanCloudAppKey;



   
    private float _startTime;
    private float _scoreAnimationStartTime;

    private int _score;
    private int _lastReward = 1;

    private Rigidbody _rigidbody;

    private GameObject _currentStage;
    private GameObject _currentWord;

    private Vector3 _cameraRelativePosition;
    private Vector3 _direction = new Vector3(1, 0, 0);   

    private bool _isUpdateScoreAnimation;
    private bool _enableInput = true;

    private LeanCloudRestAPI _leanCloud;

    void Start ()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.centerOfMass = new Vector3(0, 0, 0);

        _currentStage = Stage;
        _currentWord = WordCanvas;

        SpawnStage();

                
        _cameraRelativePosition = Camera.main.transform.position - transform.position;

        SaveButton.onClick.AddListener(OnClickSaveButton);
        RestartButton.onClick.AddListener(() => { SceneManager.LoadScene(0); });

        _leanCloud = new LeanCloudRestAPI(LeanCloudAppId, LeanCloudAppKey);
    }

    private void ShowWords(Vector3 pos)
    {

        GameObject wordprefab;
        

        if (WordTemplates.Length > 0)
        {
            wordprefab = WordTemplates[Random.Range(0, WordTemplates.Length)];
            var word = Instantiate(wordprefab,WordCanvasPos);
            _currentWord = word;
            word.transform.position = pos;
            
        }
        else
        {
            wordprefab = WordCanvas;
            var word = Instantiate(wordprefab);
            _currentWord = word;
            word.transform.position = pos;
          
        }
        
    }

    private void OnClickSaveButton()
    {
        var nickname = NameField.text;

        if (nickname.Length == 0)
        return;

        GameScore gameScore = new GameScore
            {
        score = _score,
        playerName = nickname
            };

        StartCoroutine(_leanCloud.Create("GameScore", JsonUtility.ToJson(gameScore, false), ShowRankPanel));
        SaveScorePanel.SetActive(false);
    }

    private void ShowRankPanel()
    {        
        var param = new Dictionary<string, object>();
        param.Add("order", "-score");
        param.Add("limit", 10);

        StartCoroutine(_leanCloud.Query("GameScore", param, 
        t =>
        {
            var results = JsonUtility.FromJson<QueryRankResult>(t);
            var scores = new List<KeyValuePair<string, string>>();

            foreach (var result in results.results)
            {
                scores.Add(new KeyValuePair<string, string>(result.playerName, result.score.ToString()));
            }
            foreach (var score in scores)
            {
                var item = Instantiate(RankName);
                item.SetActive(true);
                item.GetComponent<Text>().text = score.Key;
                item.transform.SetParent(RankName.transform.parent);

                item = Instantiate(RankScore);
                item.SetActive(true);
                item.GetComponent<Text>().text = score.Value;
                item.transform.SetParent(RankScore.transform.parent);
            }
            RankPanel.SetActive(true);
        }));
    }

    private void SpawnStage()
    {
        GameObject prefab;

        if (BoxTemplates.Length > 0)
        {
            prefab = BoxTemplates[Random.Range(0, BoxTemplates.Length)];
        }
        else
        {
            prefab = Stage;
        }
        var stage = Instantiate(prefab);
        stage.transform.position = _currentStage.transform.position + _direction * Random.Range(1.9f, MaxDistance);

        var randomScale = Random.Range(1.5f, 2f);
        stage.transform.localScale = new Vector3(randomScale, 0.5f, randomScale);

        stage.GetComponent<Renderer>().material.color =
        new Color(Random.Range(0f, 1), Random.Range(0f, 1), Random.Range(0f, 1));

        ShowWords(stage.transform.position + Vector3.up * (stage.transform.localScale.y / 2 + 0.01f));
    }

    
    void Update ()
    {
        if (_enableInput)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _startTime = Time.time;
                Particle.SetActive(true);
            }

            if (Input.GetMouseButtonUp(0))
            {
                var elapse = Time.time - _startTime;
                OnJump(elapse);
                Particle.SetActive(false);

                                                
                _currentStage.transform.DOLocalMoveY(-0.25f, 0.2f);
                _currentStage.transform.DOScaleY(0.5f, 0.2f);
                DestroyWord();

                _enableInput = false;
            }

            if (Input.GetMouseButton(0))
            {
                if (_currentStage.transform.localScale.y > 0.3)
                {
                    _currentStage.transform.localScale += new Vector3(0, -1, 0) * 0.15f * Time.deltaTime;
                    _currentStage.transform.localPosition += new Vector3(0, -1, 0) * 0.15f * Time.deltaTime;
                    
                }
            }
        }


        if (_isUpdateScoreAnimation)
            UpdateScoreAnimation();
    }

    private void DestroyWord()
    {
        _currentWord.transform.DOLocalMoveZ(100, 5f);
        
    }

    private void UpdateScoreAnimation()
    {
        if (Time.time - _scoreAnimationStartTime > 1)
        _isUpdateScoreAnimation = false;

        var playerScreenPos =
        RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);
        SingleScoreText.transform.position = playerScreenPos + 
                                            Vector2.Lerp(Vector2.zero, new Vector2(0, 200),
                                            Time.time - _scoreAnimationStartTime);
        SingleScoreText.color = Color.Lerp(Color.black, new Color(0, 0, 0, 0), Time.time - _scoreAnimationStartTime);
    }

    private void OnJump(float elapse)
    {
        _rigidbody.AddForce(new Vector3(0, 5f, 0) + (_direction) * elapse * Factor, ForceMode.Impulse);
        transform.DOLocalRotate(new Vector3(0, 0, -360), 0.6f, RotateMode.LocalAxisAdd); 
    }

    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Ground")
        {
            OnGameOver();
            _enableInput = false;
        }
        else
        {
            if (_currentStage != collision.gameObject)
            {
                var contacts = collision.contacts;
                if (contacts.Length == 4 && contacts[0].normal == Vector3.up)
                {
                    _currentStage = collision.gameObject;
                    AddScore(contacts);
                    RandomDirection();
                    SpawnStage();
                    MoveCamera();

                    _enableInput = true;
                }
                else
                {
                    OnGameOver();
                    _enableInput = false;
                }
            }
            else
            {
                var contacts = collision.contacts;

                if (contacts.Length == 4 && contacts[0].normal == Vector3.up)
                {
                    _enableInput = true;
                }
                else
                {
                    OnGameOver();
                    _enableInput = false;
                }
            }
        }
    }

    
    private void OnGameOver()
    {
        if (_score > 0)
        {
            SaveScorePanel.SetActive(true);
            _enableInput = false;
        }
        else
        {
            ShowRankPanel();
            _enableInput = false;
        }
    }

    private void AddScore(ContactPoint[] contacts)
    {
        if (contacts.Length > 0)
        {
            var hitPoint = contacts[0].point;
            hitPoint.y = 0;

            var stagePos = _currentStage.transform.position;
            stagePos.y = 0;

            var precision = Vector3.Distance(hitPoint, stagePos);
            if (precision < 0.1)
                _lastReward *= 2;
            else
                _lastReward = 1;

            _score += _lastReward;
            TotalScoreText.text = _score.ToString();
            ShowScoreAnimation();
        }
    }

    void RandomDirection()
    {
        var seed = Random.Range(0, 2);
        _direction = seed == 0 ? new Vector3(1, 0, 0) : new Vector3(0, 0, 1);
        transform.right = _direction;
    }

    void MoveCamera()
    {
        Camera.main.transform.DOMove(transform.position + _cameraRelativePosition, 1);
    }

    private void ShowScoreAnimation()
    {
       _isUpdateScoreAnimation = true;
       _scoreAnimationStartTime = Time.time;
       SingleScoreText.text = "+" + _lastReward;
    }


}
