using Microsoft.Xna.Framework;
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

        public double ParticleLifespanMilliseconds;
        public double ParticleLifespanVariationMilliseconds;
        public double LastSpawnedParticleMilliseconds;
        public double SpawnRate;

        public Vector3 ParticleVelocity;
        public Vector2 XVelocityVariationRange;
        public Vector2 YVelocityVariationRange;
        public Vector2 ZVelocityVariationRange;
        public float VelocityMultiplier;
        
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

            Rnd = new Random(seed);

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

            Effect = new BasicEffect(game.GraphicsDevice);
            Effect.LightingEnabled = false;
            Effect.VertexColorEnabled = true;

            RasterizerState = new RasterizerState();
            RasterizerState.CullMode = CullMode.None;
            RasterizerState.FillMode = FillMode.WireFrame;

            Axis = new Axis3D(game, Vector3.Zero, radius);

        }

        public void MakeParticles(float size, Color color)
        {
            Particles = new LineParticle[_maxParticles];
            VertexList = new VertexPositionColor[_maxParticles * 2];

            for (int i = 0, j = 0; i < _maxParticles; i++)
            {
                Particles[i] = new LineParticle(Game, color, Vector3.Zero, size);
                Particles[i].Spawner = this;

                VertexList[j++] = new VertexPositionColor(Vector3.Zero, color);
                VertexList[j++] = new VertexPositionColor(new Vector3(0f, size, 0f), color);
            }
        }

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

                        Activated = false;

                    }
                    else
                    {
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
                    // update geometry data

                    VertexList[j++].Position = p.Position;
                    VertexList[j++].Position = new Vector3(p.Position.X, p.Position.Y + p.Size, p.Position.Z);

                } else
                {

                    // update it in such a way that it is slipped by the gpu.
                    VertexList[j++].Position = Vector3.Zero;
                    VertexList[j++].Position = Vector3.Zero;

                }


            }

        }

        public void Draw(GameTime gameTime, Camera camera)
        {

            //Effect.World = WorldTransform;
            Effect.World = Matrix.Identity;
            Effect.View = camera.ViewTransform;
            Effect.Projection = camera.ProjectionTransform;

            Effect.CurrentTechnique.Passes[0].Apply();
            Game.GraphicsDevice.RasterizerState = RasterizerState;
            Game.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, VertexList, 0, VertexList.Length / 2);

            // draw the helper axis
            Axis.worldMatrix = WorldTransform;
            Axis.UpdateShaderMatrices(camera.ViewTransform, camera.ProjectionTransform);
            Axis.Draw(gameTime);


        }

        public void SetAcceleration(float x, float y, float z)
        {
            for (int i = 0; i < _maxParticles; i++)
            {
                Particles[i].Acceleration.X = x;
                Particles[i].Acceleration.Y = y;
                Particles[i].Acceleration.Z = z;
            }
        }

        public void SetDrag(float x, float y, float z)
        {
            for (int i = 0; i < _maxParticles; i++)
            {
                Particles[i].Drag.X = x;
                Particles[i].Drag.Y = y;
                Particles[i].Drag.Z = z;
            }
        }

        public void SetParticleReady(LineParticle p, GameTime gameTime)
        {

            float velocityX = ParticleVelocity.X + Rnd.Next((int)XVelocityVariationRange.X, (int)XVelocityVariationRange.Y) * 0.01f;
            float velocityY = ParticleVelocity.Y + Rnd.Next((int)YVelocityVariationRange.X, (int)YVelocityVariationRange.Y) * 0.01f;
            float velocityZ = ParticleVelocity.Z + Rnd.Next((int)ZVelocityVariationRange.X, (int)ZVelocityVariationRange.Y) * 0.01f;

            p.Reset();
            p.Revive();
            
            p.SpawnedAtMilliseconds = gameTime.TotalGameTime.TotalMilliseconds;
            p.LifespanMilliseconds = ParticleLifespanMilliseconds + (float)Rnd.Next((int)-ParticleLifespanVariationMilliseconds, (int)ParticleLifespanVariationMilliseconds);

            p.Velocity = Vector3.Transform(new Vector3(velocityX, velocityY, velocityZ), Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z));

            p.Position = Vector3.Transform(GetRandomPosition(), WorldTransform);
            
        }

        public Vector3 GetRandomPosition()
        {
            // calculates and returns a random position inside a circle

            //float PI2 = (float)Math.PI * 2;

            float angle = (float)Utils.RandomBetween(Rnd, 0f, Math.PI);
            
            float radius = (float)Utils.RandomBetween(Rnd, -Radius, Radius);

            return new Vector3(radius * (float)Math.Cos(angle), 0f, radius * (float)Math.Sin(angle));
            

        }
        
    }
}