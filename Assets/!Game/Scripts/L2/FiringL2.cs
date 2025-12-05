using JetBrains.Annotations;
using UnityEngine;

public class FiringL2 : MonoBehaviour
{
    public float Frequency = .3f;
    public GameObject Prefab;
    public Transform FiringPoint;
    private float timer = 0f;
    

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= Frequency)
        {
            //Instantiate(Prefab, FiringPoint.position, FiringPoint.rotation);
            var shot = ShotPoolL2.Instance.GetShot();
            shot.transform.position = FiringPoint.position;
            shot.transform.rotation = FiringPoint.rotation;
            timer = 0f;
        }
    }
}
