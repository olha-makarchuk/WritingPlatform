namespace Application.Interfaces
{
    public interface IApiClientGoogleDrive
    {
        public string AddFile(string fileToUpload, string folderType);
        public void UpdateFile(string filePath, string fileId);
        public bool DeleteFile(string fileId);
        public string GetTextFromDocxFile(string fileId, int pageNumber);
        public int CountPages(string filePath);
    }
}
