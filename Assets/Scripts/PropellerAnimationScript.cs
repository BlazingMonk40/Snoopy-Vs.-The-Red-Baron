using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PropellerAnimationScript : MonoBehaviour
{
    [SerializeField] private float testAniSpeed;
    [SerializeField] private MFlight.Demo.Plane planeController;
    [SerializeField] private Animator propellerAnimation;
    [SerializeField] private float thrustPercent;
    [SerializeField] private GameObject canvas;
    [SerializeField] private Text text;
    private bool wep;
    GameObject textHolder;
    //GameObject text;
    public void Start()
    {
        thrustPercent = planeController.intThrustText;
        //text = Instantiate(textHolder, canvas.transform);
        //text.AddComponent<Text>();
        //text.transform.position.Set(-195f, 250f, 0f);
        //text.GetComponent<Text>().text = "Speed of propeller: ";

    }
    void Update()
    {
        if (thrustPercent != planeController.intThrustText)
        {
            if (thrustPercent >= planeController.intThrustText)
                thrustPercent -= 5 * Time.deltaTime;
            if (thrustPercent <= planeController.intThrustText)
                thrustPercent += 5 * Time.deltaTime;
        }
        propellerAnimation.speed = testAniSpeed;

        #region RPM
        if (!wep && planeController.intThrustText == 0)
        {
            propellerAnimation.speed = 1;
            text.text = "Speed of propeller: 1200";
        }
        if (planeController.intThrustText == 10)
        {
            propellerAnimation.speed = 2;
            text.text = "Speed of propeller: 1350";
        }
        if (planeController.intThrustText == 20)
        {
            propellerAnimation.speed = 2;
            text.text = "Speed of propeller: 1500";
        }
        if (planeController.intThrustText == 30)
        {
            propellerAnimation.speed = 3;
            text.text = "Speed of propeller: 1750";
        }
        if (planeController.intThrustText == 40)
        {
            propellerAnimation.speed = 4;
            text.text = "Speed of propeller: 1900";
        }
        if (planeController.intThrustText == 50)
        {
            propellerAnimation.speed = 5;
            text.text = "Speed of propeller: 2100";
        }
        if (planeController.intThrustText == 60)
        {
            propellerAnimation.speed = 6;
            text.text = "Speed of propeller: 2250";
        }
        if (planeController.intThrustText == 70)
        {
            propellerAnimation.speed = 7;
            text.text = "Speed of propeller: 2550";
        }
        if (planeController.intThrustText == 80)
        {
            propellerAnimation.speed = 8;
            text.text = "Speed of propeller: 2780";
        }
        if (planeController.intThrustText == 90)
        {
            propellerAnimation.speed = 9;
            text.text = "Speed of propeller: 2950";
        }
        if (planeController.intThrustText == 100)
        {
            propellerAnimation.speed = 10;
            text.text = "Speed of propeller: 3125";
        }
        if (planeController.intThrustText == 110)
        {
            propellerAnimation.speed = 11;
            text.text = "Speed of propeller: 3500";
        }
        if (planeController.intThrustText == 120)
        {
            propellerAnimation.speed = 12;
            text.text = "Speed of propeller: 3750";
        }
#endregion

        //text.text = "Speed of propeller: " + propellerAnimation.speed.ToString("F0");
    }
}
