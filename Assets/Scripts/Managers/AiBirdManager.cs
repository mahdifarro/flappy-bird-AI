using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

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

        // totall 1 observations
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
    /// End the Episode if bird hits an obstacle
    /// </summary>
    /// <param name="objectTag"></param>
    public void HitObject(string objectTag)
    {
        if (objectTag == "Obstacle")
        {
            levelManager.StopGame();
            AddReward(-10);
            EndEpisode();
        }
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
