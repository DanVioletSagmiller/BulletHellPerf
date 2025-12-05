using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Demo : MonoBehaviour
{
    public static Demo Instance;
    [System.Serializable]
    public class Results
    {
        public string Name;
        public int PeekObjects;
        public float AverageCPU;
        public float AverageGPU;
        public AnimationCurve CPU_Curve;
        public AnimationCurve GPU_Curve;
    }

    // Assign this in the inspector (panel on your canvas, ~300px wide)
    public RectTransform Panel;

    Dictionary<string, Results> AllResults = new Dictionary<string, Results>();
    public IReadOnlyDictionary<string, Results> AllResultsReadOnly => AllResults;

    Results Current;

    FrameTiming[] _frameTimings = new FrameTiming[1];
    private int SceneIndex = 1;
    internal static string StartingScene;

    public void RecordFor(string name)
    {
        if (!AllResults.ContainsKey(name))
        {
            Results res = new Results();
            AllResults.Add(name, res);
        }

        Current = AllResults[name];
        Current.AverageCPU = 0;
        Current.AverageGPU = 0;
        Current.PeekObjects = 0;
        Current.CPU_Curve = new AnimationCurve();
        Current.GPU_Curve = new AnimationCurve();
    }

    public void SetPeekObjects(int count)
    {
        if (Current != null)
        {
            Current.PeekObjects = count;
        }
    }

    private void Awake() => Instance = this;    

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        if (!string.IsNullOrEmpty(StartingScene))
        {
            SceneManager.LoadScene(StartingScene);
            StartingScene = null;
            return;
        }
        else
        {
            SceneManager.LoadScene("L1");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SceneIndex++;
            if (SceneIndex > 3) SceneIndex = 1;
            SceneManager.LoadScene("L" + SceneIndex);
        }

        if (Current == null) return;

        FrameTimingManager.CaptureFrameTimings();
        uint count = FrameTimingManager.GetLatestTimings(1, _frameTimings);
        if (count == 0) return;

        var ft = _frameTimings[0];

        Current.CPU_Curve.AddKey(Time.time, (float)ft.cpuFrameTime);
        Current.GPU_Curve.AddKey(Time.time, (float)ft.gpuFrameTime);

        Current.AverageCPU = (Current.AverageCPU + (float)ft.cpuFrameTime) / 2f;
        Current.AverageGPU = (Current.AverageGPU + (float)ft.gpuFrameTime) / 2f;
    }
}
