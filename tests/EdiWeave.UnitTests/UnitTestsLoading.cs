﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EdiWeave.Core.ErrorCodes;
using EdiWeave.Core.Model.Edi;
using EdiWeave.Core.Model.Edi.ErrorContexts;
using EdiWeave.Framework.Readers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EdiWeave.UnitTests
{
    [TestClass]
    public class UnitTestsLoading
    {
        [TestMethod]
        public void TestDuplicateTs()
        {
            // ARRANGE
            const string sample = "EdiWeave.UnitTests.Edi.X12_820_00204.txt";
            var ediStream = CommonHelper.LoadStream(sample, false);
           
            List<EdiItem> ediItems;

            // ACT
            using (var ediReader = new X12Reader(ediStream, "EdiWeave.Rules.X12002040.Rep"))
            {
                ediItems = ediReader.ReadToEnd().ToList();
            }            

            // ASSERT
            var error = ediItems.OfType<ReaderErrorContext>().SingleOrDefault();
            Assert.IsNotNull(error);
            Assert.IsNotNull(error.MessageErrorContext);
            Assert.IsTrue(error.MessageErrorContext.Codes.Contains(MessageErrorCode.TransactionSetNotSupported));
        }

        [TestMethod]
        public void TestMissingTs()
        {
            // ARRANGE
            const string sample = "EdiWeave.UnitTests.Edi.Edifact_INVOIC_D00B.txt";
            var ediStream = CommonHelper.LoadStream(sample, false);

            List<EdiItem> ediItems;

            // ACT
            using (var ediReader = new EdifactReader(ediStream, "EdiWeave.UnitTests"))
            {
                ediItems = ediReader.ReadToEnd().ToList();
            }

            // ASSERT
            var error = ediItems.OfType<ReaderErrorContext>().SingleOrDefault();
            Assert.IsNotNull(error);
            Assert.IsNotNull(error.MessageErrorContext);
            Assert.IsTrue(error.MessageErrorContext.Codes.Contains(MessageErrorCode.TransactionSetNotSupported));
        }

        [TestMethod]
        public void TestMissingAssembly()
        {
            // ARRANGE
            const string sample = "EdiWeave.UnitTests.Edi.Edifact_INVOIC_D00A.txt";
            var ediStream = CommonHelper.LoadStream(sample, false);

            List<EdiItem> ediItems;

            // ACT
            using (var ediReader = new EdifactReader(ediStream, a => Assembly.Load(new AssemblyName("nosuchassembly"))))
            {
                ediItems = ediReader.ReadToEnd().ToList();
            }

            // ASSERT
            var error = ediItems.OfType<ReaderErrorContext>().SingleOrDefault();
            Assert.IsNotNull(error);
            Assert.IsNotNull(error.MessageErrorContext);
            Assert.IsTrue(error.MessageErrorContext.Codes.Contains(MessageErrorCode.TransactionSetNotSupported));
        }
    }
}
