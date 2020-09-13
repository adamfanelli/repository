﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using TimelineLib;
using TimelineLib.Themes;

using Color = SFML.Graphics.Color;
using Keyboard = SFML.Window.Keyboard;
using KeyEventArgs = SFML.Window.KeyEventArgs;
using Window = SFML.Window.Window;
using Mouse = SFML.Window.Mouse;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace Timeline_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private RenderWindow _renderWindow;
        private TimelineViewModel model;

        public bool KeyPressed_W;
        public bool KeyPressed_A;
        public bool KeyPressed_S;
        public bool KeyPressed_D;

        public bool IsMouseDown = false;

        public float PrevMouseX;
        public float PrevMouseY;

        public bool UpdateFlag = false;

        public string WindowTitle { get; set; }


        public MainWindow()
        {
            InitializeComponent();

            // Test ViewModel
            model = new TimelineViewModel()
            {
                WindowTitle = "Timeline Project",

                LineThickness = 4,
                Zoom = 1.0f,
                ZoomSpeed = 10.0f,

                EventTextCharacterSize = 25,
                EventFromLineHeight = 70,

                IntervalThresholdPx = 100,
                IntervalLengthPx = 60,
                MarkerHeight = 14,
                ScrollSpeed = 5f
            };

            // Test Model
            model.SetViewModel(new TimelineModel()
            {
                Title = "History of America",


                PrimaryFont = new Font(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName +
                    "\\Fonts\\GeosansLight.ttf"),

                SecondaryFont = new Font(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName +
                    "\\Fonts\\OptimusPrinceps.ttf"),

                Theme = new ThemeBlue()
            });

            model.AddEvent(new EventModel("Test event", 2));
            model.AddEvent(new EventModel("Test event 2", -5.25f));


            this.DataContext = model;

            this.CreateRenderWindow();

            model.OffsetX = this._renderWindow.Size.X / 2;
        }

        private void CreateRenderWindow()
        {
            if (this._renderWindow != null)
            {
                this._renderWindow.SetActive(false);
                this._renderWindow.Dispose();
            }

            var context = new ContextSettings { DepthBits = 24 };
            this._renderWindow = new RenderWindow(DrawSurface.Handle, context);
            this._renderWindow.SetActive(true);

            UpdateWindow();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateWindow();

            Thread backgroundThread = new Thread(() =>
                {
                    while (_renderWindow.IsOpen)
                    {
                        System.Windows.Application.Current?.Dispatcher.Invoke(DispatcherPriority.Loaded, new Action(() =>
                        {
                            // Update Year At Mouse
                            model.YearAtMouse = (int)Math.Round(
                                (Mouse.GetPosition().X - _renderWindow.Position.X - model.OffsetX) / (model.IntervalLengthPx * model.Zoom));

                            // Update Window, If Necessary
                            bool Update; 

                            if (UpdateFlag)
                            {
                                Update = true;
                                UpdateFlag = false;
                            }
                            else
                            {
                                Update =
                                IsMouseDown ||
                                KeyPressed_W || KeyPressed_A || KeyPressed_S || KeyPressed_D;
                            }
                            
                            if(Update) UpdateWindow();
                        }
                        ));
                    }
                });

            backgroundThread.Start();
        }

        private void UpdateWindow()
        {
            //      Process Events
            this._renderWindow.DispatchEvents();

            //      Clear Screen
            this._renderWindow.Clear(model.Theme.BackgroundColor);

            //      Draw Screen
            // SCROLL SCREEN
            if (KeyPressed_W) model.OffsetY += model.ScrollSpeed;
            if (KeyPressed_A) model.OffsetX += model.ScrollSpeed;
            if (KeyPressed_S) model.OffsetY -= model.ScrollSpeed;
            if (KeyPressed_D) model.OffsetX -= model.ScrollSpeed;

            // DRAW LINE
            model.DrawLine(this._renderWindow);

            // DRAW MARKERS
            model.DrawMarkers(this._renderWindow);

            // DRAW EVENTS
            model.DrawEvents(this._renderWindow);

            // DRAW DEBUG INFO
            //model.DrawDebugNumber("Zoom: ", (float)model.Zoom, this._renderWindow, 220);
            //model.DrawDebugNumber("Focus: ", this._renderWindow.HasFocus() ? 1 : 0, this._renderWindow, 280);
            //model.DrawDebugNumber("Column Visible: ", IsSideColumnVisible ? 1 : 0, this._renderWindow, 120);

            // PAN SCREEN
            if(IsMouseDown)
            {
                float CurrMouseX = Mouse.GetPosition().X - _renderWindow.Position.X;
                float CurrMouseY = Mouse.GetPosition().Y - _renderWindow.Position.Y;

                model.OffsetX -= PrevMouseX - CurrMouseX;
                model.OffsetY -= PrevMouseY - CurrMouseY;

                PrevMouseX = CurrMouseX;
                PrevMouseY = CurrMouseY;
            }

            // DRAW TITLE
            model.DrawTitle(this._renderWindow);


            //      Display Window
            this._renderWindow.Display();
        }

        public void ToggleSideColumn()
        {
            model.IsSideColumnVisible = !model.IsSideColumnVisible;
            UpdateFlag = true;
        }

        private void DrawSurface_SizeChanged(object sender, EventArgs e)
        {
            this.CreateRenderWindow();
        }

        private void DrawSurface_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            switch (e.KeyValue)
            {
                // Navigation
                case 'W':
                    KeyPressed_W = true;
                    break;

                case 'A':
                    KeyPressed_A = true;
                    break;

                case 'S':
                    KeyPressed_S = true;
                    break;

                case 'D':
                    KeyPressed_D = true;
                    break;

                // Keyboard Shortcuts
                case 'N':
                    model.AddEvent(new EventModel("Shortcut Event", model.YearAtMouse));
                    UpdateWindow();
                    break;
            }
        }

        private void DrawSurface_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            switch (e.KeyValue)
            {
                case 'W':
                    KeyPressed_W = false;
                    break;

                case 'A':
                    KeyPressed_A = false;
                    break;

                case 'S':
                    KeyPressed_S = false;
                    break;

                case 'D':
                    KeyPressed_D = false;
                    break;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _renderWindow.Close();
        }

        private void DrawSurface_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            float oldDelta = (Mouse.GetPosition().X - this._renderWindow.Position.X) - model.OffsetX;

            model.Zoom *= 1 + (Math.Sign(e.Delta) * model.ZoomSpeed/100);

            float newDelta = (1 + (Math.Sign(e.Delta) * model.ZoomSpeed / 100)) * oldDelta;

            model.OffsetX += oldDelta - newDelta;

            UpdateWindow();
        }

        private void DrawSurface_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            IsMouseDown = true;

            PrevMouseX = Mouse.GetPosition().X - _renderWindow.Position.X;
            PrevMouseY = Mouse.GetPosition().Y - _renderWindow.Position.Y;
        }

        private void DrawSurface_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            IsMouseDown = false;
        }

        private void DrawSurface_DoubleClick(object sender, EventArgs e)
        {
            ToggleSideColumn();
        }

        private void MenuItem_Click_NewEvent(object sender, RoutedEventArgs e)
        {
            ToggleSideColumn();
        }

        private void On_ThemeChange(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem src = (System.Windows.Controls.MenuItem)e.Source;

            switch(src.Name)
            {
                case "Beige":
                    model.Theme = new ThemeBeige();
                    break;

                case "Blue":
                    model.Theme = new ThemeBlue();
                    break;

                case "Green":
                    model.Theme = new ThemeGreen();
                    break;

                default:
                    model.Theme = new ThemeGreen();
                    break;
            }

            DrawSurface.Focus();

            UpdateWindow();
        }
    }
}
