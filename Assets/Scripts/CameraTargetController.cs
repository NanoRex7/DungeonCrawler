using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO @Jordan Make this accessable from another place
// https://stackoverflow.com/questions/63694/creating-a-math-library-using-generics-in-c-sharp
abstract class AverageProvider<T>
{
    public abstract T Divide(T a, int b);
    public abstract T Add(T a, T b);
    public abstract T Negate(T a);
    public virtual T Subtract(T a, T b)
    {
        return Add(a, Negate(b));
    }
}

class FloatAverageProvider : AverageProvider<float>
{
    public override float Divide(float a, int b)
    {
        return a / (float)b;
    }

    public override float Add(float a, float b)
    {
        return a + b;
    }

    public override float Negate(float a)
    {
        return -a;
    }
}

class Vector2AverageProvider : AverageProvider<Vector2>
{
    public override Vector2 Divide(Vector2 a, int b)
    {
        return a / (float)b;
    }

    public override Vector2 Add(Vector2 a, Vector2 b)
    {
        return a + b;
    }

    public override Vector2 Negate(Vector2 a)
    {
        return -a;
    }
}

class RunningAverage<T>
{
    static AverageProvider<T> _averageProvider;

    static RunningAverage()
    {
        if (typeof(T) == typeof(float))
            _averageProvider = new FloatAverageProvider() as AverageProvider<T>;
        if (typeof(T) == typeof(Vector2))
            _averageProvider = new Vector2AverageProvider() as AverageProvider<T>;

        if (_averageProvider == null)
            throw new System.InvalidOperationException(
                "Type " + typeof(T).ToString() + " is not supported by RunningAverage.");
    }

    private Queue<T> queue;
    private T sum;
    private int sizeLimit;

    public RunningAverage(T initialAverage, int sizeLimit)
    {
        queue = new Queue<T>();
        queue.Enqueue(initialAverage);
        sum = initialAverage;
        this.sizeLimit = sizeLimit;
    }

    public T Average()
    {
        return _averageProvider.Divide(sum, queue.Count);
    }

    public void Add(T value)
    {
        sum = _averageProvider.Add(sum, value);
        queue.Enqueue(value);
        if (queue.Count > sizeLimit)
            sum = _averageProvider.Subtract(sum, queue.Dequeue());
    }

    public void Reset(T value)
    {
        queue.Clear();
        sum = value;
    }
}

/*
class RunningAverage
{
    private Queue<float> queue;
    private float sum;
    private int sizeLimit;

    public RunningAverage(float initialAverage, int sizeLimit)
    {
        queue = new Queue<float>();
        queue.Enqueue(initialAverage);
        sum = initialAverage;
        this.sizeLimit = sizeLimit;
    }

    public float Average()
    {
        return sum / queue.Count;
    }

    public void Add(float value)
    {
        sum += value;
        queue.Enqueue(value);
        if (queue.Count > sizeLimit)
            sum -= queue.Dequeue();
    }

    public void Reset(float value)
    {
        queue.Clear();
        sum = value;
    }
}
*/

public class CameraTargetController : MonoBehaviour
{
    public float LOOK_WEIGHT;
    public float MOVE_WEIGHT;
    public int LOOK_SIZE;
    public int MOVE_SIZE;
    private Rigidbody2D body;
    RunningAverage<Vector2> lookAverage, moveAverage;
    private GameObject rotatingObject;
    public Vector2 display;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponentInParent<Rigidbody2D>();
        // TODO @Jordan use some multiple of the frame rate here
        lookAverage = new RunningAverage<Vector2>(new Vector2(0, 0), LOOK_SIZE);
        moveAverage = new RunningAverage<Vector2>(new Vector2(0, 0), MOVE_SIZE);
        rotatingObject = transform.parent.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        float theta = rotatingObject.transform.rotation.eulerAngles.z;
        lookAverage.Add(new Vector2(Mathf.Cos(Mathf.Deg2Rad * theta), Mathf.Sin(Mathf.Deg2Rad * theta)));
        moveAverage.Add(body.velocity.normalized);
        transform.localPosition = LOOK_WEIGHT * lookAverage.Average() * (1 - moveAverage.Average().magnitude)
            + MOVE_WEIGHT * moveAverage.Average();
        display = new Vector2(Mathf.Cos(Mathf.Deg2Rad * theta), Mathf.Sin(Mathf.Deg2Rad * theta));
    }
}
