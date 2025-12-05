using UnityEngine;
using UnityEngine.Pool;

public class ShotPoolL2 : MonoBehaviour
{
    public static ShotPoolL2 Instance;

    public ShotL2 Prefab;

    ObjectPool<ShotL2> _pool;

    public int Used;
    public int Peek;

    void Awake()
    {
        Instance = this;

        Used = 0;

        _pool = new ObjectPool<ShotL2>(
            createFunc: Create,
            actionOnGet: OnGet,
            actionOnRelease: OnRelease,
            actionOnDestroy: OnDestroyPooled,
            collectionCheck: false,
            defaultCapacity: 1024,
            maxSize: 2048
        );
    }

    // Called when pool needs a new instance
    ShotL2 Create()
    {
        var obj = Instantiate(Prefab);
        obj.gameObject.SetActive(false);
        return obj;
    }

    void OnGet(ShotL2 shot)
    {
        shot.gameObject.SetActive(true);
        Used++;
        if (Used > Peek) Peek = Used;
        Demo.Instance.SetPeekObjects(Peek);
    }

    void OnRelease(ShotL2 shot)
    {
        shot.gameObject.SetActive(false);
        Used--;
    }

    void OnDestroyPooled(ShotL2 shot)
    {
        Destroy(shot.gameObject);
    }

    public ShotL2 GetShot() => _pool.Get();
    public void ReturnShot(ShotL2 shot) => _pool.Release(shot);
}
