using System.Collections.Generic;
using IDE.Core.Storage;

namespace IDE.Documents.Views
{
    public static class DocumentSizeTemplates
    {
        public static List<DocumentSizeTemplate> GetTemplates()
        {
            return new List<DocumentSizeTemplate>
            {
                 new DocumentSizeTemplate
                 {
                     DocumentSize = DocumentSize.A4,
                     DocumentWidth = 297,
                     DocumentHeight = 210
                 },
                 new DocumentSizeTemplate
                 {
                     DocumentSize = DocumentSize.A3,
                     DocumentWidth = 420,
                     DocumentHeight = 297
                 },
                 new DocumentSizeTemplate
                 {
                     DocumentSize = DocumentSize.A2,
                     DocumentWidth = 594,
                     DocumentHeight = 420
                 },
                 new DocumentSizeTemplate
                 {
                     DocumentSize = DocumentSize.A1,
                     DocumentWidth = 841,
                     DocumentHeight = 594
                 },
                 new DocumentSizeTemplate
                 {
                     DocumentSize = DocumentSize.A0,
                     DocumentWidth = 1189,
                     DocumentHeight = 841
                 },
                 new DocumentSizeTemplate
                 {
                     DocumentSize = DocumentSize.Letter,
                     DocumentWidth = 279,
                     DocumentHeight = 216
                 },
                 new DocumentSizeTemplate
                 {
                     DocumentSize = DocumentSize.Custom,
                     DocumentWidth = 297,
                     DocumentHeight = 210
                 },
            };
        }
    }
}
