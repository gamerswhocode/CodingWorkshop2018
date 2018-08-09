using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    private const int DEFUALT_POOL_SIZE = 100;

    private static Dictionary<IPoolable, Pool> pools = new Dictionary<IPoolable, Pool>();

    public static Pool GetPool(IPoolable prefab)
    {
        if (pools.ContainsKey(prefab))
        {
            if (pools[prefab] != null)
                return pools[prefab];
            else
                pools.Remove(prefab);
        }

        var pool = new GameObject("Pool-" + (prefab as Component).name).AddComponent<Pool>();
        pool.Initialize(prefab);
        pools.Add(prefab, pool);
        return pool;
    }

    public static Pool Prewarm(IPoolable prefab, int initialSize)
    {
        if (pools.ContainsKey(prefab))
        {
            if (pools[prefab] != null)
            {
                Debug.LogError("Pool already created, can't prewarm");
                return pools[prefab];
            }
            else
                pools.Remove(prefab);
        }

        var pool = new GameObject("Pool-" + (prefab as Component).name).AddComponent<Pool>();
        pool.Initialize(prefab, initialSize);
        pools.Add(prefab, pool);
        return pool;
    }

    private GameObject prefab;

    private Queue<IPoolable> objects = new Queue<IPoolable>();
    private List<IPoolable> disabledObjects = new List<IPoolable>();

    private void Initialize(IPoolable poolablePrefab, int initialSize = DEFUALT_POOL_SIZE)
    {
        this.prefab = (poolablePrefab as Component).gameObject;
        for (int i = 0; i < initialSize; i++)
        {
            var pooledObject = (Instantiate(this.prefab) as GameObject).GetComponent<IPoolable>();
            (pooledObject as Component).gameObject.name += " " + i;

            pooledObject.OnDestroyEvent += () => AddObjectToAvailable(pooledObject);

            (pooledObject as Component).gameObject.SetActive(false);
        }
    }

    private void AddObjectToAvailable(IPoolable pooledObject)
    {
        disabledObjects.Add(pooledObject);
        objects.Enqueue(pooledObject);
    }

    private IPoolable Get()
    {
        lock (this)
        {
            if (objects.Count == 0)
            {
                int amountToGrowPool = Mathf.Max((disabledObjects.Count / 10), 1);
                Initialize(this.prefab.GetComponent<IPoolable>(), amountToGrowPool);
            }

            var pooledObject = objects.Dequeue();

            return pooledObject;
        }
    }

    public IPoolable Get(Vector3 position, Quaternion rotation)
    {
        var pooledObject = Get();

        (pooledObject as Component).transform.position = position;
        (pooledObject as Component).transform.rotation = rotation;
        (pooledObject as Component).gameObject.SetActive(true);

        return pooledObject;
    }

    private void Update()
    {
        MakeDisabledObjectsChildren();
    }

    private void MakeDisabledObjectsChildren()
    {
        if (disabledObjects.Count > 0)
        {
            foreach (var pooledObject in disabledObjects)
            {
                (pooledObject as Component).transform.SetParent(transform);
            }

            disabledObjects.Clear();
        }
    }
}