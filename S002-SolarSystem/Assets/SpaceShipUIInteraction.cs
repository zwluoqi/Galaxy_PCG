using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpaceShipUIInteraction : MonoBehaviour
{

    public TMP_Text info;

    public Image horitalSpeed;

    public Image verticalSpeed;
    public TMP_Text hSpeed;
    public TMP_Text vSpeed;

    public TMP_Text spaceShipEngineSpeed;
    public Toggle _Toggle;
    public Astronomical selectAstron;
    public SimpleSpaceShip spaceShip;

    public Vector2 canveSize;
    private void Start()
    {
        Interac3DObject.onGlobalClick += delegate(GameObject go)
        {
            selectAstron = go.GetComponent<Astronomical>();
        };
        canveSize = GetComponent<RectTransform>().sizeDelta;
        _Toggle.onValueChanged.AddListener(delegate(bool arg0)
        {
            spaceShip.Braking(arg0);
        });
    }

    // Update is called once per frame
     
    void Update()
    {
        if (selectAstron != null)
        {
            info.gameObject.SetActive(true);
            var dir = (selectAstron.transform.position - spaceShip._rigidbody.position);
            var distance = dir.magnitude - selectAstron.Radius-spaceShip.transform.localScale.x*0.5;
            var astronSpeed = SolarSystemSimulater.Inst.GetRelativeSpeed(selectAstron);
            //天体相对飞船的速度
            var relativeSpeed = astronSpeed - spaceShip._rigidbody.velocity;


            // var spaceShipUp = spaceShip._rigidbody.rotation * Vector3.up;
            // var spaceShipRight = spaceShip._rigidbody.rotation * Vector3.right;
            // var spaceShipForward = spaceShip._rigidbody.rotation * Vector3.forward;
            
            var upSpeed =  Vector3.Project( relativeSpeed,spaceShip.transform.up);
            var rightSpeed = Vector3.Project( relativeSpeed,spaceShip.transform.right);
            var forwardSpeed = Vector3.Project( relativeSpeed,spaceShip.transform.forward);

            var isUp = Vector3.Dot(spaceShip.transform.up, upSpeed)>0;
            var isRight = Vector3.Dot(spaceShip.transform.right, rightSpeed)>0;
            var isForward = Vector3.Dot(spaceShip.transform.forward, forwardSpeed)>0;
            
            
            horitalSpeed.rectTransform.sizeDelta = new Vector2(Mathf.Min(rightSpeed.magnitude*2,150), 2);
            horitalSpeed.transform.localScale = new Vector3(isRight ? 1 : -1, 1, 1);
            hSpeed.text = GetSpeedDesc(rightSpeed);
            hSpeed.rectTransform.anchoredPosition = new Vector2(horitalSpeed.rectTransform.sizeDelta.x*(isRight ? 1 : -1),
                hSpeed.rectTransform.anchoredPosition.y);
            
            
            verticalSpeed.rectTransform.sizeDelta = new Vector2(2, Mathf.Min(upSpeed.magnitude*2,150));
            verticalSpeed.transform.localScale = new Vector3(1, isUp?-1:1, 1);
            vSpeed.text = GetSpeedDesc(upSpeed);
            vSpeed.rectTransform.anchoredPosition = new Vector2(vSpeed.rectTransform.anchoredPosition.x,
                verticalSpeed.rectTransform.sizeDelta.y*(isUp?1:-1));

            
            StringBuilder stringBuilder = new StringBuilder();
            if (distance > 1000)
            {
                stringBuilder.AppendLine("距离:" + (int) (distance / 1000) + "KM");
            }
            else
            {
                stringBuilder.AppendLine("距离:" + (int) (distance) + "M");
            }

            var normalSpeedDesc = (isForward?"+":"-") + GetSpeedDesc(forwardSpeed);

            stringBuilder.AppendLine("径向速度:" + normalSpeedDesc);
            
            info.text = stringBuilder.ToString();
            
            
            var screenPoint = Camera.main.WorldToViewportPoint(selectAstron.transform.position);
            if (screenPoint.z <= 0)
            {
                
            }
            else
            {

                screenPoint.x = Mathf.Clamp(screenPoint.x, 0, 1);
                screenPoint.x = screenPoint.x - 0.5f;
                screenPoint.y = Mathf.Clamp(screenPoint.y, 0, 1);
                screenPoint.y = screenPoint.y - 0.5f;


                var posx = screenPoint.x * canveSize.x;
                var posy = screenPoint.y * canveSize.y;

                posy = Mathf.Clamp(posy,
                    -(canveSize.y / 2 - 100) + verticalSpeed.rectTransform.sizeDelta.y,
                    (canveSize.y / 2 - 100) - verticalSpeed.rectTransform.sizeDelta.y);

                posx = Mathf.Clamp(posx,
                    -(canveSize.x / 2 - 100) + horitalSpeed.rectTransform.sizeDelta.x,
                    (canveSize.x / 2 - 100) - horitalSpeed.rectTransform.sizeDelta.x);

                info.rectTransform.anchoredPosition = new Vector2(posx, posy);
            }
        }
        else
        {
            info.gameObject.SetActive(false);
        }
        ProcessSpaceShipInfo();
    }

    private void ProcessSpaceShipInfo()
    {
        // var spaceShipUp = spaceShip._rigidbody.rotation * Vector3.up;
        // var spaceShipRight = spaceShip._rigidbody.rotation * Vector3.right;
        // var spaceShipForward = spaceShip._rigidbody.rotation * Vector3.forward;
        //     
        // var upSpeed =  Vector3.Project( spaceShip.curEngineAcceleration,spaceShipUp);
        // var rightSpeed = Vector3.Project( spaceShip.curEngineAcceleration,spaceShipRight);
        // var forwardSpeed = Vector3.Project( spaceShip.curEngineAcceleration,spaceShipForward);
        // var isUp = Vector3.Dot(upSpeed, spaceShipUp) >= 0;
        // var isRight = Vector3.Dot(upSpeed, spaceShipUp) >= 0;
        // var isForward = Vector3.Dot(upSpeed, spaceShipUp) >= 0;
        //
        // var rightDesc = (isRight ? "+" : "-") +((int)rightSpeed.magnitude);
        // var upDesc = (isUp ? "+" : "-") +((int)upSpeed.magnitude);
        // var forwardDesc = (isForward ? "+" : "-") +((int)forwardSpeed.magnitude);
        //  spaceShipEngineSpeed.text =
        //                              $"引擎加速度:{rightDesc},{upDesc},{forwardDesc}";
    }

    private string GetSpeedDesc(Vector3 crossSpeed)
    {
        var length = crossSpeed.magnitude;
        if (length > 1000)
        {
            return ((int) (length / 1000) + "KM/S");
        }
        else
        {
            return  ((int) (length) + "M/S");
        }
    }
}
