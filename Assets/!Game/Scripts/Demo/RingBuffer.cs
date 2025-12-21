using System;

// Stores a looping collection of data. As new data comes in that exceeds the size, it overwrites the oritingal. 
public sealed class RingBuffer<T>
{
    // Once the array is created, don't erase it or alter it. If you need a new one, make a new one. 
    private readonly T[] data;

    // where do we start in the loop?
    private int head;

    // how many?
    private int count;

    public int Capacity => data.Length;
    public int Count => count;

    // Capacity is the size of Data. this cannot change during the lifetime of this object.
    public RingBuffer(int capacity)
    {
        data = new T[capacity];
    }

    // used to grow the size as a new instance
    // or to reset the data structure offsets, which can slightly improve read performance at the expense of a full copy now. 
    public RingBuffer<T> Clone(int newCapacity)
    {
        int iMax = newCapacity > count ? count : newCapacity;
        var ring = new RingBuffer<T>(iMax);
        for(int i = 0; i < iMax; i++) ring.Add(this[i]);
        return ring;
    }

    public void Add(T value)
    {
        data[head] = value;
        head = (head + 1) % data.Length;
        if (count < data.Length) count++;
    }

    // The index is not data array index. 
    // We because we may not start at 0, we need to correct for it, assuming that 0-max from head and wraps around.
    public T this[int index]
    {
        get
        {
            if ((uint)index >= (uint)count)
                throw new ArgumentOutOfRangeException();

            int idx = head - count + index;
            if (idx < 0) idx += data.Length;
            return data[idx];
        }
    }
}
