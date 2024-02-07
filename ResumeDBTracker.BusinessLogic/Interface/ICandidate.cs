using ResumeDBTracker.Business.ViewModel;
using ResumeDBTracker.Core.Models;

namespace ResumeDBTracker.Business.Interface
{
    public interface ICandidate
    {
        int GetCandidateCount();
        List<Candidate> candidatePagination(int? page, int pagesize);

        User login(string username, string password);

        FileUploadResponse fileupload(MemoryStream memoryStream, string fileName, string fileContent, string category_id);//boopathi
        List<CandidateResume> unprocessedcandidateresumeList();

        CandidateResume filedownload(string fileName);
        CandidateResponse UpdateCandidate(Candidate candidate);
        CandidateResponse DeleteCandidate(string candidateid);
        int GetTechnicalSkillCount();
        int GetCategoryCount();
        int GetCategoryCountByName(string categoryName);

		int CategoryInsert(string name, string updatedBy, string candidateIds);
        int CategoryUpdate(string name, string categoryId, string updatedBy);
        int CategoryDelete(string categoryId);
        List<Category> CategoryGetAll();
        List<Category> CategoryCandidateMapping(string candidateId);
        int CategoryCandidateMappingInsert(string candidateIds, string categoryId, string updatedBy);


	}
}
