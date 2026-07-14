using System;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;
using NUnit.Framework;

namespace iText.Pts.Tests {
    public class TagTest : ExtendedITextTest {

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

        [Test]
        public virtual void CreateTaggedPdfWithHierarchicalListTest() {
            String outFileName = DESTINATION_FOLDER + "taggedHierarchicalList.pdf";

            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            pdfDocument.SetTagged();

            Document document = new Document(pdfDocument);

            // Outer list L (Div with role L so we can add Caption as a child)
            var outerList = new Div();
            outerList.GetAccessibilityProperties().SetRole(StandardRoles.L);

            // Caption as first child of L
            var caption = new Paragraph(new Text("My Library").SetNeutralRole());
            caption.GetAccessibilityProperties().SetRole(StandardRoles.CAPTION);
            outerList.Add(caption);

            // List with neutral role so its children become direct children of L
            var list = new iText.Layout.Element.List();
            list.SetListSymbol("-");
            list.SetNeutralRole();

            // Category: Drama books
            var dramaItem = new ListItem();
            dramaItem.Add(new Paragraph(new Text("Drama books").SetNeutralRole()).SetNeutralRole());

            var dramaList = new iText.Layout.Element.List();
            dramaList.Add(CreateBookItem("\"To Kill a Mockingbird\" \u2013 Harper Lee"));
            dramaList.Add(CreateBookItem("\"The Great Gatsby\" \u2013 F. Scott Fitzgerald"));
            dramaItem.Add(dramaList);

            list.Add(dramaItem);

            // Category: Science Fiction
            var sciFiItem = new ListItem();
            sciFiItem.Add(new Paragraph(new Text("Science Fiction").SetNeutralRole()).SetNeutralRole());

            var sciFiList = new iText.Layout.Element.List();
            sciFiList.Add(CreateBookItem("\"Dune\" \u2013 Frank Herbert"));
            sciFiList.Add(CreateBookItem("\"Neuromancer\" \u2013 William Gibson"));
            sciFiItem.Add(sciFiList);

            list.Add(sciFiItem);

            // Category: Crime
            var crimeItem = new ListItem();
            crimeItem.Add(new Paragraph(new Text("Crime").SetNeutralRole()).SetNeutralRole());

            var crimeList = new iText.Layout.Element.List();
            crimeList.Add(CreateBookItem("\"The Girl with the Dragon Tattoo\" \u2013 Stieg Larsson"));
            crimeList.Add(CreateBookItem("\"Gone Girl\" \u2013 Gillian Flynn"));
            crimeItem.Add(crimeList);

            list.Add(crimeItem);

            outerList.Add(list);

            document.Add(outerList);

            document.Close();

            Assert.IsTrue(new System.IO.FileInfo(outFileName).Exists);
        }

        private static ListItem CreateBookItem(String text) {
            var item = new ListItem();
            item.Add(new Paragraph(new Text(text).SetNeutralRole()).SetNeutralRole());
            return item;
        }
    }
}
