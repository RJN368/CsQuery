﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using CollectionAssert = NUnit.Framework.CollectionAssert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.HtmlParser;
using CsQuery.Utility;

namespace CsQuery.Tests.Csharp.HtmlParser
{

    [TestFixture, TestClass]
    public class CharacterSetEncoding : CsQueryTest
    {


        [TestMethod, Test]
        public void MetaTag()
        {
            string htmlStart = @"<html><head>";
            string htmlStart2 = @"<META HTTP-EQUIV='Content-Type' content='text/html;charset=windows-1255'>";
            string htmlStart3 = @"</head><body><div id=test>";
            string htmlEnd = "</div></body></html>";

            var encoder = Encoding.GetEncoding("windows-1255");
            var hebrewChar = (char)164;

            var html = htmlStart + htmlStart2 + htmlStart3 +hebrewChar + htmlEnd;
            var htmlNoRecode = htmlStart + htmlStart3 + hebrewChar + htmlEnd;


            // read it in again, but without encoding

            //encoded.Position = 0;

            var dom = CQ.Create(html);
            var output = dom.Render(OutputFormatters.HtmlEncodingMinimum);

            // FINALLY  -- grab the character from CsQuery's output, and ensure that this all worked out.
            
            var outputHebrewChar = dom["#test"].Text();

            //  write the string to an encoded stream  to get the correct encoding manually
            MemoryStream encoded = new MemoryStream();
            var writer = new StreamWriter(encoded, encoder);
            writer.Write(html);
            writer.Flush();

            encoded.Position = 0;
            string htmlHebrew = new StreamReader(encoded, encoder).ReadToEnd();
            var sourceHebrewChar = htmlHebrew.Substring(htmlHebrew.IndexOf("test>") + 5, 1);

            Assert.AreEqual(sourceHebrewChar, outputHebrewChar);

            // Try this again WITHOUT charset encoding.

            var clone = CQ.Create(htmlNoRecode);
            string newHtml = clone.Render(OutputFormatters.HtmlEncodingMinimum);
            var dom2 = CQ.Create(newHtml);

            outputHebrewChar = dom2["#test"].Text();
            Assert.AreNotEqual(sourceHebrewChar, outputHebrewChar);

            var reader = new StreamReader(encoded);
            string interim = reader.ReadToEnd();

            encoded = new MemoryStream();
            writer = new StreamWriter(encoded, Encoding.UTF8);
            writer.Write(interim);
            writer.Flush();


            // read it back one more time

            encoded.Position = 0;
            reader = new StreamReader(encoded);
            string final = reader.ReadToEnd();


//            var encoded = encoder.GetBytes(html);



        }



    }
}