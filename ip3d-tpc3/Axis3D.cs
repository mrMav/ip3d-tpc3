﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ip3d_tpc3
{
    /// <summary>
    /// Class for representing an axis system.
    /// The code is self explanatory.
    /// It uses creates and draws a list of lines.
    /// </summary>
    class Axis3D
    {
        Game Game;

        VertexPositionColor[] vertices;
        BasicEffect effect;
        public Matrix worldMatrix;
        
        Vector3 Position;
        float Size;

        public Axis3D(Game game, Vector3 position, float size = 1f)
        {

            Game = game;

            Size = size;
            Position = position;

            worldMatrix = Matrix.Identity;

            effect = new BasicEffect(game.GraphicsDevice);
            effect.LightingEnabled = false;
            effect.VertexColorEnabled = true;

            CreateGeometry();
        }

        private void CreateGeometry()
        {
            int vertexCount = 6; 

            vertices = new VertexPositionColor[vertexCount];

            // x axis
            this.vertices[0] = new VertexPositionColor(new Vector3(Position.X, Position.Y, Position.Z), Color.Red);
            this.vertices[1] = new VertexPositionColor(new Vector3(Position.X + Size, Position.Y, Position.Z), Color.Red);

            // y axis
            this.vertices[2] = new VertexPositionColor(new Vector3(Position.X, Position.Y, Position.Z), Color.Green);
            this.vertices[3] = new VertexPositionColor(new Vector3(Position.X, Position.Y + Size, Position.Z), Color.Green);

            // z axis
            this.vertices[4] = new VertexPositionColor(new Vector3(Position.X, Position.Y, Position.Z), Color.Blue);
            this.vertices[5] = new VertexPositionColor(new Vector3(Position.X, Position.Y, Position.Z + Size), Color.Blue);
        }

        // updates the effect matrices
        public void UpdateShaderMatrices(Matrix viewTransform, Matrix projectionTransform)
        {
            effect.Projection = projectionTransform;
            effect.View = viewTransform;
            effect.World = worldMatrix;
        }

        public void Draw(GameTime gameTime)
        {
           
            effect.CurrentTechnique.Passes[0].Apply();
            Game.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>( PrimitiveType.LineList, vertices, 0, 3);

        }
    }
}
