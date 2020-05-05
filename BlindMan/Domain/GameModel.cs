﻿using System;
using System.Diagnostics;
using System.Windows.Forms;
using BlindMan.Entities;

namespace BlindMan.Domain
{
    public class GameModel
    {
        public Player Player { get; private set; }

        private GameState _gameState;
        public GameState GameState
        {
            get => _gameState;
            set
            {
                _gameState = value;
                GameStateChanged?.Invoke(value);
            }
        }

        public event Action<GameState> GameStateChanged;
        
        public event Action LeftKeyDown;
        public event Action RightKeyDown;
        public event Action UpKeyDown;
        public event Action DownKeyDown;

        public LabyrinthModel Labyrinth;

        public string GameTime =>
            $"{stopwatch.Elapsed.Minutes:d2}:" +
            $"{stopwatch.Elapsed.Seconds:d2}:" +
            $"{(stopwatch.Elapsed.Milliseconds / 10):d2}";

        private Stopwatch stopwatch = new Stopwatch();

        public void StartGame()
        {
            stopwatch.Reset();
            stopwatch.Start();
            Labyrinth = new LabyrinthGenerator().CreateLabyrinth(31, 17);
            Player = new Player(Labyrinth.PlayerPosition.X, Labyrinth.PlayerPosition.Y, 40, 40, this);
        }

        public void EndGame()
        {
            stopwatch.Stop();
            LeftKeyDown = () => { };
            RightKeyDown = () => { };
            UpKeyDown = () => { };
            DownKeyDown = () => { };
            GameState = GameState.GameWon;
        }
        
        public void Update(float deltaTime)
        {
            Player.Update(deltaTime);
        }

        public void KeyDown(Keys key)
        {
            switch (key)
            {
                case Keys.A:
                case Keys.Left:
                    LeftKeyDown?.Invoke();
                    break;
                case Keys.D:
                case Keys.Right:
                    RightKeyDown?.Invoke();
                    break;
                case Keys.W:
                case Keys.Up:
                    UpKeyDown?.Invoke();
                    break;
                case Keys.S:
                case Keys.Down:
                    DownKeyDown?.Invoke();
                    break;
                case Keys.Space:
                    GameState = GameState.GameWon;
                    Labyrinth = new LabyrinthGenerator().CreateLabyrinth(31, 17);
                    Player = new Player(Labyrinth.PlayerPosition.X, Labyrinth.PlayerPosition.Y, 40, 40, this);
                    break;
            }
        }
    }
}