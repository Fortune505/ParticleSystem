using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParticleSystem
{
    public class Emitter
    {
        List<Particle> particles = new List<Particle>();
        public int MousePositionX;
        public int MousePositionY;

        public float GravitationX = 0;
        public float GravitationY = 0;

        public List<IImpactPoint> impactPoints = new List<IImpactPoint>();

        public int ParticlesCount = 500;

        public int X;
        public int Y;
        public int Direction = 0;
        public int Spreading = 360;
        public int SpeedMin = 1;
        public int SpeedMax = 10;
        public int RadiusMin = 2;
        public int RadiusMax = 10;
        public int LifeMin = 20;
        public int LifeMax = 100;
        public int ParticlesPerTick = 1;
        public Color ColorFrom = Color.White;
        public Color ColorTo = Color.FromArgb(0, Color.Black);


        public virtual void ResetParticle(Particle particle)
        {
            particle.Life = Particle.rand.Next(LifeMin, LifeMax);
            particle.X = X;
            particle.Y = Y;

            var direction = Direction + (double)Particle.rand.Next(Spreading) - Spreading / 2;
            var speed = Particle.rand.Next(SpeedMin, SpeedMax);

            particle.SpeedX = (float)(Math.Cos(direction / 180 * Math.PI) * speed);
            particle.SpeedY = -(float)(Math.Sin(direction / 180 * Math.PI) * speed);

            particle.Radius = Particle.rand.Next(RadiusMin, RadiusMax);
        }

        public virtual Particle CreateParticle()
        {
            var particle = new ParticleColorful();
            particle.FromColor = ColorFrom;
            particle.ToColor = ColorTo;

            return particle;

        }
        public virtual void UpdateState()
        {
            int particlesToCreate = ParticlesPerTick;

            foreach (var particle in particles)
            {
                if (particle.Life <= 0)
                {
                    if (particlesToCreate > 0)
                    {
                        particlesToCreate -= 1;
                        ResetParticle(particle);
                    }
                }
                else
                {
                    particle.X += particle.SpeedX;
                    particle.Y += particle.SpeedY;

                    particle.Life -= 1;

                    foreach (var point in impactPoints)
                    {
                        point.ImpactParticle(particle);
                    }

                    particle.SpeedX += GravitationX;
                    particle.SpeedY += GravitationY;
                }

            }

            while (particlesToCreate >= 1)
            {
                particlesToCreate -= 1;
                var particle = CreateParticle();
                ResetParticle(particle);
                particles.Add(particle);
            }
        }
         

        public virtual void Render(Graphics g)
        {
            foreach (var particle in particles)
            {
                particle.Draw(g);
            }

            foreach (var point in impactPoints)
            {
                point.Render(g);
            }
        }
    }

    public abstract class IImpactPoint
    {
        public float X;
        public float Y;

        public abstract void ImpactParticle(Particle particle);

        public virtual void Render(Graphics g) {
            g.FillEllipse(
                new SolidBrush(Color.Red),
                X - 5, Y - 5, 10, 10
            );
        }
    }

    public class GravityPoint : IImpactPoint
    {
        public int Power = 100;

        public override void ImpactParticle(Particle particle)
        {
            float gX = X - particle.X;
            float gY = Y - particle.Y;

            double r = Math.Sqrt(gX * gX + gY * gY);

            if (r + particle.Radius < Power / 2)
            {
                float r2 = (float)Math.Max(100, gX * gX + gY * gY);
                particle.SpeedX += gX * Power / r2;
                particle.SpeedY += gY * Power / r2;
            }
        }

        public override void Render(Graphics g)
        {
            g.DrawEllipse(
                new Pen(Color.Red),
                X - Power / 2,
                Y - Power / 2, 
                Power, 
                Power
            );

            var stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            g.DrawString(
                $"{Power}",
                new Font("Verdana", 10),
                Brushes.White,
                X,
                Y,
                stringFormat
                );
        }
    }

    public class AntiGravityPoint : IImpactPoint
    {
        public float Power = 100;

        public override void ImpactParticle(Particle particle)
        {
            float gX = X - particle.X;
            float gY = Y - particle.Y;
            float r2 = (float)Math.Max(100, gX * gX + gY * gY);

            particle.SpeedX -= gX * Power / r2;
            particle.SpeedY -= gY * Power / r2;
        }
    }

    public class TeleportPoint : IImpactPoint
    {
        public float TargetX;
        public float TargetY;
        public int Radius = 30;

        public override void ImpactParticle(Particle particle)
        {
            float gX = X - particle.X;
            float gY = Y - particle.Y;
            double r = Math.Sqrt(gX * gX + gY * gY);

            if (r + particle.Radius < Radius)
            {
                particle.X = TargetX;
                particle.Y = TargetY;
            }
        }

        public override void Render(Graphics g)
        {
            g.DrawEllipse(new Pen(Color.DeepSkyBlue, 2), X - Radius, Y - Radius, Radius * 2, Radius * 2);
            g.DrawEllipse(new Pen(Color.Orange, 2), TargetX - Radius, TargetY - Radius, Radius * 2, Radius * 2);

            var stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            g.DrawString("Вход", new Font("Verdana", 8, FontStyle.Bold), Brushes.DeepSkyBlue, X, Y - Radius - 15, stringFormat);
            g.DrawString("Выход", new Font("Verdana", 8, FontStyle.Bold), Brushes.Orange, TargetX, TargetY - Radius - 15, stringFormat);
        }
    }

    public class TopEmitter : Emitter
    {
        public int Width;

        public override void ResetParticle(Particle particle)
        {
            base.ResetParticle(particle);

            particle.X = Particle.rand.Next(Width);
            particle.Y = 0;

            particle.SpeedX = 1;
            particle.SpeedY = Particle.rand.Next(-2, 2);
        }
    }

    public class CircularEmitter : Emitter 
    {
        public float CircularRadius = 100;
        public float RotationSpeed = 0.5f;
        public float CurrentAngle = 0;

        public override void UpdateState()
        {
            CurrentAngle += RotationSpeed;

            X = (int)(MousePositionX + CircularRadius * Math.Cos(CurrentAngle / 180 * Math.PI));
            Y = (int)(MousePositionY + CircularRadius * Math.Sin(CurrentAngle / 180 * Math.PI));

            Direction = (int)(CurrentAngle + 90);

            base.UpdateState();
        }

        public override void Render(Graphics g)
        {
            base.Render(g);

            Pen orbitPen = new Pen(Color.FromArgb(100, Color.White));
            orbitPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

            g.DrawEllipse(
                orbitPen,
                MousePositionX - CircularRadius,
                MousePositionY - CircularRadius,
                CircularRadius * 2,
                CircularRadius * 2
                );

            g.FillEllipse(Brushes.White, X - 3, Y - 3, 6, 6);
        }
    }

}
