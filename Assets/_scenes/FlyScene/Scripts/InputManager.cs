using System.Collections;
using System.Collections.Generic;
using XInputDotNetPure;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static float instance;

    public float turnRate;
    public float velocity;
    private Rigidbody flyObject;
    bool playerIndexSet = false;
    GamePadState state;
    GamePadState prevState;
    PlayerIndex playerIndex;



    // Use this for initialization
    void Start()
    {
        flyObject = GetComponent<Rigidbody>();
        flyObject.velocity = Vector3.zero;
    }

    void FixedUpdate()
    {
        if (!playerIndexSet || !prevState.IsConnected)
        {
            for (int i = 0; i < 4; ++i)
            {
                PlayerIndex testPlayerIndex = (PlayerIndex)i;
                GamePadState testState = GamePad.GetState(testPlayerIndex);
                if (testState.IsConnected)
                {
                    Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
                    playerIndex = testPlayerIndex;
                    playerIndexSet = true;
                }
            }
        }
        prevState = state;
        state = GamePad.GetState(playerIndex);
        Movement();
        UserInputs();
        transform.position += transform.forward * velocity;
        //transform.position += transform.forward * velocity;
        //flyObject.AddForce(transform.forward * velocity);
        flyObject.angularVelocity = Vector3.Lerp(flyObject.angularVelocity, Vector3.zero, 0.9f);
    }


    void Movement()
    {
        if (Input.GetKey(KeyCode.W))
        {
            rotateForward();
            //transform.position += transform.forward * velocity;
        }
        if (Input.GetKey(KeyCode.S))
        {
            rotateBackwards();
            // transform.position -= transform.forward * velocity;
        }
        if (Input.GetKey(KeyCode.D))
        {
            rotateRight();
        }
        if (Input.GetKey(KeyCode.A))
        {
            rotateLeft();
        }
        if (Input.GetKey(KeyCode.E))
        {
            turnRight();
            //transform.position += Vector3.up * velocity;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            turnLeft();
            //transform.position += Vector3.down * velocity;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            //transform.position += Vector3.up * velocity;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            //transform.position += Vector3.down * velocity;
        }

        //flyObject.angularVelocity = Vector3.Lerp(flyObject.angularVelocity, Vector3.zero, 0.1f);

        //transform.Rotate(new Vector3(0, 0, -state.ThumbSticks.Right.X));
        transform.Rotate(new Vector3(state.ThumbSticks.Right.Y, state.ThumbSticks.Left.X, -state.ThumbSticks.Right.X));
        //transform.Rotate(new Vector3(0, state.ThumbSticks.Left.X, 0));

        if (prevState.Buttons.A == ButtonState.Pressed && (state.Buttons.A == ButtonState.Released || state.Buttons.A == ButtonState.Pressed)) flyObject.AddForce(transform.forward * 100 * Time.deltaTime);
        if (prevState.Buttons.X == ButtonState.Pressed && (state.Buttons.X == ButtonState.Released || state.Buttons.X == ButtonState.Pressed)) flyObject.AddForce(transform.forward * 2000 * Time.deltaTime);
        if (state.Triggers.Right > 0.05f)
            flyObject.AddForce(transform.forward * state.Triggers.Right * 2000 * Time.deltaTime);

        if (state.Triggers.Left > 0.05f)
            flyObject.AddForce(transform.forward * state.Triggers.Left * -2000 * Time.deltaTime);

        if (prevState.Buttons.Back == ButtonState.Pressed && state.Buttons.Back == ButtonState.Pressed &&
            prevState.Buttons.Start == ButtonState.Pressed && state.Buttons.Start == ButtonState.Pressed)
        {
            flyObject.rotation = Quaternion.identity;
            flyObject.position = new Vector3(0, 200, 0);
            flyObject.velocity = Vector3.zero;
            flyObject.angularVelocity = Vector3.zero;
            flyObject.freezeRotation = true;
        }
        else
        {

            flyObject.freezeRotation = false;
        }
    }


    void UserInputs()
    {
        if (prevState.Buttons.A == ButtonState.Released && state.Buttons.A == ButtonState.Pressed)
        {
            //Debug.Log("A pressed");
        }
        if (prevState.Buttons.B == ButtonState.Released && state.Buttons.B == ButtonState.Pressed)
        {
            //Debug.Log("B pressed");
        }
        if (prevState.Buttons.Y == ButtonState.Released && state.Buttons.Y == ButtonState.Pressed)
        {
            //Debug.Log("Y pressed");
        }
        if (prevState.Buttons.X == ButtonState.Released && state.Buttons.X == ButtonState.Pressed)
        {
            //Debug.Log("X pressed");
        }
        if (prevState.Buttons.Start == ButtonState.Released && state.Buttons.Start == ButtonState.Pressed)
        {
            //Debug.Log("Start pressed");
        }
        if (prevState.Buttons.Back == ButtonState.Released && state.Buttons.Back == ButtonState.Pressed)
        {
            //Debug.Log("Back pressed");
        }
        if (prevState.Buttons.LeftStick == ButtonState.Released && state.Buttons.LeftStick == ButtonState.Pressed)
        {
            //Debug.Log("Left stick pressed");
        }
        if (prevState.Buttons.RightStick == ButtonState.Released && state.Buttons.RightStick == ButtonState.Pressed)
        {
            //Debug.Log("Right stick pressed");
        }
        if (prevState.Buttons.RightShoulder == ButtonState.Released && state.Buttons.RightShoulder == ButtonState.Pressed)
        {
            //Debug.Log("Right shoulder pressed");
        }
        if (prevState.Buttons.LeftShoulder == ButtonState.Released && state.Buttons.LeftShoulder == ButtonState.Pressed)
        {
            //Debug.Log("Left shoulder pressed");
        }
        if (state.Triggers.Left > 0)
        {

            //Debug.Log("Left trigger");
        }
        if (state.Triggers.Right > 0)
        {
            Boost();
            //Debug.Log("Right trigger");
        }
    }

    void Boost()
    {
        transform.position += transform.forward * velocity * 2;
    }

    void rotateForward()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.Rotate(new Vector3(turnRate, 0, 0));
            //transform.position += transform.forward * velocity;
        }
    }

    void rotateBackwards()
    {
        if (Input.GetKey(KeyCode.S))
        {
            transform.Rotate(new Vector3(-turnRate, 0, 0));
            // transform.position -= transform.forward * velocity;
        }
    }

    void rotateLeft()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(new Vector3(0, 0, turnRate));
        }
    }

    void rotateRight()
    {
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(new Vector3(0, 0, -turnRate));
        }
    }

    void turnLeft()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(new Vector3(0, -turnRate, 0));
            //transform.position += Vector3.down * velocity;
        }
    }

    void turnRight()
    {
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(new Vector3(0, turnRate, 0));
            //transform.position += Vector3.up * velocity;
        }
    }
}
