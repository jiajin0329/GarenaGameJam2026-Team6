using System.Collections;
using UnityEngine;

public class PumpingCircle : MonoBehaviour
{
    public Animator myAnimator;
    public float BPM = 120f;
    //Stander = 120
    //0.5 a pump

    //Speed = BPM/120

    public void SetAnimatorSpeed()
    {
        myAnimator.speed = BPM / 120f;
    }

    public void CallPump()
    {
        myAnimator.SetTrigger("SetPump");
    }

    public void CallPump_Big()
    {
        myAnimator.SetTrigger("SetPump_Big");
    }
}
