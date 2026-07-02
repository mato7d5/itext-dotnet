using System;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Test;
using NUnit.Framework;

namespace iText.Pts.Tests {
    public class TagPTest : ExtendedITextTest {

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/pts/TagPTest/";

        [OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [Test]
        public virtual void CreateTaggedPdfWithSingleParagraphTest() {
            String outFileName = DESTINATION_FOLDER + "taggedSingleParagraph.pdf";

            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            pdfDocument.SetTagged();

            Document document = new Document(pdfDocument);
            document.Add(new Paragraph("ahoj svet"));
            document.Close();

            Assert.IsTrue(new System.IO.FileInfo(outFileName).Exists);
        }
    }
}
