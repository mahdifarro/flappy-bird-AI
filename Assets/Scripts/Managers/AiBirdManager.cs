using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

public class AiBirdManager : Agent
{
    public LevelManager levelManager;

    [Tooltip("Is bird in the training mode?")]
    public bool isTraining=true;

    [SerializeField]
    private Animator bird_animator;
    private Rigidbody2D bird_rb;


    // shows the velocity of bird's jump
    [SerializeField]
    private float velocity = 1;

    private bool isGameRunning = false;

    private int highScore=0;

    public Text currentScoreText;
    public Text HighScoreText;
    public override void Initialize()
    {
        MaxStep = 0;
        bird_rb = GetComponent<Rigidbody2D>();
    }

    public override void OnEpisodeBegin()
    {
        isGameRunning = true;
        levelManager.StartGame();
    }

    /// <summary>
    /// Collect vector observations from the environment
    /// </summary>
    /// <param name="sensor">The vector sensor</param>
    public override void CollectObservations(VectorSensor sensor)
    {
        // Observe the speed of the curent level (1 observation)
        sensor.AddObservation(levelManager.levelSpeed);

        GameObject[] pipesList=FindPipes();

        // Distance from first pipe
        float firstPipeDistance_x;
        // Distance from second pipe
        float secondPipeDistance_x;
        // Distance from back pipe
        float backPipeDistance_x;

        if (pipesList[0] == null)
        {
            firstPipeDistance_x = 20;
        }
        else
        {
            firstPipeDistance_x = pipesList[0].transform.position.x;
        }

        if (pipesList[1] == null)
        {

            secondPipeDistance_x = 20;
        }
        else
        {
            secondPipeDistance_x = pipesList[1].transform.position.x;
        }

        if (pipesList[2] == null)
        {
            backPipeDistance_x = 20;
        }
        else
        {
            backPipeDistance_x = pipesList[2].transform.position.x;
        }

        // Observe the distance from first pipe in z axis (1 observation)
        sensor.AddObservation(firstPipeDistance_x);
        // Observe the distance from second pipe in z axis (1 observation)
        sensor.AddObservation(secondPipeDistance_x);
        // Observe the distance from back pipe in z axis (1 observation)
        sensor.AddObservation(backPipeDistance_x);

        float upPipeDistance_y;
        float downPipeDistance_y;
        if (pipesList[0] != null)
        {
            upPipeDistance_y = pipesList[0].GetComponent<PipeManager>().upPipe.transform.position.y;
            downPipeDistance_y = pipesList[0].GetComponent<PipeManager>().downPipe.transform.position.y;
        }
        else
        {
            upPipeDistance_y = 10;
            downPipeDistance_y = -10;
        }

        // Observe the distance from up pipe in y axis(1 observation)
        sensor.AddObservation(upPipeDistance_y);
        // Observe the distance from down pipe in y axis(1 observation)
        sensor.AddObservation(downPipeDistance_y);

        float upBackPipeDistance_y;
        float downBackPipeDistance_y;
        if (pipesList[2] != null)
        {
            upBackPipeDistance_y = pipesList[2].GetComponent<PipeManager>().upPipe.transform.position.y;
            downBackPipeDistance_y = pipesList[2].GetComponent<PipeManager>().downPipe.transform.position.y;
        }
        else
        {
            upBackPipeDistance_y = 10;
            downBackPipeDistance_y = -10;
        }

        // Observe the distance from up back pipe in y axis(1 observation)
        sensor.AddObservation(upBackPipeDistance_y);
        // Observe the distance from down back pipe in y axis(1 observation)
        sensor.AddObservation(downBackPipeDistance_y);

        // Observe distance from the ground (-2.65) (1 observation)
        sensor.AddObservation(transform.position.y - (-2.65f));

        // Observe bird's velocity (1 observation)
        sensor.AddObservation(bird_rb.velocity.y);

        //string obsArray= "";
        //foreach (float obs in GetObservations())
        //{
        //    obsArray+=obs.ToString()+" ";
        //}
        //Debug.Log(obsArray);

        //Debug.Log("END");

        // totall 11 observations
    }

    /// <summary>
    /// Called when and an action is received from either the player input or the neural network
    /// Index 0: jump if it is 1
    /// </summary>
    /// <param name="vectorAction"></param>
    public override void OnActionReceived(ActionBuffers actions)
    {
        int jumpAction = actions.DiscreteActions[0];
        Jump(jumpAction);
    }


    // <summary>
    /// When Behaviour Type is  set on 'Heuristic only" on the agent's Behaviour parameters,
    /// this function will be called. Its return values will be fed into
    /// <see cref="OnActionReceived(float[])"/> instead of using neural network
    /// </summary>
    /// <param name="actionsOut">An output action vector</param>
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        int JumpAction = 0;

        if (Input.GetKey(KeyCode.Space))
        {
            JumpAction = 1;
        }
        else
        {
            JumpAction = 0;
        }

        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        discreteActions[0] = JumpAction;
    }

    private void Update()
    {
        GiveScore();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>
    /// first index of list is first pipe
    /// second index is the second pipe
    /// third is the back pipe
    /// </returns>
    private GameObject[] FindPipes()
    {
        GameObject firstPipe;
        GameObject secondPipe;
        GameObject backPipe;
        if (levelManager.frontObserver.pipesList.Count == 0)
        {
            firstPipe = null;
            secondPipe = null;
        }
        else if (levelManager.frontObserver.pipesList.Count == 1)
        {
            firstPipe = levelManager.frontObserver.pipesList[0];
            secondPipe = null;
        }
        else
        {
            firstPipe = levelManager.frontObserver.pipesList[0];
            secondPipe = levelManager.frontObserver.pipesList[1];
        }

        if (levelManager.backObserver.pipesList.Count == 0)
        {
            backPipe = null;
        }
        else
        {
            backPipe = levelManager.backObserver.pipesList[0];
        }

        return new GameObject[] { firstPipe, secondPipe, backPipe };
    }

    /// <summary>
    /// End the Episode if bird hits an obstacle
    /// </summary>
    /// <param name="objectTag"></param>
    public void HitObject(string objectTag)
    {
        if (objectTag == "Obstacle")
        {  
            currentScoreText.text="Score: " + (int)GetCumulativeReward();
            if ((int)GetCumulativeReward() >= highScore)
            {
                highScore= (int)GetCumulativeReward();
            }
            HighScoreText.text="High Score: " + highScore;

            levelManager.StopGame();
            AddReward(-10);
            EndEpisode();
        }
    }

    public void ChangeMode(Text buttonName)
    {
        if(gameObject.GetComponent<BehaviorParameters>().BehaviorType == BehaviorType.InferenceOnly)
        {
            buttonName.text = "Heuristic";
            gameObject.GetComponent<BehaviorParameters>().BehaviorType = BehaviorType.HeuristicOnly;
            velocity = 5;
        }
        else if(gameObject.GetComponent<BehaviorParameters>().BehaviorType == BehaviorType.HeuristicOnly)
        {
            buttonName.text = "Inference";
            gameObject.GetComponent<BehaviorParameters>().BehaviorType = BehaviorType.InferenceOnly;
            velocity = 2.5f;
        }
    }

    private void CheckHighScore()
    {
        throw new NotImplementedException();
    }

    // Give Reward each frame bird stays alive
    void GiveScore()
    {
        if (isGameRunning)
        {

            AddReward(Time.deltaTime);
        }

    }

    /// <summary>
    /// Makes the bird jump if the jump action is equal to 1
    /// </summary>
    /// <param name="jumpAction"></param>
    void Jump(int jumpAction)
    {
        Debug.Assert(jumpAction!=0 || jumpAction!=1, "Action int is not valid");

        if (jumpAction == 0) return;
        else if (jumpAction == 1)
        {
            bird_rb.velocity = Vector2.up * velocity;
            bird_animator.SetTrigger("Jump");
        }
    }


}
