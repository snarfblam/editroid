using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Editroid.Graphic;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Threading;
using Editroid.ROM;

namespace Editroid
{
    class ScreenImageGenerator
    {
     
        readonly Queue<LayoutIndex> queue = new Queue<LayoutIndex>();
        readonly ScreenRenderer renderer = new ScreenRenderer();
        readonly Blitter b = new Blitter();
        readonly Bitmap screenBitmap = new Bitmap(ScreenWidth, ScreenHeight, PixelFormat.Format8bppIndexed);

        public Bitmap RenderedImage { get { return screenBitmap; } }
        public MetroidRom Rom { get; set; }
        public bool ShowEnemies { get; set; }
        public bool ShowPhysics { get; set; }
        public LayoutIndex RenderedLayout { get; private set; }

        public const int ScreenWidth = 256;
        public const int ScreenHeight = 240;

        public ScreenImageGenerator() {
             allScreens = new AllScreenEnumerator(this);
        }


        
        public void Enqueue(LevelIndex level, int layoutIndex, bool altPal) {
            if (level == LevelIndex.None || layoutIndex == 0xFF) return;

            queue.Enqueue(new LayoutIndex(level, layoutIndex, altPal));
        }
        public void Enqueue(LevelIndex level) {
            for (int i = 0; i < Rom.GetLevel(level).Screens.Count; i++) {
                LayoutIndex id = new LayoutIndex(level, i,false);

                if (!queue.Contains(id))
                    queue.Enqueue(id);
            }
            // Todo: One way or another the map will need to be scanned to figure out so we can enqueue only the combinations of screen index, palette, and animation that are actually used
            for (int i = 0; i < Rom.GetLevel(level).Screens.Count; i++) {
                LayoutIndex id = new LayoutIndex(level, i, true);

                if (!queue.Contains(id))
                    queue.Enqueue(id);
            }
        }
        internal void EnqueueAll() {
            allScreens.Reset();
        }

        public event EventHandler LayoutRendered;
        protected virtual void OnLayoutRendered() {
            if (LayoutRendered != null)
                LayoutRendered(this, new EventArgs());
        }


        bool ScreenRenderLoopInProcess;
        /// <remarks>This method is not thread safe. ScreenRenderLoopInProgess is 
        /// only for avoiding DoEvents re-entry</remarks>
        public void PerformScreenRenderLoop() {
            // Prevent doevents reentry
            if (ScreenRenderLoopInProcess) return;

            ScreenRenderLoopInProcess = true;
            while (HasRemainingTasks) {
                RenderNextScreen();
                Application.DoEvents();
                //Thread.Sleep(100);
            }
            ScreenRenderLoopInProcess = false;
        }

        bool renderInProgress = false;
        /// <returns>TRUE on completion, or false if there are no remaining tasks.</returns>
        /// <remarks>This method is not thread safe. renderInProgress is not a thread-safe
        /// re-entry protection.</remarks>
        public bool RenderNextScreen() {
            if (renderInProgress) return false;
            renderInProgress = true;

            try {
                LayoutIndex index;
                if (queue.Count > 0) {
                    index = queue.Dequeue();
                } else if (!allScreens.Finished) {
                    index = allScreens.Current;
                    allScreens.MoveNext();
                } else {
                    return false;
                }

                DrawScreen(index, index.AltPalette);
                RenderedLayout = index;

                OnLayoutRendered();
                return true;
            } finally {
                renderInProgress = false;
            }
        }

        private void DrawScreen(LayoutIndex index, bool altPalette) {
            Level levelData = Rom.GetLevel(index.Level);
            if (levelData.Screens.Count <= index.ScreenIndex) return;

            Screen screenData = levelData.Screens[index.ScreenIndex];

            renderer.Level = levelData;
            renderer.SelectedEnemy = -1;
            renderer.ApplyPalette(screenBitmap, altPalette);

            //screenData.ApplyLevelPalette(screenBitmap);
            

            renderer.DefaultPalette = screenData.ColorAttributeTable;
            renderer.Clear();
            renderer.DrawScreen(index.ScreenIndex);
            var enemies = ShowEnemies ? screenData.Enemies : null;
            renderer.Render(b, screenBitmap, enemies, screenData.Doors, ShowPhysics);
        }

        /// <summary>Clears the render queue.</summary>
        public void ClearQueue() {
            queue.Clear();
            allScreens.ClearEnumeration();
        }

        public bool HasRemainingTasks { get { return queue.Count > 0 || !allScreens.Finished; } }



        AllScreenEnumerator allScreens;
        class AllScreenEnumerator:IEnumerator<LayoutIndex>
        {
            ScreenImageGenerator owner;

            public AllScreenEnumerator(ScreenImageGenerator owner) {
                this.owner = owner;
            }

            LevelIndex level = LevelIndex.None;
            int screenIndex = 0;
            bool altPal;

            public bool MoveNext() {
                if (level == LevelIndex.None) return false;

                screenIndex++;
                if (screenIndex >= owner.Rom.GetLevel(level).Screens.Count) {
                    if (altPal) {
                        screenIndex = 0;
                        level++;
                        altPal = false;
                    } else {
                        screenIndex = 0;
                        altPal = true;
                    }
                }

                return level != LevelIndex.None;
            }

            public LayoutIndex Current { get { return new LayoutIndex(level, screenIndex, altPal); } }

            public bool Finished { get { return level == LevelIndex.None; } }

            public void Reset() { level = LevelIndex.Brinstar; screenIndex = 0; }
            public void ClearEnumeration() {
                level = LevelIndex.None;
            }

            public void Dispose() {
            }
            object System.Collections.IEnumerator.Current { get { return Current; } }
        }

    }
}
