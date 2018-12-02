using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid.ROM.Formats
{
    public abstract class ChrDumper { 
        protected MetroidRom Rom{get; private set;}
        protected LevelIndex LevelId{get; private set;}
        protected Level LevelData {get; private set;}
        protected byte[] ChrBuffer { get; private set; }

        internal ChrDumper(MetroidRom rom, LevelIndex level) {
            this.Rom = rom;
            this.LevelId = level;
            if (level != LevelIndex.None) LevelData = rom.Levels[level];
            ChrBuffer = new byte[0x2000];
        }

        /// <summary>Copies a block of bytes from the specified array to the CHR buffer
        /// with bounds checking. Returns a boolean indicating whether clipping of the
        /// data occured as a result overrunning either the source or destination buffer</summary>
        protected bool CopyChr(int sourceOffset, int destOffset, int byteLength) {
            return CopyChr(Rom.data, sourceOffset, destOffset, byteLength);
        }
        /// <summary>Copies a block of bytes from the specified array to the CHR buffer
        /// with bounds checking. Returns a boolean indicating whether clipping of the
        /// data occured as a result overrunning either the source or destination buffer</summary>
        protected bool CopyChr(byte[] source, int sourceOffset, int destOffset, int byteLength) {
            var clipped = false;

            // Overrun on source (negative)
            if (sourceOffset < 0) {
                destOffset -= sourceOffset;
                byteLength += sourceOffset;
                sourceOffset = 0;
                clipped = true;
            }

            // Overrun on source
            if (sourceOffset + byteLength > source.Length) {
                int diff = (sourceOffset + byteLength) - source.Length;
                byteLength -= diff;
                clipped = false;
            }

            // Overrun on dest (negative) 
            if (destOffset < 0) {
                sourceOffset -= destOffset;
                byteLength += destOffset;
                destOffset = 0;
                clipped = true;
            }

            // Overrun on dest
            if (destOffset + byteLength > ChrBuffer.Length) {
                int diff = (destOffset + byteLength) - ChrBuffer.Length;
                byteLength -= diff;
                clipped = true;
            }

            if (byteLength < 0) {
                return true;
            }

            Array.Copy(source, sourceOffset, ChrBuffer, destOffset, byteLength);
            return clipped;
        }
        protected byte[] ExtractChr(int offset, int size) {
                byte[] result = new byte[size];
                Array.Copy(ChrBuffer, offset, result, 0, size);
                return result;
            
        }

        public abstract ChrDump GetRawPatterns();
    }

    /// <summary>
    /// Provides a means to dump CHR from unexpanded ROMs
    /// </summary>
    public class StandardChrDumper:ChrDumper
    {
        public StandardChrDumper(MetroidRom rom, LevelIndex area)
            : base(rom, area) {
        }


        public override ChrDump GetRawPatterns() {
            // Non-Justin-Bailey graphics (load BG and sprites)
            LoadRawChr_Standard();
            // 4 KB CHR banks
            var sprBlob = ExtractChr(0x0000, 0x1000);
            var bgBlob = ExtractChr(0x1000, 0x1000);

            // Justin-Bailey graphics (load 1/2 page of sprites)
            LoadRawChr_JustinBailey();
            var sprAltBlob = ExtractChr(0x0000, 0x800); // 1/2 page

            byte[][] blobs = new byte[][] { bgBlob, sprBlob, sprAltBlob };
            var bgMeta = new FrameSectionID[] { new FrameSectionID(0) };
            return new ChrDump(blobs, bgMeta, 1, 2, null);
        }

        /// <summary>Loads Justin Bailey tiles over the specified CHR buffer.</summary>
        /// <param name="rawChrBuffer">An 8 KB buffer for CHR data.</param>
        private void LoadRawChr_JustinBailey() {
            var justinBaileyGroup = LevelData.Format.JustinBaileyChrGroup;
            if (justinBaileyGroup == null) return; // nothing to load

            var patternGroupTable = Rom.PatternGroupOffsets;
            var group = patternGroupTable[justinBaileyGroup.Value];
            LoadChrGroup(group);
        }


        /// <summary>Loads this area's tiles over the specified CHR buffer. Does not include Justin
        /// Bailey tiles, and does include title screen tiles.</summary>
        /// <param name="rawChrBuffer">An 8 KB buffer for CHR data.</param>
        private void LoadRawChr_Standard() {
            var patternGroupTable = Rom.PatternGroupOffsets;
            
            // "Global" patterns
            foreach (var groupIndex in Rom.GlobalPatternGroups) {
                if (groupIndex != LevelData.Format.JustinBaileyChrGroup) { // DONT LOAD JUSTIN BAILEY 
                    LoadChrGroup(patternGroupTable[groupIndex]);
                }
            }

            // Area-specific patterns
            if (LevelData != null) {
                foreach (var groupIndex in LevelData.PatternGroups) {
                    if (groupIndex != LevelData.Format.JustinBaileyChrGroup) { // DONT LOAD JUSTIN BAILEY 
                        LoadChrGroup(patternGroupTable[groupIndex]);
                    }
                }
            }
        }

        /// <summary>Loads a referenced CHR group into the CHR data buffer</summary>
        private void LoadChrGroup(PatternGroupOffsets group) {
            int start = group.DestPpuOffset;
            int length = group.TileCount * 0x10;

            var clipped = CopyChr(group.SourceRomOffset, start, length);
            //Array.Copy(rom.data, group.SourceRomOffset, rawChrBuffer, start, length);
            System.Diagnostics.Debug.Print("Clipping occurred.");
        }
    }


    public class ExpandoChrDumper: ChrDumper
    {
        public ExpandoChrDumper(MetroidRom rom, LevelIndex index)
            : base(rom, index) {
        }

        public override ChrDump GetRawPatterns() {
            ExpandoPatternOffsets.Entry bgChrOffsets;
            ExpandoPatternOffsets.Entry areaSprChrOffsets;
            ExpandoPatternOffsets.Entry globalSprOffsets = ExpandoPatternOffsets.GlobalGameplaySprites;
            ExpandoPatternOffsets.Entry justinBaileyOffsets = ExpandoPatternOffsets.JustinBaileySprites;


            if (LevelId == LevelIndex.None) {
                bgChrOffsets = ExpandoPatternOffsets.TitleBgGraphics;
                areaSprChrOffsets = ExpandoPatternOffsets.TitleSpriteGraphics;
            } else {
                bgChrOffsets = ExpandoPatternOffsets.GetBackgroundEntry(LevelId);
                areaSprChrOffsets = ExpandoPatternOffsets.GetSpriteEntry(LevelId);
            }

            LoadTileset(bgChrOffsets);
            LoadTileset(globalSprOffsets);
            LoadTileset(areaSprChrOffsets);
            var sprChr = ExtractChr(0x0000, 0x1000);
            var bgChr = ExtractChr(0x1000, 0x1000);

            LoadTileset(justinBaileyOffsets);
            var altSprChr = ExtractChr(0x0000, 0x800);

            if (LevelId == LevelIndex.None) {
                byte[][] blobs = new byte[][] { bgChr, sprChr };
                var bgMeta = new FrameSectionID[] { new FrameSectionID(0) };
                return new ChrDump(blobs, bgMeta, 1, null, null);
            } else {
                byte[][] blobs = new byte[][] { bgChr, sprChr, altSprChr };
                var bgMeta = new FrameSectionID[] { new FrameSectionID(0) };
                return new ChrDump(blobs, bgMeta, 1, 2, null);
            }
        }

        private void LoadTileset(ExpandoPatternOffsets.Entry location) {
            int destOffset = location.DestTileindex * 0x10;
            if (!location.IsSprite) destOffset += 0x1000;

            CopyChr(location.RomOffset, destOffset, location.TileCount * 0x10);
        }
    }

    
    public struct FrameSectionID {
        /// <summary>Specifies which frame's CHR is being identified. Null indicates the data is frame-agnostic (e.g. from a ROM that doesn't support animation).</summary>
        public int? Frame { get; private set; }
        /// <summary>Specifies which 1 KB section of a frame's CHR is being identified. Null indicates the data does not reference a section (e.g. data might be an entire 4 KB CHR page).</summary>
        public int? Section { get; private set; }
        /// <summary>Specifies which piece of CHR data is being referenced. The meaning of a Null value is context-dependant and may be an invalid value.</summary>
        public int? BlobNumber { get; set; }

        public FrameSectionID(int blob)
            : this() {
            this.BlobNumber = blob;
        }

        public FrameSectionID(int blob, int frame)
            : this() {
            this.BlobNumber = blob;
            this.Frame = frame;
        }

        public FrameSectionID(int blob, int frame, int section)
            : this() {
            this.BlobNumber = blob;
            this.Frame = frame;
            this.Section = section;
        }
    }

    /// <summary>
    /// Contains CHR data and metadata for an area of the game. 
    /// </summary>
    public class ChrDump
    {
        /// <summary>A collection of blocks of CHR data. Each blob should be 1 KB, 2 KB, or 4 KB.</summary>
        public IList<Byte[]> Blobs { get; private set; }
        /// <summary>A collection of blob references to construct pages of background CHR. See remarks.</summary>
        /// <remarks>Each frame must be fully composed, either by one 4KB blob (with a Section value of null), or by
        /// four 1 KB blobs, one associated with each section: 0, 1, 2, and 3. Frames and sections are not required 
        /// to be ordered, but must compose a set of frames whose numbers start at zero and have no gaps in sequence.
        /// There may not be multiple frames defined by the same number.</remarks>
        public IList<FrameSectionID> BgBlobs { get; private set; }
        /// <summary>The primary blob for sprite CHR. Must be either 4 KB or 2 KB.</summary>
        public int SprBlob { get; private set; }
        /// <summary>The second blob for sprite CHR. Must be null if the primary blob
        /// is 4 KB, and must specify a 2 KB blob if the primary blob is 2 KB.</summary>
        public int? Spr2Blob { get; private set; }
        /// <summary>The alternate blob for sprite CHR. Must reference a 2 KB blob.</summary>
        public int? SprAltBlob { get; private set; }

        public ChrDump(IList<Byte[]> blobs, IList<FrameSectionID> bgBlobs, int sprBlob, int? sprAltBlob, int? spr2Blob) {
            this.Blobs = new System.Collections.ObjectModel.ReadOnlyCollection<Byte[]>(blobs);
            this.BgBlobs = new System.Collections.ObjectModel.ReadOnlyCollection<FrameSectionID>(bgBlobs);

            this.SprBlob = sprBlob;
            this.SprAltBlob = sprAltBlob;
            this.Spr2Blob = spr2Blob;

            // Todo: validate blob-size and frame/section-sequence requirements
        }
    }
}
