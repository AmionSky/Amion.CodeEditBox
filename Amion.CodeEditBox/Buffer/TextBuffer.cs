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
    class TextBuffer
    {
        /// <summary>
        /// Gets the stored text.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// The Length in text elements.
        /// </summary>
        public int ElementLength => _elementIndexes.Length;

        /// <summary>
        /// The length in chars.
        /// </summary>
        public int CharLength => Text.Length;

        // Cached starting indexes of the text elements
        private int[] _elementIndexes;

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
             _elementIndexes = StringInfo.ParseCombiningCharacters(newText);
        }

        /// <summary>
        /// Get a substring by text element index.
        /// </summary>
        /// <param name="startIndex">The zero-based starting text element index.</param>
        public string SubstringByTextElements(int startElement)
        {
            if (startElement >= ElementLength) return string.Empty;
            return Text.Substring(GetCharIndexByElementIndex(startElement));
        }

        /// <summary>
        /// Get a substring by text element index and length.
        /// </summary>
        /// <param name="start">The zero-based starting text element index.</param>
        /// <param name="length">The number of text elements in the substring.</param>
        public string SubstringByTextElements(int startElement, int length)
        {
            if (startElement >= ElementLength) return string.Empty;
            int startIndex = GetCharIndexByElementIndex(startElement);
            int endIndex = GetCharIndexByElementIndex(startElement + length);
            return Text.Substring(startIndex, endIndex - startIndex);
        }

        /// <summary>
        /// Inserts text into the buffer by character indexes.
        /// </summary>
        /// <param name="text">The text to insert</param>
        /// <param name="start">The starting index in char.</param>
        /// <param name="end">The ending index in char.</param>
        public void InsertByCharIndex(string newText, int startIndex, int endIndex)
        {
            SetText(Text.Substring(0, startIndex) + newText + Text.Substring(endIndex));
        }

        /// <summary>
        /// Inserts text into the buffer by text element indexes.
        /// </summary>
        /// <param name="text">The text to insert</param>
        /// <param name="start">The starting index in text elements.</param>
        /// <param name="end">The ending index in text elements.</param>
        public void InsertByTextElements(string newText, int startElement, int endElement)
        {
            SetText(SubstringByTextElements(0, startElement) + newText + SubstringByTextElements(endElement));
        }

        /// <summary>
        /// Converts the SelectionRange to CoreTextRange.
        /// </summary>
        /// <param name="range">The range to convert</param>
        /// <returns>A new range in character indexes.</returns>
        public CoreTextRange SelectionRangeToTextRange(SelectionRange range)
        {
            if (ElementLength == 0)
            {
                return new CoreTextRange();
            }
            else
            {
                return new CoreTextRange()
                {
                    StartCaretPosition = GetCharIndexByElementIndex(range.StartPosition),
                    EndCaretPosition = GetCharIndexByElementIndex(range.EndPosition)
                };
            }
        }

        private int GetCharIndexByElementIndex(int index)
        {
            if (index < ElementLength)
            {
                return _elementIndexes[index];
            }
            else if (index == ElementLength)
            {
                return GetAfterLastIndex();
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }

        private int GetAfterLastIndex()
        {
            if (ElementLength == 0) return 0;
            int lastCharLength = StringInfo.GetNextTextElement(Text, ElementLength - 1).Length;
            return _elementIndexes[ElementLength - 1] + lastCharLength;
        }

        public SelectionRange TextRangeToSelectionRange(CoreTextRange range)
        {
            int start = 0;
            int end = 0;

            if (range.StartCaretPosition == CharLength) start = GetAfterLastIndex();
            if (range.EndCaretPosition == CharLength) end = GetAfterLastIndex();

            bool searchingStart = start == 0;
            bool searchingEnd = end == 0;

            foreach (int charIndex in _elementIndexes)
            {
                if (searchingStart)
                {
                    searchingStart = charIndex <= range.StartCaretPosition;
                    if (searchingStart) start = charIndex;
                }

                if (searchingEnd)
                {
                    searchingEnd = charIndex <= range.EndCaretPosition;
                    if (searchingEnd) end = charIndex;
                }

                if (!(searchingStart || searchingEnd)) break;
            }

            return new SelectionRange()
            {
                StartPosition = start,
                EndPosition = end
            };
        }
    }
}
