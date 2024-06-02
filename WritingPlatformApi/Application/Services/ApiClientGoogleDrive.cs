using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2;
using Application.Interfaces;
using Newtonsoft.Json.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Bibliography;
using System.IO;

namespace Application.Services
{
    public class ApiClientGoogleDrive : IApiClientGoogleDrive
    {
        private string credentialsPath = "D:\\Диплом\\project\\icons\\client_secret.json";
        public string folderId { get; set; } = string.Empty;

        public string GetIdFile(string folderId)
        {
            var jsonText = File.ReadAllText("folderIdGoogleDrive.json");
            var jsonObject = JObject.Parse(jsonText);
            return jsonObject[$"{folderId}"].ToString();
        }

        public string AddFile(string fileToUpload, string folderType)
        {
            folderId = GetIdFile(folderType);
            if (!File.Exists(fileToUpload))
            {
                throw new FileNotFoundException($"File '{fileToUpload}' does not exist.");
            }

            try
            {
                GoogleCredential credential;
                using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleCredential.FromStream(stream).CreateScoped(new[]
                    {
                        DriveService.ScopeConstants.DriveFile
                    });
                }

                var service = GetDriveService();

                var fileMetaData = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = Path.GetFileName(fileToUpload),
                    Parents = new List<string> { folderId }
                };

                FilesResource.CreateMediaUpload request;
                using (var streamFile = new FileStream(fileToUpload, FileMode.Open))
                {
                    request = service.Files.Create(fileMetaData, streamFile, "");
                    request.Fields = "id";
                    request.Upload();
                }
                var uploadedFile = request.ResponseBody;

                return uploadedFile.Id;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error uploading file: {ex.Message}", ex);
            }
        }

        public void UpdateFile(string filePath, string fileId)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File '{filePath}' does not exist.");
            }

            try
            {
                var service = GetDriveService();

                var fileMetadata = service.Files.Get(fileId).Execute();

                using (var stream2 = new FileStream(filePath, FileMode.Open))
                {
                    var updateRequest = service.Files.Update(fileMetadata, fileId, stream2, "");
                    var response = updateRequest.Upload();
                    if ((int)response.Status != 200)
                    {
                        throw new Exception($"File update failed with status code {response.Status}.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating file: {ex.Message}", ex);
            }
        }

        public bool DeleteFile(string fileId)
        {
            try
            {
                var service = GetDriveService();

                var result = service.Files.Delete(fileId).Execute();
                return result != null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting file: {ex.Message}", ex);
            }
        }

        private DriveService GetDriveService()
        {
            GoogleCredential credential;
            using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(new[]
                {
                    DriveService.ScopeConstants.DriveFile
                });
            }

            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "WritingPlatform"
            });
        }



        public string GetTextFromDocxFile(string fileId, int pageNumber)
        {
            var service = GetDriveService();

            var fileStream = new MemoryStream();

            try
            {
                // Завантажити файл за його fileId
                service.Files.Get(fileId).Download(fileStream);

                // Повернути текст з docx файлу для заданої сторінки
                return ReadTextFromDocxPage(fileStream, pageNumber);
            }
            finally
            {
                fileStream.Close();
            }
        }

        private string ReadTextFromDocxPage(MemoryStream fileStream, int pageNumber)
        {
            using (WordprocessingDocument doc = WordprocessingDocument.Open(fileStream, false))
            {
                var body = doc.MainDocumentPart.Document.Body;
                var paragraphs = body.Elements<Paragraph>().ToList();

                // Розрахувати номери рядків для заданої сторінки
                int startLine = (pageNumber - 1) * 30;
                int endLine = Math.Min(startLine + 29, paragraphs.Count - 1);

                // Витягнути текст для заданої сторінки
                var lines = paragraphs.Skip(startLine).Take(endLine - startLine + 1).Select(p => p.InnerText);
                return string.Join(Environment.NewLine, lines);
            }
        }

        public int CountPages(string filePath)
        {
            using (WordprocessingDocument doc = WordprocessingDocument.Open(filePath, false))
            {
                var count = doc.MainDocumentPart.Document.Body.Elements<Paragraph>().Count();
                return (int)Math.Ceiling((double)count / 30);
            }
        }
    }
}
