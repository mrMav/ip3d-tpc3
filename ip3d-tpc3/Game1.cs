﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace ip3d_tpc3
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont font;

        Axis3D axis;

        Plane plane;

        OrbitCamera camera;

        DirectionalLight light;

        LineParticleEmitter particleEmitter;
        LineParticleEmitter purpleRainEmitter;

        FrameRate frameRate;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.SynchronizeWithVerticalRetrace = true;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            //graphics.IsFullScreen = true;

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

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            Window.Title = $"EP3D-TPC3 - JORGE NORO - 15705 {graphics.GraphicsProfile}, Sampling: {graphics.PreferMultiSampling}, Samples: {GraphicsDevice.PresentationParameters.MultiSampleCount}";

            // set mouse cursor state
            IsMouseVisible = false;

            // TODO: Add your initialization logic here

            font = Content.Load<SpriteFont>("font");

            axis = new Axis3D(this, Vector3.Zero, 10);

            plane = new Plane(this, "checker2", 10, 10, 2, 2);

            camera = new OrbitCamera(this, Vector3.Zero, new Vector3(20, 0, 0));
            camera.Target.Y = 2f;

            light = new DirectionalLight(new Vector4(1, 1, 0, 0), Color.Yellow.ToVector4(), 0.85f);

            particleEmitter = new LineParticleEmitter(this, new Vector3(2f, 2f, 0f), 0.5f, 1000);
            particleEmitter.Rotation.X = MathHelper.ToRadians(90f);
            particleEmitter.MakeParticles(0.05f, Color.Yellow);
            particleEmitter.ParticleVelocity = new Vector3(0f, 0f, 0f);
            particleEmitter.YVelocityVariationRange = new Vector2(-1000f, 10000f);
            particleEmitter.XVelocityVariationRange = new Vector2(-100f, 100f);
            particleEmitter.ZVelocityVariationRange = new Vector2(-100f, 100f);
            particleEmitter.SpawnRate = 120f;
            particleEmitter.ParticleLifespanMilliseconds = 1000f;
            particleEmitter.ParticleLifespanVariationMilliseconds = 500f;
            particleEmitter.ParticlesPerBurst = 200;
            particleEmitter.Burst = true;
            particleEmitter.Activated = true;

            purpleRainEmitter = new LineParticleEmitter(this, new Vector3(0f, 10f, 0f), 5f, 500);            
            purpleRainEmitter.MakeParticles(0.1f, Color.Magenta);
            purpleRainEmitter.ParticleVelocity = new Vector3(0f, -9.8f, 0f);
            purpleRainEmitter.YVelocityVariationRange = new Vector2(-100f, 100f);
            purpleRainEmitter.XVelocityVariationRange = new Vector2(-100f, 100f);
            purpleRainEmitter.ZVelocityVariationRange = new Vector2(-100f, 100f);
            purpleRainEmitter.SpawnRate = 0f;
            purpleRainEmitter.ParticleLifespanMilliseconds = 1000f;
            purpleRainEmitter.ParticleLifespanVariationMilliseconds = 500f;
            purpleRainEmitter.Activated = true;

            frameRate = new FrameRate();

            // init controls
            Controls.Initilalize();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            frameRate.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            Controls.UpdateCurrentStates();

            if (Controls.IsKeyPressed(Keys.M))
                IsMouseVisible = !IsMouseVisible;

            if(!IsMouseVisible)
                Mouse.SetPosition(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            camera.Update(gameTime);
            plane.Update(gameTime);

            axis.UpdateShaderMatrices(camera.ViewTransform, camera.ProjectionTransform);
            plane.UpdateShaderMatrices(camera.ViewTransform, camera.ProjectionTransform);

            particleEmitter.Rotation.Y += MathHelper.ToRadians(89f) * (float)gameTime.ElapsedGameTime.TotalSeconds;            
            particleEmitter.Rotation.Z += MathHelper.ToRadians(89f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            particleEmitter.Rotation.X += MathHelper.ToRadians(89f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            particleEmitter.Activated = true;
            particleEmitter.Update(gameTime);

            purpleRainEmitter.Update(gameTime);

            // rain suplementary kill condition
            // (collision with floor)
            for (int i = 0; i < purpleRainEmitter.MaxParticles; i++)
            {

                LineParticle p = purpleRainEmitter.Particles[i];

                if(p.Alive)
                {

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
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(0.15f, 0.15f, 0.15f));

            // TODO: Add your drawing code here
            axis.Draw(gameTime);
            plane.DrawCustomShader(gameTime, camera, light);

            particleEmitter.Draw(gameTime, camera);
            purpleRainEmitter.Draw(gameTime, camera);


            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, null, null, null);
            spriteBatch.DrawString(font, $"{Math.Round(frameRate.AverageFramesPerSecond)}", new Vector2(10f, 10f), new Color(0f, 1f, 0f));
            spriteBatch.End();



            base.Draw(gameTime);
        }
        
    }
}
