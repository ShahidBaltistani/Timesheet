using computan.timesheet.Models.FreedCamp;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace computan.timesheet.Controllers
{
    public class FreedCampProjectController : BaseController
    {
        // GET: FreedCampProject
        public ActionResult Index()
        {
            List<core.FreedcampProject> projectsList = db.FreedcampProject.ToList();
            List<FreedCampProjectViewModel> fcpvm = new List<FreedCampProjectViewModel>();
            foreach (core.FreedcampProject item in projectsList)
            {
                FreedCampProjectViewModel project = new FreedCampProjectViewModel
                {
                    id = item.id,
                    Name = item.name,
                    isActive = item.isactive,
                    tsporjecid = Convert.ToInt64(item.tsprojectid),
                    fcprojectid = item.fcprojectid,
                    skill = Convert.ToInt64(item.skill)
                };
                fcpvm.Add(project);
            }

            ViewBag.projects = db.Project.ToList();
            ViewBag.skillslist = db.Skill.ToList();
            ViewBag.teamslist = db.Team.ToList();
            return View(fcpvm);
        }

        public JsonResult loadprojectuserandteam()
        {
            List<LoadUsersTeamViewModel> list = new List<LoadUsersTeamViewModel>();
            List<core.FreedcampProject> projects = db.FreedcampProject.ToList();
            //List<ApplicationUser> userslist = db.Users.Where(u => u.isactive).ToList();
            List<core.Team> teamslist = db.Team.Where(u => u.isactive).ToList();
            foreach (core.FreedcampProject project in projects)
            {
                LoadUsersTeamViewModel userandteam = new LoadUsersTeamViewModel();
                List<TagViewModel> usertag = new List<TagViewModel>();
                List<TagViewModel> teamtag = new List<TagViewModel>();
                userandteam.projectid = project.id;
                if (!string.IsNullOrEmpty(project.assignedto))
                {
                    string[] projectuserlist = project.assignedto.Split(',');
                    foreach (string item in projectuserlist)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            //ApplicationUser user = (from x in userslist where x.Id == item select x).FirstOrDefault();
                            core.ApplicationUser user = db.Users.Where(u => u.Id == item).FirstOrDefault();
                            if (user != null)
                            {
                                TagViewModel taguser = new TagViewModel
                                {
                                    userid = user.Id,
                                    name = user.FullName
                                };
                                usertag.Add(taguser);
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(project.team))
                {
                    string[] projectteamlist = project.team.Split(',');
                    foreach (string item in projectteamlist)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            long teamid = Convert.ToInt64(item);
                            core.Team team = (from x in teamslist where x.id == teamid select x).FirstOrDefault();
                            if (team != null)
                            {
                                TagViewModel tagteam = new TagViewModel
                                {
                                    teamid = teamid,
                                    name = team.name
                                };
                                teamtag.Add(tagteam);
                            }
                        }
                    }
                }

                userandteam.users = usertag;
                userandteam.teams = teamtag;
                list.Add(userandteam);
            }

            return Json(new { error = false, model = list }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddProject(long id, long projectid)
        {
            core.FreedcampProject fcproject = db.FreedcampProject.Find(id);
            try
            {
                if (fcproject != null)
                {
                    fcproject.tsprojectid = projectid;
                    fcproject.userid = User.Identity.GetUserId();
                    db.Entry(fcproject).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { error = false, Message = "Successfully updated" }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { error = true, Message = "Invalid project selected" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = true, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult AddSkill(long id, int skillid)
        {
            core.FreedcampProject fcproject = db.FreedcampProject.Find(id);
            try
            {
                if (fcproject != null)
                {
                    fcproject.skill = skillid;
                    fcproject.userid = User.Identity.GetUserId();
                    db.Entry(fcproject).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { error = false, Message = "Successfully updated" }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { error = true, Message = "Invalid project selected" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = true, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult AddUsers(long id, string userid)
        {
            core.FreedcampProject fcproject = db.FreedcampProject.Find(id);
            bool teamadded = false;
            bool useradded = false;
            try
            {
                List<core.TeamMember> teamMember = db.TeamMember.Where(x => x.usersid == userid).Include(x => x.Team).ToList();
                long teamid = teamMember[0].teamid;

                if (fcproject != null)
                {
                    if (string.IsNullOrEmpty(fcproject.assignedto))
                    {
                        fcproject.assignedto = userid;
                        useradded = true;
                    }
                    else
                    {
                        if (!fcproject.assignedto.Contains(userid))
                        {
                            fcproject.assignedto = fcproject.assignedto + "," + userid;
                            useradded = true;
                        }
                    }

                    if (string.IsNullOrEmpty(fcproject.team))
                    {
                        fcproject.team = teamid.ToString();
                        teamadded = true;
                    }
                    else
                    {
                        if (!fcproject.team.Contains(teamid.ToString()))
                        {
                            fcproject.team = fcproject.team + "," + teamid;
                            teamadded = true;
                        }
                    }

                    fcproject.userid = User.Identity.GetUserId();
                    db.Entry(fcproject).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(
                        new
                        {
                            error = false,
                            Message = "Successfully updated",
                            team_id = teamid,
                            teamName = teamMember[0].Team.name,
                            useradd = useradded,
                            teamadd = teamadded
                        }, JsonRequestBehavior.AllowGet);
                }

                return Json(
                    new { error = true, Message = "Invalid project selected", useradd = useradded, teamadd = teamadded },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = true, ex.Message, useradd = useradded, teamadd = teamadded },
                    JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult AddTeams(long id, long teamid)
        {
            bool isadded = false;
            core.FreedcampProject fcproject = db.FreedcampProject.Find(id);
            try
            {
                if (fcproject != null)
                {
                    char[] trimele = { ',' };
                    if (string.IsNullOrEmpty(fcproject.team))
                    {
                        fcproject.team = teamid.ToString();
                        isadded = true;
                    }

                    else
                    {
                        if (!fcproject.team.Contains(teamid.ToString()))
                        {
                            fcproject.team.Trim(trimele);
                            fcproject.team = fcproject.team + "," + teamid;
                            isadded = true;
                        }
                    }

                    fcproject.userid = User.Identity.GetUserId();
                    db.Entry(fcproject).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { error = false, Message = "Successfully updated", teamadded = isadded },
                        JsonRequestBehavior.AllowGet);
                }

                return Json(new { error = true, Message = "Invalid project selected", teamadded = isadded },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = true, ex.Message, teamadded = isadded },
                    JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult RempoveUsers(long id, string usercsv)
        {
            core.FreedcampProject fcproject = db.FreedcampProject.Find(id);
            try
            {
                if (fcproject != null)
                {
                    fcproject.assignedto = usercsv;
                    fcproject.userid = User.Identity.GetUserId();
                    db.Entry(fcproject).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { error = false, Message = "Successfully updated" }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { error = true, Message = "Invalid project selected" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = true, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult RemoveTeams(long id, string teamscsv)
        {
            core.FreedcampProject fcproject = db.FreedcampProject.Find(id);
            try
            {
                if (fcproject != null)
                {
                    fcproject.team = teamscsv;
                    fcproject.userid = User.Identity.GetUserId();
                    db.Entry(fcproject).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { error = false, Message = "Successfully updated" }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { error = true, Message = "Invalid project selected" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = true, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}