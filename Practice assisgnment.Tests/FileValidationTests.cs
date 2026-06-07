
using Xunit;
using Microsoft.AspNetCore.Http;

namespace Practice_assignment.Tests
{
        // Tests file upload validation logic.
        public class FileValidationTests
        {
            // Mirrors the exact validation logic in FileService.SaveSignedAgreementAsync
            private static readonly string[] AllowedExtensions = { ".pdf" };
            private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

            private static void ValidateFile(IFormFile? file)
            {
                if (file == null || file.Length == 0)
                    throw new InvalidOperationException("No file was uploaded.");

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!AllowedExtensions.Contains(extension))
                    throw new InvalidOperationException(
                        $"Only PDF files are allowed. You uploaded: '{extension}'.");

                if (file.Length > MaxFileSizeBytes)
                    throw new InvalidOperationException("File size exceeds the 10 MB limit.");
            }

            // Helper to create a fake IFormFile
            private static IFormFile MakeFakeFile(string fileName, long sizeBytes = 1024)
            {
                var content = new byte[sizeBytes];
                var stream = new MemoryStream(content);
                return new FormFile(stream, 0, sizeBytes, "SignedAgreement", fileName);
            }

           

            [Fact]
            public void Valid_Pdf_File_Passes_Validation()
            {
                var file = MakeFakeFile("signed_agreement.pdf");
                var ex = Record.Exception(() => ValidateFile(file));
                Assert.Null(ex);
            }

            [Fact]
            public void Pdf_With_Uppercase_Extension_Passes_Validation()
            {
                // PDF should be treated the same as .pdf
                var file = MakeFakeFile("AGREEMENT.PDF");
                var ex = Record.Exception(() => ValidateFile(file));
                Assert.Null(ex);
            }

            [Fact]
            public void Pdf_At_Max_Size_Passes_Validation()
            {
                // Exactly 10 MB should pass
                var file = MakeFakeFile("large.pdf", MaxFileSizeBytes);
                var ex = Record.Exception(() => ValidateFile(file));
                Assert.Null(ex);
            }

            // Blocked File Types 

            [Fact]
            public void Exe_File_Is_Rejected()
            {
                var file = MakeFakeFile("malware.exe");
                var ex = Assert.Throws<InvalidOperationException>(() => ValidateFile(file));
                Assert.Contains(".exe", ex.Message);
            }

            [Fact]
            public void Docx_File_Is_Rejected()
            {
                var file = MakeFakeFile("contract.docx");
                var ex = Assert.Throws<InvalidOperationException>(() => ValidateFile(file));
                Assert.Contains("PDF", ex.Message);
            }

            [Fact]
            public void Jpg_File_Is_Rejected()
            {
                var file = MakeFakeFile("photo.jpg");
                Assert.Throws<InvalidOperationException>(() => ValidateFile(file));
            }

            [Fact]
            public void Txt_File_Is_Rejected()
            {
                var file = MakeFakeFile("notes.txt");
                Assert.Throws<InvalidOperationException>(() => ValidateFile(file));
            }

            [Fact]
            public void Zip_File_Is_Rejected()
            {
                var file = MakeFakeFile("archive.zip");
                Assert.Throws<InvalidOperationException>(() => ValidateFile(file));
            }

            [Fact]
            public void Script_File_Is_Rejected()
            {
                var file = MakeFakeFile("script.bat");
                Assert.Throws<InvalidOperationException>(() => ValidateFile(file));
            }

            // Edge Cases 
            [Fact]
            public void Null_File_Throws_InvalidOperation()
            {
                var ex = Assert.Throws<InvalidOperationException>(() => ValidateFile(null));
                Assert.Contains("No file", ex.Message);
            }

            [Fact]
            public void Empty_File_Zero_Bytes_Is_Rejected()
            {
                var file = MakeFakeFile("empty.pdf", sizeBytes: 0);
                var ex = Assert.Throws<InvalidOperationException>(() => ValidateFile(file));
                Assert.Contains("No file", ex.Message);
            }

            [Fact]
            public void File_Exceeding_10MB_Is_Rejected()
            {
                // 10 MB + 1 byte = over the limit
                var file = MakeFakeFile("huge.pdf", MaxFileSizeBytes + 1);
                var ex = Assert.Throws<InvalidOperationException>(() => ValidateFile(file));
                Assert.Contains("10 MB", ex.Message);
            }

            [Fact]
            public void File_With_No_Extension_Is_Rejected()
            {
                var file = MakeFakeFile("nodotfile");
                Assert.Throws<InvalidOperationException>(() => ValidateFile(file));
            }

            [Fact]
            public void File_With_Double_Extension_Pdf_Exe_Is_Rejected()
            {
                // Security edge case: "trick.pdf.exe" should fail because
                // GetExtension returns .exe (the last extension)
                var file = MakeFakeFile("trick.pdf.exe");
                var ex = Assert.Throws<InvalidOperationException>(() => ValidateFile(file));
                Assert.Contains(".exe", ex.Message);
            }
        }
    }
