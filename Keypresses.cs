using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InputManager;
using System.Threading;
using System.Windows.Forms;
using ColdClear;

namespace tetriobrowser
{
    class Keypresses
    {
        public static int delay = 40;
        public static int delay2 = 0;
        //public static int holddelay = 1000;
        //public static bool legitM = false;

        public Keypresses()
        {

        }

        public static void executeInstructions(Move move)
        {
            if (Form1.legitmode)
            {
                Random rnd = new Random();

                dice = rnd.Next(25, 80); // creates a number between 1 and 5
                delay = rnd.Next(30,50);
                delay2 = rnd.Next(0, 100);
            }
            else
            {
                dice = 0;
                
                delay2 = 0;
            }

            if (move.Hold)
            {
                HoldPiece();
            }

            foreach (var i in move.Movements)
            {
                Console.WriteLine("pressing: " + i.ToString());
                switch (i.ToString())
                {
                    case "0":
                        SendLeft();
                        continue;
                    case "1":
                        SendRight();
                        continue;
                    case "2":
                        RotateClock();
                        continue;
                    case "3":
                        RotateCounterClock();
                        continue;
                    case "4":
                        HoldDown();
                        continue;
                }

                Thread.Sleep(dice);
            }
            
            Thread.Sleep(delay2);
            SendDrop();


        }
        public static void SendRight()
        {

            Keyboard.KeyDown(Keys.Right);
            Thread.Sleep(delay);
            Keyboard.KeyUp(Keys.Right);
            Thread.Sleep(delay);

        }
        public static void SendLeft()
        {
            Keyboard.KeyDown(Keys.Left);
            Thread.Sleep(delay);
            Keyboard.KeyUp(Keys.Left);
            Thread.Sleep(delay);
        }

        public static void HoldLeft()
        {
            for (int i = 0; i < 7; i++)
            {
                SendLeft();
            }
            /*Keyboard.KeyDown(Keys.Left);
            Thread.Sleep(200);
            Keyboard.KeyUp(Keys.Left);
            Thread.Sleep(delay);*/


        }

        public static void HoldRight()
        {
            for (int i = 0; i < 7; i++)
            {
                SendRight();
            }
            /*Keyboard.KeyDown(Keys.Right);
            Thread.Sleep(200);
            Keyboard.KeyUp(Keys.Right);
            Thread.Sleep(delay);*/

        }
        public static void SendDown()
        {
            Keyboard.KeyDown(Keys.Down);
            Thread.Sleep(delay);
            Keyboard.KeyUp(Keys.Down);
            Thread.Sleep(delay);

        }

        public static void HoldDown()
        {
            Keyboard.KeyDown(Keys.Down);
            Thread.Sleep(40);
            Keyboard.KeyUp(Keys.Down);
            Thread.Sleep(delay);

        }
        public static int dice = 0;

        //Form1.legit
        public static void RotateCounterClock()
        {

            Thread.Sleep(dice);

            Keyboard.KeyDown(Keys.Y);
            Thread.Sleep(delay + dice);
            Keyboard.KeyUp(Keys.Y);
            Thread.Sleep(delay + dice);


        }
        public static void RotateClock()
        {

            Thread.Sleep(dice);
            Keyboard.KeyDown(Keys.X);
            Thread.Sleep(delay + dice);
            Keyboard.KeyUp(Keys.X);
            Thread.Sleep(delay + dice);


        }
        public static void SendDrop()
        {
            Keyboard.KeyDown(Keys.Space);
            Thread.Sleep(delay);

            Keyboard.KeyUp(Keys.Space);
            Thread.Sleep(delay);



        }
        public static void HoldPiece()
        {

            Keyboard.KeyDown(Keys.LShiftKey);
            Thread.Sleep(delay);
            Keyboard.KeyUp(Keys.LShiftKey);
            Thread.Sleep(delay);


        }


    }

}


