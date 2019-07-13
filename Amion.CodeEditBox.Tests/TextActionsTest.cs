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
            _textDocument.Selection.SetRange(new CoreTextRange { StartCaretPosition = 2, EndCaretPosition = 6 });
            _textDocument.Actions.DeleteSelected();

            Assert.AreEqual("Soext", _textDocument.Text);
        }

        [TestMethod]
        public void Test_DeletePreviousChar()
        {
            _textDocument.SetText("Some Text");
            _textDocument.Selection.SetRange(new CoreTextRange { StartCaretPosition = 6, EndCaretPosition = 6 });
            _textDocument.Actions.DeletePreviousChar();

            Assert.AreEqual("Some ext", _textDocument.Text);
        }
        #endregion

        #region StepLeft
        [TestMethod]
        public void Test_StepLeft_NoSelection()
        {
            _textDocument.SetText("Some Text");
            _textDocument.Selection.SetRange(new CoreTextRange { StartCaretPosition = 6, EndCaretPosition = 6 });
            _textDocument.Actions.StepLeft();

            var expected = new CoreTextRange { StartCaretPosition = 5, EndCaretPosition = 5 };
            var actual = _textDocument.Selection.Range;
            Assert.AreEqual(expected.StartCaretPosition, actual.StartCaretPosition, "StartCaretPosition");
            Assert.AreEqual(expected.EndCaretPosition, actual.EndCaretPosition, "EndCaretPosition");
        }

        [TestMethod]
        public void Test_StepLeft_HasSelection()
        {
            _textDocument.SetText("Some Text");
            _textDocument.Selection.SetRange(new CoreTextRange { StartCaretPosition = 3, EndCaretPosition = 6 });
            _textDocument.Actions.StepLeft();

            var expected = new CoreTextRange { StartCaretPosition = 3, EndCaretPosition = 3 };
            var actual = _textDocument.Selection.Range;
            Assert.AreEqual(expected.StartCaretPosition, actual.StartCaretPosition, "StartCaretPosition");
            Assert.AreEqual(expected.EndCaretPosition, actual.EndCaretPosition, "EndCaretPosition");
        }
        #endregion

        #region StepRight
        [TestMethod]
        public void Test_StepRight_NoSelection()
        {
            _textDocument.SetText("Some Text");
            _textDocument.Selection.SetRange(new CoreTextRange { StartCaretPosition = 6, EndCaretPosition = 6 });
            _textDocument.Actions.StepRight();

            var expected = new CoreTextRange { StartCaretPosition = 7, EndCaretPosition = 7 };
            var actual = _textDocument.Selection.Range;
            Assert.AreEqual(expected.StartCaretPosition, actual.StartCaretPosition, "StartCaretPosition");
            Assert.AreEqual(expected.EndCaretPosition, actual.EndCaretPosition, "EndCaretPosition");
        }

        [TestMethod]
        public void Test_StepRight_HasSelection()
        {
            _textDocument.SetText("Some Text");
            _textDocument.Selection.SetRange(new CoreTextRange { StartCaretPosition = 3, EndCaretPosition = 6 });
            _textDocument.Actions.StepRight();

            var expected = new CoreTextRange { StartCaretPosition = 6, EndCaretPosition = 6 };
            var actual = _textDocument.Selection.Range;
            Assert.AreEqual(expected.StartCaretPosition, actual.StartCaretPosition, "StartCaretPosition");
            Assert.AreEqual(expected.EndCaretPosition, actual.EndCaretPosition, "EndCaretPosition");
        }
        #endregion

        #region AdjustSelectionLeft
        [TestMethod]
        public void Test_AdjustSelectionLeft_NoSelection()
        {
            _textDocument.SetText("Some Text");
            _textDocument.Selection.SetRange(new CoreTextRange { StartCaretPosition = 4, EndCaretPosition = 4 });
            _textDocument.Actions.AdjustSelectionLeft();

            var expected = new CoreTextRange { StartCaretPosition = 3, EndCaretPosition = 4 };
            var actual = _textDocument.Selection.Range;
            Assert.AreEqual(expected.StartCaretPosition, actual.StartCaretPosition, "StartCaretPosition");
            Assert.AreEqual(expected.EndCaretPosition, actual.EndCaretPosition, "EndCaretPosition");
        }

        [TestMethod]
        public void Test_AdjustSelectionLeft_LeftExtended()
        {
            _textDocument.SetText("Some Text");
            _textDocument.Selection.SetRange(new CoreTextRange { StartCaretPosition = 3, EndCaretPosition = 4 }, true);
            _textDocument.Actions.AdjustSelectionLeft();

            var expected = new CoreTextRange { StartCaretPosition = 2, EndCaretPosition = 4 };
            var actual = _textDocument.Selection.Range;
            Assert.AreEqual(expected.StartCaretPosition, actual.StartCaretPosition, "StartCaretPosition");
            Assert.AreEqual(expected.EndCaretPosition, actual.EndCaretPosition, "EndCaretPosition");
        }

        [TestMethod]
        public void Test_AdjustSelectionLeft_RightExtended()
        {
            _textDocument.SetText("Some Text");
            _textDocument.Selection.SetRange(new CoreTextRange { StartCaretPosition = 4, EndCaretPosition = 5 });
            _textDocument.Actions.AdjustSelectionLeft();

            var expected = new CoreTextRange { StartCaretPosition = 4, EndCaretPosition = 4 };
            var actual = _textDocument.Selection.Range;
            Assert.AreEqual(expected.StartCaretPosition, actual.StartCaretPosition, "StartCaretPosition");
            Assert.AreEqual(expected.EndCaretPosition, actual.EndCaretPosition, "EndCaretPosition");
        }
        #endregion

        #region AdjustSelectionRight
        [TestMethod]
        public void Test_AdjustSelectionRight_NoSelection()
        {
            _textDocument.SetText("Some Text");
            _textDocument.Selection.SetRange(new CoreTextRange { StartCaretPosition = 4, EndCaretPosition = 4 });
            _textDocument.Actions.AdjustSelectionRight();

            var expected = new CoreTextRange { StartCaretPosition = 4, EndCaretPosition = 5 };
            var actual = _textDocument.Selection.Range;
            Assert.AreEqual(expected.StartCaretPosition, actual.StartCaretPosition, "StartCaretPosition");
            Assert.AreEqual(expected.EndCaretPosition, actual.EndCaretPosition, "EndCaretPosition");
        }

        [TestMethod]
        public void Test_AdjustSelectionRight_LeftExtended()
        {
            _textDocument.SetText("Some Text");
            _textDocument.Selection.SetRange(new CoreTextRange { StartCaretPosition = 3, EndCaretPosition = 4 }, true);
            _textDocument.Actions.AdjustSelectionRight();

            var expected = new CoreTextRange { StartCaretPosition = 4, EndCaretPosition = 4 };
            var actual = _textDocument.Selection.Range;
            Assert.AreEqual(expected.StartCaretPosition, actual.StartCaretPosition, "StartCaretPosition");
            Assert.AreEqual(expected.EndCaretPosition, actual.EndCaretPosition, "EndCaretPosition");
        }

        [TestMethod]
        public void Test_AdjustSelectionRight_RightExtended()
        {
            _textDocument.SetText("Some Text");
            _textDocument.Selection.SetRange(new CoreTextRange { StartCaretPosition = 4, EndCaretPosition = 5 });
            _textDocument.Actions.AdjustSelectionRight();

            var expected = new CoreTextRange { StartCaretPosition = 4, EndCaretPosition = 6 };
            var actual = _textDocument.Selection.Range;
            Assert.AreEqual(expected.StartCaretPosition, actual.StartCaretPosition, "StartCaretPosition");
            Assert.AreEqual(expected.EndCaretPosition, actual.EndCaretPosition, "EndCaretPosition");
        }
        #endregion
    }
}
