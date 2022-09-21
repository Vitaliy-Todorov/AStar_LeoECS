using Leopotam.Ecs;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Infrastructure.Systems
{
    public class InputSystem : IEcsRunSystem
    {
        private readonly string _vertical = "Vertical";
        private readonly string _horizontal = "Horizontal";

        public Click _click = new Click();
        public Click Click { get => _click; }

        public bool LeftShift;

        public Vector3 Axis
        {
            get
            {
                Vector3 axis = new Vector3();
                axis.x = Input.GetAxis(_horizontal);
                axis.z = Input.GetAxis(_vertical);

                return axis;
            }
        }

        public bool Shift =>
            Input.GetKey(KeyCode.LeftShift);

        public Vector3 ClickPosition
        {
            get
            {
                return Input.mousePosition;
            }
        }

        public void Run()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Click.Up = true;
                Click.StaryPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }

            if (Input.GetMouseButton(0))
            {
                Click.Up = false;
                Click.Active = true;
                Click.EndPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            else
            {
                Click.Up = false;
                Click.Active = false;
            }


            if (Input.GetMouseButtonUp(0))
            {
                Click.Up = true;
                Click.EndPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }

            if (Input.GetKey(KeyCode.LeftShift))
                LeftShift = true;
            else
                LeftShift = false;
        }
    }
}