using QuizAdmin.Models;
using QuizAdmin.Models.Output.Quiz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuizAdmin.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        #region Dashboard
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region Image Upload
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase qqfile)
        {

            if (qqfile != null)
            {
                // this works for IE
                string path = Guid.NewGuid() + "_" + Path.GetFileName(qqfile.FileName);
                var filename = Path.Combine(Server.MapPath("~/Content/attachment/"), path);
                string filePath = Path.Combine(("~/Content/attachment/"), path);
                qqfile.SaveAs(filename);
                List<string> lst = new List<string>();
                lst.Add(filePath);
                return Json(new { success = true, list = lst, filename = filePath.Replace("~", "") }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                // this works for Firefox, Chrome
                var filename = Request["qqfile"];
                string path = Guid.NewGuid() + "_" + Path.GetFileName(qqfile.FileName);
                if (!string.IsNullOrEmpty(filename))
                {
                    filename = Path.Combine(Server.MapPath("~/Content/attachment/"), path);
                    string filePath = Path.Combine(("~/Content/attachment/"), path);
                    using (var output = System.IO.File.Create(path))
                    {
                        Request.InputStream.CopyTo(output);
                    }


                    return Json(new { success = true, filename = filePath.Replace("~", "") }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { success = false });
        }
        #endregion

        #region Quiz
        public ActionResult Quiz()
        {
            return View();
        }

        public ActionResult QuizPartialView()
        {
            RepoQuiz db = new RepoQuiz();

            return PartialView(db.getQuiz());
        }

        public ActionResult AddQuiz(string mode, Guid? id)
        {
            if (!string.IsNullOrEmpty(mode) && mode == "edit")
            {
                RepoQuiz db = new RepoQuiz();
                ViewBag.id = id.Value;
                return View(db.getQuizById(id.Value));
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult AddQuiz(QuizData model)
        {
            try
            {
                RepoQuiz db = new RepoQuiz();
                bool result = db.addUpdateQuiz(model);
                if (result)
                {
                    TempData["success"] = "Saved Successfully";
                    return RedirectToAction("Quiz", "Home");
                }
                else
                {
                    TempData["error"] = "Record Added & Updated Unsuccessfully";
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = "Record Added & Updated Unsuccessfully";
            }

            return View();
        }

        public ActionResult DeleteQuiz(Guid Id)
        {
            try
            {
                RepoQuiz db = new RepoQuiz();
                if (db.deleteQuiz(Id))
                {
                    TempData["success"] = "Deleted Successfully";
                    return RedirectToAction("Quiz", "Home");
                }
                else
                {
                    TempData["error"] = "Record Deletion Unsuccessfull";
                    return RedirectToAction("Quiz", "Home");
                }
            }

            catch (Exception ee)
            {
                TempData["error"] = "Record Not Found or Deleted by Another user";
                return RedirectToAction("Quiz", "Home");
            }
        }
        #endregion

        #region QuizQuestion
        public ActionResult QuizQuestion()
        {
            return View();
        }

        public ActionResult QuizQuestionPartialView()
        {
            RepoQuiz db = new RepoQuiz();
            return PartialView(db.getQuizAnswer());
        }

        public ActionResult AddQuizQuestion(string mode, int? id)
        {
            RepoQuiz db = new RepoQuiz();
            ViewBag.QuizTitle = new SelectList(db.getQuiz(), "QuizID", "QuizTitle");
            if (!string.IsNullOrEmpty(mode) && mode == "edit")
            {
                ViewBag.id = id.Value;
                return View(db.getQuizAnswerById(id.Value));
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult AddQuizQuestion(QuizQuestion model)
        {
            try
            {
                RepoQuiz db = new RepoQuiz();
                bool result = db.addUpdateQuizAnswer(model);
                if (result)
                {
                    TempData["success"] = "Saved Successfully";
                    return RedirectToAction("QuizQuestion", "Home");
                }
                else
                {
                    TempData["error"] = "Record Added & Updated Unsuccessfully";
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = "Record Added & Updated Unsuccessfully";
            }

            return View();
        }

        public ActionResult DeleteQuizQuestion(int Id)
        {
            try
            {
                RepoQuiz db = new RepoQuiz();
                if (db.deleteQuizAnswer(Id))
                {
                    TempData["success"] = "Deleted Successfully";
                    return RedirectToAction("QuizQuestion", "Home");
                }
                else
                {
                    TempData["error"] = "Record Deletion Unsuccessfull";
                    return RedirectToAction("QuizQuestion", "Home");
                }
            }

            catch (Exception ee)
            {
                TempData["error"] = "Record Not Found or Deleted by Another user";
                return RedirectToAction("Quiz", "Home");
            }
        }
        #endregion

        #region user
        public ActionResult UserMaster()
        {
            return View();
        }

        public ActionResult UserMasterPartialView()
        {
            try
            {
                RepoUserMaster db = new RepoUserMaster();
                var s = db.getUser();
                ViewBag.allusers = s;
                return PartialView();
            }
            catch(Exception ex)
            {
                return PartialView();
            }
        }
        #endregion

        #region Game
        public ActionResult UsersGame(string id)
        {
            RepoQuiz db = new RepoQuiz();
            return View(db.UsersQuizList(id));
        }
        #endregion
    }
}