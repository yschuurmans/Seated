using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.TestScene.Scripts
{
    public class Raptor : MonoBehaviour
    {
        private Rigidbody rb;
        public int ID;
        public ushort[,] StretchMatrix;
        public List<RaptorInput> ActiveInputs;
        public BoxCollider raptorCollider;
        public ContactPoint[] contactPoints;

        void Start()
        {

            rb = GetComponent<Rigidbody>();
            RaptorInput.OnInputChanged += OnInputChanged;
            contactPoints = GetComponentsInChildren<ContactPoint>();
            initMatrix();
        }

        void Update()
        {
            UpdateMatrix();
        }

        private void initMatrix()
        {
            int maxRow = 0;
            int maxColumn = 0;
            foreach (ContactPoint cp in contactPoints)
            {
                if (cp.row > maxRow) maxRow = cp.row;
                if (cp.column > maxColumn) maxColumn = cp.column;
            }

            StretchMatrix = new ushort[maxRow + 1, maxColumn + 1];
        }

        private void UpdateMatrix()
        {

            foreach (ContactPoint cp in contactPoints)
            {
                StretchMatrix[cp.row, cp.column] = cp.GetForce;
            }
        }



        private void OnInputChanged(int raptorID)
        {
            if (raptorID != ID) return;

            CalculateMatrix();
        }

        public IEnumerator AddActiveInput(RaptorInput input, float durationInSeconds = 0)
        {
            ActiveInputs.Add(input);
            CalculateMatrix();

            if (durationInSeconds == 0) yield break;
            yield return new WaitForSeconds(durationInSeconds);
            RemoveActiveInput(input);
        }

        public void RemoveActiveInput(RaptorInput input)
        {
            //Guard condition
            if (!ActiveInputs.Contains(input))
                return;

            ActiveInputs.Remove(input);
            CalculateMatrix();
        }

        private void CalculateMatrix()
        {
            for (int columnIndex = 0; columnIndex < StretchMatrix.GetLength(0); columnIndex++)
            {
                for (int rowIndex = 0; rowIndex < StretchMatrix.GetLength(1); rowIndex++)
                {
                    StretchMatrix[columnIndex, rowIndex] = ActiveInputs.Max(input => input.InputMatrix[columnIndex, rowIndex]);
                }
            }
        }
    }
}
