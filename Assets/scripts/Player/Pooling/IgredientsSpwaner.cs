using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Drawing;

public class IgredientsSpwaner : MonoBehaviour
{
    [System.Serializable]
    public class PoolEntry
    {
        public string label = "Type";
        public IngredientPool pool;
        public int targetActive = 5;
        public List<Transform> spawnPoints = new();
        public bool shufflePerSpawn = true;
        public GameObject spawnVFX;
        public float vfxLifetime = 2f;

        [HideInInspector] public int lastSpawnIndex = -1;
    }

    public List<Transform> globalspawnPoints = new();
    public float refillCheckInterval = 1.0f;
    public List<PoolEntry> entries = new();

    private void Start()
    {
        foreach (var e in entries)
        {
            if (!e.pool)
            { Debug.LogError("[IngredientSpawner] is a missing pool"); enabled = false; return; }
        }

        StartCoroutine(RefillLoop());
    }

    private IEnumerator RefillLoop()
    {
        //Inital fill
        foreach (var e in entries)
            while (e.pool && e.pool.ActiveCount < e.targetActive)
                TrySpawnOne(e);

        while (true)
        {
            yield return new WaitForSeconds(refillCheckInterval);
            foreach (var e in entries)
            {
                if (e.pool && e.pool.ActiveCount < e.targetActive)
                    TrySpawnOne(e);
            }


        }
    }

    private void TrySpawnOne(PoolEntry e)
    {
        var points = e.spawnPoints != null && e.spawnPoints.Count > 0 ? e.spawnPoints : globalspawnPoints;

        if (points == null || points.Count == 0) return;

        int idx;
        if (e.shufflePerSpawn)
        {
            idx = Random.Range(0, points.Count);
        }
        else
        {
            e.lastSpawnIndex = (e.lastSpawnIndex + 1) % points.Count;
            idx = e.lastSpawnIndex;
        }

        var p = points[idx];
        var it = e.pool.SpawnAt(p.position, p.rotation);
        if (!it) return;

        if (e.spawnVFX)
        {
            var fx = Instantiate(e.spawnVFX, p.position, p.rotation);
            Destroy(fx, e.vfxLifetime);
        }
      
    }

    }
//no adding here
// How To Make A HORROR Game In Unity | Basic Health System | Horror Series Part 024
//date accessed 2025/9/17
//created by: User1 Productions
//created on: 2022
//url: https://www.youtube.com/watch?v=qRgKB8l9GIg&list=PLlcgaDpDEvw05IgKGZo9FYA8Fo38RtAqH&index=24
//Online Video
//youtube

// Awesome UNIQUE Crafting System! (Max Immersion, No Inventory, Hydroneer, Unity Tutorial
//date accessed 2025/9/17
//created by: Code monkey
//created on: 2022
//url: https://www.youtube.com/watch?v=_aC3NVIQ-ok 
//Online Video
//youtube

