using System;

namespace _1_2D_Top_Down
{
    public sealed class PlayerAnimationDefinition
    {
        public string TextureAsset { get; }
        public int SheetColumns { get; }
        public int SheetRows { get; }
        public int FrameCount { get; }
        public float FrameDuration { get; }
        public bool Loop { get; }
        private readonly int[] frameCountsByRow;

        public PlayerAnimationDefinition(
            string textureAsset,
            int sheetColumns,
            int sheetRows,
            int frameCount,
            float frameDuration,
            bool loop = true,
            int[] frameCountsByRow = null)
        {
            if (string.IsNullOrWhiteSpace(textureAsset))
                throw new ArgumentException(
                    "A texture asset is required.",
                    nameof(textureAsset));
            if (sheetColumns <= 0)
                throw new ArgumentOutOfRangeException(nameof(sheetColumns));
            if (sheetRows <= 0)
                throw new ArgumentOutOfRangeException(nameof(sheetRows));
            if (frameCount <= 0 || frameCount > sheetColumns)
                throw new ArgumentOutOfRangeException(nameof(frameCount));
            if (frameDuration <= 0f)
                throw new ArgumentOutOfRangeException(nameof(frameDuration));
            if (frameCountsByRow != null)
            {
                if (frameCountsByRow.Length != sheetRows)
                {
                    throw new ArgumentException(
                        "Directional frame counts must match the sheet rows.",
                        nameof(frameCountsByRow));
                }

                foreach (int directionalFrameCount in frameCountsByRow)
                {
                    if (directionalFrameCount <= 0 ||
                        directionalFrameCount > sheetColumns)
                    {
                        throw new ArgumentOutOfRangeException(
                            nameof(frameCountsByRow));
                    }
                }
            }

            TextureAsset = textureAsset;
            SheetColumns = sheetColumns;
            SheetRows = sheetRows;
            FrameCount = frameCount;
            FrameDuration = frameDuration;
            Loop = loop;
            this.frameCountsByRow = frameCountsByRow == null
                ? null
                : (int[])frameCountsByRow.Clone();
        }

        public int GetFrameCount(int row)
        {
            if (row < 0 || row >= SheetRows)
                throw new ArgumentOutOfRangeException(nameof(row));

            return frameCountsByRow?[row] ?? FrameCount;
        }
    }
}
