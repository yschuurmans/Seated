using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.TestScene.Scripts.HelperClasses
{
    public class CameraCullingMaskHelper
    {
        public static void ShowLayer(string layerName, Camera targetCamera = null)
        {
            if (targetCamera == null)
                targetCamera = Camera.main;

            targetCamera.cullingMask |= 1 << LayerMask.NameToLayer(layerName);
        }

        public static void HideLayer(string layerName, Camera targetCamera = null)
        {
            if (targetCamera == null)
                targetCamera = Camera.main;

            targetCamera.cullingMask &= ~(1 << LayerMask.NameToLayer(layerName));
        }

        public static void ToggleLayer(string layerName, Camera targetCamera = null)
        {
            if (targetCamera == null)
                targetCamera = Camera.main;

            targetCamera.cullingMask ^= 1 << LayerMask.NameToLayer(layerName);
        }
    }
}
