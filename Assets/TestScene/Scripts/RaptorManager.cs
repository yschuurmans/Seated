using Raptus;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.TestScene.Scripts;
using UnityEngine;

public class RaptorManager : MonoBehaviour
{
    public List<Raptor> raptors = new List<Raptor>();
    public static RaptorManager Instance;

    public double waveTime = 2000; //ms
    public double waveLength = 10; 

    double waveSeatPos;
    double waveBack1Pos;
    double waveBack2Pos;
    double waveHeadPos;

    bool massageState;
    bool heartbeat;
    bool beating;

    public ushort[,] StretchMatrix;
    public List<RaptorInput> ActiveInputs;

    protected byte raptorID;
    protected ushort[,] raptorHead;
    protected ushort[,] raptorBack;
    protected ushort[,] raptorSeat;

    Capabilities Capabilities;

    

    void Awake()
    {
        if (Instance != null)
            Destroy(this.gameObject);
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        initWave();

        Raptus.API.OnReady += OnReady;
        Raptus.API.OnDeviceConnected += OnDeviceConnected;
        Raptus.API.OnDeviceDisconnected += OnDeviceDisconnected;
        Raptus.API.OnError += OnError;

        Raptus.API.Start();

        RaptorInput.OnInputChanged += CalculateMatrix;
    }

    // Update is called once per frame
    void Update()
    {
       


        if (massageState)
        {
            doWave();
        }

        else if (heartbeat)
        {
           StartCoroutine(doHeartBeat());
        }
        else
        {
            foreach (Raptor raptor in raptors)
            {
                API.Vibrate(raptor.StretchMatrix, API.Mode.Stretch);
            }
        }

    }

    private void CalculateMatrix(int raptorID)
    {
        /*for (int columnIndex = 0; columnIndex < StretchMatrix.GetLength(0); columnIndex++)
        {
            for (int rowIndex = 0; rowIndex < StretchMatrix.GetLength(1); rowIndex++)
            {
                raptorHead[columnIndex, rowIndex] = ActiveInputs.Max(input => input.InputMatrix[columnIndex, rowIndex]);
            }
        }*/
    }

    private void initWave()
    {

        waveHeadPos = waveLength * 0.125;
        waveBack1Pos = waveLength * 0.25;
        waveBack2Pos = waveLength * 0.375;
        waveSeatPos = waveLength * 0.5;

    }

    protected void OnReady()
    {
        // This event fires after the API has made a successful connection to the Raptus driver and is
        // ready to receive vibration commands.

        //TODO
    }

    protected void OnDeviceConnected(Raptus.Device raptus)
    {
        // This event fires for each connected Raptus seperately. It can fire multiple times after OnReady,
        // or when a Raptus is plugged in later. The first parameter will always contain a Raptus.Device
        // instance that details its ID and capabilities.

        this.raptorID = raptus.ID;
        Capabilities = raptus.Capabilities;

        this.raptorHead = new ushort[Capabilities.Head.Rows, Capabilities.Head.Columns];
        this.raptorBack = new ushort[Capabilities.Back.Rows, Capabilities.Back.Columns];
        this.raptorSeat = new ushort[Capabilities.Seat.Rows, Capabilities.Seat.Columns];
        API.Off(this.raptorID); // reset any prior vibration



        this.raptorHead[0, 0] = API.MaxValue;
    }

    protected void OnDeviceDisconnected(byte raptusID)
    {
        // This event fires when a Raptus is disconnected for whatever reason.


        if (raptusID == this.raptorID)
        {
            //TODO
            API.Off(this.raptorID); // reset any prior vibration

        }
    }

    protected void OnError(byte ErrorCode, object ErrorContext = null)
    {
        // The Raptus API may report an error by passing an error code. You should always use the error code
        // constants under Raptus.API.ErrorCode to handle these errors. The error context may pass some 
        // aditional information related to the error, but this is currently unused.

        switch (ErrorCode)
        {
            case API.ErrorCode.CouldNotConnect:
                //TODO
                break;

            case API.ErrorCode.DriverDisconnected:
                //TODO
                break;

            default:
                //TODO
                break;
        }
    }

    public void btnStartWave()
    {
        massageState = !massageState;
        if (!massageState || heartbeat)
        {
            API.Off(this.raptorID);
            heartbeat = false;
        }
    }

   

    private void doWave()
    {
        //currentTime += Update.Interval;
        //if (currentTime > waveTime) currentTime = 0;

        waveHeadPos = updatePos(waveHeadPos);
        waveBack1Pos = updatePos(waveBack1Pos);
        waveBack2Pos = updatePos(waveBack2Pos);
        waveSeatPos = updatePos(waveSeatPos);

        //0.437
        raptorHead[0, 0] = getVibrateSpeed(getHeightOfWave(waveHeadPos));

        raptorBack[0, 0] = getVibrateSpeed(getHeightOfWave(waveBack1Pos));
        raptorBack[0, 1] = raptorBack[0, 0];
        raptorBack[1, 0] = getVibrateSpeed(getHeightOfWave(waveBack2Pos));
        raptorBack[1, 1] = raptorBack[1, 0];

        raptorSeat[0, 0] = getVibrateSpeed(getHeightOfWave(waveSeatPos));
        raptorSeat[0, 1] = raptorSeat[0, 0];

        API.Vibrate(raptorID, this.raptorHead, API.Mode.Head);
        API.Vibrate(raptorID, this.raptorBack, API.Mode.Back);
        API.Vibrate(raptorID, this.raptorSeat, API.Mode.Seat);

        //API.Vibrate(raptorID, this.raptorBack, API.Mode.);

        //API.Vibrate(raptorID, this.raptorHead);
        //API.Vibrate(raptorID, this.raptorBack);
        //API.Vibrate(raptorID, this.raptorSeat);
    }

    private double updatePos(double pos)
    {
        pos += getInterval(Time.deltaTime * 1000);
        if (pos > waveLength)
        {
            pos -= waveLength;
        }
        return pos;
    }

    private double getInterval(double interval)
    {
        //elke tick moet de positie X veranderen
        //100 ticks, wave is 10 lang
        //dus elke tick 100/10 x opzij
        double ticks = waveTime / interval;
        return waveLength / ticks;

        //double ratio = (interval / (waveTime / 100)) / 100;
        //return waveLength * ratio;
    }



    private double getHeightOfWave(double X)
    {
        //double currentX = getCurrentX();
        double amplitude = -1;
        double frequency = 0.2;
        double elevation = -amplitude / 2;
        double freqUnits = 1 / frequency;

        double newX = frequency * X;
        double xMulPi = newX * Math.PI;
        double cos = Math.Cos(xMulPi);
        double devide = cos / 2;
        double amp = devide * amplitude;
        double y = amp + elevation;                

        return y;
    }

    private ushort getVibrateSpeed(double height)
    {
        //if wave height is 1
        double ratio = height;
        return (ushort)(API.MaxValue * ratio);
    }

    private void startVibration(byte ratpusID, ushort[,] matrix, byte mode, float milliseconds)
    {
        //ratpusID, matrix, mode, milliseconds
        StartCoroutine(vibrate(ratpusID, matrix, mode, milliseconds));
    }

    private IEnumerator vibrate(byte ratpusID, ushort[,] matrix, byte mode, float milliseconds)
    {
        float seconds = milliseconds / 1000;
        API.Vibrate(ratpusID, matrix, mode);
        yield return new WaitForSeconds(seconds);
        API.Off(this.raptorID);
        yield return 0;
    }

    public void btnStartHeartbeat()
    {
        heartbeat = !heartbeat;
        if (!heartbeat || massageState)
        {
            API.Off(this.raptorID);
            massageState = false;
        }
    }

    private IEnumerator doHeartBeat()
    {
        if (!beating)
        {
            beating = true;
            raptorHead[0, 0] = API.MaxValue;

            API.Vibrate(raptorID, raptorHead, API.Mode.Head);

            yield return new WaitForSeconds(0.06f);

            API.Off(this.raptorID);

            yield return new WaitForSeconds(0.1f);

            API.Vibrate(raptorID, raptorHead, API.Mode.Head);

            yield return new WaitForSeconds(0.05f);

            API.Off(this.raptorID);


            yield return new WaitForSeconds(1);
            beating = false;
        }

        yield return 0;
    }
  

    public void btnToggleAllRaptors()
    {        
        raptorID = 0;
    }

    void OnApplicationQuit()
    {
        API.Stop();
    }

    public void randomJolt()
    {
        int rows = 4;
        int columns = 2;
        ushort[,] matrix = new ushort[rows, columns];

        int randomRow = UnityEngine.Random.Range(0, rows);
        int randomColumn = UnityEngine.Random.Range(0, columns);

        
        matrix[randomRow, randomColumn] = API.MaxValue;
        startVibration(this.raptorID, matrix, API.Mode.Stretch, 50);
    }

    //matrix[0, 1] = API.MaxValue;
    //matrix[1, 0] = API.MaxValue;
    ////startVibration(this.raptorID, matrix, API.Mode.Stretch, 2000);
    //API.Vibrate(raptorID, matrix, API.Mode.Stretch);

    //Debug.Log("First virbration");
    //System.Threading.Thread.Sleep(2000);

    //Debug.Log("second virbration");
    //matrix[0, 1] = 30000;

    //API.Vibrate(raptorID, matrix, API.Mode.Stretch);
    //// startVibration(this.raptorID, matrix, API.Mode.Stretch, 500);

    //System.Threading.Thread.Sleep(2000);
}
