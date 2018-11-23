﻿using Microsoft.Xna.Framework;
using System;

namespace ip3d_tpc3
{
    public class OrbitCamera : Camera
    {        
        // the offset to keep the camera away from teh tank
        Vector3 Offset;
        
        // sensitivity
        public float MouseSensitivity = 0.1f;

        // minimum offset from the floor
        public float OffsetFromFloor = 1.76f;

        // yaw and pitch angles
        float Yaw = 45;

        float Pitch = -45;  // default;

        float Zoom = 45f;

        float TargetZoom = 45f;
        
        // the length of the offset
        float OffsetDistance;

        // constructor
        public OrbitCamera(Game game, Vector3 target, Vector3 offset, float fieldOfView = 45f) : base(game, fieldOfView)
        {
            
            Target = target;

            Offset = offset;
            
            OffsetDistance = offset.Length();
            
        }

        public override void Update(GameTime gameTime)
        {

            float midWidth = Game.GraphicsDevice.Viewport.Width / 2;
            float midHeight = Game.GraphicsDevice.Viewport.Height / 2;

            // processing the mouse movements
            // the mouse delta is calculated with the middle of the screen
            // because we will snap the mouse to it                
            ProcessMouseMovement(Controls.CurrMouseState.Position.X - midWidth, Controls.CurrMouseState.Position.Y - midHeight);
            ProcessMouseScroll();


            Vector3 position = new Vector3(0f, 0f, OffsetDistance);

            position = Vector3.Transform(position, Matrix.CreateRotationX(MathHelper.ToRadians(Pitch)));

            position = Vector3.Transform(position, Matrix.CreateRotationY(MathHelper.ToRadians(Yaw)));

            Position = position + Target;


            // finally, update view transform
            ViewTransform = Matrix.CreateLookAt(Position, Target, Vector3.Up);

            if(Zoom != TargetZoom)
            {

                Zoom += (TargetZoom - Zoom) * (float)gameTime.ElapsedGameTime.TotalSeconds;

            }

            ProjectionTransform = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(Zoom), (float)Game.GraphicsDevice.Viewport.Width / (float)Game.GraphicsDevice.Viewport.Height, 0.1f, 1000f);

            base.Update(gameTime);

        }

        // handles the mouse movement, updating the yaw, pitch and vectors
        // constrain the picth to avoid angles lock
        private void ProcessMouseMovement(float xoffset, float yoffset, bool constrainPitch = true)
        {
            // the given offset, is the diference from the previous mouse position and the current one
            xoffset *= MouseSensitivity;
            yoffset *= MouseSensitivity;

            Yaw -= xoffset;
            Pitch -= yoffset;  // here we can invert the Y

            if (constrainPitch)
            {
                if (Pitch > 0f)
                    Pitch = 0f;
                if (Pitch < -89.0f)
                    Pitch = -89.0f;
            }

        }

        // used to update the camera zoom based on mouse scroll
        // the code is self explanatory
        private void ProcessMouseScroll()
        {

            float value = Controls.CurrMouseState.ScrollWheelValue - Controls.LastMouseState.ScrollWheelValue;
            value *= 0.1f;

            if (TargetZoom >= 1.0f && TargetZoom <= 80.0f)
            {
                TargetZoom -= value;
            }

            if (TargetZoom <= 1.0f) TargetZoom = 1.0f;

            if (TargetZoom >= 80.0f) TargetZoom = 80.0f;

        }


        public override string About()
        {
            return "Orbit camera.";
        }

    }
}
