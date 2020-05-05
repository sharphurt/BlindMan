﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using BlindMan.Domain;

namespace BlindMan.View.Controls
{
    public class GameControl : BaseControl
    {
        Images images = new Images();
        private int fps;

        public GameControl(GameModel gameModel) : base(gameModel)
        {
            images.Load();

            StartGameUpdaterTimer(gameModel);
            StartFpsCounterTimer();
            
            BackColor = Color.Black;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            gameModel.KeyDown(keyData);
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Draw(Graphics graphics)
        {
            var lab = gameModel.Labyrinth;
            
            for (var i = 0; i < lab.Height; i++)
            {
                for (var j = 0; j < lab.Width; j++)
                {
                    if (IsVisibleByPlayer(new Point(j, i)))
                    {
                        if (lab.Labyrinth[i, j] == LabyrinthModel.LabyrinthElements.WALL)
                        {
                            graphics.FillRectangle(Brushes.MidnightBlue, j * 40, i * 40, 40, 40);
                        }
                        else
                        {
                            graphics.FillRectangle(Brushes.Beige, j * 40, i * 40, 40, 40);
                        }
                    }
                }
            }
            if (IsVisibleByPlayer(lab.ExitPosition))
                graphics.DrawImage(images.ClosedDoor, lab.ExitPosition.X * 40, lab.ExitPosition.Y * 40, 40, 40);
            if (IsVisibleByPlayer(lab.KeyPosition))
                graphics.DrawImage(images.Key, lab.KeyPosition.X * 40, lab.KeyPosition.Y * 40, 40, 40);
            
            graphics.DrawEntity(images.Player, gameModel.Player);
        }

        private bool IsVisibleByPlayer(Point objectPosition)
        {
            var player = gameModel.Player;
            if (Math.Pow(Math.Abs(player.labyrinthPosition.X - objectPosition.X), 2) +
                Math.Pow(Math.Abs(player.labyrinthPosition.Y - objectPosition.Y), 2) < player.Vision)
                return true;
            return false;
        }

        private void StartGameUpdaterTimer(GameModel model)
        {
            var deltaTimeWatch = new Stopwatch();
            deltaTimeWatch.Start();
            var updateTimer = new Timer();
            updateTimer.Interval = 1000 / GameSettings.GameTPS;
            updateTimer.Tick += (sender, args) =>
            {
                var deltaTime = (float) deltaTimeWatch.Elapsed.TotalSeconds;
                model.Update(deltaTime);
                deltaTimeWatch.Restart();
                Invalidate();
            };
            updateTimer.Start();
        }

        private void StartFpsCounterTimer()
        {
            var fpsTimer = new Timer();
            fpsTimer.Interval = 1000;
            fpsTimer.Tick += (sender, args) =>
            {
                Parent.Text = GameSettings.GameName + "   [FPS: " + fps + "]";
                fps = 0;
            };
            fpsTimer.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            fps++;
            var graphics = e.Graphics;

            var graphicsState = graphics.Save();
            
            var widthRatio = ClientSize.Width / (float) GameSettings.GameWidth;
            var heightRatio = ClientSize.Height / (float) GameSettings.GameHeight;
            
            graphics.ScaleTransform(widthRatio, heightRatio);
            
            Draw(graphics);
            
            graphics.Restore(graphicsState);
        }
    }
}