using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UlimateSnakeGame
{
    public partial class Form1 : Form
    {
        // List Array for snake
        private List<Circle> Snake = new List<Circle>();
        // Circle object that will be the food
        private Circle food = new Circle();
        private int highScore;

        public Form1()
        {
            InitializeComponent();
            // Settings Class to form
            new Settings();
            setDefaultGameSpeed();
            if (highScore >= Settings.Score)
            {
                string tempScore = setHighScore();
                highScore = Int32.Parse(tempScore);
            }
            
            label5.Text = setHighScore();
            startGame();
        }

        private void setDefaultGameSpeed()
        {
            // Set game timer to settings speed
            gameplayTimer.Interval = 1000 / Settings.Speed;
            gameplayTimer.Tick += updateScreen;
            gameplayTimer.Start();
        }

        private void startGame()
        {
            label3.Visible = false;
            new Settings();
            Snake.Clear();
            Circle head = new Circle { X = 10, Y = 5 };
            Snake.Add(head);

            label2.Text = Settings.Score.ToString();
            generateFood();
        }

        private void updateScreen(object sender, EventArgs e)
        {
            if(Settings.GameOver == true)
            {
                if(Input.KeyPressed(Keys.Enter))
                {
                    startGame();
                }
            }
            else
            {
                if(Input.KeyPressed(Keys.Right) && Settings.direction != Directions.Left)
                {
                    Settings.direction = Directions.Right;
                }
                else if(Input.KeyPressed(Keys.Left) && Settings.direction != Directions.Right)
                {
                    Settings.direction = Directions.Left;
                }
                else if (Input.KeyPressed(Keys.Up) && Settings.direction != Directions.Down)
                {
                    Settings.direction = Directions.Up;
                }
                else if (Input.KeyPressed(Keys.Down) && Settings.direction != Directions.Up)
                {
                    Settings.direction = Directions.Down;
                }
                movePlayer();
            }
            pbCanvas.Invalidate();
        }

        private void movePlayer()
        {
            for (int i = Snake.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    switch (Settings.direction)
                    {
                        case Directions.Right:
                            Snake[i].X++;
                            break;
                        case Directions.Left:
                            Snake[i].X--;
                            break;
                        case Directions.Up:
                            Snake[i].Y--;
                            break;
                        case Directions.Down:
                            Snake[i].Y++;
                            break;
                        default:
                            break;
                    }

                    // Bind Snake to Picture box canvas
                    int maxXpos = pbCanvas.Size.Width / Settings.Width;
                    int maxYpos = pbCanvas.Size.Width / Settings.Height;

                    if (Snake[i].X < 0 || Snake[i].Y < 0 ||
                        Snake[i].X > maxXpos || Snake[i].Y > maxYpos)
                    {
                        // Snake has reached edge
                        die();
                    }

                    // Snake body collision detection
                    for (int j = 1; j < Snake.Count; j++)
                    {
                        if (Snake[i].X == Snake[j].X && Snake[i].Y == Snake[j].Y)
                        {
                            die();
                        }
                    }

                    // Detect snake food collection
                    if (Snake[0].X == food.X && Snake[0].Y == food.Y)
                    {
                        eat();
                    }
                }
                else
                {
                    Snake[i].X = Snake[i - 1].X;
                    Snake[i].Y = Snake[i - 1].Y;
                }
            }
        }

        /*public void changeSpeed(int score)
        {
            score = Settings.Score;
            if (score % 50 == 0)
            {
                int count = 2000;
                count += count;
                Settings.Speed += 1;
                gameplayTimer.Interval = count / Settings.Speed;
                gameplayTimer.Tick += updateScreen;
                gameplayTimer.Start();
            }
            // Debug Speed
            Console.WriteLine(Settings.Speed);
            Console.WriteLine(gameplayTimer);
        }*/
        

        private void die()
        {
            Settings.GameOver = true;
        }

        private void eat()
        {
            Circle body = new Circle
            {
                X = Snake[Snake.Count - 1].X,
                Y = Snake[Snake.Count - 1].Y
            };

            Snake.Add(body);
            Settings.Score += Settings.Points;
            label2.Text = Settings.Score.ToString();
            generateFood();
           //changeSpeed(Settings.Score);
        }

        private void generateFood()
        {
            int maxXpos = pbCanvas.Size.Width / Settings.Width;
            int maxYpos = pbCanvas.Size.Height / Settings.Height;

            Random random = new Random();
            food = new Circle { X = random.Next(0, maxXpos), Y = random.Next(0, maxYpos) };
        }

        private void keyisdown(object sender, KeyEventArgs e)
        {
            Input.changeState(e.KeyCode, true);
        }

        private void keyisup(object sender, KeyEventArgs e)
        {
            Input.changeState(e.KeyCode, false);
        }

        private void updateGraphics(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            if(Settings.GameOver == false)
            {
                Brush snakeColour;
                for (int i = 0; i < Snake.Count; i++)
                {
                    if (i == 0)
                    {
                        snakeColour = Brushes.Black;
                    }
                    else
                    {
                        snakeColour = Brushes.Green;
                    }
                    canvas.FillEllipse(snakeColour,
                        new Rectangle(
                            Snake[i].X * Settings.Width,
                            Snake[i].Y * Settings.Height,
                            Settings.Width, Settings.Height
                            ));

                    canvas.FillEllipse(Brushes.Red,
                        new Rectangle(
                            food.X * Settings.Width,
                            food.Y * Settings.Height,
                            Settings.Width, Settings.Height
                            ));
                }
            }
            else
            {
                string gameOver = "Game Over \n" + "Final Score: " + Settings.Score + "\nPress Enter to Restart\n";
                label3.Text = gameOver;
                checkHighScore();
                label3.Visible = true;
            }
        }

        private void checkHighScore()
        {
            if (Settings.Score >= highScore)
            {
                highScore = Settings.Score;
                label5.Text = highScore.ToString();
                using (StreamWriter writer =
                    new StreamWriter(
                        "C:\\Users\\Opatile\\Downloads\\UlimateSnakeGame\\UlimateSnakeGame\\assets\\highscore.txt"))
                {
                    writer.WriteLine(highScore.ToString());
                    writer.Close();
                }
            }
        }

        public string setHighScore()
        {
            string readText = File.ReadAllText("C:\\Users\\Opatile\\Downloads\\UlimateSnakeGame\\UlimateSnakeGame\\assets\\highscore.txt");
            return readText;
        }


        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}


