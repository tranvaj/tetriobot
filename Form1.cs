using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Xml;
using System.Diagnostics;
using ColdClear;
using System.Threading;
using CefSharp.Handler;
using System.IO;
using System.Reflection;
using System.Net;

namespace tetriobrowser
{
    public partial class Form1 : Form
    {
        public ChromiumWebBrowser chromeBrowser;
        
        //game data
        public static bool[,] tetrioArray = new bool[40, 10];
        public static bool[,] ccArray = new bool[40, 10];
        public static int b2b;
        public static int upcomingGb;
        public static bool playing;
        public static bool gamelocked;
        public static CCPiece hold;
        public static uint combo;

        public static string[] stringPieces;
        public static CCPiece[] CCpieces;
        public static bool initCC;
        Form2 overlay;

       
        bool devTools = false;
        Bot cc;
        public Form1()
        {
            InitializeComponent();
            Init();
            textBox1.Text = "100";
            textBox2.Text = "40";
            //string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string path = Path.Combine("file:///" + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "test.js");

            textBox3.Text = path;
            KeyboardIntercept keyboardIntercept = new KeyboardIntercept();
            keyboardIntercept.KeyIntercepted += new EventHandler<KeyEventArgs>(Form1_KeyDown);
            textBox4.Text = pixelSize.ToString();
            
            //InitCC();
            //timer1.Start();
        }
        
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }
        
        private void InitCC()
        {
            
            
            
            label5.Text = "starting";

        }

        private void DisableProgram()
        {
            stop = true;
            initCC = true;
            gamelocked = true;
            playing = false;
            isRunning = false;
            plannedLocations2 = new string[40, 10];
            botpcplaced = 0;
            p2pcplaced = 0;
            //timer1.Stop();
            Console.WriteLine("disablingprogram");
        }
        private void Init() {
            gamelocked = true;
            playing = false;
           overlay = new Form2();
            //overlay.Show();
            CefSettings settings = new CefSettings();
            
            //settings.LogSeverity = LogSeverity.Disable;
            settings.CachePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\CEF";

            Cef.Initialize(settings);
            chromeBrowser = new ChromiumWebBrowser("https://tetr.io");
            //chromeBrowser.RequestHandler = new CustomRequestHandler(); 
            //var x = new RequestContext();

            //Controls.Add(browser);
            this.panel1.Controls.Add(chromeBrowser);
            chromeBrowser.Dock = DockStyle.Fill;
            chromeBrowser.IsBrowserInitializedChanged += ChromeBrowser_IsBrowserInitializedChanged;
            //chromeBrowser.RenderProcessMessageHandler = new IRenderProcessMessageHandler();
        
        }

        private void ChromeBrowser_IsBrowserInitializedChanged(object sender, EventArgs e)
        {
            devTools = true;

        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cef.Shutdown();
        }

        private void gettingPieces(Bot cc)
        {
            
        }
        static string GetLocation(byte[] x, byte[] y)
        {
            return $"{x[0]},{y[0]} {x[1]},{y[1]} {x[2]},{y[2]} {x[3]},{y[3]}";
        }
        public  bool misdropdetect(MovePlans mp)
        {
            if (!(trainerOn))
            {
                int count = 0;
                var plan = mp.Plans[0];

                for (int j = 0; j < 4; j++)
                {
                    int cl = plan.ClearedLines[j];
                    if (cl == -1)
                    {
                        count++;
                    }

                }
                if (count != 4)
                {
                    return false;
                }
            }
                
            

            //int cd = 0;
            for (int i = 0; i < 4; i++)
            {
                
                if(ccArray[mp.Move.ExpectedY[i], mp.Move.ExpectedX[i]] == true)
                {
                    //cd++;
                    //setLabel11TextSafe(Print2DArraybool(ccArray));
                    //return false;
                }
                else
                {
                    Console.WriteLine($"misdrop - not filled: {mp.Move.ExpectedY[i]}, {mp.Move.ExpectedX[i]}");
                    setLabel8TextSafe("misdrop");
                    return true;
                    
                }
            }
            //setLabel10TextSafe(cd.ToString());

            return false;

        }
        public static bool[,] plannedLocations = new bool[40, 10];
        public static string[,] plannedLocations2 = new string[40, 10];

        private void setLabel1TextSafe(string txt)
        {
            if (richTextBox1.InvokeRequired)
                richTextBox1.Invoke(new Action(() => richTextBox1.Text = txt));
            else
                richTextBox1.Text = txt;
        }
        private void setLabel8TextSafe(string txt)
        {
            if (label8.InvokeRequired)
                label8.Invoke(new Action(() => label8.Text = txt));
            else
                label8.Text = txt;
        }
        private void setLabel11TextSafe(string txt)
        {
            if (label1.InvokeRequired)
                label1.Invoke(new Action(() => label1.Text = txt));
            else
                label1.Text = txt;
        }
        private void setLabel10TextSafe(string txt)
        {
            if (label10.InvokeRequired)
                label10.Invoke(new Action(() => label10.Text = txt));
            else
                label10.Text = txt;
        }
        public static bool lineclear = false;
        public void PrintMovePlan(MovePlans mp)
        {
            for(int i = 0; i < 40; i++)
            {
                for(int x = 0; x < 10; x++)
                {
                    plannedLocations2[i,x] = "";
                }
            }
            string report = "";
            Console.WriteLine($"Node: {mp.Move.Nodes}, Depth: {mp.Move.Depth}, Rank: {mp.Move.OriginalRank}");
            report += $"\nNode: {mp.Move.Nodes}, Depth: {mp.Move.Depth}, Rank: {mp.Move.OriginalRank}";
            string s = "";
            foreach(var i in mp.Move.Movements)
            {
                s = s + i + ",";
            }
            Console.WriteLine($"Moves: {s}");
            report += $"\nMoves: {s}";
            Console.WriteLine($"UseHold: {mp.Move.Hold}");
            report += $"\nUseHold: {mp.Move.Hold}";
            report += $"\nLocation: {GetLocation(mp.Move.ExpectedX, mp.Move.ExpectedY)}";
            Console.WriteLine($"Location: {GetLocation(mp.Move.ExpectedX, mp.Move.ExpectedY)}");
            /*for (int i = 0; i < 4; i++)
            {
                plannedLocations2[mp.Move.ExpectedY[i], mp.Move.ExpectedX[i]] = mp.P;

            }*/
            Console.WriteLine($"Plans: {mp.Plans.Length}");
            int id = 0;
            foreach (var plan in mp.Plans)
            {
                Console.WriteLine($"Piece: {plan.Piece}, Tspin: {plan.TSpin}");
                report += $"\nPiece: {plan.Piece}, Tspin: {plan.TSpin}";
                report += $"\nLocation: {GetLocation(plan.ExpectedX, plan.ExpectedY)}";
                Console.WriteLine($"Location: {GetLocation(plan.ExpectedX, plan.ExpectedY)}");
                if (id <= 1)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if(id == 0)
                        {
                            plannedLocations2[plan.ExpectedY[i], plan.ExpectedX[i]] = "current";
                            continue;


                        }
                        
                        plannedLocations2[plan.ExpectedY[i], plan.ExpectedX[i]] = plan.Piece.ToString().Replace("CC_", "");

                    }
                }
                
                for (int j = 0; j < 4; j++)
                {
                    int cl = plan.ClearedLines[j];
                    if (cl == -1) {
                       // lineclear = true;
                        break;
                    }
                    report += $"\nCleared line: {cl}";
                    Console.WriteLine($"Cleared line: {cl}");
                }
                id++;

            }
            setLabel1TextSafe(report);

        }
        public static CCPiece spawn;
        public static CCPiece[] CCpieces2 = new CCPiece[7];
        public static int delay;
        void ccstarting(CancellationToken token2)
        {
            //await getData();
            if (gamelocked == false && playing == true)
            {
                if (!(Int32.TryParse(textBox1.Text, out delay)))
                {
                    MessageBox.Show("Wrong value input.");

                }
                if (!(Int32.TryParse(textBox2.Text, out Keypresses.delay)))
                {
                    MessageBox.Show("Wrong value input.");

                }
                Task t = getbagrewrite();
                t.Wait();
                
                //label5.Text = "started";
                //CCMovementMode mode = "CC_0G";
                initCC = true;
                var options = CCOptions.Default();
                 options.pcloop = false;
                options.spawn_rule = CCSpawnRule.CC_ROW_21_AND_FALL;
                options.threads = 2;
                //options.min_nodes = 10;
                options.max_nodes = 500000;
                options.speculate = speculation;
                //options.mode = CCMovementMode.CC_HARD_DROP_ONLY
                var weights = CCWeights.Default();
                weights.perfect_clear = 100;
                if (pcpriority)
                {
                    weights.perfect_clear = 999;
                    options.pcloop = true;
                }
                weights.combo_garbage = 300;
                //NextGenerator g = new NextGenerator();
                CCPieceBag bag = CCPieceBag.Full();
                Task t3;
                t3 = getData();
                t3.Wait();
                Console.WriteLine("starting");
                //timer1.Start();


                //cc = new Bot(options, weights, Converter(ccArray), bag, hold, hasBtb(b2b), combo);
                using (var cc = new Bot(options, weights, Converter(ccArray), bag, hold, hasBtb(b2b), combo)) {
                    for (int i = 0; i < CCpieces.Length; i++)
                    {
                        cc.AddNextPiece(CCpieces[i]);
                        Console.WriteLine(CCpieces[i].ToString());

                    }
                    while (true)
                    {
                        int pieceplaceholder = 0;
                        Task gameover_check = gameoverCheck();
                        gameover_check.Wait();
                        //Task t2 = getBag();
                        //t2.Wait();
                        
                        Task t4 = getData();
                        t4.Wait();
                        if (gameover)
                        {
                            DisableProgram();
                            return;
                        }
                        if (stop)
                        {
                            DisableProgram();
                            Console.WriteLine("stopped");
                            return;
                        }
                        if (trainerOn)
                        {
                            Task h2 = getPiecesPlacedBot();
                            h2.Wait();
                            pieceplaceholder = botpcplaced;
                        }
                        /*Task t2 = getFutureBag();
                        t2.Wait();
                        if (addPieces)
                        {
                            addPieces = false;
                            for (int i = 0; i < CCpieces.Length; i++)
                            {
                                cc.AddNextPiece(CCpieces[i]);
                                Console.WriteLine(CCpieces[i].ToString());

                            }
                        }*/
                        Task t23 = overlimitbag();
                        t23.Wait();
                        if (!overlimit)
                        {
                            Task t22 = generatebag();
                            t22.Wait();


                            for (int i = 0; i < generatedbag.Length; i++)
                            {
                                cc.AddNextPiece(generatedbag[i]);
                                Console.WriteLine(generatedbag[i].ToString());

                            }
                        }
                        Console.WriteLine("overlimit: " + overlimit);

                        //CCpieces = generatedbag;

                        /*if (!(CCpieces.SequenceEqual(CCpieces2)))
                        {
                            //Task future = getFutureBag();
                            //future.Wait();
                            for (int i = 0; i < CCpieces.Length; i++)
                            {
                                cc.AddNextPiece(CCpieces[i]);
                                Console.WriteLine(CCpieces[i].ToString());

                            }
                        }*/

                        //CCpieces2 = CCpieces;

                        /*if (initCC)
                        {
                            cc.AddNextPiece(spawn);
                            Console.WriteLine("spawnpiece:" + spawn);
                            for (int i = 0; i < 6; i++)
                            {
                                cc.AddNextPiece(CCpieces[i]);
                                Console.WriteLine(CCpieces[i].ToString());

                            }
                            initCC = false;
                        }
                        else
                        {
                            if (CCpieces.Length == 12)
                            {
                                for (int i = 5; i < 12; i++)
                                {
                                    cc.AddNextPiece(CCpieces[i]);
                                }
                            }
                        }*/
                        try
                        {
                            Task upcomingTsk = getUpcoming();
                            upcomingTsk.Wait();
                            MovePlans mp = cc.GetNextMoveAndPlans(upcomingGb);
                            int lol = upcomingGb;
                            
                            PrintMovePlan(mp);
                            
                            Console.WriteLine("placing \n");
                            int totalblocks = 0;
                            int totalblocks2 = 0;
                            foreach (var x in ccArray)
                            {
                                if (x)
                                {
                                    totalblocks++;
                                }
                            }


                            if (piece2piece)
                            {
                                Task h = getPiecesPlaced();
                                h.Wait();
                                pieceplaceholder = botpcplaced;
                                if (mp.Move.Hold)
                                {
                                    Keypresses.HoldPiece();
                                }
                                while (botpcplaced >= p2pcplaced)
                                {

                                    if (gameover)
                                    {
                                        DisableProgram();
                                        return;
                                    }
                                    if (stop)
                                    {
                                        DisableProgram();
                                        Console.WriteLine("stopped");
                                        return;
                                    }
                                    Task h2 = getPiecesPlaced();
                                    h2.Wait();
                                    Thread.Sleep(10);
                                    Console.WriteLine(botpcplaced + " vs player - " + p2pcplaced);
                                    if(botpcplaced > pieceplaceholder)
                                    {
                                        goto end_of_loop;
                                    }
                                }
                                Console.WriteLine(botpcplaced + " vs player - " + p2pcplaced + ":: executing moves");
                                executeMoves(mp.Move);
                            }
                            else if(trainerOn)
                            {
                                if (mp.Move.Hold)
                                {

                                    Keypresses.HoldPiece();
                                }
                                overlay.setMatrix(RotateMatrixCounterClockwiseString(RotateMatrixCounterClockwiseString(RotateMatrixCounterClockwiseString(plannedLocations2))));
                                overlay.Invalidate();

                                
                                
                                while (botpcplaced != (pieceplaceholder + 1) )
                                {
                                    Task h3 = getPiecesPlacedBot();
                                    h3.Wait();
                                    Thread.Sleep(40);
                                    if (gameover)
                                    {
                                        DisableProgram();
                                        return;
                                    }
                                    if (stop)
                                    {
                                        DisableProgram();
                                        Console.WriteLine("stopped");
                                        return;
                                    }

                                }
                                
                            }
                            else
                            {
                                //overlay.setMatrix(RotateMatrixCounterClockwiseString(RotateMatrixCounterClockwiseString(RotateMatrixCounterClockwiseString(plannedLocations2))));
                                //overlay.Invalidate();
                                executeMoves(mp.Move);

                            }

                            

                            //setLabel9TextSafe(Print2DArray(FlipArrayString(RotateMatrixCounterClockwiseString(RotateMatrixCounterClockwiseString(plannedLocations2)))));
                            end_of_loop:
                            int delay2 = 0;
                            if (legitmode)
                            {
                                Random rnd = new Random();
                                delay2 = rnd.Next(0, delay);

                            } else
                            {
                                delay2 = delay;
                            }
                            Thread.Sleep(delay2+10);
                            Task t5 = getData();
                            t5.Wait();
                            foreach (var x in ccArray)
                            {
                                if (x)
                                {
                                    totalblocks2++;
                                }
                            }
                            if (lol > 0 || misdropdetect(mp) || totalblocks2 > (totalblocks + 4))
                            {
                                //lol = false;
                                Console.WriteLine($"misdrop or garbage received: ");
                                setLabel8TextSafe("MISDROP");
                                cc.Reset(Converter(ccArray), hasBtb(b2b), combo);
                                //Thread.Sleep(1);
                            }
                            else
                            {
                                setLabel8TextSafe("no misdrop");
                            }
                            //Thread.Sleep(delay);
                        }
                        catch (Exception es)
                        {
                            Console.WriteLine("dead");
                            DisableProgram();
                            return;

                        }

                    }
                }
                    

            }
            else
            {
                Console.Write("errorrr");
               
            }
        }

        public static void executeMoves(Move mp)
        {
            Keypresses.executeInstructions(mp);
        }
        public static Boolean hasBtb(int b2b)
        {
            if(b2b > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static string Print2DArraybool(bool[,] smth)
        {
            bool[,] matrix = smth;
            string lol = "";
            for (int i = 0; i < 40; i++)
            {
                //lol += matrix[i, j] + "\t";
                for (int j = 0; j < 10; j++)
                {
                    if (matrix[i, j])
                    {
                        lol += 1;

                    }
                    else
                    {
                        lol += 0;
                    }
                }
                lol += "\n";

            }
            return lol;
        }
        public static string Print2DArray(string[,] smth)
        {
            string[,] matrix = new string[40,10];
            string lol = "";
            for (int i = 0; i < 40; i++)
            {
                //lol += matrix[i, j] + "\t";
                for (int j = 0; j < 10; j++)
                {
                    if (!(String.IsNullOrEmpty(smth[i, j])))
                    {
                        lol += smth[i, j];

                    }
                    else
                    {
                        lol += " ";
                    }
                }
                lol += "\n";

            }
            return lol;
        }
        public static bool[] Converter(bool[,] intfield)
        {
            if (!(intfield.GetLength(0) == 40 && intfield.GetLength(1) == 10))
                throw new IndexOutOfRangeException("intfield is not int[40, 10]");

            bool[] boolfield = new bool[400];
            int count = 0;

            for (int i = 0; i < intfield.GetLength(0); i++)
            {
                for (int j = 0; j < intfield.GetLength(1); j++)
                {
                    boolfield[count] = intfield[i, j] == true ? true : false;
                    count++;
                }
            }

            return boolfield;
        }
        public static bool[,] RotateMatrixCounterClockwise(bool[,] oldMatrix)
        {
            bool[,] newMatrix = new bool[oldMatrix.GetLength(1), oldMatrix.GetLength(0)];
            int newColumn, newRow = 0;
            for (int oldColumn = oldMatrix.GetLength(1) - 1; oldColumn >= 0; oldColumn--)
            {
                newColumn = 0;
                for (int oldRow = 0; oldRow < oldMatrix.GetLength(0); oldRow++)
                {
                    newMatrix[newRow, newColumn] = oldMatrix[oldRow, oldColumn];
                    newColumn++;
                }
                newRow++;
            }
            return newMatrix;
        }

        public static string[,] RotateMatrixCounterClockwiseString(string[,] oldMatrix)
        {
            string[,] newMatrix = new string[oldMatrix.GetLength(1), oldMatrix.GetLength(0)];
            int newColumn, newRow = 0;
            for (int oldColumn = oldMatrix.GetLength(1) - 1; oldColumn >= 0; oldColumn--)
            {
                newColumn = 0;
                for (int oldRow = 0; oldRow < oldMatrix.GetLength(0); oldRow++)
                {
                    newMatrix[newRow, newColumn] = oldMatrix[oldRow, oldColumn];
                    newColumn++;
                }
                newRow++;
            }
            return newMatrix;
        }

        public static bool[,] FlipArray(bool[,] tetrisField)
        {
            bool[,] arrayToFlip = tetrisField;
            int rows = arrayToFlip.GetLength(0);
            int columns = arrayToFlip.GetLength(1);
            bool[,] flippedArray = new bool[rows, columns];
            bool temp;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    temp = arrayToFlip[i, j];

                    flippedArray[i, j] = arrayToFlip[i, columns - j - 1];
                    flippedArray[i, columns - j - 1] = temp;
                }
            }
            return flippedArray;
        }
        public static string[,] FlipArrayString(string[,] tetrisField)
        {
            string[,] arrayToFlip = tetrisField;
            int rows = arrayToFlip.GetLength(0);
            int columns = arrayToFlip.GetLength(1);
            string[,] flippedArray = new string[rows, columns];
            string temp;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    temp = arrayToFlip[i, j];

                    flippedArray[i, j] = arrayToFlip[i, columns - j - 1];
                    flippedArray[i, columns - j - 1] = temp;
                }
            }
            return flippedArray;
        }
        public static CCPiece convertStringToCC(string a)
        {
            switch (a.ToUpper())
            {
                case "S":
                    return CCPiece.CC_S;
                case "O":
                    return CCPiece.CC_O;
                case "Z":
                    return CCPiece.CC_Z;
                case "L":
                    return CCPiece.CC_L;
                case "J":
                    return CCPiece.CC_J;
                case "T":
                    return CCPiece.CC_T;
                case "I":
                    return CCPiece.CC_I;
                default:
                    
                    Console.WriteLine("unknownpiece:" + a);
                    return CCPiece.CC_None;
            }
        }
        public static CCPiece[] arrToCC()
        {
            CCPiece[] cCPieces = new CCPiece[stringPieces.Length];
            for(int i = 0; i < stringPieces.Length; i++)
            {
                cCPieces[i] = convertStringToCC(stringPieces[i]);
                
            }
            return cCPieces;
        }
        public static CCPiece[] arrToCC2(string[] yea)
        {
            CCPiece[] cCPieces = new CCPiece[yea.Length];
            for (int i = 0; i < yea.Length; i++)
            {
                cCPieces[i] = convertStringToCC(yea[i]);

            }
            return cCPieces;
        }
        public static void processBag(string bag)
        {
            
            
        }
        public static void processBoard(List<List<string>> results)
        {
            int i = 0;

            foreach (var item in results)
            {
                int j = 0;
                foreach (var x in item)
                {
                    //x == "s" || x == "i" || x == "j" || x == "o" || x=="t" || x == "l" || x == "z"
                    if (x != null)
                    {
                        tetrioArray[i, j] = true;

                    }
                    else
                    {
                        tetrioArray[i, j] = false;
                    }
                    j++;
                }
                i++;
                //Console.WriteLine(string.Join(", ", item));
            }
            arrayintoCC();
            
        }
        public static void arrayintoCC()
        {
            bool[,] flippedTetris = FlipArray(RotateMatrixCounterClockwise(RotateMatrixCounterClockwise(tetrioArray)));
            ccArray = flippedTetris;
        }
        public static int queueSize = 5;
        public static bool addPieces = false;
        private async Task getFutureBag()
        {
            if (chromeBrowser.CanExecuteJavascriptInMainFrame)
            {
                int bagLengthth = 0 ;
                    JavascriptResponse bagLength = await chromeBrowser.EvaluateScriptAsync("lol.export().game.bag.toString()");
                    if (bagLength.Result != null)
                    {
                        string jaa = bagLength.Result.ToString();
                        string[] nextPieces2 = jaa.Split(',');
                    //int[] ccPieces = new int[nextPieces.Length];
                        bagLengthth = nextPieces2.Length;

                    }
                if (bagLengthth > queueSize * 7)
                {
                    return;
                }
               
                
                    
                    string theArr = "";
                    for (int i = 0; i < queueSize; i++)
                    {
                    JavascriptResponse bag = await chromeBrowser.EvaluateScriptAsync("lol.getBag().toString()");
                    if (bag.Result == null)
                    {
                        return;
                    }

                        theArr += bag.Result.ToString();
                        if(i != (queueSize-1))
                        {
                            theArr += ",";
                        }
                    }
                     string[] nextPieces = theArr.Split(',');

                   
                    stringPieces = nextPieces;
                    Console.WriteLine("bag: " + string.Join(",", stringPieces));

                    CCpieces = arrToCC();
                    addPieces = true;
                

            }
        }
        public static int botpcplaced = 0;
        public static int p2pcplaced = 0;
        private async Task getPiecesPlaced()
        {
                if (chromeBrowser.CanExecuteJavascriptInMainFrame)
            {
                JavascriptResponse piecesPlaced = await chromeBrowser.EvaluateScriptAsync("otherPlayer.export().stats.piecesplaced.toString();");
                if (piecesPlaced.Result != null)
                {
                    
                    int piecesplacednum = Int16.Parse(piecesPlaced.Result.ToString());
                    p2pcplaced = piecesplacednum;
                }
                JavascriptResponse piecesPlaced2 = await chromeBrowser.EvaluateScriptAsync("lol.export().stats.piecesplaced.toString();");
                if (piecesPlaced2.Result != null)
                {
                    //string pieces = piecesPlaced2.Result.ToString();
                    int piecesplacednum = Int16.Parse(piecesPlaced2.Result.ToString());
                    botpcplaced = piecesplacednum;
                }

            }
        }
        private async Task getPiecesPlacedBot()
        {
            if (chromeBrowser.CanExecuteJavascriptInMainFrame)
            {
                
                JavascriptResponse piecesPlaced2 = await chromeBrowser.EvaluateScriptAsync("lol.export().stats.piecesplaced.toString();");
                if (piecesPlaced2.Result != null)
                {
                    //string pieces = piecesPlaced2.Result.ToString();
                    int piecesplacednum = Int32.Parse(piecesPlaced2.Result.ToString());
                    botpcplaced = piecesplacednum;
                }

            }
        }

        private async Task getCurrentPiece()
        {
            if (chromeBrowser.CanExecuteJavascriptInMainFrame)
            {

                JavascriptResponse curr = await chromeBrowser.EvaluateScriptAsync("currentpiece.type;");
                if (curr.Result != null)
                {
                    //string pieces = piecesPlaced2.Result.ToString();
                    string currpiece = curr.Result.ToString();
                    currentpiece = currpiece;
                }

            }
        }
        public static string currentpiece;
        public static bool overlimit = false;
        private async Task generatebag()
        {
            JavascriptResponse bag = await chromeBrowser.EvaluateScriptAsync("lol.getBag().toString()");
            if (bag.Result != null)
            {
                string jaa = bag.Result.ToString();
                string[] nextPieces = jaa.Split(',');
                generatedbag = arrToCC2(nextPieces);
                
            }
           
        }
        private async Task overlimitbag()
        {
            if (chromeBrowser.CanExecuteJavascriptInMainFrame)
            {
                JavascriptResponse bagLength = await chromeBrowser.EvaluateScriptAsync("lol.export().game.bag.toString()");
                if (bagLength.Result != null)
                {
                    string jaa = bagLength.Result.ToString();
                    string[] nextPieces = jaa.Split(',');

                    if (nextPieces.Length > 20)
                    {
                        overlimit = true;
                        Console.WriteLine("overlimit: " + overlimit + nextPieces.Length);
                    }
                    else
                    {
                        overlimit = false;
                    }
                }
            }
        }
        public static CCPiece[] generatedbag;
        private async Task getbagrewrite()
        {
            if (chromeBrowser.CanExecuteJavascriptInMainFrame)
            {
                JavascriptResponse bagLength = await chromeBrowser.EvaluateScriptAsync("lol.export().game.bag.toString()");
                if (bagLength.Result != null)
                {
                    string jaa = bagLength.Result.ToString();
                    string[] nextPieces = jaa.Split(',');
                    //int[] ccPieces = new int[nextPieces.Length];
                    int length = nextPieces.Length;
                    Task t2 = getCurrentPiece();
                    t2.Wait();
                    string[] nextPieces2 = new string[length + 1];
                    nextPieces2[0] = currentpiece;

                    int i = 1;
                   foreach(var x in nextPieces)
                    {
                        nextPieces2[i] = x;
                        i++;
                    }
                    stringPieces = nextPieces2;
                    Console.WriteLine("bag: " + string.Join(",", stringPieces));
                    CCpieces = arrToCC();

                }
            }
        }
        private async Task getBag()
        {
            if (chromeBrowser.CanExecuteJavascriptInMainFrame)
            {
                JavascriptResponse bag = await chromeBrowser.EvaluateScriptAsync("lol.getFirstBag().toString();");
                if (bag.Result != null)
                {
                    string jaa = bag.Result.ToString();
                    string[] nextPieces = jaa.Split(',');
                    //int[] ccPieces = new int[nextPieces.Length];
                    Task t2 = getCurrentPiece();
                    t2.Wait();
                    int i = 0;
                    int length = nextPieces.Length;
                    string[] nextPieces2;
                    //string[] nextPieces3;
                    foreach (var x in nextPieces)
                    {
                        if(x == currentpiece)
                        {
                            nextPieces2 = new string[length-i];
                            int l = 0;
                            for(int j = i; j < length; j++)
                            {
                                nextPieces2[l] = nextPieces[j];
                                l++;
                            }
                            //nextPieces2[0] = nextPieces[i];
                            stringPieces = nextPieces2;

                            break;
                        }
                        i++;
                    }

                    
                    Console.WriteLine("bag: " + string.Join(",", stringPieces));
                    Console.WriteLine("currentpiece: " + currentpiece);

                    CCpieces = arrToCC();

                }
               
            }


        }

        private async Task getUpcoming()
        {
            if (chromeBrowser.CanExecuteJavascriptInMainFrame)
            {

                var upcoming = await chromeBrowser.EvaluateScriptAsync("lol.export().upcoming;");
                if (upcoming.Result != null)
                {
                    upcomingGb = Int16.Parse(upcoming.Result.ToString());
                }


            }
               
        }
        public static bool gameover;
        private async Task gameoverCheck()
        {
            if (chromeBrowser.CanExecuteJavascriptInMainFrame)
            {

                var gameover1 = await chromeBrowser.EvaluateScriptAsync("lol.export().gameoverreason;");
                if (gameover1.Result != null)
                {
                    gameover = true;
                }
                else
                {
                    gameover = false;
                }


            }
        }
        private async Task getData()
        {
            if (chromeBrowser.CanExecuteJavascriptInMainFrame)
            {

                
                
                    
                    
                    
                    //label1.Text = bag.Result.ToString();


                    var username = await chromeBrowser.EvaluateScriptAsync("lol.export().options.username;");
                    if (username.Result != null)
                    {
                        //label2.Text = username.Result.ToString();                      
                    }
                    var board = await chromeBrowser.EvaluateScriptAsync("lol.export().game.board;");
                    if (board.Result != null)
                    {
                        String jsonboard = board.Result.ToString();
                        JArray a = JArray.Parse(jsonboard);
                       // label3.Text = a.ToString();
                        var results = JsonConvert.DeserializeObject<List<List<string>>>(a.ToString());
                        processBoard(results);
                       //label3.Text = Print2DArray(ccArray);
                        
                    }
                    
                    var back2back = await chromeBrowser.EvaluateScriptAsync("lol.export().stats.btb;");
                    if (back2back.Result != null)
                    {
                        b2b = Int16.Parse(back2back.Result.ToString());
                    }
                    var cmbo = await chromeBrowser.EvaluateScriptAsync("lol.export().stats.combo;");
                    if (cmbo.Result != null)
                    {
                        combo = Convert.ToUInt32(Int16.Parse(cmbo.Result.ToString()));
                    }
                    var locked1 = await chromeBrowser.EvaluateScriptAsync("lol.export().NotStart;");
                    if (locked1.Result != null)
                    {
                        var lockedd = Boolean.Parse(locked1.Result.ToString());
                        var locked2 = await chromeBrowser.EvaluateScriptAsync("lol.export().NotStart2;");
                        if (locked2.Result != null)
                        {
                            var lockedd2 = Boolean.Parse(locked2.Result.ToString());
                            if (lockedd == lockedd2)
                                gamelocked = lockedd;
                            else
                            {
                                Console.WriteLine("error");
                            }
                        }
                    }
                    var play = await chromeBrowser.EvaluateScriptAsync("lol.export().game.playing;");
                    if (play.Result != null)
                    {
                        playing = Boolean.Parse(play.Result.ToString());
                    }

                    var hld = await chromeBrowser.EvaluateScriptAsync("lol.export().game.hold.piece;");
                    if (hld.Result != null)
                    {
                        hold = convertStringToCC(hld.Result.ToString());
                    }
                    else
                    {
                        hold = CCPiece.CC_None;
                    }


                    //Console.WriteLine("status: " + hold + "\n" + playing + "\n" + gamelocked + "\n" + combo + "\n" + b2b + "\n" + upcomingGb);

                //

            }
        }



        private void timer1_Tick(object sender, EventArgs e)
        {
            Task upcomingTsk = getUpcoming();
            upcomingTsk.Wait();
            Thread.Sleep(2);

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            chromeBrowser.Reload(true);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (devTools) {
               
                    chromeBrowser.ShowDevTools();
                

            }

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Home)
            {
                DisableProgram();


            }
            if (e.KeyCode == Keys.Insert)
            {
                startGame();


            }
        }
        public static bool isRunning = false;
        private void startGame()
        {
            token = source.Token;
            stop = false;
            if (isRunning)
            {
                return;
            }
            else
            {
                isRunning = true;
            }
            //InitCC();
            var loop1Task = Task.Run(async () => {
                while (!stop)
                {
                    await getData();
                    ccstarting(token);

                    await Task.Delay(5);
                }
            }, token);
        }
        CancellationTokenSource source = new CancellationTokenSource();
        CancellationToken token;
        private void button3_Click(object sender, EventArgs e)
        {
            startGame();
            
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
        public static bool stop = false;
        public static bool speculation = true;
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (speculation)
            {
                speculation = false;
            }
            else
            {
                speculation = true;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        public static bool legitmode = false;
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (legitmode)
            {
                legitmode = false;

            }
            else
            {
                legitmode = true;
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
        public static bool pcpriority = true;
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (pcpriority) {
                pcpriority = false;
              }  
            else {
                pcpriority = true;
                    };
            Console.WriteLine(pcpriority);
        }

        public static bool piece2piece = false;
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (piece2piece)
            {
                piece2piece = false;
            }
            else
            {
                piece2piece = true;
            }
        }
        public static bool trainerOn = false;
        public static int pixelSize = 32;

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (trainerOn)
            {
                trainerOn = false;
                overlay.Hide();
            }
            else
            {
                trainerOn = true;
                overlay.Show();
            }
            if (!(Int32.TryParse(textBox4.Text, out pixelSize)))
            {
                MessageBox.Show("Wrong value input.");

            }
            else
            {
                overlay.Invalidate();

            }
        }
    }

    public class CustomResourceRequestHandler : ResourceRequestHandler
    {
       
        protected override CefReturnValue OnBeforeResourceLoad(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
        {
            string path = Path.Combine("file:///" + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "test.js");

            string originalUrl = request.Url;
            if (request.Url.Contains("tetrio.js"))
            {
                Console.WriteLine("fileserver.com: " + request.Url);
                Console.WriteLine(path);

                string filename = path;
                // e.g. http://fileserver.com/abc.png = abc.png
                //request.
                // We want to use the files from myfileserver.com instead
                //request.Url = "file:///C:/Users/nuvas/source/repos/tetriobot/bin/x64/Release/test.js";

                //request.
                //return new FlashResourceHandler();
            }

            callback.Dispose();
            return CefReturnValue.Continue;
        }
    }
    public class CustomRequestHandler : RequestHandler
    {
        protected override IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
        {
            return new CustomResourceRequestHandler();
        }
    }

   


}
