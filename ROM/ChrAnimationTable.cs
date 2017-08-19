using System;
using System.Collections.Generic;
using System.Text;
using Editroid.ROM.Projects;

namespace Editroid.ROM
{

    /// <summary>
    /// Utility class for serializing and deserializing animation data
    /// </summary>
    static class ChrAnimation
    {

        //-1E:B600 (0x7B610) = CHR animation table
        //-B600:  Slot 0 bank index
        //-B700:  Slot 1 bank index
        //-B800:  Slot 2 bank index
        //-B900:  Slot 3 bank index
        //-BA00:  Lower 7 bits: frame delay / High bit: if set, last frame

        ////public const int TableOffset_Bank0 = 0x7B610;
        ////public const int TableOffset_Bank1 = 0x7B710;
        ////public const int TableOffset_Bank2 = 0x7B810;
        ////public const int TableOffset_Bank3 = 0x7B910;
        ////public const int TableOffset_FrameData = 0x7BA10;

        public static ChrAnimationRomData DeserializeChrAnimation(MetroidRom r, Project p) {
            ChrAnimationRomData result = new ChrAnimationRomData();

            var titleStart = r.ChrUsage.GetBgFirstPage(LevelIndex.None);
            var brinStart = r.ChrUsage.GetBgFirstPage(LevelIndex.Brinstar);
            var norStart = r.ChrUsage.GetBgFirstPage(LevelIndex.Norfair);
            var tourStart = r.ChrUsage.GetBgFirstPage(LevelIndex.Tourian);
            var kraidStart = r.ChrUsage.GetBgFirstPage(LevelIndex.Kraid);
            var ridStart = r.ChrUsage.GetBgFirstPage(LevelIndex.Ridley);

            result.Add(LevelIndex.None, LoadAnimationsForLevel(r, LevelIndex.None, brinStart));
            result.Add(LevelIndex.Brinstar, LoadAnimationsForLevel(r, LevelIndex.Brinstar, norStart));
            result.Add(LevelIndex.Norfair, LoadAnimationsForLevel(r, LevelIndex.Norfair, tourStart));
            result.Add(LevelIndex.Tourian, LoadAnimationsForLevel(r, LevelIndex.Tourian, kraidStart));
            result.Add(LevelIndex.Kraid, LoadAnimationsForLevel(r, LevelIndex.Kraid, ridStart));
            result.Add(LevelIndex.Ridley, LoadAnimationsForLevel(r, LevelIndex.Ridley, 0xFF));

            return result;
        }

        /// <summary>
        /// Loads animation data/frames for the specified level.
        /// </summary>
        /// <param name="level">The level to load data for</param>
        /// <param name="lastFrame">The index of the first frame after the current area's frame data, or FF to leave unspecified. If an FF terminator is encountered in frame data, parsing will stop sooner.</param>
        /// <returns></returns>
        private static ChrAnimationLevelData LoadAnimationsForLevel(MetroidRom rom, LevelIndex level, int lastFrame) {
            
            ChrAnimationLevelData data = new ChrAnimationLevelData(level);
            // CHR usage table has been repurposed in MMC3 ROMs (hence accessor names don't match usage)
            data.SprBank0 = rom.ChrUsage.GetSprPage(level);
            data.SprBank1 = rom.ChrUsage.GetBgLastPage(level);
            // First frame index
            int frameIndex = rom.ChrUsage.GetBgFirstPage(level);
            bool keepGoing = true;

            while (keepGoing && frameIndex < lastFrame && frameIndex < 0xFF) {
                ChrAnimationTable loadedAnimation = LoadOneAnimation(rom, level, ref frameIndex);
                if (loadedAnimation == null) {
                    if (data.Animations.Count == 0) {
                        loadedAnimation = new ChrAnimationTable();
                        ChrAnimationFrame frame = new ChrAnimationFrame();
                        frame.FrameTime = 1;
                        loadedAnimation.Frames.Add(frame);
                    } else {
                        return data;
                    }
                }
                data.Animations.Add(loadedAnimation);
            }

            return data;
        }

        private static ChrAnimationTable LoadOneAnimation(MetroidRom rom, LevelIndex level, ref int frameIndex) {
            ChrAnimationTable result = new ChrAnimationTable();
            if (isTerminator(rom, frameIndex)) {
                return null;
            }

            bool lastFrame = false;
            while (!lastFrame) {
                ChrAnimationFrame? loadedFrame = LoadOneFrame(rom, frameIndex, out lastFrame);
                // If we fail to load a frame, that means there is an erroneous condition and this animation is invalid.
                if (loadedFrame == null) return null;
                result.Frames.Add(loadedFrame.Value);

                frameIndex++;
            }

            return result;
        }

        private static ChrAnimationFrame? LoadOneFrame(MetroidRom rom, int frameIndex, out bool LastFrame) {
            LastFrame = false;
            if (frameIndex == 0xFF) return null;

            ChrAnimationFrame result = new ChrAnimationFrame();
            result.Bank0 = rom.GetAnimationTable_Bank0(frameIndex); //rom.data[TableOffset_Bank0 + frameIndex];
            result.Bank1 = rom.GetAnimationTable_Bank1(frameIndex); //rom.data[TableOffset_Bank0 + frameIndex];
            result.Bank2 = rom.GetAnimationTable_Bank2(frameIndex); //rom.data[TableOffset_Bank0 + frameIndex];
            result.Bank3 = rom.GetAnimationTable_Bank3(frameIndex); //rom.data[TableOffset_Bank0 + frameIndex];
            // Don't store last-frame bit in result.Frame!
            result.FrameTime = rom.GetAnimationTable_FrameTime(frameIndex); //0x7F & rom.data[TableOffset_Bank0 + frameIndex];

            LastFrame = rom.GetAnimationTable_FrameLast(frameIndex); //0 != (rom.data[TableOffset_Bank0 + frameIndex] & 0x80);
            return result;
        }

        private static bool isTerminator(MetroidRom rom, int frameIndex) {
            return
                rom.GetAnimationTable_Bank0(frameIndex) == 0xFF &&
                rom.GetAnimationTable_Bank1(frameIndex) == 0xFF &&
                rom.GetAnimationTable_Bank2(frameIndex) == 0xFF &&
                rom.GetAnimationTable_Bank3(frameIndex) == 0xFF &&
                rom.GetAnimationTable_FrameData(frameIndex) == 0xFF;
        }


        /// <summary>
        /// Writes frame data to the chr animation table for the specified animations. The CHR usage table is not updated by this method.
        /// </summary>
        /// <param name="rom"></param>
        /// <param name="animationData"></param>
        /// <param name="frameIndex"></param>
        internal static void SerializeChrAnimation(MetroidRom rom, ChrAnimationLevelData animationData, ref int frameIndex) {
            for (int iAnimation = 0; iAnimation < animationData.Animations.Count; iAnimation++) {
                var animation = animationData.Animations[iAnimation];
                animation.SerializedFrameIndex = frameIndex;
                for (int iFrame = 0; iFrame < animation.Frames.Count; iFrame++) {
                    var frame = animation.Frames[iFrame];

                    rom.SetAnimationTable_Bank0(frameIndex, frame.Bank0);
                    rom.SetAnimationTable_Bank1(frameIndex, frame.Bank1);
                    rom.SetAnimationTable_Bank2(frameIndex, frame.Bank2);
                    rom.SetAnimationTable_Bank3(frameIndex, frame.Bank3);
                    rom.SetAnimationTable_FrameTime(frameIndex, frame.FrameTime);
                    rom.SetAnimationTable_FrameLast(frameIndex, iFrame == animation.Frames.Count - 1);

                    frameIndex++;

                }
            }
        }

        internal static void SerializeChrAnimationTerminator(MetroidRom rom, ref int frameIndex) {
            rom.SetAnimationTable_Bank0(frameIndex, 0xFF);
            rom.SetAnimationTable_Bank1(frameIndex, 0xFF);
            rom.SetAnimationTable_Bank2(frameIndex, 0xFF);
            rom.SetAnimationTable_Bank3(frameIndex, 0xFF);
            rom.SetAnimationTable_FrameTime(frameIndex, 0xFF);
            rom.SetAnimationTable_FrameLast(frameIndex, true);

            frameIndex++;
        }
    }

    /// <summary>
    /// Contains all the data for a ROM's CHR animations
    /// </summary>
    public class ChrAnimationRomData : Dictionary<LevelIndex, ChrAnimationLevelData>
    {
    }
    /// <summary>
    /// Contains all the data for a level's CHR animations
    /// </summary>
    public class ChrAnimationLevelData
    {
        public ChrAnimationLevelData(LevelIndex level) {
            this.Level = level;
        }
        public LevelIndex Level { get; private set; }

        public byte SprBank0 { get; set; }
        public byte SprBank1 { get; set; }

        List<ChrAnimationTable> _Animations = new List<ChrAnimationTable>();
        public List<ChrAnimationTable> Animations { get { return _Animations; } }
    }

    /// <summary>
    /// Contains all the data specific to a CHR animation
    /// </summary>
    public class ChrAnimationTable
    {
        /// <summary>
        /// Name of the animation (only applicable if the ROM has a project)
        /// </summary>
        public string Name { get; set; }
        List<ChrAnimationFrame> _Frames = new List<ChrAnimationFrame>();
        public List<ChrAnimationFrame> Frames { get { return _Frames; } }

        /// <summary>
        /// Index of the first frame of this animation. This field is populated when the animation is serialized, and is for reference only.
        /// </summary>
        public int SerializedFrameIndex { get; set; }
    }

    /// <summary>
    /// Contains all the data specific to a CHR animation frame
    /// </summary>
    public struct ChrAnimationFrame
    {
        public byte Bank0;
        public byte Bank1;
        public byte Bank2;
        public byte Bank3;
        public byte FrameDataByte;

        public int FrameTime {
            get { return FrameDataByte & 0x7F; }
            set {
                if (value > 0x7F) throw new ArgumentException("Invalid frame time value.");
                FrameDataByte = (byte)((FrameDataByte & 0x80) | (value & 0x7F));
            }
        }
    }
}
