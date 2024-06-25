using Application.Interfaces;
using iTextSharp.text.pdf;

namespace Application.Services
{
    public class PdfReaderPaginatorService : IPdfReaderService
    {
        public int GetPageCount(Stream filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            using (var pdfReader = new PdfReader(filePath))
            {
                return pdfReader.NumberOfPages;
            }
        }
    }

}
