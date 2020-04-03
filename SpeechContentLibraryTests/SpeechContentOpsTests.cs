using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpeechContent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechContent.Tests
{
    [TestClass()]
    public class SpeechContentOpsTests
    {
        [TestMethod()]
        public void AddContentTest()
        {
            string txt = "Call of Duty®: Warzone3";
            string url = @"https://docs.microsoft.com/ru-ru/visualstudio/test/unit-test-basics?view=vs-2019
                https://www.google.ru/search?newwindow=1&sxsrf=ALeKk03roJlEjDHCaObPnTgIBNBbstl-6g%3A1584764647328&ei=55Z1Xr3SE47T6AT4u4aoDA&q=windows+clipboard+size&oq=windows+clipboard+size&gs_l=psy-ab.3..0i203l2j0i22i30l8.47832.55345..55644...0.2..0.176.2356.0j14......0....1..gws-wiz.......0i71j0i20i263j0.op5p0saEO6A&ved=0ahUKEwj94eyu3KroAhWOKZoKHfidAcUQ4dUDCAs&uact=5
                https://web.whatsapp.com/";

            SpeechContentOps.Init(@"E:\Google Drive\+SpeechContent", 5);
            SpeechContentOps.AddContent(txt);

            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void AddContentFromClipboardTest()
        {
            SpeechContentOps.AddContentFromClipboard();

            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void GetContentDataTest()
        {
            SpeechContentOps.AddContent("https://3dnews.ru/1006474");
            Task<SpeechContentData> scd = SpeechContentOps.GetContentData();
        }
    }
}