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

using Color = SFML.Graphics.Color;
using Keyboard = SFML.Window.Keyboard;
using KeyEventArgs = SFML.Window.KeyEventArgs;
using Window = SFML.Window.Window;

namespace Timeline_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private RenderWindow _renderWindow;
        private TimelineModel model;

        public System.Timers.Timer timer;

        public bool KeyPressed_W;
        public bool KeyPressed_A;
        public bool KeyPressed_S;
        public bool KeyPressed_D;

        public int c = 8;
        public static int tCount = 0;

        public const int RefreshRate = 400;

        public bool requiresUpdate = false;

        public MainWindow()
        {
            InitializeComponent();

            this.CreateRenderWindow();

            model = new TimelineModel()
            {
                WindowTitle = "Timeline Project",
                WindowWidth = 1200,
                WindowHeight = 900,

                TimelineTitle = "History of America",

                OffsetX = this._renderWindow.Size.X / 2,
                OffsetY = 0,

                Zoom = 1.0f,
                ZoomSpeed = 10.0f,

                font = new Font(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName +
                    "\\Fonts\\OptimusPrinceps.ttf"),

                EventTextCharacterSize = 20,
                EventBackgroundColor = new Color(230, 230, 230),

                EventFromLineHeight = 50,

                MarkerInterval = 60,
                minMarkerInterval = 100,
                MarkerHeight = 14,
                BackgroundColor = new Color(255, 253, 244),
                PanSpeed = 5f
            };

            model.AddEvent(new TimelineEvent("Test event", 2));
            model.AddEvent(new TimelineEvent("Test event 2", -5.25f));
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
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Thread backgroundThread = new Thread(() =>
                {
                    while (_renderWindow.IsOpen)
                    {
                        System.Windows.Application.Current?.Dispatcher.Invoke(DispatcherPriority.Loaded, new Action(() =>
                        {
                            UpdateWindow();
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
            this._renderWindow.Clear(model.BackgroundColor);

            //      Draw Screen
            // PAN SCREEN
            if (KeyPressed_W) model.OffsetY += model.PanSpeed;
            if (KeyPressed_A) model.OffsetX += model.PanSpeed;
            if (KeyPressed_S) model.OffsetY -= model.PanSpeed;
            if (KeyPressed_D) model.OffsetX -= model.PanSpeed;

            // DRAW LINE
            model.DrawLine(this._renderWindow);

            // DRAW MARKERS
            model.DrawMarkers(this._renderWindow);

            // DRAW EVENTS
            model.DrawEvents(this._renderWindow);

            // DRAW DEBUG INFO
            model.DrawDebugNumber("Zoom: ", (float)model.Zoom, this._renderWindow, 160);

            // DRAW TITLE
            model.DrawTitle(this._renderWindow);


            //      Display Window
            this._renderWindow.Display();
        }

        private void Button_Click_Random_Color(object sender, RoutedEventArgs e)
        {
            var rand = new Random();
            var color = new Color((byte)rand.Next(), (byte)rand.Next(), (byte)rand.Next());
            model.BackgroundColor = color;
        }

        private void DrawSurface_SizeChanged(object sender, EventArgs e)
        {
            this.CreateRenderWindow();
        }

        private void RenderWindow_MouseButtonPressed(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            
        }

        private void Button_Click_New_Event(object sender, RoutedEventArgs e)
        {
            model.AddEvent(new TimelineEvent("Button Event", c));
            c += 4;
        }

        private void DrawSurface_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            switch (e.KeyValue)
            {
                // Navigation
                case 'W':
                    if (!KeyPressed_W) KeyPressed_W = true;
                    break;

                case 'A':
                    if (!KeyPressed_A) KeyPressed_A = true;
                    break;

                case 'S':
                    if (!KeyPressed_S) KeyPressed_S = true;
                    break;

                case 'D':
                    if (!KeyPressed_D) KeyPressed_D = true;
                    break;

                // Keyboard Shortcuts
                case 'N':
                    model.AddEvent(new TimelineEvent("Shortcut Event", c));
                    c += 4;
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
            model.Zoom *= 1 + (Math.Sign(e.Delta) * model.ZoomSpeed/100);
        }
    }
}
