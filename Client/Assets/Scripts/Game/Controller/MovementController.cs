using System;
using Entity.Request;
using UnityEngine;

namespace Game.Controller
{
    public class MovementController : MonoBehaviour
    {
        public float speed;
        private bool _isInitialized;
        private string _id;
        private Camera _camera;
        private Action<MoveRequest> _move;

        private void Awake()
        {
            gameObject.GetComponent<Renderer>().material.color = new Color(0.0f, 0.9608f, 0.8784f);
            _camera = Camera.main;
            if (_camera != null)
            {
                var cameraTransform = _camera.transform;
                cameraTransform.parent = gameObject.transform;
                cameraTransform.localPosition = new Vector3(0, 0, -10);
            }
            _isInitialized = false;
        }

        private void Update()
        {
            if (!_isInitialized) return;
            float dx = 0;
            float dy = 0;

            if (Input.GetKey(KeyCode.W)) dy += speed;
            if (Input.GetKey(KeyCode.S)) dy -= speed;
            if (Input.GetKey(KeyCode.A)) dx -= speed;
            if (Input.GetKey(KeyCode.D)) dx += speed;

            if (dx != 0 || dy != 0)
            {
                _move(new MoveRequest(_id, dx * Time.deltaTime, dy * Time.deltaTime));
            }
        }
        
        public void Initialize(string id, Action<MoveRequest> move)
        {
            speed = 2f;
            _id = id;
            _move = move;
            _isInitialized = true;
        }
    }
}