using GUtils.Timing;
using PolyBasedCircleDrawing.Drawing;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PolyBasedCircleDrawing
{
    public partial class MainForm : Form
    {
        private enum CircleMode
        {
            FixedStep,
            MaxDistance
        }

        private class ArcProperties
        {
            [DisplayName ( "Inner Radius" )]
            [Category ( "Radiuses" )]
            [DefaultValue ( 20f )]
            [Description ( "The radius of the empty inner circle" )]
            public Single InnerRadius { get; set; }

            [DisplayName ( "Outer Radius" )]
            [Category ( "Radiuses" )]
            [DefaultValue ( 40f )]
            [Description ( "The radius of the filled outer circle" )]
            public Single OuterRadius { get; set; }

            [DisplayName ( "Starting Angle" )]
            [Category ( "Angles" )]
            [DefaultValue ( 45f )]
            [Description ( "The angle at which to start the arc" )]
            public Single StartingAngle { get; set; }

            [DisplayName ( "Final Angle" )]
            [Category ( "Angles" )]
            [DefaultValue ( 135f )]
            [Description ( "The angle at which to end the arc" )]
            public Single FinalAngle { get; set; }

            [DisplayName ( "Filling Mode" )]
            [Category ( "Rendering Options" )]
            [DefaultValue ( FillMode.Alternate )]
            public FillMode FillMode { get; set; }

            [DisplayName ( "Maximum Distance" )]
            [Category ( "Rendering Options" )]
            [DefaultValue ( 1f )]
            [Description ( "The maximum distance between each of the vertices" )]
            public Single MaxDistance { get; set; }

            [DisplayName ( "Calculation Mode" )]
            [Category ( "Rendering Options" )]
            [DefaultValue ( CircleMode.MaxDistance )]
            [Description ( "The mode that we use to calculate the points" )]
            public CircleMode CircleMode { get; set; }
        }

        private readonly ArcProperties ArcPropertiesInstance = new ArcProperties
        {
            StartingAngle = 45f,
            FinalAngle = 90f + 45f,
            InnerRadius = 25f,
            OuterRadius = 75f,
            FillMode = FillMode.Alternate,
            MaxDistance = 1f
        };

        public String StatusString
        {
            get => this.statusLabel.Text;

            set
            {
                if ( this.statusStrip.InvokeRequired )
                {
                    this.statusStrip.Invoke ( ( Action ) ( ( ) => this.StatusString = value ) );
                    return;
                }

                this.statusLabel.Text = $"[{DateTime.Now:hh\\:mm\\:ss\\.ffffff}] {value}";
            }
        }

        private PointF[] Points;

        public MainForm ( )
        {
            this.InitializeComponent ( );

            this.UpdateArc ( null, null );
            this.panelArc.Paint += this.PanelArc_Paint;
            this.panelArc.Resize += this.UpdateArc;
            this.propertyGrid.PropertyValueChanged += this.UpdateArc;
            this.propertyGrid.SelectedObject = this.ArcPropertiesInstance;
        }

        private void PanelArc_Paint ( Object sender, PaintEventArgs e ) =>
            e.Graphics.DrawPolygon ( Pens.Red, this.Points );

        private void UpdateArc ( Object sender, EventArgs e )
        {
            if ( this.ArcPropertiesInstance.StartingAngle > this.ArcPropertiesInstance.FinalAngle )
            {
                this.StatusString = "ERROR: final angle is bigger than ending angle.";
                return;
            }

            if ( this.ArcPropertiesInstance.InnerRadius > this.ArcPropertiesInstance.OuterRadius )
            {
                this.StatusString = "ERROR: Inner radius is bigger than outer radius.";
                return;
            }

            var sw = Stopwatch.StartNew ( );
            if ( this.ArcPropertiesInstance.CircleMode == CircleMode.MaxDistance )
            {
                this.Points = ArcUtilities.GetCircularArcVertices (
                  new Point ( this.panelArc.Width / 2, this.panelArc.Height / 2 ),
                  this.ArcPropertiesInstance.StartingAngle,
                  this.ArcPropertiesInstance.FinalAngle,
                  this.ArcPropertiesInstance.InnerRadius,
                  this.ArcPropertiesInstance.OuterRadius,
                  this.ArcPropertiesInstance.MaxDistance
              );
            }
            else
            {
                this.Points = ArcUtilities.GetCircularArcVerticesWithFixedStep (
                  new Point ( this.panelArc.Width / 2, this.panelArc.Height / 2 ),
                  this.ArcPropertiesInstance.StartingAngle,
                  this.ArcPropertiesInstance.FinalAngle,
                  this.ArcPropertiesInstance.InnerRadius,
                  this.ArcPropertiesInstance.OuterRadius,
                  this.ArcPropertiesInstance.MaxDistance
              );
            }
            this.StatusString = $"SUCESS: Rebuilt arc in {Duration.Format ( sw.ElapsedTicks )}.";
            this.panelArc.Invalidate ( );
        }
    }
}