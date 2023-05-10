using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class AgentMovement : Agent
{
    public float moveSpeed = 3f;
    public float rotateSpeed = 50f;
    public float jumpForce = 300f;
    public GameObject target;

    private Rigidbody playerRigidbody;
    private float startTime = 10f;
    private float playerTime;
    private float moveX;
    private float moveZ;

    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        playerTime -= Time.deltaTime;
        if (playerTime < 0)
        {
            AddReward(-1f);
            EndEpisode();
        }

        AddReward(-0.01f * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        Vector3 playerMove = new Vector3(moveX, 0f, moveZ) * moveSpeed * Time.deltaTime;
        transform.position += playerMove;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Target"))
        {
            AddReward(1f);
            EndEpisode();
        }
        else if (other.gameObject.CompareTag("Wall"))
        {
            AddReward(-1f);
            EndEpisode();
        }
    }

    public override void OnEpisodeBegin()
    {
        playerRigidbody.velocity = Vector3.zero;
        playerRigidbody.angularVelocity = Vector3.zero;
        transform.localPosition = new Vector3(Random.Range(-4f, 4f), 0.5f, Random.Range(-4f, 4f));
        transform.localRotation = Quaternion.identity;

        target.transform.localPosition = new Vector3(Random.Range(-4f, 4f), 0.5f, Random.Range(-4f, 4f));
        while (Vector3.Distance(target.transform.localPosition, transform.localPosition) < 2f)
        {
            target.transform.localPosition = new Vector3(Random.Range(-4f, 4f), 0.5f, Random.Range(-4f, 4f));
        }

        playerTime = startTime;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.transform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        switch (actions.DiscreteActions[0])
        {
            case 0:
                moveX = -1f;
                break;
            case 1:
                moveX = 0f;
                break;
            case 2:
                moveX = 1f;
                break;
        }
        switch (actions.DiscreteActions[1])
        {
            case 0:
                moveZ = -1f;
                break;
            case 1:
                moveZ = 0f;
                break;
            case 2:
                moveZ = 1f;
                break;
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        discreteActions[0] = (int)Input.GetAxisRaw("Horizontal") + 1;
        discreteActions[1] = (int)Input.GetAxisRaw("Vertical") + 1;
    }
}
