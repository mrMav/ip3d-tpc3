using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ip3d_tpc3
{
    public class LineParticle
    {

        public LineParticleEmitter Spawner;

        public double SpawnedAtMilliseconds = 0f;
        public double MillisecondsAfterSpawn = 0f;
        public double LifespanMilliseconds = 0f;
        
        public Vector3 Position;
        public Vector3 InitialPosition;

        public Vector3 Acceleration;
        public Vector3 Velocity;
        public Vector3 Drag;

        public bool Alive;

        public float Size;
        
        /*
         * Constructor
         */
        public LineParticle(Game game, Color color, Vector3 position, float size)
        {
            
            Acceleration = Vector3.Zero;
            Velocity = Vector3.Zero;

            Drag = new Vector3(1f);

            Position = position;
            InitialPosition = position;

            Size = size;

            Kill();

        }

        public void Update(GameTime gameTime)
        {

            if (Alive)
            {
                MillisecondsAfterSpawn += gameTime.ElapsedGameTime.TotalMilliseconds;

                Velocity += Acceleration;
                Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

                Velocity *= Drag;

            }

            if (MillisecondsAfterSpawn >= LifespanMilliseconds)
            {
                Kill();
            }

        }

        public void Kill()
        {

            Alive = false;

        }

        public void Revive()
        {
            Alive = true;
        }

        public void Reset()
        {
            Alive = false;

            SpawnedAtMilliseconds = 0f;
            MillisecondsAfterSpawn = 0f;

            Position = Vector3.Zero;
            Velocity = Vector3.Zero;
            Acceleration = Vector3.Zero;

        }

    }

}