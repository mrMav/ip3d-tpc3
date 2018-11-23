using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace ip3d_tpc3
{
    /// <summary>
    /// Holds the global data of the application
    /// </summary>
    static class Stats
    {

        // the count of alive particles in the game
        public static int AliveParticles = 0;

        // extra features activation
        public static bool MadnessEnabled = false;

    }

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        // graphics manager
        GraphicsDeviceManager graphics;

        // spritbatch for gui render
        SpriteBatch spriteBatch;

        // font for debug text
        SpriteFont font;

        // world axis helper
        Axis3D axis;

        // floor plane
        Plane plane;

        // active camera
        OrbitCamera camera;

        // light properties to use on shader
        DirectionalLight light;

        // a particle generator, simulating rain
        LineParticleEmitter purpleRainEmitter;

        // couple of other emitters for extra functionality
        LineParticleEmitter particleEmitter1;
        LineParticleEmitter particleEmitter2;
        LineParticleEmitter particleEmitter3;
        LineParticleEmitter particleEmitter4;

        // framerate counter
        FrameRate frameRate;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // prepare game rendering options:
            graphics.SynchronizeWithVerticalRetrace = true;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            graphics.PreparingDeviceSettings += Graphics_PreparingDeviceSettings;
            graphics.ApplyChanges();
        }

        // callback for preparing device settings, see link above for more info
        private void Graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            graphics.PreferMultiSampling = true;
            e.GraphicsDeviceInformation.PresentationParameters.MultiSampleCount = 8;  // samples count
        }

        protected override void Initialize()
        {

            Window.Title = $"EP3D-TPC3 - JORGE NORO - 15705 {graphics.GraphicsProfile}, Sampling: {graphics.PreferMultiSampling}, Samples: {GraphicsDevice.PresentationParameters.MultiSampleCount}";

            // set mouse cursor state
            IsMouseVisible = false;

            // disable the fixed time step
            // solves buggy mouse lock
            IsFixedTimeStep = false;
            
            // load font
            font = Content.Load<SpriteFont>("font");

            // create axis
            axis = new Axis3D(this, Vector3.Zero, 10);

            // create plane with the preferred texture and size
            plane = new Plane(this, "checker2", 10, 10, 2, 2);

            // create camera to our preferences
            camera = new OrbitCamera(this, Vector3.Zero, 20f);
            camera.Target.Y = 2f;

            // create light 
            light = new DirectionalLight(new Vector4(1, 1, 0, 0), Color.Yellow.ToVector4(), 0.85f);

            // create a particle emitter
            particleEmitter1 = new LineParticleEmitter(this, new Vector3(5f, 2.5f, 0f), 0.1f, 20000);

            // rotate it
            particleEmitter1.Rotation.X = MathHelper.ToRadians(90f);

            // generate the particle pool
            particleEmitter1.MakeParticles(0.05f, Color.Yellow);

            // define a standard velocity to all particles
            particleEmitter1.ParticleVelocity = new Vector3(0f, -9.8f, 0f);

            // create a variation in the Y velocity
            particleEmitter1.YVelocityVariationRange = new Vector2(0f, 1000f);

            // specify the rate at which this emitter spawns particles
            particleEmitter1.SpawnRate = 0f;

            // life time of the particles
            particleEmitter1.ParticleLifespanMilliseconds = 1000f;

            // add some variation to the lifetime of the particles
            particleEmitter1.ParticleLifespanVariationMilliseconds = 500f;

            // enable burst mode for this particle emitter
            particleEmitter1.Burst = true;

            // define how many particles it releases at each time
            particleEmitter1.ParticlesPerBurst = 1000;

            // activate the emitter
            particleEmitter1.Activated = true;

            // another emitter
            particleEmitter2 = new LineParticleEmitter(this, new Vector3(-5f, 2.5f, 0f), 0.5f, 10000);
            particleEmitter2.MakeParticles(0.05f, Color.LightGreen);
            particleEmitter2.ParticleVelocity = new Vector3(0f, 0f, 0f);
            particleEmitter2.YVelocityVariationRange = new Vector2(-1000f, 10000f);
            particleEmitter2.XVelocityVariationRange = new Vector2(-100f, 100f);
            particleEmitter2.ZVelocityVariationRange = new Vector2(-100f, 100f);
            particleEmitter2.SpawnRate = 120f;
            particleEmitter2.ParticleLifespanMilliseconds = 1000f;
            particleEmitter2.ParticleLifespanVariationMilliseconds = 500f;
            particleEmitter2.ParticlesPerBurst = 2000;
            particleEmitter2.Burst = true;
            particleEmitter2.Activated = true;

            // another emitter
            particleEmitter3 = new LineParticleEmitter(this, new Vector3(0f, 2.5f, 5f), 0.5f, 10000);
            particleEmitter3.MakeParticles(0.05f, Color.LightSkyBlue);
            particleEmitter3.ParticleVelocity = new Vector3(0f, 0f, 0f);
            particleEmitter3.YVelocityVariationRange = new Vector2(-1000f, 10000f);
            particleEmitter3.XVelocityVariationRange = new Vector2(-100f, 100f);
            particleEmitter3.ZVelocityVariationRange = new Vector2(-100f, 100f);
            particleEmitter3.SpawnRate = 60f;
            particleEmitter3.ParticleLifespanMilliseconds = 1000f;
            particleEmitter3.ParticleLifespanVariationMilliseconds = 500f;
            particleEmitter3.ParticlesPerBurst = 2000;
            particleEmitter3.Burst = true;
            particleEmitter3.Activated = true;

            // another emitter
            particleEmitter4 = new LineParticleEmitter(this, new Vector3(0f, 2.5f, -5f), 1f, 10000);
            particleEmitter4.MakeParticles(0.05f, Color.OrangeRed);
            particleEmitter4.ParticleVelocity = new Vector3(0f, 0f, 0f);
            particleEmitter4.YVelocityVariationRange = new Vector2(0f, 10000f);
            particleEmitter4.XVelocityVariationRange = new Vector2(-50f, 50f);
            particleEmitter4.ZVelocityVariationRange = new Vector2(-50f, 50f);
            particleEmitter4.SpawnRate = 0f;
            particleEmitter4.ParticleLifespanMilliseconds = 1000f;
            particleEmitter4.ParticleLifespanVariationMilliseconds = 900f;
            particleEmitter4.ParticlesPerBurst = 500;
            particleEmitter4.Burst = true;
            particleEmitter4.Activated = true;

            // another emitter. This one won't be burst mode
            // this one simulates the rain
            purpleRainEmitter = new LineParticleEmitter(this, new Vector3(0f, 10f, 0f), 5f, 500);            
            purpleRainEmitter.MakeParticles(0.1f, Color.Magenta);
            purpleRainEmitter.ParticleVelocity = new Vector3(0f, -9.8f, 0f);  // simulate gravity
            purpleRainEmitter.YVelocityVariationRange = new Vector2(-100f, 100f);
            purpleRainEmitter.XVelocityVariationRange = new Vector2(-100f, 100f);
            purpleRainEmitter.ZVelocityVariationRange = new Vector2(-100f, 100f);
            purpleRainEmitter.SpawnRate = 0f;
            purpleRainEmitter.ParticleLifespanMilliseconds = 1000f;
            purpleRainEmitter.ParticleLifespanVariationMilliseconds = 500f;
            purpleRainEmitter.Activated = true;

            // create the counter
            frameRate = new FrameRate();

            // init controls
            Controls.Initilalize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

        }

        protected override void UnloadContent()
        {
            
        }

        /// <summary>
        /// Update loop
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(GameTime gameTime)
        {
            // update frame counter with ellapsed time
            frameRate.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            // update the current input devices state
            Controls.UpdateCurrentStates();

            // release mouse toggle
            if (Controls.IsKeyPressed(Keys.R))
            {

                IsMouseVisible = !IsMouseVisible;

            }

            // madness toggle
            if(Controls.IsKeyPressed(Keys.M))
            {

                Stats.MadnessEnabled = !Stats.MadnessEnabled;

            }

            if(!IsMouseVisible)
            {
                // mosue capture
                Mouse.SetPosition(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);
            }

            // exit point
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // update game objects
            camera.Update(gameTime);
            plane.Update(gameTime);

            // update object matrices
            axis.UpdateShaderMatrices(camera.ViewTransform, camera.ProjectionTransform);
            plane.UpdateShaderMatrices(camera.ViewTransform, camera.ProjectionTransform);

            // update extra emitters on madness enabled
            if (Stats.MadnessEnabled)
            {

                particleEmitter1.Rotation.Y += MathHelper.ToRadians(90f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                particleEmitter1.Activated = true;

                particleEmitter1.Update(gameTime);

                particleEmitter2.Rotation.X += MathHelper.ToRadians(90f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                particleEmitter2.Activated = true;
                particleEmitter2.Update(gameTime);

                particleEmitter3.Rotation.Z += MathHelper.ToRadians(90f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                particleEmitter3.Activated = true;
                particleEmitter3.Update(gameTime);

                particleEmitter4.Activated = true;
                particleEmitter4.Update(gameTime);

            }

            // update our rain
            purpleRainEmitter.Update(gameTime);

            // rain suplementary kill condition
            // (collision with floor)
            for (int i = 0; i < purpleRainEmitter.MaxParticles; i++)
            {

                LineParticle p = purpleRainEmitter.Particles[i];

                if(p.Alive)
                {
                    // if the particle is bellow floor
                    // kill it
                    if(p.Position.Y < plane.Position.Y)
                    {
                        p.Kill();
                    }

                }
            }

            Controls.UpdateLastStates();

            base.Update(gameTime);
        }

        /// <summary>
        /// Draw function
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // clear screen
            GraphicsDevice.Clear(new Color(0.15f, 0.15f, 0.15f));

            // draw world axis helper
            axis.Draw(gameTime);

            // draw the plane using a custom shader
            plane.DrawCustomShader(gameTime, camera, light);

            // draw all the madness
            if(Stats.MadnessEnabled)
            {

                particleEmitter1.Draw(gameTime, camera);
                particleEmitter2.Draw(gameTime, camera);
                particleEmitter3.Draw(gameTime, camera);
                particleEmitter4.Draw(gameTime, camera);

            }

            // draw the rain
            purpleRainEmitter.Draw(gameTime, camera);

            // draw the GUI info
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, null, null, null);
            spriteBatch.DrawString(font, $"{Math.Round(frameRate.AverageFramesPerSecond)}", new Vector2(10f, 10f), new Color(0f, 1f, 0f));
            spriteBatch.DrawString(font, $"Alive Particles: {Stats.AliveParticles}", new Vector2(10f, 30f), new Color(0f, 1f, 0f));
            spriteBatch.DrawString(font, $"Madness (Toggle, 'M'): {Stats.MadnessEnabled}", new Vector2(10f, 50f), new Color(0f, 1f, 0f));
            spriteBatch.DrawString(font, $"Release Mouse (Toggle, 'R'): {Stats.MadnessEnabled}", new Vector2(10f, 70f), new Color(0f, 1f, 0f));
            spriteBatch.DrawString(font, camera.About(), new Vector2(graphics.PreferredBackBufferWidth / 2, 10f), new Color(0f, 1f, 0f));
            spriteBatch.End();
            
            base.Draw(gameTime);
        }
        
    }
}
