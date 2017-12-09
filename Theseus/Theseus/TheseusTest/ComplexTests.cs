using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sprache;
using System.IO;
using System.Reflection;
using Theseus.Elements;
using Theseus.Interfaces;
using Theseus.Parser;

namespace TheseusTest
{
    [TestClass]
    public class ComplexTests
    {
        [TestMethod]
        public void ArtSectionOK()
        {
            VerifyLocation("ArtSection.txt", "artSection");
        }

        [TestMethod]
        public void BathroomOK()
        {
            VerifyLocation("Bathroom.txt", "bathroom");
        }

        [TestMethod]
        public void FictionSectionOK()
        {
            VerifyLocation("FictionSection.txt", "fictionSection");
        }

        [TestMethod]
        public void HistorySectionOK()
        {
            VerifyLocation("HistorySection.txt", "historySection");
        }

        [TestMethod]
        public void KitchenOK()
        {
            VerifyLocation("Kitchen.txt", "kitchen");
        }

        [TestMethod]
        public void OfficeOK()
        {
            VerifyLocation("Office.txt", "office");
        }

        [TestMethod]
        public void TravelSectionOK()
        {
            VerifyLocation("TravelSection.txt", "travelSection");
        }

        [TestMethod]
        public void UncleAilbertOK()
        {
            VerifyCharacter("UncleAilbert.txt", "uncleAilbert");
        }

        private string LoadFile(string fileName)
        {
            var f = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + fileName;
            return File.ReadAllText(f);
        }

        private void VerifyLocation(string fileName, string locationName)
        {
            var code = LoadFile(fileName);
            var document = TheseusParser.DocumentParser.Parse(code);
            Assert.IsInstanceOfType(document, typeof(Location));
            Assert.AreEqual(locationName, (document as Location).Name);
            Assert.AreEqual(code, (document as ITheseusCodeEmitter)?.EmitTheseusCode());
        }

        private void VerifyCharacter(string fileName, string characterName)
        {
            var code = LoadFile(fileName);
            var document = TheseusParser.DocumentParser.Parse(code);
            Assert.IsInstanceOfType(document, typeof(Character));
            Assert.AreEqual(characterName, (document as Character).Name);
            Assert.AreEqual(code, (document as ITheseusCodeEmitter)?.EmitTheseusCode());
        }
    }
}
