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
            list.Add(dramaItem);

            // Nested list as direct child of L (sibling of LI, not inside LBody)
            var dramaList = new iText.Layout.Element.List();
            dramaList.Add(CreateBookItem("\"To Kill a Mockingbird\" \u2013 Harper Lee"));
            dramaList.Add(CreateBookItem("\"The Great Gatsby\" \u2013 F. Scott Fitzgerald"));
            list.Add(dramaList);

            // Category: Science Fiction
            var sciFiItem = new ListItem();
            sciFiItem.Add(new Paragraph(new Text("Science Fiction").SetNeutralRole()).SetNeutralRole());
            list.Add(sciFiItem);

            // Nested list as direct child of L
            var sciFiList = new iText.Layout.Element.List();
            sciFiList.Add(CreateBookItem("\"Dune\" \u2013 Frank Herbert"));
            sciFiList.Add(CreateBookItem("\"Neuromancer\" \u2013 William Gibson"));
            list.Add(sciFiList);

            // Category: Crime
            var crimeItem = new ListItem();
            crimeItem.Add(new Paragraph(new Text("Crime").SetNeutralRole()).SetNeutralRole());
            list.Add(crimeItem);

            // Nested list as direct child of L
            var crimeList = new iText.Layout.Element.List();
            crimeList.Add(CreateBookItem("\"The Girl with the Dragon Tattoo\" \u2013 Stieg Larsson"));
            crimeList.Add(CreateBookItem("\"Gone Girl\" \u2013 Gillian Flynn"));
            list.Add(crimeList);

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

        [Test]
        public virtual void CreateTaggedPdfWithNestedListTest() {
            String outFileName = DESTINATION_FOLDER + "taggedNestedList.pdf";

            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            pdfDocument.SetTagged();

            Document document = new Document(pdfDocument);

            // Outer list L with Caption as a direct child
            var outerList = new iText.Layout.Element.List();
            outerList.SetListSymbol("-");

            // Caption as direct child of L
            var caption = new Paragraph(new Text("My Library").SetNeutralRole());
            caption.GetAccessibilityProperties().SetRole(StandardRoles.CAPTION);
            outerList.Add(caption);

            // Category: Drama books
            var dramaItem = new ListItem();
            dramaItem.Add(new Paragraph(new Text("Drama books").SetNeutralRole()).SetNeutralRole());
            outerList.Add(dramaItem);

            // Nested list as direct child of L (sibling of LI, not inside LBody)
            var dramaList = new iText.Layout.Element.List();
            dramaList.Add(CreateBookItem("\"To Kill a Mockingbird\" \u2013 Harper Lee"));
            dramaList.Add(CreateBookItem("\"The Great Gatsby\" \u2013 F. Scott Fitzgerald"));
            outerList.Add(dramaList);

            // Category: Science Fiction
            var sciFiItem = new ListItem();
            sciFiItem.Add(new Paragraph(new Text("Science Fiction").SetNeutralRole()).SetNeutralRole());
            outerList.Add(sciFiItem);

            // Nested list as direct child of L
            var sciFiList = new iText.Layout.Element.List();
            sciFiList.Add(CreateBookItem("\"Dune\" \u2013 Frank Herbert"));
            sciFiList.Add(CreateBookItem("\"Neuromancer\" \u2013 William Gibson"));
            outerList.Add(sciFiList);

            // Category: Crime
            var crimeItem = new ListItem();
            crimeItem.Add(new Paragraph(new Text("Crime").SetNeutralRole()).SetNeutralRole());
            outerList.Add(crimeItem);

            // Nested list as direct child of L
            var crimeList = new iText.Layout.Element.List();
            crimeList.Add(CreateBookItem("\"The Girl with the Dragon Tattoo\" \u2013 Stieg Larsson"));
            crimeList.Add(CreateBookItem("\"Gone Girl\" \u2013 Gillian Flynn"));
            outerList.Add(crimeList);

            document.Add(outerList);

            document.Close();

            Assert.IsTrue(new System.IO.FileInfo(outFileName).Exists);
        }
    }
}
