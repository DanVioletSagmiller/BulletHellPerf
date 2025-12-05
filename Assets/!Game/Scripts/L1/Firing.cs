using UnityEngine;

public class Firing : MonoBehaviour
{
    public float Frequency = .3f;
    public GameObject Prefab;
    public Transform FiringPoint;
    private float timer = 0f;
    public static int Count;
    public int _Count;
    public static int Peek;
    public int _Peek;
    public void Awake()
    {
        Count = 0;
        _Peek = 0;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= Frequency)
        {
            Instantiate(Prefab, FiringPoint.position, FiringPoint.rotation);
            timer = 0f;
            Count++;
            if (Count > Peek) Peek = Count;
            Demo.Instance.SetPeekObjects(Peek);
        }
        _Count = Count;
        _Peek = Peek;
    }
}
