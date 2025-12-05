using JetBrains.Annotations;
using UnityEngine;

public class FiringL3 : MonoBehaviour
{
    public float Frequency = .3f;
    public GameObject Prefab;
    public Transform FiringPoint;
    private float timer = 0f;

    public AnimationCurve DistanceByYAngle;
    public float CheckAngle = 1f;


    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= Frequency)
        {
            ShotSystemL3.Instance.AddShot(
                FiringPoint.position, 
                FiringPoint.rotation, 
                lifetime: DistanceByYAngle.Evaluate(FiringPoint.rotation.eulerAngles.y));

            timer = 0f;
        }
    }


}
