using Microsoft.Xna.Framework;
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

            //// calculate coordinates
            //// the camera will rotate around the tank
            //float x = (float)Math.Sin(MathHelper.ToRadians(Yaw)) * OffsetDistance;
            //float z = (float)Math.Cos(MathHelper.ToRadians(Yaw)) * OffsetDistance;

            //float y = MathHelper.ToRadians(Pitch) * OffsetDistance;

            //// the result will be an offset in the world space, offseted by the tank position
            //// the offset will also push back when accelerating
            //Position = new Vector3(x, y, z) + Target;


            //float x = OffsetDistance * (float)Math.Sin(MathHelper.ToRadians(Yaw)) * (float)Math.Cos(MathHelper.ToRadians(Pitch));
            //float y = OffsetDistance * (float)Math.Sin(MathHelper.ToRadians(Yaw)) * (float)Math.Sin(MathHelper.ToRadians(Pitch));
            //float z = OffsetDistance * (float)Math.Cos(MathHelper.ToRadians(Yaw));
            
            Vector3 position = new Vector3(0f, 0f, OffsetDistance);

            position = Vector3.Transform(position, Matrix.CreateRotationX(MathHelper.ToRadians(Pitch)));

            position = Vector3.Transform(position, Matrix.CreateRotationY(MathHelper.ToRadians(Yaw)));

            Position = position + Target;


            // finally, update view transform
            ViewTransform = Matrix.CreateLookAt(Position, Target, Vector3.Up);

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


        public override string About()
        {
            return "Orbit camera.";
        }

    }
}
