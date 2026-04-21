using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParticleSystem
{
    public partial class Form1 : Form
    {
        List<Emitter> emitters = new List<Emitter>();
        CircularEmitter emitter;
        GravityPoint point1;
        GravityPoint point2;
        TeleportPoint teleport;
        public Form1()
        {
            InitializeComponent();

            picDisplay.Image = new Bitmap(picDisplay.Width, picDisplay.Height);

            this.emitter = new CircularEmitter
            {
                Direction = 0,
                Spreading = 10,
                SpeedMin = 10,
                SpeedMax = 10,
                ColorFrom = Color.Gold,
                ColorTo = Color.FromArgb(0, Color.Red),
                ParticlesPerTick = 10,
                MousePositionX = picDisplay.Width / 2,
                MousePositionY = picDisplay.Height / 2,
                CircularRadius = 100,
                RotationSpeed = 2
            };

            emitters.Add(this.emitter);

            point1 = new GravityPoint
            {
                X = picDisplay.Width / 2 + 100,
                Y = picDisplay.Height / 2
            };

            point2 = new GravityPoint
            {
                X = picDisplay.Width / 2 - 100,
                Y = picDisplay.Height / 2
            };

            emitter.impactPoints.Add(point1);
            emitter.impactPoints.Add(point2);

            teleport = new TeleportPoint
            {
                X = picDisplay.Width / 2 - 200,
                Y = picDisplay.Height / 2 -100,
                TargetX = picDisplay.Width / 2 + 200,
                TargetY = picDisplay.Height / 2 + 100
            };

            emitter.impactPoints.Add(teleport);

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            foreach (var em in emitters)
            {
                em.UpdateState();
            }

            using (var g = Graphics.FromImage(picDisplay.Image)) {
                g.Clear(Color.Black);
                foreach (var em in emitters)
                {
                    em.Render(g);
                }
            }

            picDisplay.Invalidate();
        }


        private void picDisplay_MouseMove(object sender, MouseEventArgs e)
        {
            
            point2.X = e.X;
            point2.Y = e.Y;
            
        }
              
        private void tbGraviton1_Scroll(object sender, EventArgs e)
        {
            point1.Power = tbGraviton1.Value;
        }

        private void tbGraviton2_Scroll(object sender, EventArgs e)
        {
            point2.Power = tbGraviton2.Value;
        }

        private void tbSpreading_Scroll(object sender, EventArgs e)
        {
            emitter.Spreading = tbSpreading.Value;
        }

        private void tbRadius_Scroll(object sender, EventArgs e)
        {
            emitter.CircularRadius = tbRadius.Value;
        }

        private void tbRotationSpeed_Scroll(object sender, EventArgs e)
        {
            emitter.RotationSpeed = tbRotationSpeed.Value; 
        }

        private void tbParticlesPerTick_Scroll(object sender, EventArgs e)
        {
            emitter.ParticlesPerTick = tbParticlesPerTick.Value;
        }

        private void picDisplay_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                teleport.X = e.X;
                teleport.Y = e.Y;
            } else if (e.Button == MouseButtons.Right)
            {
                teleport.TargetX = e.X;
                teleport.TargetY = e.Y;
            }
        }
    }
}
