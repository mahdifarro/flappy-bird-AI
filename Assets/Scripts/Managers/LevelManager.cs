using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Transform finishPoint;
    public Transform upPoint;
    public Transform downPoint;

    public GameObject pipeCouplePrefab;
    public GameObject obstaclesPool;

    public BackgroundAnimation backgroundAnimation;
    public Levels levels;

    public int minObstaclePoolSize = 15;
    public int levelIndex = 0;
    public float changeStep = 0.5f;

    private List<Transform> obstaclesList=new List<Transform>();
    int obstaclessPassed = 0;
    Coroutine routine=null;

    public static LevelManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        CreateObstaclesPool();
        obstaclessPassed = 0;
        StartLevel();
    }

    public void ToggleLevel(gameStats gameStat)
    {
        if (gameStat == gameStats.lose)
        {
            backgroundAnimation.animationSpeed = 0;
            if(routine != null)
            {
                StopCoroutine(routine);
                routine = null;
            }
            levelIndex = 0;
            ObjectTweener.StopTweens();
        }
        
    }

    private void CreateObstaclesPool()
    {
        if (obstaclesPool.transform.childCount >= minObstaclePoolSize)
        {
            foreach(Transform t in obstaclesPool.transform)
            {
                t.gameObject.SetActive(false);
            }
            return;
        }

        var diff = minObstaclePoolSize - obstaclesPool.transform.childCount;

        for (int i= diff; i > 0; i--)
        {
            var obj=Instantiate(pipeCouplePrefab,obstaclesPool.transform);
            obstaclesList.Add(obj.transform);
            obj.SetActive(false);
        }
    }

    void StartLevel(int levelIndex=0, bool loadNextLevel=false)
    {
        var currentLevel = levels.levelsList[levelIndex];
        backgroundAnimation.animationSpeed = Mathf.Lerp(backgroundAnimation.animationSpeed, 0.1f * currentLevel.levelSpeed, 0.5f);
        if(routine == null || loadNextLevel==true)
        {
            routine = StartCoroutine(placeObstacles(currentLevel));
        }
    }

    IEnumerator placeObstacles(Levels.Level currentLevel)
    {
        float speed = currentLevel.levelSpeed;
        float offset = currentLevel.obstaclesOffset;
        Vector3 currentPos = new Vector3(
            obstaclesPool.transform.position.x,
            UnityEngine.Random.Range(downPoint.position.y,upPoint.position.y),
            obstaclesPool.transform.position.z);
        float pipePos_Y = currentPos.y;

        int obstaclesCount = currentLevel.obstaclesCount + obstaclessPassed;
        int direction = 1;


        for (int i = obstaclessPassed; i < obstaclesCount; i++)
        {
            if (UnityEngine.Random.Range(0.0f, 1.0f) < currentLevel.changeDirecrionChance 
                || (pipePos_Y + changeStep) > upPoint.position.y
                || (pipePos_Y - changeStep) < downPoint.position.y)
            {
                direction *= -1;
            }

            pipePos_Y += changeStep * direction;
            
            var currentPipePos= new Vector3(currentPos.x,pipePos_Y,currentPos.z);
            var finishPipePos = new Vector3(finishPoint.position.x, pipePos_Y, finishPoint.position.z);

            var pipeIndex = i;
            if (i >= obstaclesList.Count)
            {
                pipeIndex = i % obstaclesList.Count;
            }
            ObjectTweener.MoveObject(
                obstaclesList[pipeIndex], currentPipePos, finishPipePos, (Math.Abs(finishPoint.position.x - currentPos.x) / speed));
            yield return new WaitForSeconds(offset / speed);
        }

        obstaclessPassed += currentLevel.obstaclesCount;

        yield return new WaitForSeconds(currentLevel.restTime);
        if (levelIndex+1 < levels.levelsList.Count)
        {
            levelIndex++;
        }

        StartLevel(levelIndex, true);
    }

}
