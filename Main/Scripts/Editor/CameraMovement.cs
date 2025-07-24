using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transportme.Main.Scripts.Editor
{
    public partial class CameraMovement : Node3D
    {
        protected Camera3D camera;
        public override void _Ready()
        {
            camera = GetChild<Camera3D>(0);
        }


        public override void _Process(double delta)
        {
            Vector3 move = new Vector3();
            if (Input.IsActionPressed("move_forward"))
            {
                move += Vector3.Forward;
            }

            if (Input.IsActionPressed("move_backward"))
            {
                move += Vector3.Back;
            }
            if (Input.IsActionPressed("move_left"))
            {
                move += Vector3.Left;
            }
            if (Input.IsActionPressed("move_right"))
            {
                move += Vector3.Right;
            }

            camera.GetParent<Node3D>().Position += GetNoHeightVector(move * Quaternion.FromEuler(camera.Rotation)) * (float)delta * 4;
            base._Process(delta);
        }

        private Vector3 GetNoHeightVector(Vector3 v)
        {
            v.Y = 0;
            return v;
        }
    }
}
