using Data_Access_Layer.Repository;
using Data_Access_Layer.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer
{
    public class DALMission
    {
        private readonly AppDbContext _cIDbContext;

        public DALMission(AppDbContext cIDbContext)
        {
            _cIDbContext = cIDbContext;
        }
        public List<Missions> MissionList()
        {
            return _cIDbContext.Missions.Where(x => !x.IsDeleted).ToList();
        }

        public string AddMission(Missions mission)
        {
            string result = "";
            try
            {
                _cIDbContext.Missions.Add(mission);
                _cIDbContext.SaveChanges();
                result = "Mission added Successfully.";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public async Task<string> DeleteMissionAsync(int id)
        {
            try
            {
                var existing = await _cIDbContext.Missions.Where(x => !x.IsDeleted && x.Id == id).FirstOrDefaultAsync();
                if (existing != null)
                {
                    existing.IsDeleted = true;
                    await _cIDbContext.SaveChangesAsync();
                    return "Delete Successfully.";
                }
                else
                {
                    throw new Exception("Mission is not found.");
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error in deleting.", ex);
            }
        }

        public async Task<string> UpdateMissionAsync(Missions mission)
        {
            try
            {
                var existing = await _cIDbContext.Missions.Where(x => !x.IsDeleted && x.Id == mission.Id).FirstOrDefaultAsync();
                if (existing != null)
                {
                    existing.MissionTitle = mission.MissionTitle;
                    existing.MissionStatus = mission.MissionStatus;
                    await _cIDbContext.SaveChangesAsync();
                    return "Update Successfully.";
                }
                else
                {
                    throw new Exception("Mission is not found.");
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error in updating.", ex);
            }
        }
        public List<Missions> ClientSideMissionList(int userId)
        {
            List<Missions> clientSideMissionList = new List<Missions>();
            try
            {
                clientSideMissionList = _cIDbContext.Missions
                    .Where(m => !m.IsDeleted)
                    .OrderBy(m => m.CreatedDate)
                    .Select(m => new Missions
                    {
                        Id = m.Id,
                        CountryId = m.CountryId,
                        CountryName = m.CountryName,
                        CityId = m.CityId,
                        CityName = m.CityName,
                        MissionTitle = m.MissionTitle,
                        MissionDescription = m.MissionDescription,
                        MissionOrganisationName = m.MissionOrganisationName,
                        MissionOrganisationDetail = m.MissionOrganisationDetail,
                        TotalSheets = m.TotalSheets,
                        RegistrationDeadLine = m.RegistrationDeadLine,
                        MissionThemeId = m.MissionThemeId,
                        MissionThemeName = m.MissionThemeName,
                        MissionImages = m.MissionImages,
                        MissionDocuments = m.MissionDocuments,
                        MissionSkillId = m.MissionSkillId,
                        MissionSkillName = string.Join(",", m.MissionSkillName),
                        MissionAvilability = m.MissionAvilability,
                        MissionVideoUrl = m.MissionVideoUrl,
                        MissionType = m.MissionType,
                        StartDate = m.StartDate,
                        EndDate = m.EndDate,
                        MissionStatus = m.RegistrationDeadLine < DateTime.Now.AddDays(-1) ? "Closed" : "Available",
                        MissionApplyStatus = _cIDbContext.MissionApplication.Any(ma => ma.MissionId == m.Id && ma.UserId == userId) ? "Applied" : "Apply",
                        MissionApproveStatus = _cIDbContext.MissionApplication.Any(ma => ma.MissionId == m.Id && ma.UserId == userId && ma.Status == true) ? "Approved" : "Applied",
                        MissionDateStatus = m.EndDate <= DateTime.Now.AddDays(-1) ? "MissionEnd" : "MissionRunning",
                        MissionDeadLineStatus = m.RegistrationDeadLine <= DateTime.Now.AddDays(-1) ? "Closed" : "Running",
                        MissionFavouriteStatus = _cIDbContext.MissionFavourites.Any(mf => mf.MissionId == m.Id && mf.UserId == userId) ? "1" : "0",
                        Rating = _cIDbContext.MissionRating.FirstOrDefault(mr => mr.MissionId == m.Id && mr.UserId == userId).Rating ?? 0
                    }).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
            return clientSideMissionList;
        }
        public string ApplyMission(MissionApplication missionApplication)
        {
            string result = "";
            try
            {
                using (var transaction = _cIDbContext.Database.BeginTransaction())
                {
                    try
                    {
                        var mission = _cIDbContext.Missions.FirstOrDefault(m => m.Id == missionApplication.MissionId && m.IsDeleted == false);
                        if (mission != null)
                        {
                            if (mission.TotalSheets > 0)
                            {
                                var newApplication = new MissionApplication
                                {
                                    MissionId = missionApplication.MissionId,
                                    UserId = missionApplication.UserId,
                                    AppliedDate = DateTime.UtcNow,
                                    Status = false,
                                    CreatedDate = DateTime.UtcNow,
                                    IsDeleted = false,
                                };

                                _cIDbContext.MissionApplication.Add(newApplication);
                                _cIDbContext.SaveChanges();

                                mission.TotalSheets = mission.TotalSheets - 1;
                                _cIDbContext.SaveChanges();

                                result = "Mission Apply Successfully.";
                            }
                            else
                            {
                                result = "Mission Housefull";
                            }
                        }
                        else
                        {
                            result = "Mission Not Found.";
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }

        public string MissionApplicationApprove(int id)
        {
            var result = "";
            try
            {
                var missionApplication = _cIDbContext.MissionApplication.FirstOrDefault(ma => ma.Id == id);
                if (missionApplication != null)
                {
                    missionApplication.Status = true;
                    _cIDbContext.SaveChanges();
                    result = "Mission is approved";
                }
                else
                {
                    result = "Mission Application is not found.";
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }
    }
}