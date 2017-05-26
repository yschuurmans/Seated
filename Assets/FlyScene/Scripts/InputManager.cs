using System.Collections;
using System.Collections.Generic;
using XInputDotNetPure;
using UnityEngine;

public class InputManager : MonoBehaviour
{
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
    }

    void Update()
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
        //transform.position += transform.forward * velocity;
        //flyObject.AddForce(transform.forward * velocity);
    }

    
    void Movement()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * velocity;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= transform.forward * velocity;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(new Vector3(0, 1, 0));
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(new Vector3(0, -1, 0));
        }
        if (Input.GetKey(KeyCode.Space))
        {
            transform.position += Vector3.up * velocity;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.position += Vector3.down * velocity;
        }



        transform.Rotate(new Vector3(0, 0, -state.ThumbSticks.Right.X));
        transform.Rotate(new Vector3(state.ThumbSticks.Right.Y, 0, 0));
        transform.Rotate(new Vector3(0, state.ThumbSticks.Left.X, 0));

        if(prevState.Buttons.A == ButtonState.Pressed && (state.Buttons.A == ButtonState.Released || state.Buttons.A == ButtonState.Pressed)) flyObject.AddForce(transform.forward * 100 * Time.deltaTime);
    }

    
    void UserInputs()
    {
        if (prevState.Buttons.A == ButtonState.Released && state.Buttons.A == ButtonState.Pressed)
        {
            Debug.Log("A pressed");
        }
        if (prevState.Buttons.B == ButtonState.Released && state.Buttons.B == ButtonState.Pressed)
        {
            Debug.Log("B pressed");
        }
        if (prevState.Buttons.Y == ButtonState.Released && state.Buttons.Y == ButtonState.Pressed)
        {
            Debug.Log("Y pressed");
        }
        if (prevState.Buttons.X == ButtonState.Released && state.Buttons.X == ButtonState.Pressed)
        {
            Debug.Log("X pressed");
        }
        if (prevState.Buttons.Start == ButtonState.Released && state.Buttons.Start == ButtonState.Pressed)
        {
            Debug.Log("Start pressed");
        }
        if (prevState.Buttons.Back == ButtonState.Released && state.Buttons.Back == ButtonState.Pressed)
        {
            Debug.Log("Back pressed");
        }
        if (prevState.Buttons.LeftStick == ButtonState.Released && state.Buttons.LeftStick == ButtonState.Pressed)
        {
            Debug.Log("Left stick pressed");
        }
        if (prevState.Buttons.RightStick == ButtonState.Released && state.Buttons.RightStick == ButtonState.Pressed)
        {
            Debug.Log("Right stick pressed");
        }
        if (prevState.Buttons.RightShoulder == ButtonState.Released && state.Buttons.RightShoulder == ButtonState.Pressed)
        {
            Debug.Log("Right shoulder pressed");
        }
        if (prevState.Buttons.LeftShoulder == ButtonState.Released && state.Buttons.LeftShoulder == ButtonState.Pressed)
        {
            Debug.Log("Left shoulder pressed");
        }
        if (state.Triggers.Left > 0)
        {
            Debug.Log("Left trigger");
        }
        if (state.Triggers.Right > 0)
        {
            Debug.Log("Right trigger");
        }
    }
}
