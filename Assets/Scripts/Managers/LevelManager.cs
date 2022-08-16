using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject bird;

    public Transform finishPoint;
    public Transform upPoint;
    public Transform downPoint;

    public GameObject pipeCouplePrefab;
    public GameObject obstaclesPool;

    public BackgroundAnimation backgroundAnimation;
    public Levels levels;

    public int minObstaclePoolSize = 15;
    public int levelIndex = 0;
    public float levelSpeed = 0;
    public float changeStep = 0.5f;

    private List<Transform> obstaclesList=new List<Transform>();

    private Rigidbody2D bird_rb;
    private Vector3 birdDefaultPlace;
    private int obstaclessPassed = 0;
    Coroutine routine=null;

    private void Awake()
    {
        birdDefaultPlace = bird.transform.position;
        bird_rb=bird.GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Starts the game 
    /// </summary>
    public void StartGame()
    {

        obstaclessPassed = 0;
        bird_rb.gravityScale = 2;
        bird_rb.velocity = Vector2.zero;
        levelSpeed = 0;

        CreateObstaclesPool();
        StartLevel();
    }

    /// <summary>
    /// Stops the game (Must be called when the bird lost)
    /// </summary>
    public void StopGame()
    {
        bird.transform.position = birdDefaultPlace;
        bird.transform.GetChild(0).position=birdDefaultPlace;

        bird_rb.gravityScale = 0;
        bird_rb.velocity = Vector2.zero;

        backgroundAnimation.animationSpeed = 0;
        if (routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }
        levelIndex = 0;
        ObjectTweener.StopTweens();

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

    private void StartLevel(int levelIndex=0, bool loadNextLevel=false)
    {
        var currentLevel = levels.levelsList[levelIndex];
        levelSpeed = currentLevel.levelSpeed;
        backgroundAnimation.animationSpeed = Mathf.Lerp(backgroundAnimation.animationSpeed, 0.1f * currentLevel.levelSpeed, 0.5f);
        if(routine == null || loadNextLevel==true)
        {
            routine = StartCoroutine(placeObstacles(currentLevel));
        }
    }

    private IEnumerator placeObstacles(Levels.Level currentLevel)
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
