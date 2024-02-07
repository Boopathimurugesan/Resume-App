using Dapper;
using DocumentFormat.OpenXml.Wordprocessing;
using ResumeDBTracker.Business.Interface;
using ResumeDBTracker.Business.ViewModel;
using ResumeDBTracker.Core.Models;
using System.Data;
using System.Data.SqlClient;

namespace ResumeDBTracker.Data
{
    public class CandidateService : ICandidate, IUser
    {
        public List<Candidate> candidatePagination(int? pageNumber, int pageSize)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationStringManager.ResumeTrackerDB))
            {
                var param = new DynamicParameters();
                param.Add("@PageNumber", pageNumber);
                param.Add("@PageSize", pageSize);
                var data = con.Query<Candidate>("CandidatePagination", param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return data;
            }
        }

        public List<CandidateResume> unprocessedcandidateresumeList()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationStringManager.ResumeTrackerDB))
            {
                var data = con.Query<CandidateResume>("[sb].[usp_candidate_resume_get_unprocessed]", commandType: System.Data.CommandType.StoredProcedure).ToList();
                return data;
            }
        }

        public List<User> UserList(string email)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationStringManager.ResumeTrackerDB))
            {
                var param = new DynamicParameters();
                param.Add("@email", email);
                var data = con.Query<User>("usp_user_info_list", param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return data;
            }
        }

        public UserResponse InsertUser(User user)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationStringManager.ResumeTrackerDB))
            using (SqlCommand cmd = new SqlCommand("dbo.usp_user_info_insert", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // set up the parameterss
                cmd.Parameters.Add("@username", SqlDbType.NVarChar, 255).Value = user.first_name + " " + user.last_name;
                cmd.Parameters.Add("@first_name", SqlDbType.NVarChar, 255).Value = user.first_name;
                cmd.Parameters.Add("@last_name", SqlDbType.NVarChar, 3000).Value = user.last_name;
                cmd.Parameters.Add("@email", SqlDbType.NVarChar, 255).Value = user.email;
                cmd.Parameters.Add("@Password", SqlDbType.NVarChar, 3000).Value = user.password;
                cmd.Parameters.Add("@count", SqlDbType.Int, 10).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@message", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;


                // open connection and execute stored procedure
                conn.Open();
                cmd.ExecuteNonQuery();

                // read output value from @NewId
                UserResponse userResponse = new UserResponse();
                userResponse.count = Convert.ToInt32(cmd.Parameters["@count"].Value);
                userResponse.message = Convert.ToString(cmd.Parameters["@message"].Value);
                conn.Close();
                return userResponse;
            }
        }

        public UserResponse UpdateUser(User user)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationStringManager.ResumeTrackerDB))
            using (SqlCommand cmd = new SqlCommand("dbo.usp_user_info_update", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // set up the parameters
                cmd.Parameters.Add("@user_id", SqlDbType.NVarChar, 255).Value = user.user_id;
                cmd.Parameters.Add("@first_name", SqlDbType.NVarChar, 255).Value = user.first_name;
                cmd.Parameters.Add("@last_name", SqlDbType.NVarChar, 3000).Value = user.last_name;
                cmd.Parameters.Add("@email", SqlDbType.NVarChar, 255).Value = user.email;
                cmd.Parameters.Add("@password", SqlDbType.NVarChar, 3000).Value = user.password;
                //cmd.Parameters.Add("@count", SqlDbType.Int, 10).Direction = ParameterDirection.Output;
                // cmd.Parameters.Add("@message", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;


                // open connection and execute stored procedure
                conn.Open();
                int count = cmd.ExecuteNonQuery();

                // read output value from @NewId
                UserResponse userResponse = new UserResponse();
                //userResponse.count = Convert.ToInt32(cmd.Parameters["@count"].Value);
                //userResponse.message = Convert.ToString(cmd.Parameters["@message"].Value);
                userResponse.count = count;
                userResponse.message = "Updated successfully";
                conn.Close();
                return userResponse;
            }
        }

        public UserResponse DeleteUser(string userid)
        {
            //using (SqlConnection con = new SqlConnection(ConfigurationStringManager.ResumeTrackerDB))
            //{
            //    var param = new DynamicParameters();
            //    param.Add("@user_id", userid.Trim());
            //    var data = con.Query<int>("usp_user_info_delete", param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
            //    return data;
            //}

            using (SqlConnection conn = new SqlConnection(ConfigurationStringManager.ResumeTrackerDB))
            using (SqlCommand cmd = new SqlCommand("dbo.usp_user_info_delete", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // set up the parameters
                cmd.Parameters.Add("@user_id", SqlDbType.NVarChar, 255).Value = userid;


                // open connection and execute stored procedure
                conn.Open();
                int count = cmd.ExecuteNonQuery();

                // read output value from @NewId
                UserResponse userResponse = new UserResponse();
                //userResponse.count = Convert.ToInt32(cmd.Parameters["@count"].Value);
                //userResponse.message = Convert.ToString(cmd.Parameters["@message"].Value);
                userResponse.count = count;
                //userResponse.message = "Deleted successfully";
                conn.Close();
                return userResponse;
            }
        }

        public int GetCandidateCount()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationStringManager.ResumeTrackerDB))
            {
                var param = new DynamicParameters();
                var data = con.Query<int>("getCandidateCount", param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                return data;
            }
        }

        public User login(string username, string password)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationStringManager.ResumeTrackerDB))
            {
                var param = new DynamicParameters();
                param.Add("@Email", username.Trim());
                param.Add("@Password", password.Trim());
                var data = con.Query<User>("AdminLogin", param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                return data;
            }
        }

        public CandidateResponse DeleteCandidate(string candidateid)
        {
            //using (SqlConnection con = new SqlConnection(ConfigurationStringManager.ResumeTrackerDB))
            //{
            //    var param = new DynamicParameters();
            //    param.Add("@candidate_id", candidateid.Trim());
            //    var data = con.Query<int>("usp_candidate_info_delete", param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
            //    return data;
            //}

            using (SqlConnection conn = new SqlConnection(ConfigurationStringManager.ResumeTrackerDB))
            using (SqlCommand cmd = new SqlCommand("dbo.usp_candidate_info_delete", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // set up the parameters
                cmd.Parameters.Add("@candidate_id", SqlDbType.NVarChar, 255).Value = candidateid.Trim();


                // open connection and execute stored procedure
                conn.Open();
                int count = cmd.ExecuteNonQuery();

                // read output value from @NewId
                CandidateResponse candidateResponse = new CandidateResponse();
                //userResponse.count = Convert.ToInt32(cmd.Parameters["@count"].Value);
                //userResponse.message = Convert.ToString(cmd.Parameters["@message"].Value);
                candidateResponse.count = count;
                //userResponse.message = "Deleted successfully";
                conn.Close();
                return candidateResponse;
            }
        }

        public FileUploadResponse fileupload(MemoryStream memoryStream, string fileName, string fileContent, string category_id)//boopathi
        {
            // define connection and command, in using blocks to ensure disposal
            using (SqlConnection conn = new SqlConnection(ConfigurationStringManager.ResumeTrackerDB))
            using (SqlCommand cmd = new SqlCommand("dbo.usp_candidate_resume_upload", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                byte[] memorystreamarray = memoryStream.ToArray();
                // set up the parameters
                cmd.Parameters.Add("@resume_file", SqlDbType.VarBinary).Value = memorystreamarray;
                cmd.Parameters.Add("@file_name", SqlDbType.NVarChar, 255).Value = fileName;
                cmd.Parameters.Add("@file_content", SqlDbType.NVarChar, 3000).Value = fileContent;
				cmd.Parameters.Add("@category_id", SqlDbType.NVarChar, 255).Value = category_id;// boopathi
				cmd.Parameters.Add("@count", SqlDbType.Int, 10).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@message", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;

                // open connection and execute stored procedure
                conn.Open();
                cmd.ExecuteNonQuery();

                // read output value from @NewId
                FileUploadResponse fileUploadResponse = new FileUploadResponse();
                fileUploadResponse.count = Convert.ToInt32(cmd.Parameters["@count"].Value);
                fileUploadResponse.message = Convert.ToString(cmd.Parameters["@message"].Value);
                conn.Close();
                return fileUploadResponse;
            }
        }

        public CandidateResume filedownload(string resume_id)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationStringManager.ResumeTrackerDB))
            {
                var param = new DynamicParameters();
                param.Add("@resume_id", resume_id);
                var data = con.Query<CandidateResume>("CandidateResumeDownload", param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                return data;
            }
        }

        public CandidateResponse UpdateCandidate(Candidate candidate)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationStringManager.ResumeTrackerDB))
            using (SqlCommand cmd = new SqlCommand("dbo.usp_candidate_info_update", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // set up the parameters
                cmd.Parameters.Add("@candidate_id", SqlDbType.NVarChar, 255).Value = candidate.candidate_id;
                cmd.Parameters.Add("@first_name", SqlDbType.NVarChar, 255).Value = candidate.first_name;
                cmd.Parameters.Add("@phone_number", SqlDbType.NVarChar, 255).Value = candidate.phone_number;
                cmd.Parameters.Add("@email", SqlDbType.NVarChar, 255).Value = candidate.email;
                cmd.Parameters.Add("@location", SqlDbType.NVarChar, 3000).Value = candidate.location;
                cmd.Parameters.Add("@total_exp", SqlDbType.NVarChar, 255).Value = candidate.total_exp;
                cmd.Parameters.Add("@technical_skill", SqlDbType.NVarChar, 3000).Value = candidate.technical_skill;

                // open connection and execute stored procedure
                conn.Open();
                int count = cmd.ExecuteNonQuery();

                // read output value from @NewId
                CandidateResponse candidateResponse = new CandidateResponse();
                if (count == 1)
                {
                    candidateResponse.count = count;
                    candidateResponse.message = "Updated successfully";
                }
                else
                {
                    candidateResponse.count = count;
                    candidateResponse.message = "Not updated!";
                }
                conn.Close();
                return candidateResponse;
            }
        }

        public int GetTechnicalSkillCount()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationStringManager.ResumeTrackerDB))
            {
                var param = new DynamicParameters();
                var data = con.Query<int>("getTechnicalSkillCount", param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                return data;
            }
        }

        public int GetCategoryCount()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationStringManager.ResumeTrackerDB))
            {
                var param = new DynamicParameters();
                var data = con.Query<int>("getCategoryCount", param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                return data;
            }
        }

        //Boopathi
		public int GetCategoryCountByName(string categoryName)
		{
			using (SqlConnection con = new SqlConnection(ConfigurationStringManager.ResumeTrackerDB))
			{
				var param = new DynamicParameters();
				param.Add("category_name", categoryName);
				var data = con.Query<int>("getCategoryCountByName", param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
				return data;
			}
		}
		public int CategoryInsert(string name, string updatedBy, string candidateIds)
		{
			using (SqlConnection con = new SqlConnection(ConfigurationStringManager.ResumeTrackerDB))
			{
				var param = new DynamicParameters();
                param.Add("name", name);
                param.Add("updated_by", updatedBy);
                param.Add("candidate_ids", candidateIds);
				var data = con.Execute("usp_Category_insert", param, commandType: System.Data.CommandType.StoredProcedure);
				return data;
			}
		}

		public int CategoryUpdate(string name, string categoryId, string updatedBy)
		{
			using (SqlConnection con = new SqlConnection(ConfigurationStringManager.ResumeTrackerDB))
			{
				var param = new DynamicParameters();
				param.Add("name", name);
				param.Add("category_id", categoryId);
				param.Add("updated_by", updatedBy);
				var data = con.Execute("usp_Category_update", param, commandType: System.Data.CommandType.StoredProcedure);
				return data;
			}
		}

		public int CategoryDelete(string categoryId)
		{
			using (SqlConnection con = new SqlConnection(ConfigurationStringManager.ResumeTrackerDB))
			{
				var param = new DynamicParameters();
				param.Add("category_id", categoryId);
				var data = con.Execute("usp_Category_delete", param, commandType: System.Data.CommandType.StoredProcedure);
				return data;
			}
		}

		public List<Core.Models.Category> CategoryGetAll()
		{
			using (SqlConnection con = new SqlConnection(ConfigurationStringManager.ResumeTrackerDB))
			{
				var param = new DynamicParameters();
				var data = con.Query<Core.Models.Category>("usp_Category_GetAll", param, commandType: System.Data.CommandType.StoredProcedure).ToList();
				return data;
			}
		}

		public List<Core.Models.Category> CategoryCandidateMapping(string categoryId)
		{
			using (SqlConnection con = new SqlConnection(ConfigurationStringManager.ResumeTrackerDB))
			{
				var param = new DynamicParameters();
                param.Add("category_id", categoryId);
				var data = con.Query<Core.Models.Category>("usp_category_candidate_mapping_getby_category_id", param, commandType: System.Data.CommandType.StoredProcedure).ToList();
				return data;
			}
		}

		public int CategoryCandidateMappingInsert(string candidateIds, string categoryId, string updatedBy)
		{
			using (SqlConnection con = new SqlConnection(ConfigurationStringManager.ResumeTrackerDB))
			{
				var param = new DynamicParameters();
                param.Add("@candidate_ids", candidateIds);
				param.Add("category_id", categoryId);
                param.Add("updated_by", updatedBy);
				var data = con.Execute("usp_category_candidate_mapping_insert", param, commandType: System.Data.CommandType.StoredProcedure);
				return data;
			}
		}
	}
}