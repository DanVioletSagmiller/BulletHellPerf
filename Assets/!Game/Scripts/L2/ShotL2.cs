using UnityEngine;

public class ShotL2 : MonoBehaviour
{
    public float Speed = 3f;

    private void Update()
    {
        transform.position += transform.forward * Speed * Time.deltaTime;
    }
    private void OnTriggerEnter(Collider other)
    {
        ShotPoolL2.Instance.ReturnShot(this);
    }
}
