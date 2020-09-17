﻿using System;
using System.IO;
using Microsoft.Win32;
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
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using Newtonsoft.Json;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

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
        public bool KeyPressed_CTRL;

        public bool IsMouseDown = false;

        public float PrevMouseX;
        public float PrevMouseY;

        public float DefaultRefreshRate = 0.0005f;
        public float RefreshCount = 0;


        public MainWindow()
        {
            InitializeComponent();

            // Load a new Timeline
            LoadTimeline();

            model.AddEvent(new EventViewModel("Test Event"));
        }

        private void CreateRenderWindow()
        {
            if (this._renderWindow != null)
            {
                this._renderWindow.SetActive(false);
                this._renderWindow.Dispose();
            }

            var context = new ContextSettings { DepthBits = 24, AntialiasingLevel = 50};
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


                            bool Update;

                            // Update Window
                            RefreshCount += DefaultRefreshRate;
                            if (RefreshCount > 0.5)
                            {
                                Update = true;
                                RefreshCount = 0;
                            }
                            else
                            {
                                Update =
                                    IsMouseDown ||
                                    KeyPressed_W || KeyPressed_A || KeyPressed_S || KeyPressed_D;
                            }

                            foreach (EventViewModel n in model.ListOfEvents)
                            {
                                if (n.IsMouseOver(_renderWindow)) Update = true;
                            }

                            if (Update) UpdateWindow();
                        }
                        ));
                    }
                });

            backgroundThread.Start();

            DrawSurface.Focus();
        }

        public void LoadTimeline(TimelineViewModel timelineViewModel = null)
        {
            // If no viewmodel is given, create a new viewmodel
            if(timelineViewModel == null)
            {
                timelineViewModel = new TimelineViewModel();

                timelineViewModel.SetViewModel(new TimelineModel()
                {
                    Title = "New Timeline",
                    ThemeID = 3
                });
            }

            this.model = null;

            this.model = timelineViewModel;

            // Set the XAML DataContext
            this.DataContext = model;

            // Create the Window
            this.CreateRenderWindow();

            // Set the OffsetX
            if (this.model.ListOfEvents.Count > 0)
                this.model.OffsetX = this.model.ListOfEvents.First().StartYear * this.model.IntervalLengthPx * -1;
            else
                model.OffsetX = this._renderWindow.Size.X / 2;

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

        public void ToggleNewEventForm()
        {
            if(!model.IsSideColumnVisible)
            {
                model.NewEventName = "";
                model.NewEventYear = model.YearAtMouse;
                NameTextBox.Focus();
            }

            model.IsSideColumnVisible = !model.IsSideColumnVisible;

            UpdateWindow();
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
                    if(e.Control)
                    {
                        e.SuppressKeyPress = true;
                        ToggleNewEventForm();
                    }
                    break;

                case 'R':
                    model.OffsetX = model.OffsetX = this._renderWindow.Size.X / 2;
                    model.OffsetY = 0;
                    model.Zoom = 1;
                    model.ScrollCount = 0;
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
            model.ScrollCount += Math.Sign(e.Delta);

            //Cap Zoom
            if (model.ScrollCount < -132) model.ScrollCount = model.ZoomMaxCap;
            if (model.ScrollCount > 12) model.ScrollCount = model.ZoomMinCap;

            double oldZoom = model.Zoom;
            model.Zoom = (float)Math.Pow(1 + (model.ZoomSpeed / 100 * Math.Sign(model.ScrollCount)), Math.Abs(model.ScrollCount));

            double oldDelta = (Mouse.GetPosition().X - this._renderWindow.Position.X) - model.OffsetX;
            double newDelta = oldDelta * (model.Zoom / oldZoom);

            model.OffsetX += (float)(oldDelta - newDelta);

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
            ToggleNewEventForm();
        }

        private void menuItemNewEvent_Click(object sender, RoutedEventArgs e)
        {
            if(!model.IsSideColumnVisible) ToggleNewEventForm();
        }

        private void On_ThemeChange(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem src = (System.Windows.Controls.MenuItem)e.Source;
            model.Theme = Theme.GetThemeByName(src.Name);
            DrawSurface.Focus();
            UpdateWindow();
        }

        private void btnSubmitNewEvent_Click(object sender, RoutedEventArgs e)
        {
            NewEventSubmitButton.Focus();
            if (model.NewEventName != "") SubmitNewEvent();
        }

        private void SubmitNewEvent()
        {
            NewEventSubmitButton.Focus();

            ToggleNewEventForm();
            model.AddEvent(new EventViewModel(model.NewEventName, model.NewEventYear));

            DrawSurface.Focus();
        }
        private void btnCancelNewEvent_Click(object sender, RoutedEventArgs e)
        {
            ToggleNewEventForm();
            DrawSurface.Focus();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.TextBox textbox = (System.Windows.Controls.TextBox)e.Source;
            textbox.SelectAll();
        }

        private void btnNewFile_Click(object sender, RoutedEventArgs e)
        {
            this.LoadTimeline();
        }

        private void btnSaveFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file (*.txt)|*.txt";
            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, JsonConvert.SerializeObject(model.ConvertToSaveModel(), Formatting.Indented));
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string json = File.ReadAllText(openFileDialog.FileName);
                TimelineViewModel timelineViewModel = new TimelineViewModel();
                timelineViewModel.SetViewModel(JsonConvert.DeserializeObject<TimelineModel>(json));
                this.LoadTimeline(timelineViewModel);
            }
        }

        private void DrawSurface_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            foreach (EventViewModel n in model.ListOfEvents)
            {
                if (n.IsMouseOver(_renderWindow))
                {
                    // Do something with that event
                    n.StartYear++;
                }
            }
        }

    }
}
