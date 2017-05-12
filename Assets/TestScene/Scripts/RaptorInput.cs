namespace Assets.TestScene.Scripts
{
    public class RaptorInput
    {
        public delegate void InputChangedEventHandler(int raptorID);

        public static event InputChangedEventHandler OnInputChanged = (raptorID) => { };

        public int RaptorID { get; private set; }

        public ushort[,] InputMatrix { get; private set; }

        public RaptorInput(int raptorID, ushort[,] inputMatrix)
        {
            RaptorID = raptorID;
            InputMatrix = inputMatrix;
        }

        public void EditMatrix(ushort[,] inputMatrix)
        {
            InputMatrix = inputMatrix;
            if (!RaptorManager.Instance.ActiveInputs.Contains(this))
                return;

            OnInputChanged(RaptorID);
        }
    }
}
