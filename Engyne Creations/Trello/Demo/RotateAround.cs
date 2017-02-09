/*
 * RotateAround.cs
 * Rotate an object around a pivot.
 *  
 * by Adam Carballo under GPLv3 license.
 * https://github.com/AdamEC/Unity-Trello
 */

using UnityEngine;

namespace Trello {
    public class RotateAround : MonoBehaviour {

        [SerializeField] private Transform target;

 
        private void FixedUpdate() {

            transform.LookAt(target);
            transform.RotateAround(target.position, Vector3.up, Time.deltaTime * 4);
        }
    }
}