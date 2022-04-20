using Unity.Burst; 
using Unity.Collections; 
using Unity.Jobs; 
using Unity.Mathematics; 
using UnityEngine; 
using UnityEngine.Profiling; 


public class TestJobSystem : MonoBehaviour
{
    public int DataCount;
    private NativeArray<float3> m_JobDatas;
    private NativeArray<float> m_JobResults;
    private Vector3[] m_NormalDatas;
    private float[] m_NormalResults; // Job adding two floating point values together 
    
    public static void TestStaticFunc()
    {
        int a = 0;
        a++;
    }

    [BurstCompile]
    public struct MyJobParallel : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<float3> data;
        public NativeArray<float> result;
        public void Execute(int i)
        {
            Vector3 item = data[i];
            result[i] = Mathf.Sqrt(item.x * item.x + item.y * item.y + item.z * item.z);
            TestStaticFunc();
        }
    }

    public struct MyJobParallelNoBurst : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<float3> data;
        public NativeArray<float> result;
        public void Execute(int i)
        {
            Vector3 item = data[i];
            result[i] = Mathf.Sqrt(item.x * item.x + item.y * item.y + item.z * item.z);
        }
    }

    [BurstCompile]
    public struct MyJob : IJob
    {
        [ReadOnly]
        public NativeArray<float3> data;
        public NativeArray<float> result;
        public void Execute()
        {
            for(int i=0;i< data.Length;i++)
            {
                Vector3 item = data[i];
                result[i] = Mathf.Sqrt(item.x * item.x + item.y * item.y + item.z * item.z);
            }
        }
    }

    public struct MyJobNoBurst : IJob
    {
        [ReadOnly]
        public NativeArray<float3> data;
        public NativeArray<float> result;
        public void Execute()
        {
            for (int i = 0; i < data.Length; i++)
            {
                Vector3 item = data[i];
                result[i] = Mathf.Sqrt(item.x * item.x + item.y * item.y + item.z * item.z);
            }
        }
    }

    [BurstCompile]
    public struct MyJob2 : IJob
    {
        [ReadOnly]
        public float3 data;
        public float result;
        public void Execute()
        {
            result = Mathf.Sqrt(data.x * data.x + data.y * data.y + data.z * data.z);
        }
    }

    private void Awake()
    {
        m_JobDatas = new NativeArray<float3>(DataCount, Allocator.Persistent);
        m_JobResults = new NativeArray<float>(DataCount,Allocator.Persistent);
        m_NormalDatas = new Vector3[DataCount];
        m_NormalResults = new float[DataCount];
        for (int i = 0; i < DataCount; i++)
        {
            m_JobDatas[i] = new float3(1, 1, 1);
            m_NormalDatas[i] = new Vector3(1, 1, 1);
        }
    }

    // Update is called once per frame 
    void Update()
    {
        Profiler.BeginSample("ParallelJob / Burst");
        MyJobParallel jobParallelData = new MyJobParallel();
        jobParallelData.data = m_JobDatas;
        jobParallelData.result = m_JobResults;
        JobHandle handleParallel = jobParallelData.Schedule(DataCount, 64);
        handleParallel.Complete();
        Profiler.EndSample();

        //Profiler.BeginSample("ParallelJob");
        //MyJobParallelNoBurst jobParallelDataNoBurst = new MyJobParallelNoBurst();
        //jobParallelDataNoBurst.data = m_JobDatas;
        //jobParallelDataNoBurst.result = m_JobResults;
        //JobHandle handleParallelNoBurst = jobParallelDataNoBurst.Schedule(DataCount, 1);
        //handleParallelNoBurst.Complete();
        //Profiler.EndSample();

        Profiler.BeginSample("Job / Burst");
        MyJob jobData = new MyJob();
        jobData.data = m_JobDatas;
        jobData.result = m_JobResults;
        JobHandle handle = jobData.Schedule();
        handle.Complete();
        Profiler.EndSample();

        //Profiler.BeginSample("Job");
        //MyJobNoBurst jobDataNoBurst = new MyJobNoBurst();
        //jobDataNoBurst.data = m_JobDatas;
        //jobDataNoBurst.result = m_JobResults;
        //JobHandle handleNoBurst = jobDataNoBurst.Schedule();
        //handleNoBurst.Complete();
        //Profiler.EndSample();

        Profiler.BeginSample("Normal");
        //正常数据运算 
        for (var i = 0; i < DataCount; i++)
        {
            var item = m_NormalDatas[i];
            m_NormalResults[i] = Mathf.Sqrt(item.x * item.x + item.y * item.y + item.z * item.z);
        }
        Profiler.EndSample();

        //Profiler.BeginSample("Job / Burst / 2");
        //for(int i=0;i<DataCount;i++)
        //{
        //    MyJob2 jobData2 = new MyJob2();
        //    jobData2.data = m_JobDatas[i];
        //    jobData2.result = m_JobResults[i];
        //    JobHandle handle2 = jobData2.Schedule();
        //    handle2.Complete();
        //}
        //Profiler.EndSample();
    }

    public void OnDestroy()
    {
        m_JobDatas.Dispose();
        m_JobResults.Dispose();
        m_NormalDatas = null;
        m_NormalResults = null;
    }
}