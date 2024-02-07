using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Hosting;
using NPOI.SS.Formula.Functions;
using NPOI.XWPF.UserModel;
using ResumeDBTracker.Business.Interface;
using ResumeDBTracker.Business.ViewModel;
using ResumeDBTracker.Common;
using ResumeDBTracker.Core.Models;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;
using Document = DocumentFormat.OpenXml.Wordprocessing.Document;
using TempDataDictionary = Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary;

namespace ResumeDBTracker.Business
{
    public class BusinessLogic
    {
        ICandidate _Icandidate;
        IUser _IUser;

        public BusinessLogic(ICandidate Icandidate, IUser IUser)
        {
            _Icandidate = Icandidate;
            _IUser = IUser;
        }

        public User login(string username, string password)
        {
            string decryptPassword = EncryptionHelper.ConvertStringToHex(password, System.Text.Encoding.Unicode);
            User result = _Icandidate.login(username, decryptPassword);
            return result;
        }

        public FileUploadResponse fileupload(IFormFile postedFile, string category_id)//boopathi
        {
            string fileName = System.IO.Path.GetFileName(postedFile.FileName);
            string fileExtension = System.IO.Path.GetExtension(postedFile.FileName);

            using (MemoryStream memoryStream = new MemoryStream())
			{
				memoryStream.Position = 0;
				postedFile.CopyTo(memoryStream);
                string fileContent = "";
                if (fileExtension.Contains(".doc"))
                {
                    fileContent = ReadResumeTextContent(memoryStream, fileExtension);
                }
                return _Icandidate.fileupload(memoryStream, fileName, fileContent, category_id);
            }
        }

        public List<CandidateResume> unprocessedcandidateresumeList()
        {
            return _Icandidate.unprocessedcandidateresumeList();
        }

        public String ReadResumeTextContent(MemoryStream memoryStream, string fileExtension)
        {
            //To avoid EOF in header issue
            memoryStream.Position = 0;


            //Please 
            string textExtractResult = string.Empty;
            if (string.IsNullOrEmpty(fileExtension)) return textExtractResult;
            switch (fileExtension)
            {
                case ".pdf":
                    memoryStream.Position = 0;
                    textExtractResult = ExtractPDFFile(memoryStream);
                    break;

                case ".docx":
                    textExtractResult = ExtractDOCXFile(memoryStream);
                    break;

                case ".doc": //TODO: Doc files are changing to extract to text. Either Admin should modified doc to docx while uploading resume.
                    MemoryStream docxConvertedMS = ConvertDocToDocxUsingMemoryStream(memoryStream);
                    docxConvertedMS.Position = 0;
                    textExtractResult = ExtractDOCXFile(memoryStream);
                    break;
            }
            return textExtractResult;
        }

        private string ExtractDOCXFile(MemoryStream memoryStream)
        {
            try
            {

                XWPFDocument doc = new XWPFDocument(memoryStream);
                string text = string.Join(" ", doc.Paragraphs.Select(p => p.ParagraphText));
                return text;
            }
            catch
            {
                return string.Empty;
            }
        }

        private string ExtractPDFFile(MemoryStream memoryStream)
        {
            StringBuilder text = new StringBuilder();

            using (PdfReader pdfReader = new PdfReader(memoryStream))
            {
                for (int page = 1; page <= pdfReader.NumberOfPages; page++)
                {
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    string currentText = PdfTextExtractor.GetTextFromPage(pdfReader, page, strategy);
                    text.Append(currentText);
                }
            }
            return text.ToString();
        }

        private static MemoryStream ConvertDocToDocxUsingMemoryStream(MemoryStream memoryStream)
        {
            try
            {
                // Create a new Word document (DOCX) using the memory stream.
                using (WordprocessingDocument docx = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document))
                {
                    MainDocumentPart mainPart = docx.AddMainDocumentPart();
                    mainPart.Document = new Document(new Body(new Paragraph()));

                    // Save the DOCX document.
                    mainPart.Document.Save();
                }

                // Return the memory stream containing the DOCX document.
                return memoryStream;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }

        public bool genarateEmail(String to, String cc, String subject, String body)
        {
            bool EmailIsSent = false;

            MailMessage m = new MailMessage();
            SmtpClient sc = new SmtpClient();
            try
            {
                m.From = new MailAddress("kamaraj.balu@skybridgeinfotech.com");
                m.To.Add(new MailAddress(to));
                //m.CC.Add(new MailAddress("xxx@gmail.com", "Display name CC"));

                m.Subject = subject;
                m.IsBodyHtml = true;
                m.Body = body;

                sc.Host = "smtp.gmail.com";
                sc.Port = 587;
                sc.UseDefaultCredentials = false;
                sc.Credentials = new System.Net.NetworkCredential("kamaraj.balu@skybridgeinfotech.com", "Bdsksa020@");
                sc.DeliveryMethod = SmtpDeliveryMethod.Network;
                sc.EnableSsl = true;
                sc.Send(m);


                EmailIsSent = true;

            }
            catch (Exception ex)
            {
                EmailIsSent = false;
            }

            return EmailIsSent;
        }
    

        public List<User> UserList(string email)
        {
            List<User> users = new List<User>();
            var response= _IUser.UserList(email);
            foreach (var user in response)
            {
                User user1 = new User();
                string hextostring = EncryptionHelper.ConvertHexToString(user.password, System.Text.Encoding.Unicode);
                user1.password = hextostring;
                user1.email = user.email;
                user1.first_name = user.first_name;
                user1.last_name = user.last_name;
                user1.user_id = user.user_id;
                users.Add(user1);
            }
            return users;
        }

        public UserResponse UpdateUser(User user)
        {
            string stringtohex = EncryptionHelper.ConvertStringToHex(user.password, System.Text.Encoding.Unicode);
            user.password = stringtohex;
            return _IUser.UpdateUser(user);
        }
        public UserResponse InsertUser(User user)
        {
            return _IUser.InsertUser(user);
        }

        public UserResponse DeleteUser(string userid)
        {
            return _IUser.DeleteUser(userid);
        }

        public CandidateResponse UpdateCandidate(Candidate candidate)
        {
            return _Icandidate.UpdateCandidate(candidate);
        }
        public CandidateResponse DeleteCandidate(string candidateid)
        {
            return _Icandidate.DeleteCandidate(candidateid);
        }

    }
}
