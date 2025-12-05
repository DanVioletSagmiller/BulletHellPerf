using UnityEngine;

public class Shot : MonoBehaviour
{
    public float Speed = 3f;

    private void Update()
    {
        transform.position += transform.forward * Speed * Time.deltaTime;
    }
    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
        Firing.Count--;
    }
}
