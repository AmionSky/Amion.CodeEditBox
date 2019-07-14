using Amion.CodeEditBox.Document;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.UI.Text.Core;

namespace Amion.CodeEditBox.Tests
{
    [TestClass]
    public class TextActionsTest
    {
        private TextDocument _textDocument;

        public TextActionsTest()
        {
            Helper.ExecuteOnUIThread(() =>
            {
                CoreTextServicesManager manager = CoreTextServicesManager.GetForCurrentView();
                _textDocument = new TextDocument(manager.CreateEditContext());
            }).Wait();
        }

        #region Delete
        [TestMethod]
        public void Test_DeleteSelected()
        {
            _textDocument.SetText("Some Text");
            _textDocument.Selection.SetRange(new SelectionRange { StartPosition = 2, EndPosition = 6 });
            _textDocument.Actions.DeleteSelected();

            Assert.AreEqual("Soext", _textDocument.TextBuffer.Text);
        }

        [TestMethod]
        public void Test_DeletePreviousChar()
        {
            _textDocument.SetText("Some Text");
            _textDocument.Selection.SetRange(new SelectionRange { StartPosition = 6, EndPosition = 6 });
            _textDocument.Actions.DeletePreviousChar();

            Assert.AreEqual("Some ext", _textDocument.TextBuffer.Text);
        }
        #endregion

        #region StepLeft
        [TestMethod]
        public void Test_StepLeft_NoSelection()
        {
            _textDocument.SetText("Some Text");
            _textDocument.Selection.SetRange(new SelectionRange { StartPosition = 6, EndPosition = 6 });
            _textDocument.Actions.StepLeft();

            var expected = new SelectionRange { StartPosition = 5, EndPosition = 5 };
            var actual = _textDocument.Selection.Range;
            Assert.AreEqual(expected.StartPosition, actual.StartPosition, "StartPosition");
            Assert.AreEqual(expected.EndPosition, actual.EndPosition, "EndPosition");
        }

        [TestMethod]
        public void Test_StepLeft_HasSelection()
        {
            _textDocument.SetText("Some Text");
            _textDocument.Selection.SetRange(new SelectionRange { StartPosition = 3, EndPosition = 6 });
            _textDocument.Actions.StepLeft();

            var expected = new SelectionRange { StartPosition = 3, EndPosition = 3 };
            var actual = _textDocument.Selection.Range;
            Assert.AreEqual(expected.StartPosition, actual.StartPosition, "StartPosition");
            Assert.AreEqual(expected.EndPosition, actual.EndPosition, "EndPosition");
        }
        #endregion

        #region StepRight
        [TestMethod]
        public void Test_StepRight_NoSelection()
        {
            _textDocument.SetText("Some Text");
            _textDocument.Selection.SetRange(new SelectionRange { StartPosition = 6, EndPosition = 6 });
            _textDocument.Actions.StepRight();

            var expected = new SelectionRange { StartPosition = 7, EndPosition = 7 };
            var actual = _textDocument.Selection.Range;
            Assert.AreEqual(expected.StartPosition, actual.StartPosition, "StartPosition");
            Assert.AreEqual(expected.EndPosition, actual.EndPosition, "EndPosition");
        }

        [TestMethod]
        public void Test_StepRight_HasSelection()
        {
            _textDocument.SetText("Some Text");
            _textDocument.Selection.SetRange(new SelectionRange { StartPosition = 3, EndPosition = 6 });
            _textDocument.Actions.StepRight();

            var expected = new SelectionRange { StartPosition = 6, EndPosition = 6 };
            var actual = _textDocument.Selection.Range;
            Assert.AreEqual(expected.StartPosition, actual.StartPosition, "StartPosition");
            Assert.AreEqual(expected.EndPosition, actual.EndPosition, "EndPosition");
        }
        #endregion

        #region AdjustSelectionLeft
        [TestMethod]
        public void Test_AdjustSelectionLeft_NoSelection()
        {
            _textDocument.SetText("Some Text");
            _textDocument.Selection.SetRange(new SelectionRange { StartPosition = 4, EndPosition = 4 });
            _textDocument.Actions.AdjustSelectionLeft();

            var expected = new SelectionRange { StartPosition = 3, EndPosition = 4 };
            var actual = _textDocument.Selection.Range;
            Assert.AreEqual(expected.StartPosition, actual.StartPosition, "StartPosition");
            Assert.AreEqual(expected.EndPosition, actual.EndPosition, "EndPosition");
        }

        [TestMethod]
        public void Test_AdjustSelectionLeft_LeftExtended()
        {
            _textDocument.SetText("Some Text");
            _textDocument.Selection.SetRange(new SelectionRange { StartPosition = 3, EndPosition = 4 }, true);
            _textDocument.Actions.AdjustSelectionLeft();

            var expected = new SelectionRange { StartPosition = 2, EndPosition = 4 };
            var actual = _textDocument.Selection.Range;
            Assert.AreEqual(expected.StartPosition, actual.StartPosition, "StartPosition");
            Assert.AreEqual(expected.EndPosition, actual.EndPosition, "EndPosition");
        }

        [TestMethod]
        public void Test_AdjustSelectionLeft_RightExtended()
        {
            _textDocument.SetText("Some Text");
            _textDocument.Selection.SetRange(new SelectionRange { StartPosition = 4, EndPosition = 5 });
            _textDocument.Actions.AdjustSelectionLeft();

            var expected = new SelectionRange { StartPosition = 4, EndPosition = 4 };
            var actual = _textDocument.Selection.Range;
            Assert.AreEqual(expected.StartPosition, actual.StartPosition, "StartPosition");
            Assert.AreEqual(expected.EndPosition, actual.EndPosition, "EndPosition");
        }
        #endregion

        #region AdjustSelectionRight
        [TestMethod]
        public void Test_AdjustSelectionRight_NoSelection()
        {
            _textDocument.SetText("Some Text");
            _textDocument.Selection.SetRange(new SelectionRange { StartPosition = 4, EndPosition = 4 });
            _textDocument.Actions.AdjustSelectionRight();

            var expected = new SelectionRange { StartPosition = 4, EndPosition = 5 };
            var actual = _textDocument.Selection.Range;
            Assert.AreEqual(expected.StartPosition, actual.StartPosition, "StartPosition");
            Assert.AreEqual(expected.EndPosition, actual.EndPosition, "EndPosition");
        }

        [TestMethod]
        public void Test_AdjustSelectionRight_LeftExtended()
        {
            _textDocument.SetText("Some Text");
            _textDocument.Selection.SetRange(new SelectionRange { StartPosition = 3, EndPosition = 4 }, true);
            _textDocument.Actions.AdjustSelectionRight();

            var expected = new SelectionRange { StartPosition = 4, EndPosition = 4 };
            var actual = _textDocument.Selection.Range;
            Assert.AreEqual(expected.StartPosition, actual.StartPosition, "StartPosition");
            Assert.AreEqual(expected.EndPosition, actual.EndPosition, "EndPosition");
        }

        [TestMethod]
        public void Test_AdjustSelectionRight_RightExtended()
        {
            _textDocument.SetText("Some Text");
            _textDocument.Selection.SetRange(new SelectionRange { StartPosition = 4, EndPosition = 5 });
            _textDocument.Actions.AdjustSelectionRight();

            var expected = new SelectionRange { StartPosition = 4, EndPosition = 6 };
            var actual = _textDocument.Selection.Range;
            Assert.AreEqual(expected.StartPosition, actual.StartPosition, "StartPosition");
            Assert.AreEqual(expected.EndPosition, actual.EndPosition, "EndPosition");
        }
        #endregion
    }
}
