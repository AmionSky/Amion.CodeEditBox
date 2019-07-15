using Amion.CodeEditBox.Document;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Text.Core;

namespace Amion.CodeEditBox.Buffer
{
    class TextBuffer : ITextBuffer
    {
        /// <summary>
        /// Gets the stored text.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// The Length in text elements.
        /// </summary>
        public int ElementLength => _charIndexes.Length;

        /// <summary>
        /// The length in chars.
        /// </summary>
        public int CharLength => Text.Length;

        // Cached starting indexes of the text elements in characters
        private int[] _charIndexes;

        public TextBuffer()
        {
            SetText(string.Empty);
        }

        /// <summary>
        /// Set the text in the buffer. Each time it called, it recalculates the text element indexes
        /// </summary>
        /// <param name="newText"></param>
        public void SetText(string newText)
        {
            Text = newText;
             _charIndexes = StringInfo.ParseCombiningCharacters(newText);
        }

        /// <summary>
        /// Inserts text into the buffer by character indexes.
        /// </summary>
        /// <param name="text">The text to insert</param>
        /// <param name="start">The starting index in char.</param>
        /// <param name="end">The ending index in char.</param>
        public void Insert(string newText, int startIndex, int endIndex)
        {
            SetText(Text.Substring(0, startIndex) + newText + Text.Substring(endIndex));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentIndex">Current character index</param>
        /// <param name="offset">Offset in text elements</param>
        /// <returns></returns>
        public int GetIndexWithElementOffset(int currentIndex, int offset)
        {
            int index = 0;

            if (currentIndex != CharLength)
            {
                // Get the text element index (index) of the specified character index (currentIndex).
                for (int elementIndex = 0; elementIndex < ElementLength; elementIndex++)
                {
                    if (_charIndexes[elementIndex] <= currentIndex)
                        index = elementIndex;
                    else break;
                }
            }
            else
            {
                // If currentIndex is the last character just set the index to the last element.
                index = ElementLength;
            }

            // Offset the text element index by 'offset' and clamp it. Max can be ElementLength
            // if we want the index after the last element (Ex: for selection).
            int desiredElement = Math.Clamp(index + offset, 0, ElementLength);

            // Return the character index of the text element
            return GetCharIndexByElementIndex(desiredElement);
        }

        private int GetCharIndexByElementIndex(int index)
        {
            if (index < ElementLength)
            {
                return _charIndexes[index];
            }
            else if (index == ElementLength)
            {
                if (ElementLength == 0) return 0;
                int lastCharIndex = _charIndexes[ElementLength - 1];
                int lastCharLength = StringInfo.GetNextTextElement(Text, lastCharIndex).Length;
                return lastCharIndex + lastCharLength;
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }
    }
}
