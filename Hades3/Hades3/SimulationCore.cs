using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace Hades3
{
    /// <summary>
    /// Handles ticks and drawing
    /// </summary>
    public class SimulationCore : Microsoft.Xna.Framework.Game
    {
        public static bool DEBUG = false;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SpriteFont spriteFont;

        public SimulationParameters SimulationParams;

        private static SimulationCore instance;
        public static SimulationCore Instance
        {
            get
            {
                if(instance == null)
                    throw new Exception("uninitialized Simulation Core");
                return instance;
            }
        }

        private UIWindow uiWindow;

        private Environment environment;

        private bool useSelection = true;
        private bool runningSimulation = false;
        private bool allowMovement = false;
        public bool AllowMovement
        {
            get
            {
                return allowMovement;
            }
        }

        private GeometricPrimitive matrixBlock;
        private GeometricPrimitive cellModel;
        private GeometricPrimitive signalModel;
        private Matrix view;
        private Matrix projection;

        private CameraComponent camera;
        private int windowWidth;
        private int windowHeight;
        private const float CAMERA_FOV = 90.0f;
        private const float CAMERA_ZNEAR = 0.01f;
        private const float CAMERA_ZFAR = 200.0f;
        private const float CAMERA_OFFSET = 0.5f;


        private KeyboardState currentKeyboardState;
        private KeyboardState prevKeyboardState;
        private int currTargetIndex;


        private Vector3 selection;
        
        public int SelectionDistance = 10;

        private bool drawCells;
        private bool drawPressurePoints = false;
        private bool drawSignals;
        private bool drawPipes;

        private VertexPositionColor[] tissueBorderVertices;
        private BasicEffect simulationBorderEffect;

        private Color tissueBorderColor = Color.Black;
        private Color finalCellColor    = Color.Green;
        private Color blastCellColor    = Color.Red;
        private Color mutationColor     = Color.Black;
        private Color pipeColor         = Color.Violet;
        private Color selectionColor    = Color.Yellow;
        private Color signalColor       = Color.PeachPuff;

        private List<Mutation> latestMutation;

        private RasterizerState wireFrameState;

        public SimulationCore()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            instance = this;
        }


        private void initializeCamera(Vector3 eye, Vector3 lookAt)
        {
            camera = new CameraComponent(this);
            Components.Add(camera);
            float aspectRatio = (float)windowWidth / (float)windowHeight;
            camera.Perspective(CAMERA_FOV, aspectRatio, CAMERA_ZNEAR, CAMERA_ZFAR);

            camera.CurrentBehavior = Camera.Behavior.Spectator;
            camera.ClickAndDragMouseRotation = true;
            camera.CameraEye = eye;
            camera.LookAt(lookAt);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            windowWidth = GraphicsDevice.DisplayMode.Width / 2;
            windowHeight = GraphicsDevice.DisplayMode.Height / 2;

            cellModel = new CubePrimitive(this.GraphicsDevice, 0.8f, true);
            signalModel = new CubePrimitive(this.GraphicsDevice, 0.5f);
            matrixBlock = new CubePrimitive(this.GraphicsDevice, 1f);

            uiWindow = new UIWindow(this);
            uiWindow.Show();

            selectionColor.A = 255;

            // Setup frame buffer.
            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.PreferredBackBufferWidth = windowWidth;
            graphics.PreferredBackBufferHeight = windowHeight;
            graphics.PreferMultiSampling = true;
            graphics.ApplyChanges();

            wireFrameState = new RasterizerState()
            {
                FillMode = FillMode.WireFrame,
                CullMode = CullMode.None//CullMode.CullClockwiseFace//None,
            };

            interferanceQueue = new Queue<OneTimeInterference>();

            base.Initialize();
        }

        private void loadFont()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = Content.Load<SpriteFont>("hudFont");

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            simulationBorderEffect = new BasicEffect(GraphicsDevice);
            simulationBorderEffect.VertexColorEnabled = true;

            loadFont();

            Console.WriteLine("le content directory! " + Content.RootDirectory.ToString());
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            processInput();
            base.Update(gameTime);
        }

        private SimulationParameters readParamsFromFile(String fileName)
        {
            String paramDirectory = "Content\\";
            fileName = paramDirectory + fileName;
            StringBuilder sb = new StringBuilder();
            using (StreamReader sr = new StreamReader(fileName))
            {
                sb.Append(sr.ReadToEnd());
            }
            return JsonConvert.DeserializeObject<SimulationParameters>(sb.ToString());
        }

        private byte transparency = 0;
        public byte Transparency
        {
            set
            {
                transparency = value;
            }
        }

        private Vector3 calculateSelection(Vector3 cameraPosition, Vector3 viewDirection)
        {
            Vector3 selection = cameraPosition + (viewDirection * 2) * SelectionDistance;
            return selection;
        }

        private byte calculateTransparency(int pressure)
        {
            if (pressure > 1000)
                return 150;
            return (byte)(pressure / 12);
        }

        //pressure settings
        Color firstColor = Color.WhiteSmoke;
        Color secondColor = Color.SaddleBrown;
        int maxPressure = 4500;
        private Color calculatePressureColor(int pressure)
        {
            return Color.Lerp(firstColor, secondColor, pressure / maxPressure);
        }

        private string getStatus(LocationContents loc)
        {
            string status = "";
            status += "position: " + loc.Position.ToString() + "\n";
            status += "pressure: " + loc.Pressure + "\n";
            status += "num cells: " + loc.TissueCells.Count + "\n";
            status += "num pipes: " + loc.pipes.Count + "\n";

            return status;
        }

        #region UIInteraction

        public void StartSimulation(String configName)
        {
            Console.WriteLine("reading config file [" + configName + "]");
            SimulationParameters simulationParams = readParamsFromFile(configName); //new SimulationParameters();
            this.SimulationParams = simulationParams;

            Console.WriteLine("instantiating environment");
            environment = new Environment(simulationParams);

            Console.WriteLine("setting up tissue borders");
            tissueBorderVertices = getTissueBorderVertices(simulationParams.tissueParams);

            Console.WriteLine("initializing camera");
            Console.WriteLine("eye: " + simulationParams.CameraEye);
            Console.WriteLine("look at: " + simulationParams.CameraLookAt);
            initializeCamera(simulationParams.CameraEye, simulationParams.CameraLookAt);

            runningSimulation = true;
            allowMovement = false;
        }

        public void StopSimulation()
        {
            runningSimulation = false;
        }

        public void AcceptMutationBlueprint(List<Mutation> newMutationList)
        {
            Console.WriteLine("accepting...");
            foreach (Mutation m in newMutationList)
                Console.WriteLine(m.ToString());

            latestMutation = new List<Mutation>(newMutationList);
            Console.WriteLine("latest mutation is now [" + latestMutation.Count + "] ...");
            foreach (Mutation m in latestMutation)
                Console.WriteLine(m.ToString());
            Console.WriteLine();
        }

        #endregion

        #region toggles

        public void ToggleDrawSimulation()
        {
        }

        public void ToggleDrawSignals()
        {
            drawSignals = !drawSignals;
        }

        public void ToggleDrawCells()
        {
            drawCells = !drawCells;
        }

        public void ToggleDrawBloodVessels()
        {
            drawPipes = !drawPipes;
        }

        public void ToggleDrawPressure()
        {
            drawPressurePoints = !drawPressurePoints;
        }

        private void togglePause()
        {
            runningSimulation = !runningSimulation;
        }

        private void toggleMovement()
        {
            allowMovement = !allowMovement;

            /*
            if (allowMovement)
                camera.ClickAndDragMouseRotation = false;
            else
                camera.ClickAndDragMouseRotation = true;
             * */
        }

        #endregion

        #region processKeyPresses

        private void processInput()
        {
            prevKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            if (KeyJustPressed(Keys.Space))
            {
                togglePause();
                toggleMovement();
            }

            else if (KeyJustPressed(Keys.PageUp))
            {
                SelectionDistance++;
            }

            else if (KeyJustPressed(Keys.PageDown))
            {
                SelectionDistance--;
            }

            else if (KeyJustPressed(Keys.G))
            {
                interferanceQueue.Enqueue(new ToggleAllowPleaseDieSignal());
            }

            /*
            else if (KeyJustPressed(Keys.Enter))
                ToggleFullScreen();
            */
        }

        private bool KeyJustPressed(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key) && prevKeyboardState.IsKeyUp(key);
        }

        #endregion


        private void printStatus()
        {
            Console.WriteLine("*********************************************************************");
            Console.WriteLine("number of tissues [" + environment.Tissues + "]");
            for (int i = 0; i < environment.Tissues.Count; i++)
            {
                Console.WriteLine("in tissue number [" + i + "] there are [" + environment.Cells.Count + "] cells");
            }
        }

        private long tickCounter = 0;
        private void updateGraphs()
        {
            uiWindow.UpdateNumberFinalOfCells(environment.FinalCellTotal, tickCounter);
            uiWindow.UpdateNumberOfBlastCells(environment.BlastCellTotal, tickCounter);
            uiWindow.UpdateNumberOfMutations(environment.MutatedCellTotal, tickCounter);
            uiWindow.UpdateNumberOfPipes(CirculatorySystem.Instance.PipeCells.Count, tickCounter);
            tickCounter++;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //if (camera != null)
            //{
            //    Console.WriteLine("pos: " + camera.CameraEye);
            //    //Console.WriteLine("dir: " + camera.ViewDirection);
            //    Console.WriteLine("\n\n");
            //}

            //testGraphics();
            runSimulation();

            base.Draw(gameTime);
        }

        private void testGraphics()
        {
            Random r = new Random();
            Color tehTestColor = Color.Green;
            //tehTestColor.A = 100;

            if (camera != null)
            {
                if (allowMovement)
                {
                    Rectangle clientBounds = Window.ClientBounds;
                    int centerX = clientBounds.Width / 2;
                    int centerY = clientBounds.Height / 2;
                    Mouse.SetPosition(centerX, centerY);
                }

                view = camera.ViewMatrix;
                projection = camera.ProjectionMatrix;

                cellModel.Draw(view, projection, Color.Red, new Vector3(0, 0, 0));
                cellModel.Draw(view, projection, tehTestColor, new Vector3(5, 0, 0));
                cellModel.Draw(view, projection, tehTestColor, new Vector3(0, 5, 0));
                cellModel.Draw(view, projection, tehTestColor, new Vector3(0, 0, 5));

                cellModel.Draw(view, projection, Color.Black, new Vector3(5, 5, 3));

                float currSize = 1;


                
                for (int x = 0; x < 100; x++)
                {
                    GraphicsDevice.RasterizerState = wireFrameState;
                    matrixBlock.Draw(view, projection, Color.Black, new Vector3(x, 7, 7));
                    GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

                    matrixBlock.Draw(view, projection, Color.Fuchsia, new Vector3(x - (1 - currSize) / 2, 7, 7), Matrix.CreateScale(1, currSize, currSize));

                    
                    GraphicsDevice.RasterizerState = wireFrameState;
                    matrixBlock.Draw(view, projection, Color.Black, new Vector3(9, x, 9));
                    GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

                    matrixBlock.Draw(view, projection, Color.Aquamarine, new Vector3(9, x - (1 - currSize) / 2, 9), Matrix.CreateScale(currSize, 1, currSize));


                    GraphicsDevice.RasterizerState = wireFrameState;
                    matrixBlock.Draw(view, projection, Color.Black, new Vector3(55, x, x));
                    matrixBlock.Draw(view, projection, Color.Black, new Vector3(55, x + 1, x));
                    GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

                    matrixBlock.Draw(view, projection, Color.Goldenrod, new Vector3(55, x, x-(1-currSize)/2), Matrix.CreateScale(currSize, currSize, 1));
                    matrixBlock.Draw(view, projection, Color.DarkViolet, new Vector3(55, x+1 - (1-currSize)/2, x), Matrix.CreateScale(currSize, 1, currSize));


                    GraphicsDevice.RasterizerState = wireFrameState;
                    matrixBlock.Draw(view, projection, Color.Black, new Vector3(15, 15, x));
                    GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

                    matrixBlock.Draw(view, projection, Color.Crimson, new Vector3(15, 15, x - (1 - currSize) / 2), Matrix.CreateScale(currSize, currSize, 1));

                    currSize *= 0.95f;
                }
               
                /*
                for (float x = 0; x < 30; x++)
                {
                    for (float y = 0; y < 30; y++)
                    {
                        for (float z = 0; z < 30; z++)
                        {
                            //if (r.NextDouble() < 0.6)
                                cellModel.Draw(view, projection, tehTestColor, new Vector3(x, y, z));
                        }
                    }
                }
                 * */
            }
        }

        private void runSimulation()
        {
            if (runningSimulation)    // maybe this should be in Update(), but this doesn't work with STA
            {
                environment.Tick(); // update simulation
                updateGraphs();
                executeInterferences();
            }

            if (allowMovement)
            {
                Rectangle clientBounds = Window.ClientBounds;
                int centerX = clientBounds.Width / 2;
                int centerY = clientBounds.Height / 2;
                Mouse.SetPosition(centerX, centerY);
            }

            if (environment != null)
            {
                view = camera.ViewMatrix;
                projection = camera.ProjectionMatrix;
                selection = calculateSelection(camera.CameraEye, camera.ViewDirection);

                drawSimulation();

                if (useSelection)
                {
                    Vector3 intSelection = new Vector3((int)selection.X, (int)selection.Y, (int)selection.Z);
                    
                    if (environment.GoodPoint(intSelection))
                    {
                        LocationContents selectedLocation = environment.GetContentsAt(intSelection);
                        if (selectedLocation.TissueCells.Count > 0)
                            selectionColor.A = 200;
                        else
                            selectionColor.A = 50;
                        matrixBlock.Draw(view, projection, selectionColor, intSelection);

                        if (KeyJustPressed(Keys.M))
                        {
                            if (latestMutation == null)
                                Console.WriteLine("no mutation selected");
                            else
                            {
                                if (selectedLocation.TissueCells.Count > 0)
                                {
                                    Console.WriteLine("applying mutation [" + latestMutation.Count + "] ... { ");
                                    foreach (Mutation m in latestMutation)
                                        Console.WriteLine(m.ToString());
                                    Console.WriteLine(" } ... to " + selectedLocation.Position + "\n");
                                    ((TissueCell)selectedLocation.TissueCells[0]).AcceptMutations(latestMutation);
                                }
                                else
                                    Console.WriteLine("no cells at selected location");
                            }
                        }
                        //Console.WriteLine(getStatus(selectedLocation));
                    }
                }   // endif useSelection

                string text = "number of blast cells: " + environment.BlastCellTotal;
                spriteBatch.Begin();
                spriteBatch.DrawString(spriteFont, text, new Vector2(15, 15), Color.White);
                spriteBatch.End();

            }   // end if environment != null
        }


        #region drawThings

        private void drawSimulation()
        {
            if (drawCells)
                drawTheCells();

            if (drawPressurePoints)
                drawThePressure();

            if (drawSignals)
                drawTheSignals();

            if (drawPipes)
                drawThePipes();

            drawTissueBorders();
        }

        private void drawTheCells()
        {
            foreach (TissueCell cell in environment.Cells)
            {
                if (cell.PipeTravel == null)        // only draw cell if it is not traveling!
                {
                    float scale = environment.GetContentsAt(cell.CellLocation).TissueCells.Count;

                    Color cellColor;

                    if (cell.Mutated)
                        cellColor = mutationColor;
                    else
                    {
                        cellColor = finalCellColor;
                        if (cell.GetType() == typeof(BlastCell))
                            cellColor = blastCellColor;
                    }

                    cellColor.A = 200;

                    cellModel.Draw(view, projection, cellColor, cell.CellLocation, Matrix.CreateScale(scale));
                }
            }
        }

        private void drawThePipes()
        {
            Color drawColor;
            foreach (EndothelialCell e in CirculatorySystem.Instance.PipeCells)
            {
                if (e.travelingCells.Count > 0)
                    drawColor = Color.DarkViolet;
                else
                    drawColor = pipeColor;

                float shift = (1 - e.Size) / 2;

                if (e.Orientation == EndothelialCell.PipeOrientation.posX)
                    matrixBlock.Draw(view, projection, drawColor, e.CellLocation - new Vector3(shift, 0, 0), Matrix.CreateScale(new Vector3(1, e.Size, e.Size)));
                else if (e.Orientation == EndothelialCell.PipeOrientation.negX)
                    matrixBlock.Draw(view, projection, drawColor, e.CellLocation + new Vector3(shift, 0, 0), Matrix.CreateScale(new Vector3(1, e.Size, e.Size)));
                else if (e.Orientation == EndothelialCell.PipeOrientation.posY)
                    matrixBlock.Draw(view, projection, drawColor, e.CellLocation - new Vector3(0, shift, 0), Matrix.CreateScale(new Vector3(e.Size, 1, e.Size)));
                else if (e.Orientation == EndothelialCell.PipeOrientation.negY)
                    matrixBlock.Draw(view, projection, drawColor, e.CellLocation + new Vector3(0, shift, 0), Matrix.CreateScale(new Vector3(e.Size, 1, e.Size)));
                else if (e.Orientation == EndothelialCell.PipeOrientation.posZ)
                    matrixBlock.Draw(view, projection, drawColor, e.CellLocation - new Vector3(0, 0, shift), Matrix.CreateScale(new Vector3(e.Size, e.Size, 1)));
                else if (e.Orientation == EndothelialCell.PipeOrientation.negZ)
                    matrixBlock.Draw(view, projection, drawColor, e.CellLocation + new Vector3(0, 0, shift), Matrix.CreateScale(new Vector3(e.Size, e.Size, 1)));
                else
                    throw new Exception("unsupported pipe orientation");
            }
        }

        private void drawTheSignals()
        {
            foreach (Signal signal in environment.Signals)
            {
                foreach (Vector3 affectedLoc in signal.AffectedLocations)
                    signalModel.Draw(view, projection, signalColor, affectedLoc);
            }
        }

        private void drawThePressure()
        {
            foreach (LocationContents loc in environment.PressurePoints)
            {
                Color pressureColor = calculatePressureColor(loc.Pressure);
                pressureColor.A = 30;

                matrixBlock.Draw(view, projection, pressureColor, loc.Position);
            }
        }

        private void drawTissueBorders()
        {
            if (camera != null)
            {
                simulationBorderEffect.View = camera.ViewMatrix;
                simulationBorderEffect.Projection = projection = camera.ProjectionMatrix;
                simulationBorderEffect.CurrentTechnique.Passes[0].Apply();
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, tissueBorderVertices, 0, tissueBorderVertices.Length / 2);
            }
        }


        #endregion


        private VertexPositionColor[] getTissueBorderVertices(List<SimulationParameters.TissueParameters> tissueParamList)
        {
            VertexPositionColor[] allTheVertices = new VertexPositionColor[tissueParamList.Count * 24];

            int index = 0;
            foreach (SimulationParameters.TissueParameters oneTissue in tissueParamList)
            {
                VertexPositionColor[] someVertices = getSingleTissueBorderVertices(oneTissue);
                someVertices.CopyTo(allTheVertices, index);
                index += 24;
            }

            return allTheVertices;
        }

        private VertexPositionColor[] getSingleTissueBorderVertices(SimulationParameters.TissueParameters tissueParams)
        {
            // cubes are drawn from center of 3d coordinate, they spill over 0.5 units into surrounding locations
            // because of this, need to offset border vertices for clean look

            Vector3 first = tissueParams.firstCorner - new Vector3(0.5f, 0.5f, 0.5f);
            Vector3 second = tissueParams.secondCorner + new Vector3(0.5f, 0.5f, 0.5f);

            VertexPositionColor a = new VertexPositionColor(first, tissueBorderColor);
            VertexPositionColor b = new VertexPositionColor(new Vector3(first.X, first.Y, second.Z), tissueBorderColor);
            VertexPositionColor c = new VertexPositionColor(new Vector3(second.X, first.Y, second.Z), tissueBorderColor);
            VertexPositionColor d = new VertexPositionColor(new Vector3(second.X, first.Y, first.Z), tissueBorderColor);

            VertexPositionColor e = new VertexPositionColor(new Vector3(first.X, second.Y, first.Z), tissueBorderColor);
            VertexPositionColor f = new VertexPositionColor(new Vector3(first.X, second.Y, second.Z), tissueBorderColor);
            VertexPositionColor g = new VertexPositionColor(second, tissueBorderColor);
            VertexPositionColor h = new VertexPositionColor(new Vector3(second.X, second.Y, first.Z), tissueBorderColor);

            VertexPositionColor[] tissueBorderVertices = {a,b, b,c, c,d, d,a, a,e, b,f, c,g, d,h, e,f, f,g, g,h, h,e};

            return tissueBorderVertices;
        }

        #region interferanceHandling

        private void executeInterferences()
        {
            while (interferanceQueue.Count != 0)
            {
                interferanceQueue.Dequeue().doOnce();
            }
        }

        private Queue<OneTimeInterference> interferanceQueue;
        public Queue<OneTimeInterference> InterferanceQueue
        {
            get
            {
                return interferanceQueue;
            }
        }

        private bool usePleaseDieSignal;
        public bool UsePleaseDieSignal
        {
            get
            {
                return usePleaseDieSignal;
            }
        }

        private bool requireBloodvessels;
        public bool RequireBloodvessels
        {
            get
            {
                return requireBloodvessels;
            }
        }

        public interface Interference
        {
        }

        public abstract class OneTimeInterference : Interference
        {
            public abstract void doOnce();
        }

        public class ToggleAllowPleaseDieSignal : OneTimeInterference
        {
            public override void doOnce()
            {
                SimulationCore.instance.usePleaseDieSignal = !SimulationCore.instance.usePleaseDieSignal;
                if (SimulationCore.instance.usePleaseDieSignal)
                    Console.WriteLine("pleaseDie signal allowed");
                else
                    Console.WriteLine("pleaseDie signal forbidden");
            }
        }

        public class ToggleRequireBloodvessels : OneTimeInterference
        {
            public override void doOnce()
            {
                SimulationCore.instance.requireBloodvessels = !SimulationCore.instance.requireBloodvessels;
                if(SimulationCore.instance.requireBloodvessels == true)
                    Console.WriteLine("blood vessels now necessary for growth");
                else
                    Console.WriteLine("blood vessels no longer required for growth");
            }
        }

        public abstract class ContinuousInterferance : Interference
        {
            //todo
        }

        #endregion

    }
}
