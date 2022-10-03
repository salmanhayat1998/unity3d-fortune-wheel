
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
    [SerializeField]private bool _antiClockwise;
    public bool antiClockwise
    {
        get
        {
            return _antiClockwise;
        }
        set
        {
            if (sectors.Count > 0)
            {
                sectors.Reverse();
            }
            _antiClockwise = value;
        }
    }
    public Button TurnButton;
    public GameObject Circle; 			// Rotatable Object with rewards
    public GameObject slicePrefab;
    public Text CurrentCoinsText; 		// Pop-up text with wasted or rewarded coins amount
    public RadialLayout radialLayout;
    [Range(120f, 420f)]
    public float radius = 173f;
    [Range(3, 8)]
    public int spinIterations = 5;
    [Range(4, 32)]
    public int sections = 4;
    [SerializeField ]private float _finalAngle;
    [SerializeField] private float _startAngle = 0;
    [SerializeField] float anglediff;
    [SerializeField] private List<float> sectors = new List<float>();
    public item[] spinWheelItems;
    private float _currentLerpRotationTime;
    float temp;
    private void Start()
    {
        createWheel();
    }

    private void createWheel()
    {
        radialLayout.fDistance = radius;
        anglediff = radialLayout.MaxAngle / sections;

        for (int i = 0; i < sections; i++)
        {
            var _item = Instantiate(slicePrefab, radialLayout.transform);
            if (sections > 16)
            {
                _item.transform.localScale = new Vector2(0.5f, 0.5f);
            }
            _item.GetComponent<spinItem>().itemname.text = spinWheelItems[i].label;
            _item.GetComponent<spinItem>().icon.sprite = spinWheelItems[i].icon;
            temp += anglediff;
            sectors.Add(temp);

            _item.transform.rotation = Quaternion.Euler(0, 0, anglediff * i);

        }
        if (antiClockwise)
        {
            sectors.Reverse();
        }
    }
    private void OnValidate()
    {
        Array.Resize(ref spinWheelItems, sections);
        antiClockwise = _antiClockwise;
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
            _startAngle = _finalAngle % 360.0f;
           // Debug.LogError(Mathf.RoundToInt(Mathf.Abs(_startAngle - anglediff)));
            Debug.LogError((Mathf.Abs(_startAngle - anglediff)));
            // add _startangle if it is positive //
            for (int i = 0; i < sectors.Count; i++)
            {
                if ((Mathf.Abs(_startAngle - anglediff)) == sectors[i])
                {
                    calculateReward(i);
                    break;
                }
            }
        }

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

    public void TurnWheel()
    {
        // Player has enough money to turn the wheel //
        if (canSpin)
        {
            _currentLerpRotationTime = 0f;

            float randomFinalAngle = sectors[UnityEngine.Random.Range(0, sectors.Count)];

            // Here we set up how many circles our wheel should rotate before stop
            _finalAngle = antiClockwise ? (spinIterations * 360 + randomFinalAngle) : -(spinIterations * 360 + randomFinalAngle);
            _isStarted = true;
            TurnButton.interactable = false;

        }
    }
}
