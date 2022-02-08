using IDE.Core.Importers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace IDE.Core.Model.Tests.Importers.Dxf
{
    public class DxfImporterTests
    {
        [Fact]
        public void RunImport()
        {
            var filePath = @"_Files\Dxf\outline.dxf";

            var importer = new DxfImporter();


            var r = importer.Import(filePath);

            Assert.True(r.Count > 0);
        }
    }
}
