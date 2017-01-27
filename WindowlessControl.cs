using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Editroid
{
    /// <summary>
    /// Provedes a very lightweight foundation for implementing a windowless control.
    /// </summary>
    public class WindowlessControl: IDisposable
    {
        WindowlessControlCollection controls;
        public WindowlessControlCollection Controls { get { return controls; } }

        public WindowlessControl() {
            controls = new WindowlessControlCollection(this);
        }

        /// <summary>Tracks a rectangle bounding all invalid areas when batch invalidating.</summary>
        Rectangle invalidAccumulation = nullRect;
        static Rectangle nullRect = new Rectangle(0,0,-1,-1);
        private bool enableBatchInvalidation = true;

        /// <summary>
        /// If true (default), areas invalidated after a call to
        /// SuspendPaint will all be painted when ResumePaint is called.
        /// </summary>
        public bool EnableBatchInvalidation {
            get { return enableBatchInvalidation; }
            set { enableBatchInvalidation = value;
                if(value == false && suspendPaintLevel > 0)
                    PerformSuspendedInvalidation();
            }
        }

        int suspendPaintLevel = 0;

        public SuspendPaintOperation SuspendPaint() {
            suspendPaintLevel++;
            return new SuspendPaintOperation(this);
        }

        public void ResumePaint() {
            suspendPaintLevel--;

            PerformSuspendedInvalidation();
        }

        private void PerformSuspendedInvalidation() {
            if (suspendPaintLevel == 0) {
                if (invalidAccumulation != nullRect)
                    Invalidate(invalidAccumulation);
                invalidAccumulation = nullRect;
            }
        }

        public class SuspendPaintOperation : IDisposable
        {
            WindowlessControl source;
            public SuspendPaintOperation(WindowlessControl source) {
                this.source = source;
            }

            public void Dispose() {
                if(source != null)
                    source.ResumePaint();

                source = null;
            }
        }



        private Control host;

        public Control Host {
            get { return host; }
            set {
                if (host != null) {
                    host.Paint -= new PaintEventHandler(host_Paint);
                    host.MouseDown -= new MouseEventHandler(host_MouseDown);
                    host.MouseUp -= new MouseEventHandler(host_MouseUp);
                    host.MouseMove -= new MouseEventHandler(host_MouseMove);
                    host.MouseEnter -= new EventHandler(host_MouseEnter);
                    host.MouseLeave -= new EventHandler(host_MouseLeave);
                    host.KeyUp -= new KeyEventHandler(host_KeyUp);
                    host.KeyDown -= new KeyEventHandler(host_KeyDown);
                    host.KeyPress -= new KeyPressEventHandler(host_KeyPress);

                }

                host = value;

                if (host != null) {
                    host.Paint += new PaintEventHandler(host_Paint);
                    host.MouseDown += new MouseEventHandler(host_MouseDown);
                    host.MouseUp += new MouseEventHandler(host_MouseUp);
                    host.MouseMove += new MouseEventHandler(host_MouseMove);
                    host.MouseEnter += new EventHandler(host_MouseEnter);
                    host.MouseLeave += new EventHandler(host_MouseLeave);
                    host.KeyUp += new KeyEventHandler(host_KeyUp);
                    host.KeyDown += new KeyEventHandler(host_KeyDown);
                    host.KeyPress += new KeyPressEventHandler(host_KeyPress);
                }
            
            }
        }

        void host_KeyPress(object sender, KeyPressEventArgs e) {
        }

        void host_KeyDown(object sender, KeyEventArgs e) {
        }

        void host_KeyUp(object sender, KeyEventArgs e) {
        }

        void host_MouseLeave(object sender, EventArgs e) {
            if (mouseContainer != null)
                SwitchMouseFocus(null);
            ////LoseMouse();
            ////foreach (WindowlessControl c in controls)
            ////    c.host_MouseLeave(sender, e);
        }

        void host_MouseEnter(object sender, EventArgs e) {
        }


        bool isRoot { get { return parent == null; } }
        bool containsMouse;
        public bool ContainsMouse { get { return containsMouse; } }

        

        /// <summary>
        /// Tracks which control has a lock on the mouse. When the button is pressed in a control, that control 
        /// recieves all mouse events until any button is released, or a button is pressed over another
        /// control. This works recursively by all contianing controls pointing at a direct child
        /// that contains the lock, creating a chain to the most-nested control
        /// with the lock. A control points to itself when it has mouse-lock.
        /// </summary>
        WindowlessControl mouseLock;
        bool HasMouseLock { get { return mouseLock == this; } }
        bool ContainsMouseLock { get { return mouseLock != null; } }

        WindowlessControl mouseContainer;
        void host_MouseMove(object sender, MouseEventArgs e) {
            if (!visible) return;

            MouseEventArgs arg = new MouseEventArgs(e.Button, e.Clicks, e.X - Left, e.Y - Top, e.Delta);

            if (mouseLock == null) {
                // Root control manages mouse focus
                if (isRoot) {
                    WindowlessControl newMouseContainer = FindControlAt(e.Location);
                    if (newMouseContainer != mouseContainer)
                        SwitchMouseFocus(newMouseContainer);
                }

                // If there is no lock, route event normally.
                if (!bounds.Contains(e.X, e.Y)) {
                    //LoseMouse();
                } else {
                    WindowlessControl eventReciever = null;

                    // Event needs to go to most-nested control than contains mouse
                    foreach (WindowlessControl c in controls) {
                        if (!c.bounds.Contains(arg.X, arg.Y)) {
                            //c.LoseMouse();
                        } else {
                            c.host_MouseMove(this, arg);
                            eventReciever = c;
                        }
                    }

                    // If no nested control gets click, this control gets it
                    if (eventReciever == null) {
                        OnMouseMove(arg);
                    }
                    //GetMouse();
                }
            } else if (mouseLock == this) {
                OnMouseMove(arg);
            } else {
                mouseLock.host_MouseMove(this, arg);
            }
            

        }

        private void SwitchMouseFocus(WindowlessControl newMouseContainer) {
            WindowlessControl oldMouseContainer = mouseContainer;
            mouseContainer = newMouseContainer;
            if(oldMouseContainer != null)
                oldMouseContainer.OnMouseLeave();
            if(newMouseContainer != null)
                newMouseContainer.OnMouseEnter();
        }

        /// <summary>
        /// Finds the control at the specified location relative to this 
        /// control's bounds.
        /// </summary>
        /// <param name="point"></param>
        /// <returns>The most-nested control at the specified point. If there
        /// are overlapping controls at the point, the top-most will be
        /// returned. If the point is outside this controls bounds, the return
        /// value is null.</returns>
        private WindowlessControl FindControlAt(Point point) {
            if (point.X < 0 || point.Y < 0 || point.X >= Width || point.Y >= Height)
                return null;
            
            // We loop through all controls, rather than returning first hit.
            // This way, when controls overlap, we find the top-most (will be
            // later in collection).

            WindowlessControl result = this; // If no contained controls are found, we should return this.
            foreach (WindowlessControl c in controls) {
                if (c.bounds.Contains(point)) {
                    Point relativePoint = point;
                    relativePoint.Offset(-c.Left, -c.Top);

                    // Recursion finds most-nested control
                    WindowlessControl containedControl = c.FindControlAt(relativePoint);
                    if (containedControl == null)
                        result = c;
                    else
                        result = containedControl;
                }
            }

            return result;
        }

        ////private void LoseMouse() {
        ////    if (ContainsMouse) {
        ////        containsMouse = false;
        ////        OnMouseLeave();
        ////        foreach (WindowlessControl c in controls)
        ////            c.LoseMouse();
        ////    }
        ////}

        protected virtual void OnMouseLeave() {
        }

        ////private void GetMouse() {
        ////    if (!ContainsMouse) {
        ////        containsMouse = true;
        ////        OnMouseEnter();
        ////    }
        ////}

        protected virtual void OnMouseEnter() {
        }

        protected virtual void OnMouseMove(MouseEventArgs e) {
        }

        void host_MouseUp(object sender, MouseEventArgs e) {
            MouseEventArgs arg = new MouseEventArgs(e.Button, e.Clicks, e.X - Left, e.Y - Top, e.Delta);

            if (mouseLock == null) { 
                // If there is no lock, route event normally.

                if (bounds.Contains(e.X, e.Y)) {
                    WindowlessControl eventReciever = null;

                    // Event needs to go to most-nested control than contains mouse
                    foreach (WindowlessControl c in controls) {
                        if (c.bounds.Contains(arg.X, arg.Y)) {
                            c.host_MouseUp(this, arg);
                            eventReciever = c;
                        }
                    }

                    // If no nested control get click, this control gets it
                    if (eventReciever == null) {
                        OnMouseUp( arg);
                    }
                }
            } else if (mouseLock == this) {
                OnMouseUp(arg);
            } else {
                mouseLock.host_MouseUp(this, arg);
            }
            

            mouseLock = null;
        }

        protected virtual void OnMouseUp(MouseEventArgs e) {
        }

        void host_MouseDown(object sender, MouseEventArgs e) {
            if (bounds.Contains(e.X, e.Y)) {
                // arg is relative to this control
                MouseEventArgs arg = new MouseEventArgs(e.Button, e.Clicks, e.X - Left, e.Y - Top, e.Delta);
                WindowlessControl eventReciever = null;

                // Event needs to go to most-nested control than contains click
                foreach (WindowlessControl c in controls) {
                    if (c.bounds.Contains(arg.X, arg.Y)) {
                        c.host_MouseDown(this, arg);
                        eventReciever = c;
                        mouseLock = eventReciever;
                    }
                }

                // If no nested control get click, this control gets it
                if (eventReciever == null) {
                    OnMouseDown(arg);
                    mouseLock = this;
                }
            }
        }

        protected virtual void OnMouseDown(MouseEventArgs e) {

        }

        void host_Paint(object sender, PaintEventArgs e) {
            if(visible)
                Paint(e.ClipRectangle, e.Graphics);
        }

        private bool visible = true;

        public bool Visible {
            get { return visible; }
            set {
                if (visible == value) return;

                visible = value;
                Invalidate();
                if (! visible ) {
                    FreeMouse();
                }
            }
        }

        /// <summary>
        /// Causes the MouseLeave event to raised if necessary by removing this control as the mouse container.
        /// </summary>
        private void FreeMouse() {
            WindowlessControl root = Root;
            if (root.mouseContainer == this)
                root.ClearMouseContainer();

            foreach (WindowlessControl c in controls)
                if (root.mouseContainer == c)
                    root.ClearMouseContainer();
        }

        private void ClearMouseContainer() {
            WindowlessControl c = mouseContainer;
            mouseContainer = null;
            if(c != null)
                c.OnMouseLeave();
        }


        private Rectangle bounds;

        public Rectangle Bounds {
            get { return bounds; }
            set {
                if (bounds == value) return;
                Rectangle oldBounds = bounds;
                bounds = value;

                if (parent != null) {
                    parent.SuspendPaint();

                    parent.Invalidate(oldBounds);
                    parent.Invalidate(bounds);

                    parent.ResumePaint();
                } else if(host != null) {
                    if (enableBatchInvalidation) {
                        host.Invalidate(Rectangle.Union(oldBounds, bounds));
                    }else{
                        host.Invalidate(oldBounds);
                        host.Invalidate(bounds);
                    }
                }
            }
        }

        public void Invalidate() {
            Invalidate(new Rectangle(0, 0, Width, Height));
        }
        public void Invalidate(Rectangle area) {
            if (suspendPaintLevel > 0 && enableBatchInvalidation) {
                if (invalidAccumulation == nullRect)
                    invalidAccumulation = area;
                else
                    invalidAccumulation = Rectangle.Union(area, invalidAccumulation);

            } else {
                if (parent != null) {
                    area.Offset(bounds.Location);
                    parent.Invalidate(area);
                } else {
                    // If painting is suspended, add the rect to the batch invalid
                    area.Offset(Left, Top);
                    if (host != null) {
                        host.Invalidate(area);
                    }
                }
            }

        }
        /// <summary>
        /// Gets the control that contains this control (or is this control) that is directly hosted
        /// in a windowed control.
        /// </summary>
        public WindowlessControl Root {
            get {
                WindowlessControl p = this;
                while (p.parent != null)
                    p = p.parent;

                return p;
            }
        }

        public int Left {
            get { return bounds.X; }
            set {
                Rectangle b = bounds;
                b.X = value;
                Bounds = b;
            }
        }

	    public int Top
	    {
		    get { return bounds.Y;}
		    set {
                Rectangle b = bounds;
                b.Y = value;
                Bounds = b;
            }
	    }

        public int Width {
            get { return bounds.Width; }
            set {
                Rectangle b = bounds;
                b.Width = value;
                Bounds = b;
            }
        }

        public int Height {
            get { return bounds.Height; }
            set {
                Rectangle b = bounds;
                b.Height = value;
                Bounds = b;
            }
        }
        public Point Location { get { return bounds.Location; }
            set {
                Rectangle b = bounds;
                b.Location = value;
                bounds = b;
            }
        }

        private WindowlessControl parent;
        public WindowlessControl Parent {
            get { return parent; }
            private set {
                WindowlessControl oldParent = parent;
                this.parent = value;
                OnParentChanged(oldParent);
            }
        }

        protected virtual void OnParentChanged(WindowlessControl oldParent) {
        }

        public void Paint(Rectangle invalidRect, Graphics g) {
            PaintEventArgs pe = new PaintEventArgs(g, invalidRect);
            OnPaint(pe);

            foreach (WindowlessControl c in controls) {
                if(c.visible && c.AbsoluteBounds.IntersectsWith(invalidRect))
                    c.Paint(invalidRect, g);
            }
        }
        /// <remarks>All coordinates are relative to containing windowed control.</remarks>
        protected virtual void OnPaint(PaintEventArgs pe) {

        }

        /// <summary>
        /// Gets the bounds of this control relative to the location of the containing windowed control.
        /// </summary>
        public Rectangle AbsoluteBounds {
            get {
                Rectangle b = bounds;
                WindowlessControl p = parent;

                while (p != null) {
                    b.X += p.Left;
                    b.Y += p.Top;

                    p = p.parent;
                }

                return b;
            }
        }

        public class WindowlessControlCollection : iLab.VirtualCollection<WindowlessControl>
        {
            WindowlessControl owner;
            internal WindowlessControlCollection(WindowlessControl owner) {
                this.owner = owner;
            }

            #region Overrides
            public override void Add(WindowlessControl item) {
                AssertNoParent(item);

                item.Parent = owner;
                owner.Invalidate(item.bounds);
                base.Add(item);
            }

            private void AssertNoParent(WindowlessControl item) {
                if (item.Parent != null)
                    except(Exceptions.ControlIsBoundToParent);
            }

            public override void Clear() {
                WindowlessControl[] controls = this.ToArray();

                foreach (WindowlessControl c in this) {
                    c.Parent = null;
                }
                base.Clear();

                owner.SuspendPaint();
                foreach (WindowlessControl c in controls)
                    owner.Invalidate(c.bounds);
                owner.ResumePaint();
            }

            public override void Insert(int index, WindowlessControl item) {
                AssertNoParent(item);

                item.Parent = owner;
                owner.Invalidate(item.bounds);
                base.Insert(index, item);
            }

            public override bool Remove(WindowlessControl item) {
                item.FreeMouse();

                bool result = base.Remove(item);
                item.Parent = null;
                owner.Invalidate(item.bounds);

                return result;
            }

            public override void RemoveAt(int index) {
                this[index].FreeMouse();

                Rectangle invalid = this[index].bounds;
                this[index].Parent = null;
                base.RemoveAt(index);
                owner.Invalidate(invalid);
            }

            public override WindowlessControl this[int index] {
                get {
                    return base[index];
                }
                set {
                    except(Exceptions.SetByIndex);
                }
            }
            #endregion

            #region Exceptions
            void except(Exceptions error) {
                except(error, null);
            }
            void except(Exceptions error, string message) {
                switch (error) {
                    case Exceptions.ControlIsBoundToParent:
                        throw new InvalidOperationException("The specified control is contained by another control an can not be added to another control.");
                    case Exceptions.SetByIndex:
                        throw new InvalidOperationException("This collection does not support setting via the indexer. Use the Insert and RemoveAt methods instead.");
                }
            }
            enum Exceptions
            {
                ControlIsBoundToParent,
                SetByIndex
            }
            #endregion
        }

        public virtual void  Dispose() {
            
        }
    }
}
