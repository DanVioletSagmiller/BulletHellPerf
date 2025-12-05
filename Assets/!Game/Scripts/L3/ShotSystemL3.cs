using JetBrains.Annotations;
using System;
using UnityEngine;

public class ShotSystemL3 : MonoBehaviour
{
    public static ShotSystemL3 Instance;
    public float ShotSpeed = 3f;
    public Mesh ShotMesh;
    public Material ShotMaterial;   

    public struct ShotData
    {
        public float TimeRemaining;
        public Vector3 Velocity;
    }

    public ShotData[] Shots;
    public Matrix4x4[] ShotTransforms;

    public int Capacity = 1024;
    public int Count = 0;


    private int ReplaceIndexCount = 0;
    private Int16[] ReplacementIndexes;

    private void Awake() => Instance = this;

    public void Start()
    {
        Shots = new ShotData[Capacity];
        ShotTransforms = new Matrix4x4[Capacity];
        ReplacementIndexes = new Int16[Capacity];
    }

    public void AddShot(Vector3 position, Quaternion rotation, float lifetime)
    {
        if (Count >= Capacity)
        {
            Debug.LogError("ShotSystemL3: Cannot add more shots, capacity reached.");
            return;
        }

        Shots[Count] = new ShotData
        {
            TimeRemaining = lifetime,
            Velocity = rotation * Vector3.forward * ShotSpeed
        };

        ShotTransforms[Count] = Matrix4x4.TRS(position, rotation, Vector3.one);

        Count++;
        Demo.Instance.SetPeekObjects(Count);
    }

    public void Update()
    {

        ReplaceIndexCount = 0;

        // Update shot positions and lifetimes, Mark for removal;
        for (int i = 0; i < Count; i++)
        {
            Shots[i].TimeRemaining -= Time.deltaTime;
            if (Shots[i].TimeRemaining <= 0f)
            {
                ReplacementIndexes[ReplaceIndexCount++] = (Int16)i;
            }
            else
            {
                Vector3 position = ShotTransforms[i].GetColumn(3);
                position += Shots[i].Velocity * Time.deltaTime;
                ShotTransforms[i].SetColumn(3, new Vector4(position.x, position.y, position.z, 1f));
            }
        }

        // Replace Expired Shots if needed. Reduce ActiveCount.
        for(int i = 0; i < ReplaceIndexCount; i++)
        {
            Int16 replaceIndex = ReplacementIndexes[i];
            Count--;

            if (replaceIndex < Count)
            {
                Shots[replaceIndex] = Shots[Count];
                ShotTransforms[replaceIndex] = ShotTransforms[Count];
            }
        }

        Graphics.DrawMeshInstanced(
            ShotMesh,
            0,
            ShotMaterial,
            ShotTransforms,
            Count
        );
    }
}
