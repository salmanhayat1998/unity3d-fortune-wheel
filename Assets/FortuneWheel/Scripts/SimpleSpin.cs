
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class SimpleSpin : MonoBehaviour
{
    public bool canSpin = true;
    [Range(4,12)]
    public int sections=4;
    [Range(120f, 420f)]
    public float radius=173f;
    public int spinIterations = 5;
    public Button TurnButton;
    public GameObject Circle; 			// Rotatable Object with rewards
    public GameObject slicePrefab;
    public Text CurrentCoinsText; 		// Pop-up text with wasted or rewarded coins amount
    public RadialLayout radialLayout;
    public bool rotateItems = true;
    public List<int> sectors = new List<int>();
    public List<item> spinWheelItems = new List<item>();


    private bool _isStarted;
    //private float[] _sectorsAngles;
    private float _finalAngle;
    [SerializeField]private float _startAngle = 0;
    private float _currentLerpRotationTime;

    private void Start()
    {
        createWheel();
    }
    float temp;
    private void createWheel()
    {
        radialLayout.fDistance = radius;
        float angle = radialLayout.MaxAngle / sections;
     //   temp -= angle;
        for (int i = 0; i < sections; i++)
        {
           var _item = Instantiate(slicePrefab,radialLayout.transform);
            _item.GetComponent<spinItem>().itemname.text = spinWheelItems[i].label;
            _item.GetComponent<spinItem>().icon.sprite = spinWheelItems[i].icon;
            temp += angle;
            sectors.Add((int)temp);
            if (rotateItems)
            {   
                _item.transform.rotation = Quaternion.Euler(0, 0, angle*i);
            }
        }
    }
    private void OnValidate()
    {
        
        //Debug.LogError("changed");
        //item[] arr = new item[sections];
        //spinWheelItems.Clear();
        //spinWheelItems.AddRange(arr);
    }
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TurnWheel();
        }

        // Make turn button non interactable if user has not enough money for the turn
        //if (_isStarted || CurrentCoinsAmount < TurnCost)
        //{
        //    TurnButton.interactable = false;
        //    TurnButton.GetComponent<Image>().color = new Color(255, 255, 255, 0.5f);
        //}
        //else
        //{
        //    TurnButton.interactable = true;
        //    TurnButton.GetComponent<Image>().color = new Color(255, 255, 255, 1);
        //}

        if (!_isStarted)
            return;

        float maxLerpRotationTime = 4f;

        // increment timer once per frame
        _currentLerpRotationTime += Time.deltaTime;
        if (_currentLerpRotationTime > maxLerpRotationTime || Circle.transform.eulerAngles.z == _finalAngle)
        {
          
            _currentLerpRotationTime = maxLerpRotationTime;
            _isStarted = false;
            _startAngle = _finalAngle % 360;

            for (int i = 0; i < sectors.Count; i++)
            {
                if (Mathf.Abs(_startAngle)== sectors[i])
                {
                    calculateReward(i);
                    break;
                }
            }

          //  GiveAwardByAngle();
            //    StartCoroutine(HideCoinsDelta ());
        }

        // Calculate current position using linear interpolation
        float t = _currentLerpRotationTime / maxLerpRotationTime;

        // This formulae allows to speed up at start and speed down at the end of rotation.
        // Try to change this values to customize the speed
        t = t * t * t * (t * (6f * t - 15f) + 10f);

        float angle = Mathf.Lerp(_startAngle, _finalAngle, t);
        Circle.transform.eulerAngles = new Vector3(0, 0, angle);
    }
    private void calculateReward(int ind)
    {
        Debug.LogError("Reward "+spinWheelItems[ind].amount);
    }
    private void GiveAwardByAngle()
    {
        // Here you can set up rewards for every sector of wheel
        switch ((int)_startAngle)
        {
            case 0:
                RewardCoins(1000);
                break;
            case -330:
                RewardCoins(200);
                break;
            case -300:
                RewardCoins(100);
                break;
            case -270:
                RewardCoins(500);
                break;
            case -240:
                RewardCoins(300);
                break;
            case -210:
                RewardCoins(100);
                break;
            case -180:
                RewardCoins(900);
                break;
            case -150:
                RewardCoins(200);
                break;
            case -120:
                RewardCoins(100);
                break;
            case -90:
                RewardCoins(700);
                break;
            case -60:
                RewardCoins(300);
                break;
            case -30:
                RewardCoins(100);
                break;
            default:
                RewardCoins(300);
                break;
        }
    }
    private void RewardCoins(int awardCoins)
    {
        CurrentCoinsText.text = awardCoins.ToString();
        //   CoinsDeltaText.text = "+" + awardCoins;
        //   CoinsDeltaText.gameObject.SetActive (true);
        // StartCoroutine(UpdateCoinsAmount());
    }



    private IEnumerator UpdateCoinsAmount()
    {
        // Animation for increasing and decreasing of coins amount
        const float seconds = 0.5f;
        float elapsedTime = 0;

        while (elapsedTime < seconds)
        {
           // CurrentCoinsText.text = Mathf.Floor(Mathf.Lerp(0, CurrentCoinsAmount, (elapsedTime / seconds))).ToString();
            elapsedTime += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

      //  CurrentCoinsText.text = CurrentCoinsAmount.ToString();
    }
    public void TurnWheel()
    {
        // Player has enough money to turn the wheel
        if (canSpin)
        {
            _currentLerpRotationTime = 0f;

            // Fill the necessary angles (for example if you want to have 12 sectors you need to fill the angles with 30 degrees step)
           // _sectorsAngles = new float[] { 30, 60, 90, 120, 150, 180, 210, 240, 270, 300, 330, 360 };

          //  float randomFinalAngle = _sectorsAngles[UnityEngine.Random.Range(0, _sectorsAngles.Length)];
            float randomFinalAngle = sectors[UnityEngine.Random.Range(0, sectors.Count)];

            // Here we set up how many circles our wheel should rotate before stop
            _finalAngle = -(spinIterations * 360 + randomFinalAngle);
            _isStarted = true;


            // Decrease money for the turn
          //  CurrentCoinsAmount -= TurnCost;

            // Show wasted coins
            //   CoinsDeltaText.text = "-" + TurnCost;
            //  CoinsDeltaText.gameObject.SetActive (true);

            // Animate coins
            StartCoroutine(UpdateCoinsAmount());
        }
    }
}
