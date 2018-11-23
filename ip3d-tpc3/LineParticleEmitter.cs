﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace ip3d_tpc3
{
    public class LineParticleEmitter
    {

        /*
         * game reference
         */
        public Game Game;

        /*
         * Radius of the emitter disc
         */
        public float Radius;

        /*
         * The emitter position
         */
        public Vector3 Position;

        /*
         * emitter rotation
         */
        public Vector3 Rotation;

        /*
         * World transform
         */
        public Matrix WorldTransform;

        /*
         * The maximum number of particles.
         */
        private int _maxParticles;
        public int MaxParticles
        {
            get
            {
                return _maxParticles;
            }
            set
            {
            }
        }

        /*
         * If this emitter is active.
         */
        public bool Activated { get; set; }

        /*
         * The array containing all the particles
         * Arrays are faster than lists.
         */
        public LineParticle[] Particles;

        /*
         * The geometry of each particle
         */
        public VertexPositionColor[] VertexList;

        /*
         * Shader
         */
        BasicEffect Effect;

        /*
         * Rasterizer
         */
        RasterizerState RasterizerState;

        /*
         * helper axis
         */
        Axis3D Axis;

        /*
         * This emitter RNG
         */
        private Random Rnd;

        /*
         * Burst Mode Flag
         */
        public bool Burst;

        /*
         * How Many Particles per burst
         */
        private int _particlesPerBurst;
        public int ParticlesPerBurst
        {
            get
            {
                return _particlesPerBurst;
            }
            set
            {
                if (value > _maxParticles)
                {
                    _particlesPerBurst = _maxParticles;
                }
                else
                {
                    _particlesPerBurst = value;
                }
            }
        }

        #region [Particles Properties]

        /*
         * Particles lifespan
         */ 
        public double ParticleLifespanMilliseconds;

        /*
         * The variation of the lifespan
         */ 
        public double ParticleLifespanVariationMilliseconds;

        /*
         * The last spawned particle time
         */ 
        public double LastSpawnedParticleMilliseconds;

        /*
         * The rate at which this emitter spawns particles
         */ 
        public double SpawnRate;

        /*
         * The particle standard velocity
         */ 
        public Vector3 ParticleVelocity;

        /*
         * Variation in X velocity
         */ 
        public Vector2 XVelocityVariationRange;

        /*
         * Variation in Y velocity
         */
        public Vector2 YVelocityVariationRange;

        /*
         * Variation in Z velocity
         */
        public Vector2 ZVelocityVariationRange;
                
        #endregion


        /*
         * Constructor
         */
        public LineParticleEmitter(Game game, Vector3 position, float radius = 5f, int maxParticles = 100, int seed = 0)
        {
            Game = game;

            Radius = radius;
            Position = position;
            Rotation = Vector3.Zero;

            _maxParticles = maxParticles;

            // generates the rng based on seed
            Rnd = new Random(seed);

            /*
             * Set some default properties
             */ 

            Activated = true;
            ParticleLifespanMilliseconds = 1000f;
            LastSpawnedParticleMilliseconds = 0f;
            ParticleLifespanVariationMilliseconds = 0f;
            SpawnRate = 500f;

            ParticleVelocity = Vector3.Zero;
            XVelocityVariationRange = Vector2.Zero;
            YVelocityVariationRange = Vector2.Zero;
            ZVelocityVariationRange = Vector2.Zero;

            ParticlesPerBurst = 5;
            Burst = false;

            // the effect to render the particles
            Effect = new BasicEffect(game.GraphicsDevice);
            Effect.LightingEnabled = false;
            Effect.VertexColorEnabled = true;

            // define a wireframe rasterizer
            RasterizerState = new RasterizerState();
            RasterizerState.CullMode = CullMode.None;
            RasterizerState.FillMode = FillMode.WireFrame;

            // create helper axis
            Axis = new Axis3D(game, Vector3.Zero, radius);

        }

        /// <summary>
        /// Generates the particles pool
        /// </summary>
        /// <param name="size">the particle size</param>
        /// <param name="color">the color</param>
        public void MakeParticles(float size, Color color)
        {
            // allocate Particles array
            Particles = new LineParticle[_maxParticles];

            // allocate vertexlist array
            VertexList = new VertexPositionColor[_maxParticles * 2];

            for (int i = 0, j = 0; i < _maxParticles; i++)
            {
                // create the new new particle
                Particles[i] = new LineParticle(Game, color, Vector3.Zero, size);
                Particles[i].Spawner = this;

                // create its geometry
                VertexList[j++] = new VertexPositionColor(Vector3.Zero, color);
                VertexList[j++] = new VertexPositionColor(new Vector3(0f, size, 0f), color);
            }
        }

        /// <summary>
        /// Update method
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {

            // update the transform
            Matrix translation = Matrix.CreateTranslation(Position);
            Matrix rotation = Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);

            WorldTransform = rotation * translation;


            if (Activated)
            {
                // spawn particle
                if (LastSpawnedParticleMilliseconds < gameTime.TotalGameTime.TotalMilliseconds)
                {
                    // update timer
                    LastSpawnedParticleMilliseconds = gameTime.TotalGameTime.TotalMilliseconds + SpawnRate;

                    if (Burst)
                    {

                        // the burst mode collects particles until reaching the ParticlesPerBurst
                        // basicly, it sets that ammount of particles to be alive

                        int count = 0;

                        for (int i = 0; i < _maxParticles; i++)
                        {

                            if (count >= ParticlesPerBurst)
                            {
                                break;
                            }

                            if (!Particles[i].Alive)
                            {
                                SetParticleReady(Particles[i], gameTime);
                                count++;
                            }
                        }

                        // deactivate after burst
                        Activated = false;

                    }
                    else
                    {
                        // if it is not on burst mode, we get the first particle in the array that is dead

                        // get the first dead particle
                        LineParticle p = null;
                        for (int i = 0; i < _maxParticles; i++)
                        {
                            if (!Particles[i].Alive)
                            {
                                p = Particles[i];
                                break;
                            }
                        }

                        // if we found one, reset it and set it ready to be updated
                        if (p != null)
                        {

                            SetParticleReady(p, gameTime);

                        }
                    }

                }

            }

            // update all particles
            for (int i = 0, j = 0; i < _maxParticles; i++)
            {
                LineParticle p = Particles[i];

                p.Update(gameTime);

                if(p.Alive)
                {
                    // if the particle is alive, we update the geometry
                    // to the correct position

                    VertexList[j++].Position = p.Position;
                    VertexList[j++].Position = new Vector3(p.Position.X, p.Position.Y + p.Size, p.Position.Z);

                } else
                {
                    // if dead, we make it a line with dimension 0, so it is skipped by the gpu
                    VertexList[j++].Position = Vector3.Zero;
                    VertexList[j++].Position = Vector3.Zero;

                }


            }

        }

        /// <summary>
        /// Draw method
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="camera"></param>
        public void Draw(GameTime gameTime, Camera camera)
        {

            // set the shader matrices
            Effect.World = Matrix.Identity;  // we don't want to use the worldtransform for rendering
            Effect.View = camera.ViewTransform;
            Effect.Projection = camera.ProjectionTransform;

            // prepare for render the line list
            Effect.CurrentTechnique.Passes[0].Apply();
            Game.GraphicsDevice.RasterizerState = RasterizerState;

            // draw call
            Game.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, VertexList, 0, VertexList.Length / 2);

            // draw the helper axis
            Axis.worldMatrix = WorldTransform;
            Axis.UpdateShaderMatrices(camera.ViewTransform, camera.ProjectionTransform);
            Axis.Draw(gameTime);


        }

        /// <summary>
        /// Sets the acceleration for each particle
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void SetAcceleration(float x, float y, float z)
        {
            for (int i = 0; i < _maxParticles; i++)
            {
                Particles[i].Acceleration.X = x;
                Particles[i].Acceleration.Y = y;
                Particles[i].Acceleration.Z = z;
            }
        }

        /// <summary>
        /// Sets the drag for each particle
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void SetDrag(float x, float y, float z)
        {
            for (int i = 0; i < _maxParticles; i++)
            {
                Particles[i].Drag.X = x;
                Particles[i].Drag.Y = y;
                Particles[i].Drag.Z = z;
            }
        }

        /// <summary>
        /// Sets the initial state of the particle
        /// </summary>
        /// <param name="p"></param>
        /// <param name="gameTime"></param>
        public void SetParticleReady(LineParticle p, GameTime gameTime)
        {

            // calculte new velocitys
            float velocityX = ParticleVelocity.X + Rnd.Next((int)XVelocityVariationRange.X, (int)XVelocityVariationRange.Y) * 0.01f;
            float velocityY = ParticleVelocity.Y + Rnd.Next((int)YVelocityVariationRange.X, (int)YVelocityVariationRange.Y) * 0.01f;
            float velocityZ = ParticleVelocity.Z + Rnd.Next((int)ZVelocityVariationRange.X, (int)ZVelocityVariationRange.Y) * 0.01f;

            // reset and revive
            p.Reset();
            p.Revive();
            
            // update lifespans and age
            p.SpawnedAtMilliseconds = gameTime.TotalGameTime.TotalMilliseconds;
            p.LifespanMilliseconds = ParticleLifespanMilliseconds + (float)Rnd.Next((int)-ParticleLifespanVariationMilliseconds, (int)ParticleLifespanVariationMilliseconds);

            // udpate position and velocity
            p.Velocity = Vector3.Transform(new Vector3(velocityX, velocityY, velocityZ), Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z));
            p.Position = Vector3.Transform(GetRandomPosition(), WorldTransform);
            
        }

        /// <summary>
        /// calculates and returns a random position inside a circle
        /// </summary>
        /// <returns></returns>
        public Vector3 GetRandomPosition()
        {
            
            float angle = (float)Utils.RandomBetween(Rnd, 0f, Math.PI);
            
            float radius = (float)Utils.RandomBetween(Rnd, -Radius, Radius);

            return new Vector3(radius * (float)Math.Cos(angle), 0f, radius * (float)Math.Sin(angle));
            

        }
        
    }
}