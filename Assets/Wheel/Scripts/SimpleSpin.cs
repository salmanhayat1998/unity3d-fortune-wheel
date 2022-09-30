
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class SimpleSpin : MonoBehaviour
{
    public bool canSpin = true;
    private bool _isStarted;
    private float _finalAngle;
   // public bool rotateItems = true;
    public bool antiClockwise;
    public Button TurnButton;
    public GameObject Circle; 			// Rotatable Object with rewards
    public GameObject slicePrefab;
    public Text CurrentCoinsText; 		// Pop-up text with wasted or rewarded coins amount
    public RadialLayout radialLayout;
    [Range(120f, 420f)]
    public float radius=173f;
    [Range(3,8)]
    public int spinIterations = 5;
    [Range(4, 12)]
    public int sections = 4;
    public item[] spinWheelItems;
    [SerializeField]private float _startAngle = 0;
    private List<int> sectors = new List<int>();
    private float _currentLerpRotationTime;
    float temp;
    float anglediff;
    private void Start()
    {
        createWheel();
    }
  
    private void createWheel()
    {
        radialLayout.fDistance = radius;
        anglediff = radialLayout.MaxAngle / sections;
     //   temp -= angle;
        for (int i = 0; i < sections; i++)
        {
           var _item = Instantiate(slicePrefab,radialLayout.transform);
            _item.GetComponent<spinItem>().itemname.text = spinWheelItems[i].label;
            _item.GetComponent<spinItem>().icon.sprite = spinWheelItems[i].icon;
            temp += anglediff;
            sectors.Add((int)temp);
         //   if (rotateItems)
            {   
                _item.transform.rotation = Quaternion.Euler(0, 0, anglediff * i);
            }
        }
    }
    private void OnValidate()
    {
        Array.Resize(ref spinWheelItems, sections);
    }
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TurnWheel();
        }

        if (!_isStarted)
            return;

        float maxLerpRotationTime = 4f;

        // increment timer once per frame
        _currentLerpRotationTime += Time.deltaTime;

        // on stop wheel //
        if (_currentLerpRotationTime > maxLerpRotationTime || Circle.transform.eulerAngles.z == _finalAngle)
        {
          
            _currentLerpRotationTime = maxLerpRotationTime;
            _isStarted = false;
            _startAngle = _finalAngle % 360;
          //  _startAngle -= anglediff;
            for (int i = 0; i < sectors.Count; i++)
            {
                if (((int)Mathf.Abs(_startAngle-anglediff))== sectors[i])
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
        CurrentCoinsText.text = spinWheelItems[ind].amount.ToString();
        TurnButton.interactable = true;
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

            float randomFinalAngle = sectors[UnityEngine.Random.Range(0, sectors.Count)];

            // Here we set up how many circles our wheel should rotate before stop
            _finalAngle = antiClockwise? (spinIterations * 360 + randomFinalAngle): -(spinIterations * 360 + randomFinalAngle);
            _isStarted = true;

            TurnButton.interactable = false;
            // Decrease money for the turn
          //  CurrentCoinsAmount -= TurnCost;

            // Show wasted coins
            //   CoinsDeltaText.text = "-" + TurnCost;
            //  CoinsDeltaText.gameObject.SetActive (true);

            // Animate coins
          //  StartCoroutine(UpdateCoinsAmount());
        }
    }
}
